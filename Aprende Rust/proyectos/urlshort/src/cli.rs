//! Interfaz de línea de comandos con Clap (API derive).
//!
//! La CLI se define como datos: una estructura `Cli` con un `enum Command`.
//! Clap genera `--help`, `--version`, validación y códigos de error a partir
//! de esa definición, y aquí solo hay que despachar cada variante.

use std::path::PathBuf;

use clap::{Parser, Subcommand};

/// Acortador de URLs del curso Aprende Rust.
#[derive(Debug, Parser)]
#[command(
    name = "urlshort",
    version,
    about = "Acorta URLs desde la terminal o sírvelas por HTTP",
    long_about = None,
    subcommand_required = true,
    arg_required_else_help = true
)]
pub struct Cli {
    /// URL base que se usa para imprimir los enlaces cortos.
    #[arg(long, global = true, default_value = "http://localhost:3000")]
    pub base_url: String,

    /// Fichero JSON donde se guardan los enlaces.
    #[arg(long, global = true, default_value = "data/links.json")]
    pub store: PathBuf,

    /// Subcomando a ejecutar.
    #[command(subcommand)]
    pub command: Command,
}

/// Subcomandos disponibles.
#[derive(Debug, Subcommand)]
pub enum Command {
    /// Acorta una URL y muestra el enlace corto.
    Short {
        /// URL completa, con esquema http o https.
        url: String,
    },

    /// Resuelve un código y muestra la URL de destino.
    Resolve {
        /// Código corto (por ejemplo, k7m2qd).
        code: String,
    },

    /// Lista los enlaces guardados, ordenados por visitas.
    List,

    /// Borra un código del almacén.
    Delete {
        /// Código corto que se quiere eliminar.
        code: String,
    },

    /// Arranca el servidor HTTP.
    Serve {
        /// Puerto de escucha en 127.0.0.1.
        #[arg(long, default_value_t = 3000)]
        port: u16,
    },
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn parsea_short_con_url() {
        let cli = Cli::try_parse_from(["urlshort", "short", "https://example.com"])
            .expect("parsea");
        assert!(matches!(cli.command, Command::Short { .. }));
        assert_eq!(cli.base_url, "http://localhost:3000");
    }

    #[test]
    fn acepta_flags_globales_antes_del_subcomando() {
        let cli = Cli::try_parse_from([
            "urlshort",
            "--base-url",
            "https://sho.rt",
            "short",
            "https://example.com",
        ])
        .expect("parsea");
        assert_eq!(cli.base_url, "https://sho.rt");
    }

    #[test]
    fn exige_un_subcomando() {
        assert!(Cli::try_parse_from(["urlshort"]).is_err());
    }

    #[test]
    fn el_puerto_por_defecto_del_servidor_es_3000() {
        let cli = Cli::try_parse_from(["urlshort", "serve"]).expect("parsea");
        match cli.command {
            Command::Serve { port } => assert_eq!(port, 3000),
            _ => panic!("esperaba Serve"),
        }
    }
}
