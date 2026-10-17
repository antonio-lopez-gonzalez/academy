# ADR 0001: almacén en JSON plano

- Fecha: 2026-10-04
- Contexto: Lumen necesita persistir notas con cero dependencias y edición humana ocasional.
- Decisión: un fichero `data/notes.json` con un array de notas validado contra `schema/note.schema.json`.
- Consecuencias: ganamos simplicidad y transparencia (el almacén se lee con cualquier editor); perdemos concurrencia y consultas; migrar a SQLite más tarde es compatible porque el contrato de Nota no cambia.
