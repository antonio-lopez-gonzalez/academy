//! Dominio: qué es un enlace, cómo se valida una URL y cómo se genera un código.
//!
//! Esta capa no sabe nada de ficheros, de HTTP ni de terminales. Solo depende
//! de la biblioteca estándar y de `serde` para poder serializarse.

use std::collections::hash_map::DefaultHasher;
use std::hash::{Hash, Hasher};

use chrono::{SecondsFormat, Utc};
use serde::{Deserialize, Serialize};

use crate::error::Error;

/// Longitud de los códigos generados. 6 caracteres de un alfabeto de 31
/// símbolos dan ~887 millones de combinaciones: de sobra para este proyecto.
pub const CODE_LEN: usize = 6;

/// Alfabeto sin caracteres ambiguos al dictarlos o teclearlos: sin `0`, `1`,
/// `l`, `o` ni vocales que puedan confundirse con números.
const ALPHABET: &[u8] = b"23456789abcdefghjkmnpqrstuvwxyz";

/// Un enlace acortado.
#[derive(Debug, Clone, PartialEq, Eq, Serialize, Deserialize)]
pub struct Link {
    /// Código corto (`k7m2qd`).
    pub code: String,
    /// URL original completa.
    pub url: String,
    /// Fecha de creación en RFC 3339 (UTC).
    pub created_at: String,
    /// Veces que se ha resuelto el código.
    #[serde(default)]
    pub hits: u64,
}

impl Link {
    /// Crea un enlace nuevo con la fecha actual y el contador a cero.
    pub fn new(code: String, url: String) -> Self {
        Self {
            code,
            url,
            created_at: Utc::now().to_rfc3339_opts(SecondsFormat::Secs, true),
            hits: 0,
        }
    }

    /// Suma una visita. La llama el caso de uso `resolve`.
    pub fn register_hit(&mut self) {
        self.hits += 1;
    }

    /// URL corta lista para pintar o devolver por HTTP.
    pub fn short_url(&self, base_url: &str) -> String {
        format!("{}/{}", base_url.trim_end_matches('/'), self.code)
    }
}

/// Valida y normaliza una URL de entrada.
///
/// No pretende ser un parser de RFC 3986: comprueba lo que de verdad rompe un
/// acortador (esquema ausente o raro, host vacío) y deja el resto al navegador.
pub fn validate_url(raw: &str) -> Result<String, Error> {
    let trimmed = raw.trim();
    if trimmed.is_empty() {
        return Err(Error::InvalidUrl("la URL está vacía".to_string()));
    }

    let (scheme, rest) = trimmed.split_once("://").ok_or_else(|| {
        Error::InvalidUrl("falta el esquema http:// o https://".to_string())
    })?;

    if !scheme.eq_ignore_ascii_case("http") && !scheme.eq_ignore_ascii_case("https") {
        return Err(Error::InvalidUrl(format!("esquema no permitido: {scheme}")));
    }

    let host = rest.split(['/', '?', '#']).next().unwrap_or("");
    if host.is_empty() {
        return Err(Error::InvalidUrl("la URL no tiene host".to_string()));
    }

    Ok(trimmed.to_string())
}

/// Deriva un código estable y reproducible a partir de la URL y una sal.
///
/// Determinista a propósito: la misma URL con la misma sal da el mismo código,
/// lo que hace los tests triviales. La sal resuelve las colisiones.
pub fn generate_code(seed: &str, salt: u64) -> String {
    let mut hasher = DefaultHasher::new();
    seed.hash(&mut hasher);
    salt.hash(&mut hasher);
    let mut n = hasher.finish();

    let mut code = String::with_capacity(CODE_LEN);
    for _ in 0..CODE_LEN {
        let index = (n % ALPHABET.len() as u64) as usize;
        code.push(ALPHABET[index] as char);
        n /= ALPHABET.len() as u64;
    }
    code
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn acepta_http_y_https() {
        assert!(validate_url("http://example.com").is_ok());
        assert!(validate_url("https://example.com/ruta?x=1").is_ok());
    }

    #[test]
    fn rechaza_sin_esquema_y_esquemas_raros() {
        assert!(validate_url("example.com").is_err());
        assert!(validate_url("ftp://example.com").is_err());
        assert!(validate_url("   ").is_err());
    }

    #[test]
    fn rechaza_host_vacio() {
        assert!(validate_url("https:///ruta").is_err());
    }

    #[test]
    fn el_codigo_es_estable_y_tiene_la_longitud_pedida() {
        let a = generate_code("https://example.com", 0);
        let b = generate_code("https://example.com", 0);
        assert_eq!(a, b);
        assert_eq!(a.len(), CODE_LEN);
    }

    #[test]
    fn sales_distintas_dan_codigos_distintos_casi_siempre() {
        let a = generate_code("https://example.com", 0);
        let b = generate_code("https://example.com", 1);
        assert_ne!(a, b);
    }

    #[test]
    fn la_url_corta_no_duplica_barras() {
        let link = Link::new("abc234".to_string(), "https://example.com".to_string());
        assert_eq!(link.short_url("https://sho.rt/"), "https://sho.rt/abc234");
    }
}
