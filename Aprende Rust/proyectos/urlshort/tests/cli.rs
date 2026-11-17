//! Tests de integración: usan el crate como lo haría un consumidor externo.
//!
//! A diferencia de los tests unitarios (dentro de cada módulo con
//! `#[cfg(test)]`), estos solo pueden llamar a la API **pública**. Es lo que
//! el usuario del crate ve, así que es lo que se prueba aquí.

use std::sync::Arc;

use clap::Parser;

use urlshort::cli::{Cli, Command};
use urlshort::domain::CODE_LEN;
use urlshort::error::Error;
use urlshort::service::Service;
use urlshort::store::{JsonStore, MemoryStore};

fn servicio() -> Service {
    Service::new(Arc::new(MemoryStore::new()), "https://sho.rt")
}

#[test]
fn el_flujo_completo_acorta_resuelve_y_borra() {
    let service = servicio();

    let link = service.short("https://www.rust-lang.org").expect("acortar");
    assert_eq!(link.code.len(), CODE_LEN);
    assert_eq!(link.short_url("https://sho.rt"), format!("https://sho.rt/{}", link.code));

    let resuelto = service.resolve(&link.code).expect("resolver");
    assert_eq!(resuelto.url, "https://www.rust-lang.org");
    assert_eq!(resuelto.hits, 1);

    service.delete(&link.code).expect("borrar");
    assert!(matches!(
        service.resolve(&link.code),
        Err(Error::NotFound(_))
    ));
}

#[test]
fn una_url_invalida_devuelve_invalid_url() {
    let service = servicio();
    let error = service.short("sin-esquema").expect_err("debe fallar");
    assert!(matches!(error, Error::InvalidUrl(_)));
    assert!(error.to_string().contains("URL inválida"));
}

#[test]
fn la_lista_llega_ordenada_por_visitas() {
    let service = servicio();
    let poco = service.short("https://example.com/a").expect("a");
    let mucho = service.short("https://example.com/b").expect("b");

    for _ in 0..3 {
        service.resolve(&mucho.code).expect("visita");
    }

    let lista = service.list().expect("listar");
    assert_eq!(lista.len(), 2);
    assert_eq!(lista[0].code, mucho.code);
    assert_eq!(lista[1].code, poco.code);
}

#[test]
fn el_almacen_json_sobrevive_a_reinicios() {
    let dir = tempfile::tempdir().expect("tmpdir");
    let path = dir.path().join("links.json");

    let code = {
        let service = Service::new(
            Arc::new(JsonStore::open(&path).expect("abrir")),
            "https://sho.rt",
        );
        service.short("https://example.com").expect("acortar").code
    };

    // Proceso "nuevo": se abre el mismo fichero y el enlace sigue ahí.
    let service = Service::new(
        Arc::new(JsonStore::open(&path).expect("reabrir")),
        "https://sho.rt",
    );
    let link = service.resolve(&code).expect("resolver");
    assert_eq!(link.url, "https://example.com");
}

#[test]
fn la_cli_parsea_los_cuatro_subcomandos() {
    let casos = [
        vec!["urlshort", "short", "https://example.com"],
        vec!["urlshort", "resolve", "k7m2qd"],
        vec!["urlshort", "list"],
        vec!["urlshort", "delete", "k7m2qd"],
    ];

    for caso in casos {
        assert!(Cli::try_parse_from(caso.clone()).is_ok(), "fallo en {caso:?}");
    }
}

#[test]
fn serve_usa_el_puerto_indicado() {
    let cli = Cli::try_parse_from(["urlshort", "serve", "--port", "4321"]).expect("parsea");
    match cli.command {
        Command::Serve { port } => assert_eq!(port, 4321),
        _ => panic!("esperaba el subcomando serve"),
    }
}
