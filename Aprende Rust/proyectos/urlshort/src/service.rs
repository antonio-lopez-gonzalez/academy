//! Casos de uso: la lógica que comparten la CLI y el servidor HTTP.
//!
//! [`Service`] no sabe si le llaman desde un terminal o desde una petición
//! HTTP. Ese es justo el objetivo: una sola implementación de las reglas.

use std::sync::Arc;

use crate::domain::{generate_code, validate_url, Link};
use crate::error::Error;
use crate::store::Store;

/// Número de sales que se prueban antes de rendirse con una URL.
const MAX_SALTS: u64 = 16;

/// Fachada de los casos de uso de urlshort.
#[derive(Clone)]
pub struct Service {
    store: Arc<dyn Store>,
    base_url: String,
}

impl Service {
    /// Crea el servicio con su almacén y la URL base para pintar enlaces cortos.
    pub fn new(store: Arc<dyn Store>, base_url: impl Into<String>) -> Self {
        Self {
            store,
            base_url: base_url.into(),
        }
    }

    /// URL base configurada (`https://sho.rt`, `http://localhost:3000`…).
    pub fn base_url(&self) -> &str {
        &self.base_url
    }

    /// Valida la URL, genera un código libre y lo guarda.
    pub fn short(&self, raw_url: &str) -> Result<Link, Error> {
        let url = validate_url(raw_url)?;

        for salt in 0..MAX_SALTS {
            let code = generate_code(&url, salt);
            if !self.store.exists(&code)? {
                // `url` se clona solo al ganar: el bucle puede seguir probando sales.
                let link = Link::new(code, url.clone());
                self.store.insert(link.clone())?;
                return Ok(link);
            }
        }

        Err(Error::Conflict(
            "no se pudo generar un código único; prueba otra vez".to_string(),
        ))
    }

    /// Busca el código, suma una visita y devuelve el enlace actualizado.
    pub fn resolve(&self, code: &str) -> Result<Link, Error> {
        let mut link = self
            .store
            .get(code)?
            .ok_or_else(|| Error::NotFound(code.to_string()))?;
        link.register_hit();
        self.store.update(link.clone())?;
        Ok(link)
    }

    /// Todos los enlaces, ordenados por visitas.
    pub fn list(&self) -> Result<Vec<Link>, Error> {
        self.store.list()
    }

    /// Borra un código; si no existe, es un error de dominio.
    pub fn delete(&self, code: &str) -> Result<(), Error> {
        if self.store.delete(code)? {
            Ok(())
        } else {
            Err(Error::NotFound(code.to_string()))
        }
    }
}

#[cfg(test)]
mod tests {
    use super::*;
    use crate::store::MemoryStore;

    fn service() -> Service {
        Service::new(Arc::new(MemoryStore::new()), "https://sho.rt")
    }

    #[test]
    fn acorta_y_resuelve_sumando_visitas() {
        let service = service();
        let link = service.short("https://www.rust-lang.org").expect("acortar");
        assert_eq!(link.hits, 0);

        let primero = service.resolve(&link.code).expect("resolver");
        assert_eq!(primero.url, "https://www.rust-lang.org");
        assert_eq!(primero.hits, 1);

        let segundo = service.resolve(&link.code).expect("resolver otra vez");
        assert_eq!(segundo.hits, 2);
    }

    #[test]
    fn una_url_sin_esquema_no_se_acorta() {
        let service = service();
        assert!(matches!(
            service.short("example.com"),
            Err(Error::InvalidUrl(_))
        ));
    }

    #[test]
    fn dos_urls_distintas_generan_codigos_distintos() {
        let service = service();
        let a = service.short("https://example.com/a").expect("a");
        let b = service.short("https://example.com/b").expect("b");
        assert_ne!(a.code, b.code);
    }

    #[test]
    fn borrar_un_codigo_inexistente_es_not_found() {
        let service = service();
        assert!(matches!(service.delete("nada23"), Err(Error::NotFound(_))));
    }

    #[test]
    fn la_lista_sale_ordenada_por_visitas() {
        let service = service();
        let poco = service.short("https://example.com/poco").expect("poco");
        let mucho = service.short("https://example.com/mucho").expect("mucho");

        service.resolve(&mucho.code).expect("1");
        service.resolve(&mucho.code).expect("2");

        let lista = service.list().expect("listar");
        assert_eq!(lista[0].code, mucho.code);
        assert_eq!(lista[1].code, poco.code);
    }
}
