# Proyecto CMS — Modelos de dominio (Módulo 1)

## ¿Qué añadimos?

Los modelos de dominio del CMS como `record` inmutable con value equality:

| Modelo | Descripción |
|---|---|
| `Post` | Artículo con título, contenido, estado, fechas |
| `Category` | Categorías jerárquicas (parentCategoryId) |
| `Tag` | Etiquetas planas sin jerarquía |
| `User` | Usuario con rol (Author, Editor, Admin) |

## Decisiones de diseño

- **`record` en lugar de `class`**: inmutabilidad + value equality.
- **`Guid` como ID**: independencia de la BD, seguro en APIs (no secuencial).
- **`IReadOnlyList<Guid>` para TagIds**: referencia por ID en lugar de navegación directa (mejor para DDD y CQRS).
- **`string?` para campos opcionales**: NRT activados, el compilador ayuda a evitar nulls.
- **`init` accessor**: permite inicializar con `with` expressions pero no modificar después.
- **`PostStatus` y `UserRole` como enums**: código más expresivo que magic strings.

## Estructura

```
m01-cms-modelos/src/Cms.Domain/
├── Cms.Domain.csproj
└── Models.cs    ← Post, Category, Tag, User + enums
```

## Build

```bash
dotnet build proyectos/m01-cms-modelos
```

---

**Próximo módulo (M02):** Añadiremos servicios con interfaces (`IPostService`, `ICategoryService`) aplicando POO.
