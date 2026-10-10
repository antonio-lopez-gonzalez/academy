# Lumen

Bandeja de notas mínima del curso **Spec-Driven Development**: una CLI en Node.js que crea, lista y cierra notas sobre un almacén JSON local.

La **fuente de verdad** de este proyecto es su spec: `spec/lumen.md`. El código de `src/` la implementa; si discrepan, manda la spec (actualízala en Git con motivo, no parchees a ciegas).

## Requisitos

- Node.js LTS (sin dependencias: cero `npm install`).
- Funciona en Linux, macOS y Windows (PowerShell o bash).

## Uso

```bash
# Desde esta carpeta (proyectos/lumen)
node src/cli.js --help
node src/cli.js add "comprar pan"
node src/cli.js list
node src/cli.js close <id>
node src/cli.js list --all   # incluye cerradas

# Almacén alternativo (por defecto: data/notes.json)
node src/cli.js add "leer spec" --store data/notes.json

# (opcional) exponer el alias `lumen` si quieres escribir `lumen add ...`
npm link   # package.json declara bin.lumen → node src/cli.js
```

## Validación del contrato (scripts)

La spec declara que el almacén debe ser un JSON válido según `schema/note.schema.json`.
Dos scripts sin dependencias lo comprueban (módulo 4 del curso):

```bash
node scripts/validate-notes.mjs data/notes.json schema/note.schema.json
# ✓ todas las notas válidas contra el esquema  (o lista la fila rota + exit 1)

node scripts/validate-api.mjs lumen.openapi.json
# ✓ contrato OpenAPI válido (openapi/info/paths)  (o lista el error + exit 1)
```

## Verificación (definition of done = la spec)

```bash
node src/cli.js add "primera nota" --store data/demo.json
node src/cli.js list --store data/demo.json
# → muestra la nota con id único
node src/cli.js close <id> --store data/demo.json
node src/cli.js list --store data/demo.json
# → la nota ya no aparece (sigue en el fichero con done:true; ver con --all)
```

## Estructura

```text
lumen/
├── README.md
├── package.json
├── spec/
│   ├── lumen.md         ← spec de producto (fuente de verdad)
│   ├── changelog.md     ← deltas aplicados
│   └── adr/
│       └── 0001-almacen-json.md
├── schema/
│   └── note.schema.json ← contrato de datos (una Nota)
├── scripts/
│   ├── validate-notes.mjs ← valida el almacén contra el esquema
│   └── validate-api.mjs   ← valida un contrato OpenAPI mínimo
├── src/
│   └── cli.js           ← CLI: add | list | close | --help (con funciones de almacén)
└── data/
    └── notes.json       ← almacén (se crea al usar el CLI)
```
