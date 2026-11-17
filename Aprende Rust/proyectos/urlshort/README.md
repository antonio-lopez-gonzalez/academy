# urlshort

Acortador de URLs del curso **Aprende Rust**: un mismo dominio expuesto por
línea de comandos y por HTTP.

```text
$ urlshort short "https://www.rust-lang.org/learn"
https://sho.rt/k7m2qd

$ urlshort resolve k7m2qd
https://www.rust-lang.org/learn

$ urlshort list
CÓDIGO    HITS  CREADO                  URL
k7m2qd       1  2026-11-17T09:14:22Z    https://www.rust-lang.org/learn
```

## Requisitos

- Rust estable 1.85 o superior (`rustup override set stable` si usas otra).
- No hay servicios externos: el almacén es un fichero JSON.

## Uso

```bash
# Compilar y ejecutar sin instalar
cargo run -- short "https://example.com"

# Instalar el binario en ~/.cargo/bin
cargo install --path .

# Servidor HTTP en el puerto 3000
cargo run -- serve --port 3000

curl -s -X POST localhost:3000/links \
  -H "content-type: application/json" \
  -d '{"url":"https://example.com"}'

curl -i localhost:3000/links/k7m2qd
```

## Diseño

```text
src/
├── main.rs      cablea las piezas (no tiene lógica)
├── lib.rs       API pública del crate
├── domain.rs    Link, validación de URL y generación de códigos
├── error.rs     Error de dominio (thiserror)
├── store.rs     trait Store + MemoryStore + JsonStore
├── service.rs   casos de uso: short, resolve, list, delete
├── server.rs    adaptador HTTP con Axum
└── cli.rs       CLI con Clap (derive)
```

La regla es siempre la misma: **el dominio no depende de nadie**. `Http` y
`Cli` son adaptadores que llaman al mismo `Service`; por eso los tests del
dominio no necesitan ni puerto ni terminal.

## Tests

```bash
cargo test                  # unitarios + integración + doctests
cargo clippy -- -D warnings
cargo fmt --check
```

Los tests usan `MemoryStore` para no tocar el disco y un `tempdir` cuando el
objetivo es precisamente comprobar la persistencia.

## Limitaciones conocidas (y por qué están)

- **Sin verificación del destino**: no se hace `HEAD` a la URL. Se explica en el
  módulo 6 cómo añadirlo con `reqwest`.
- **Almacén JSON**: sencillo y suficiente para miles de enlaces; a partir de ahí
  el curso cambia a SQLite con `rusqlite`.
- **Sin autenticación**: es un proyecto didáctico; la API es de solo lectura
  pública y escritura abierta.
