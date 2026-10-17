# Laboratorio — Aprende Git

Escenarios de práctica del curso [`Aprende Git`](../../spec.md). Cada uno crea, en un
directorio **temporal fuera del repositorio**, un repositorio pequeño con una situación
real: un merge con conflictos, una historia sucia, un commit perdido, un bug escondido,
un remoto divergente o un repo desordenado.

No necesita red y no toca la Academy.

## Uso

```bash
node src/lab.mjs list          # escenarios disponibles
node src/lab.mjs conflicto     # crea el escenario e imprime qué hacer
node src/lab.mjs clean         # borra todos los escenarios
```

Escenarios:

| Escenario | Qué contiene |
|---|---|
| `conflicto` | `main` y `feature/busqueda` chocan en `notas.md`; merge sin resolver |
| `historia` | commits diminutos y mensajes pobres para practicar `rebase -i` |
| `perdido` | un commit importante que solo se recupera con `reflog` |
| `bisect` | un bug introducido en un commit; se caza con `bisect run node verificar.mjs` |
| `equipo` | repositorio bare local + dos clones divergentes |
| `auditoria` | repositorio desordenado para el proyecto final |

## Tests

Comprueban que cada escenario queda realmente en el estado esperado:

```bash
node --test test
```

Requiere **Git 2.23+** (para `switch`) y **Node 20+**. El laboratorio usa `os.tmpdir()`,
así que en Windows los repositorios aparecen en `%TEMP%\aprende-git-lab\`.
