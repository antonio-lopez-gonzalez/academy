//! urlshort — acortador de URLs del curso **Aprende Rust**.
//!
//! El crate está partido en capas para poder probarlas por separado:
//!
//! - [`domain`]: el concepto `Link` y las reglas del negocio (validar, generar código).
//! - [`store`]: el puerto `Store` y sus adaptadores en memoria y en JSON.
//! - [`service`]: los casos de uso (`short`, `resolve`, `list`, `delete`).
//! - [`server`]: el adaptador HTTP con Axum.
//! - [`cli`]: la interfaz de línea de comandos con Clap.
//! - [`error`]: el error de dominio compartido por todas las capas.
//!
//! El binario (`src/main.rs`) solo cablea piezas: no contiene lógica propia.

pub mod cli;
pub mod domain;
pub mod error;
pub mod server;
pub mod service;
pub mod store;

pub use error::Error;
