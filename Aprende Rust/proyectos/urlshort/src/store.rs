//! Almacenamiento: el puerto [`Store`] y dos adaptadores intercambiables.
//!
//! El dominio y los casos de uso hablan del *trait*, nunca de ficheros: en los
//! tests se usa [`MemoryStore`] (rápido, sin disco) y en el binario
//! [`JsonStore`] (persistente, escritura atómica).

use std::collections::HashMap;
use std::fs;
use std::path::{Path, PathBuf};
use std::sync::{Mutex, MutexGuard};

use crate::domain::Link;
use crate::error::Error;

/// Contrato de persistencia. `Send + Sync` para poder compartirlo entre hilos
/// y guardarlo en el estado de Axum.
pub trait Store: Send + Sync {
    /// Guarda un enlace nuevo. Falla si el código ya existe.
    fn insert(&self, link: Link) -> Result<(), Error>;
    /// Devuelve el enlace, si existe.
    fn get(&self, code: &str) -> Result<Option<Link>, Error>;
    /// Devuelve todos los enlaces ordenados por visitas.
    fn list(&self) -> Result<Vec<Link>, Error>;
    /// Borra un enlace. Devuelve `false` si no existía.
    fn delete(&self, code: &str) -> Result<bool, Error>;
    /// Sustituye un enlace existente (se usa al sumar una visita).
    fn update(&self, link: Link) -> Result<(), Error>;

    /// Atajo: ¿existe ya este código?
    fn exists(&self, code: &str) -> Result<bool, Error> {
        Ok(self.get(code)?.is_some())
    }
}

/// Almacén en memoria. Es el que usan los tests.
#[derive(Default)]
pub struct MemoryStore {
    links: Mutex<HashMap<String, Link>>,
}

impl MemoryStore {
    /// Crea un almacén vacío.
    pub fn new() -> Self {
        Self::default()
    }

    fn guard(&self) -> Result<MutexGuard<'_, HashMap<String, Link>>, Error> {
        self.links.lock().map_err(|_| Error::Lock)
    }
}

impl Store for MemoryStore {
    fn insert(&self, link: Link) -> Result<(), Error> {
        let mut links = self.guard()?;
        if links.contains_key(&link.code) {
            return Err(Error::Conflict(link.code));
        }
        links.insert(link.code.clone(), link);
        Ok(())
    }

    fn get(&self, code: &str) -> Result<Option<Link>, Error> {
        Ok(self.guard()?.get(code).cloned())
    }

    fn list(&self) -> Result<Vec<Link>, Error> {
        let mut links: Vec<Link> = self.guard()?.values().cloned().collect();
        sort_by_hits(&mut links);
        Ok(links)
    }

    fn delete(&self, code: &str) -> Result<bool, Error> {
        Ok(self.guard()?.remove(code).is_some())
    }

    fn update(&self, link: Link) -> Result<(), Error> {
        let mut links = self.guard()?;
        if !links.contains_key(&link.code) {
            return Err(Error::NotFound(link.code));
        }
        links.insert(link.code.clone(), link);
        Ok(())
    }
}

/// Almacén persistente en un único fichero JSON.
///
/// Escribe en un fichero temporal y luego renombra: si el proceso muere a
/// mitad, el fichero bueno sigue intacto. Es la versión "didáctica" que se
/// sustituye por SQLite cuando el volumen crece (módulo 6).
pub struct JsonStore {
    path: PathBuf,
    links: Mutex<HashMap<String, Link>>,
}

impl JsonStore {
    /// Abre el fichero si existe, o empieza vacío si no.
    pub fn open(path: impl AsRef<Path>) -> Result<Self, Error> {
        let path = path.as_ref().to_path_buf();
        let links = match fs::read_to_string(&path) {
            Ok(raw) if raw.trim().is_empty() => HashMap::new(),
            Ok(raw) => serde_json::from_str(&raw)?,
            Err(error) if error.kind() == std::io::ErrorKind::NotFound => HashMap::new(),
            Err(error) => return Err(Error::Io(error)),
        };
        Ok(Self {
            path,
            links: Mutex::new(links),
        })
    }

    /// Ruta del fichero de datos (útil para mensajes de la CLI).
    pub fn path(&self) -> &Path {
        &self.path
    }

    fn guard(&self) -> Result<MutexGuard<'_, HashMap<String, Link>>, Error> {
        self.links.lock().map_err(|_| Error::Lock)
    }

    fn persist(&self, links: &HashMap<String, Link>) -> Result<(), Error> {
        if let Some(dir) = self.path.parent() {
            if !dir.as_os_str().is_empty() {
                fs::create_dir_all(dir)?;
            }
        }
        let tmp = self.path.with_extension("json.tmp");
        fs::write(&tmp, serde_json::to_string_pretty(links)?)?;
        fs::rename(&tmp, &self.path)?;
        Ok(())
    }
}

impl Store for JsonStore {
    fn insert(&self, link: Link) -> Result<(), Error> {
        let mut links = self.guard()?;
        if links.contains_key(&link.code) {
            return Err(Error::Conflict(link.code));
        }
        links.insert(link.code.clone(), link);
        self.persist(&links)
    }

    fn get(&self, code: &str) -> Result<Option<Link>, Error> {
        Ok(self.guard()?.get(code).cloned())
    }

    fn list(&self) -> Result<Vec<Link>, Error> {
        let mut links: Vec<Link> = self.guard()?.values().cloned().collect();
        sort_by_hits(&mut links);
        Ok(links)
    }

    fn delete(&self, code: &str) -> Result<bool, Error> {
        let mut links = self.guard()?;
        let removed = links.remove(code).is_some();
        if removed {
            self.persist(&links)?;
        }
        Ok(removed)
    }

    fn update(&self, link: Link) -> Result<(), Error> {
        let mut links = self.guard()?;
        if !links.contains_key(&link.code) {
            return Err(Error::NotFound(link.code));
        }
        links.insert(link.code.clone(), link);
        self.persist(&links)
    }
}

/// Ordena por visitas descendentes y, a igualdad, por código.
fn sort_by_hits(links: &mut [Link]) {
    links.sort_by(|a, b| b.hits.cmp(&a.hits).then_with(|| a.code.cmp(&b.code)));
}

#[cfg(test)]
mod tests {
    use super::*;

    fn link(code: &str) -> Link {
        Link::new(code.to_string(), "https://example.com".to_string())
    }

    #[test]
    fn el_almacen_en_memoria_guarda_y_devuelve() {
        let store = MemoryStore::new();
        store.insert(link("abc234")).expect("insertar");
        let found = store.get("abc234").expect("leer").expect("existe");
        assert_eq!(found.url, "https://example.com");
    }

    #[test]
    fn insertar_dos_veces_el_mismo_codigo_da_conflicto() {
        let store = MemoryStore::new();
        store.insert(link("abc234")).expect("primera");
        assert!(matches!(
            store.insert(link("abc234")),
            Err(Error::Conflict(_))
        ));
    }

    #[test]
    fn el_json_store_persiste_entre_aperturas() {
        let dir = tempfile::tempdir().expect("tmpdir");
        let path = dir.path().join("links.json");

        {
            let store = JsonStore::open(&path).expect("abrir");
            store.insert(link("abc234")).expect("insertar");
        }

        let store = JsonStore::open(&path).expect("reabrir");
        assert!(store.get("abc234").expect("leer").is_some());
    }

    #[test]
    fn borrar_devuelve_false_si_no_existe() {
        let store = MemoryStore::new();
        assert!(!store.delete("nada23").expect("borrar"));
    }
}
