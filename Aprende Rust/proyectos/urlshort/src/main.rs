//! Punto de entrada del binario: parsea la CLI y cablea las piezas.
//!
//! Aquí no hay lógica de negocio. Si algo falla, se imprime el error de
//! dominio en `stderr` y se devuelve un código de salida distinto de cero:
//! así un script que invoque `urlshort` puede detectar el fallo.

use std::process::ExitCode;
use std::sync::Arc;

use clap::Parser;

use urlshort::cli::{Cli, Command};
use urlshort::error::Error;
use urlshort::server;
use urlshort::service::Service;
use urlshort::store::{JsonStore, Store};

#[tokio::main]
async fn main() -> ExitCode {
    let cli = Cli::parse();

    match run(cli).await {
        Ok(()) => ExitCode::SUCCESS,
        Err(error) => {
            eprintln!("error: {error}");
            ExitCode::FAILURE
        }
    }
}

async fn run(cli: Cli) -> Result<(), Error> {
    match &cli.command {
        Command::Serve { port } => serve(&cli, *port).await,
        Command::Short { url } => {
            let service = build_service(&cli)?;
            let link = service.short(url)?;
            // La salida principal va a stdout: es lo que un script captura.
            println!("{}", link.short_url(service.base_url()));
            Ok(())
        }
        Command::Resolve { code } => {
            let service = build_service(&cli)?;
            let link = service.resolve(code)?;
            println!("{}", link.url);
            Ok(())
        }
        Command::List => {
            let service = build_service(&cli)?;
            let links = service.list()?;

            if links.is_empty() {
                println!("no hay enlaces guardados en {}", cli.store.display());
                return Ok(());
            }

            println!("{:<8} {:>5}  {:<22}  URL", "CÓDIGO", "HITS", "CREADO");
            for link in &links {
                println!(
                    "{:<8} {:>5}  {:<22}  {}",
                    link.code, link.hits, link.created_at, link.url
                );
            }
            println!("\n{} enlace(s)", links.len());
            Ok(())
        }
        Command::Delete { code } => {
            let service = build_service(&cli)?;
            service.delete(code)?;
            println!("borrado {code}");
            Ok(())
        }
    }
}

/// Arranca el servidor HTTP con el mismo dominio que la CLI.
async fn serve(cli: &Cli, port: u16) -> Result<(), Error> {
    let service = build_service(cli)?;
    let app = server::router(service);
    let address = ("127.0.0.1", port);

    let listener = tokio::net::TcpListener::bind(address).await?;
    println!("urlshort escuchando en http://127.0.0.1:{port}");
    println!("  POST   /links        {{\"url\": \"https://...\"}}");
    println!("  GET    /links/{{code}}  redirige al destino");
    println!("  DELETE /links/{{code}}  borra el enlace");

    axum::serve(listener, app).await?;
    Ok(())
}

/// Abre el almacén JSON y construye el servicio.
fn build_service(cli: &Cli) -> Result<Service, Error> {
    let store: Arc<dyn Store> = Arc::new(JsonStore::open(&cli.store)?);
    Ok(Service::new(store, cli.base_url.clone()))
}
