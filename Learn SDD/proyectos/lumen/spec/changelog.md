# Lumen — changelog de specs

Historial de deltas aplicados a la spec. Cada entrada: qué cambió, por qué y qué criterio lo demuestra.

## 2026-10-04 — Spec inicial (v1)

- Se crea `lumen.md`: inbox mínimo con `add`, `list`, `close` sobre `data/notes.json`.
- No-objetivos explícitos: sin multiusuario, sin web, sin cifrado.
- Contrato: `schema/note.schema.json` con `{ id, title, done, createdAt }`.
- Decisión de almacén registrada en `adr/0001-almacen-json.md`.
