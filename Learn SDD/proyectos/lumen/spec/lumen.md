# Lumen — spec de producto

## Objetivo

Lumen es una bandeja de notas mínima para una sola persona: capturar una idea en segundos desde la terminal y recuperarla después, para que **un agente (o un humano) pueda implementarla sin adivinar**.

## No-objetivos

- No es un gestor de proyectos (sin prioridades, sin fechas, sin recordatorios).
- No es multiusuario (sin login, sin sincronización, sin conflictos).
- No es una app web (la UI estática es un posible delta futuro, no parte de esta spec).
- No cifra el almacén (las notas son texto plano local).

## Glosario

- **Nota**: `{ id, title, done, createdAt }`. `id`: cadena única; `title`: texto no vacío; `done`: booleano; `createdAt`: fecha ISO 8601.
- **Inbox**: el conjunto de notas con `done: false`. Es lo que muestra `list` por defecto.
- **Almacén**: fichero JSON con un array de notas. Por defecto `data/notes.json`.

## Restricciones

- Node.js LTS, JavaScript vanilla, **cero dependencias**.
- El almacén es un array JSON válido según `schema/note.schema.json`.
- Comandos en inglés (`add`, `list`, `close`); mensajes y ayuda en español.

## Supuestos

- Una sola persona usa el almacén a la vez (sin concurrencia).
- El reloj del sistema es razonable (los `createdAt` no se validan contra el futuro).

## CLI

- `node src/cli.js add "<título>" [--store <ruta>]`: crea una nota con id único. Falla con mensaje en español si falta el título.
- `node src/cli.js list [--all] [--store <ruta>]`: muestra las notas del inbox (o todas con `--all`), una por línea: `<id> <título>`.
- `node src/cli.js close <id> [--store <ruta>]`: marca `done: true`. Falla si el id no existe.
- `node src/cli.js --help`: imprime ayuda en español y sale con código 0.
- Cualquier comando desconocido o argumento inválido: mensaje en español + código de salida distinto de 0.

## Criterios de aceptación

- [ ] Dado el almacén vacío, cuando ejecuto `add "pan"`, entonces `list` muestra una nota con id único y título "pan".
- [ ] Dada una nota abierta, cuando ejecuto `close <id>`, entonces `list` ya no la muestra y `list --all` la muestra como cerrada.
- [ ] Cuando ejecuto `close <id-inexistente>`, entonces falla con mensaje en español y código ≠ 0.
- [ ] Cuando ejecuto `add` sin título, entonces falla con mensaje en español y código ≠ 0.
- [ ] Cuando ejecuto `--help`, entonces veo `add`, `list`, `close` descritos en español y el código es 0.
- [ ] Dada cualquier ejecución que escribe, entonces `data/notes.json` (o el `--store`) sigue siendo un array JSON válido según el schema.

## Fuera de alcance del plan

- Búsqueda, etiquetas, edición y borrado físico: son deltas futuros (ver `spec/changelog.md`), no parte de esta implementación.
