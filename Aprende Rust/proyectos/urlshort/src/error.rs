//! Errores del dominio.
//!
//! Una sola enumeración para toda la librería: el binario, el servidor HTTP y
//! el almacén hablan el mismo idioma de errores. `#[from]` convierte
//! automáticamente los errores de E/S y de JSON, así que `?` funciona sin
//! `map_err` en todas partes.

use thiserror::Error;

/// Todo lo que puede fallar en urlshort.
#[derive(Debug, Error)]
pub enum Error {
    /// La URL no tiene esquema, no tiene host o viene vacía.
    #[error("URL inválida: {0}")]
    InvalidUrl(String),

    /// No existe ningún enlace con ese código.
    #[error("no existe el código {0}")]
    NotFound(String),

    /// El código generado ya está ocupado (o no se encontró uno libre).
    #[error("conflicto con el código {0}")]
    Conflict(String),

    /// El `Mutex` interno del almacén quedó envenenado por un panic anterior.
    #[error("el almacén está bloqueado; reinicia el proceso")]
    Lock,

    /// Error de sistema de ficheros al leer o escribir el almacén.
    #[error("error de E/S: {0}")]
    Io(#[from] std::io::Error),

    /// El fichero de datos no es JSON válido o no encaja con `Link`.
    #[error("error de formato: {0}")]
    Json(#[from] serde_json::Error),
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn el_mensaje_de_not_found_incluye_el_codigo() {
        let error = Error::NotFound("abc123".to_string());
        assert_eq!(error.to_string(), "no existe el código abc123");
    }

    #[test]
    fn un_error_de_io_se_convierte_con_from() {
        let io = std::io::Error::new(std::io::ErrorKind::NotFound, "no está");
        let error: Error = io.into();
        assert!(matches!(error, Error::Io(_)));
    }
}
