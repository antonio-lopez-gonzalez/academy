//! Adaptador HTTP: el mismo [`Service`] expuesto con Axum.
//!
//! Rutas (Axum 0.8 usa llaves para los parámetros):
//!
//! | Método | Ruta            | Qué hace                              |
//! |--------|-----------------|---------------------------------------|
//! | GET    | `/health`       | Comprobación de vida                   |
//! | POST   | `/links`        | Acorta una URL (`{"url": "..."}`)      |
//! | GET    | `/links/{code}` | Redirige al destino (302)              |
//! | DELETE | `/links/{code}` | Borra el enlace                        |

use axum::{
    extract::{Path, State},
    http::StatusCode,
    response::{IntoResponse, Redirect},
    routing::{get, post},
    Json, Router,
};
use serde::{Deserialize, Serialize};

use crate::domain::Link;
use crate::error::Error;
use crate::service::Service;

/// Cuerpo de `POST /links`.
#[derive(Debug, Deserialize)]
pub struct CreateLinkRequest {
    /// URL que se quiere acortar.
    pub url: String,
}

/// Respuesta de `POST /links`.
#[derive(Debug, Serialize)]
pub struct LinkResponse {
    /// Código corto generado.
    pub code: String,
    /// URL original.
    pub url: String,
    /// URL corta completa, lista para copiar.
    pub short_url: String,
    /// Fecha de creación en RFC 3339.
    pub created_at: String,
    /// Visitas acumuladas.
    pub hits: u64,
}

impl LinkResponse {
    fn new(link: &Link, base_url: &str) -> Self {
        Self {
            code: link.code.clone(),
            url: link.url.clone(),
            short_url: link.short_url(base_url),
            created_at: link.created_at.clone(),
            hits: link.hits,
        }
    }
}

/// Construye el router con el servicio como estado compartido.
///
/// `Service` es `Clone` y barato de clonar: dentro solo hay un `Arc`.
pub fn router(service: Service) -> Router {
    Router::new()
        .route("/health", get(health))
        .route("/links", post(create_link))
        .route("/links/{code}", get(resolve_link).delete(delete_link))
        .with_state(service)
}

async fn health() -> &'static str {
    "ok"
}

async fn create_link(
    State(service): State<Service>,
    Json(body): Json<CreateLinkRequest>,
) -> Result<(StatusCode, Json<LinkResponse>), ApiError> {
    let link = service.short(&body.url)?;
    let response = LinkResponse::new(&link, service.base_url());
    Ok((StatusCode::CREATED, Json(response)))
}

async fn resolve_link(
    State(service): State<Service>,
    Path(code): Path<String>,
) -> Result<Redirect, ApiError> {
    let link = service.resolve(&code)?;
    Ok(Redirect::temporary(&link.url))
}

async fn delete_link(
    State(service): State<Service>,
    Path(code): Path<String>,
) -> Result<StatusCode, ApiError> {
    service.delete(&code)?;
    Ok(StatusCode::NO_CONTENT)
}

/// Envoltura para convertir un error de dominio en una respuesta HTTP.
pub struct ApiError(Error);

impl From<Error> for ApiError {
    fn from(error: Error) -> Self {
        Self(error)
    }
}

impl IntoResponse for ApiError {
    fn into_response(self) -> axum::response::Response {
        let status = match &self.0 {
            Error::InvalidUrl(_) => StatusCode::BAD_REQUEST,
            Error::NotFound(_) => StatusCode::NOT_FOUND,
            Error::Conflict(_) => StatusCode::CONFLICT,
            Error::Lock | Error::Io(_) | Error::Json(_) => StatusCode::INTERNAL_SERVER_ERROR,
        };

        // No filtramos detalles internos en 500: el cliente solo ve el mensaje
        // de dominio, que está escrito para ser leído.
        let body = Json(serde_json::json!({ "error": self.0.to_string() }));
        (status, body).into_response()
    }
}

#[cfg(test)]
mod tests {
    use super::*;
    use crate::store::MemoryStore;
    use std::sync::Arc;

    fn servicio() -> Service {
        Service::new(Arc::new(MemoryStore::new()), "http://localhost:3000")
    }

    #[tokio::test]
    async fn el_router_se_construye_sin_panicos() {
        let app = router(servicio());
        // Si el router compila y se construye, el estado está bien cableado.
        let _ = app;
    }
}
