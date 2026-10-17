# Módulo 5 — Inyección de Dependencias en el CMS

## ¿Qué es este proyecto?

`Cms.DependencyInjection` es una biblioteca de clases que centraliza toda la
configuración del contenedor de Inyección de Dependencias (DI) del CMS.

Su propósito es que cualquier aplicación que quiera usar los servicios del CMS
solo tenga que llamar a un método:

```csharp
builder.Services.AddCmsServices();
```

Y automáticamente se registran todos los repositorios, validadores y servicios
de aplicación con los lifetimes correctos.

---

## ¿Qué lifetimes se usan y por qué?

### Singleton — Repositorios en memoria

Los repositorios en memoria (`InMemoryPostRepository`, `InMemoryCategoryRepository`,
`InMemoryTagRepository`, `InMemoryUserRepository`) se registran como **Singleton**.

**Razón:** Mantienen el estado de los datos en memoria (la lista de entidades).
Si fueran Scoped, cada petición HTTP tendría su propia lista vacía y los datos
no se compartirían entre peticiones. Singleton garantiza que los datos persistan
mientras la aplicación esté corriendo.

**Cuidado:** Cuando migremos a Entity Framework Core (Módulo 7), los repositorios
usarán `DbContext`, que **sí es Scoped**. En ese momento cambiaremos los
repositorios a Scoped también.

### Singleton — Validadores

Los validadores (`PostValidator`, `CategoryValidator`, `TagValidator`, `UserValidator`)
se registran como **Singleton**.

**Razón:** Son objetos stateless (sin estado). Cada método `Validate` recibe la
entidad y devuelve errores sin modificar estado interno. Al no tener estado,
son thread-safe por naturaleza. Singleton evita crear instancias nuevas en cada
petición, ahorrando memoria y presión sobre el GC.

### Scoped — Servicios de aplicación

Los servicios de aplicación (`PostService`, `CategoryService`, `TagService`,
`UserService`) se registran como **Scoped**.

**Razón:** En ASP.NET Core, Scoped significa "una instancia por petición HTTP".
Aunque ahora mismo los servicios no tienen estado por petición, en el futuro
dependerán de `DbContext` (que es Scoped). Mantener Scoped desde el principio
evita problemas de **Captive Dependencies** (cuando un Singleton depende de
un Scoped).

---

## Cómo añadir los servicios a una aplicación ASP.NET Core

En `Program.cs` de tu API:

```csharp
using Cms.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// ── Añadir todos los servicios del CMS ──
builder.Services.AddCmsServices();

// ── (Opcional) Activar validación de scopes en desarrollo ──
builder.Services.AddCmsValidation(validateScopes: true);

// ── (Opcional) Imprimir servicios registrados para depurar ──
if (builder.Environment.IsDevelopment())
{
    builder.Services.DebugPrintRegistrations();
}

var app = builder.Build();

// ... configurar middleware, endpoints, etc.

app.Run();
```

---

## El patrón de método de extensión `AddCmsServices`

En .NET moderno, es una buena práctica que cada biblioteca exponga sus propios
métodos de extensión sobre `IServiceCollection`. Esto se conoce como el patrón
**"Add[Something]"** y es usado por el propio framework:

- `AddControllers()`
- `AddDbContext<T>()`
- `AddAuthentication()`
- `AddCmsServices()` ← Nuestra contribución

**Ventajas de este patrón:**

1. **Encapsulación:** El consumidor no necesita saber qué implementaciones
   concretas se usan. Solo llama a `AddCmsServices()` y todo funciona.

2. **Consistencia:** Todos los servicios del CMS se registran con los
   lifetimes correctos. Evita que cada proyecto configure DI a mano
   y potencialmente cometa errores.

3. **Mantenibilidad:** Si en el futuro cambiamos de repositorios en memoria
   a repositorios de base de datos, solo cambiamos ESTE archivo. Las
   aplicaciones que consumen `AddCmsServices()` no se modifican.

4. **Testabilidad:** Los tests pueden usar `AddCmsServices()` tal cual para
   tests de integración, o pueden mockear servicios individuales para
   tests unitarios.

---

## Árbol de dependencias

```
builder.Services.AddCmsServices()
│
├── AddCmsRepositories()           ← Singleton
│   ├── IPostRepository → InMemoryPostRepository
│   ├── ICategoryRepository → InMemoryCategoryRepository
│   ├── ITagRepository → InMemoryTagRepository
│   └── IUserRepository → InMemoryUserRepository
│
├── AddCmsValidators()             ← Singleton
│   ├── PostValidator
│   ├── CategoryValidator
│   ├── TagValidator
│   └── UserValidator
│
└── AddCmsApplicationServices()    ← Scoped
    ├── IPostService → PostService
    ├── ICategoryService → CategoryService
    ├── ITagService → TagService
    └── IUserService → UserService
```

---

## Futuro: cambiar a repositorios reales de base de datos

En el Módulo 7 (Entity Framework Core), los repositorios en memoria se
reemplazarán por repositorios que usan `DbContext` contra PostgreSQL.

El cambio será **transparente** para los servicios de aplicación:

```csharp
// ANTES (Módulo 5 — Memoria)
services.AddSingleton<IPostRepository, InMemoryPostRepository>();

// DESPUÉS (Módulo 7 — PostgreSQL)
services.AddScoped<IPostRepository, PostRepository>();
```

Como `IPostService` depende de la abstracción `IPostRepository` (no de la
implementación concreta), el servicio no necesita ningún cambio. Esto es
el principio DIP (Dependency Inversion Principle) en acción.

---

## Captive Dependencies: el error más común

Una **Captive Dependency** ocurre cuando un servicio con un lifetime más largo
(Singleton) depende de un servicio con un lifetime más corto (Scoped o Transient).

```
❌ INCORRECTO:
services.AddSingleton<IPostRepository, PostgresPostRepository>(); // Singleton
services.AddScoped<CmsDbContext>();                                // Scoped

// PostgresPostRepository depende de CmsDbContext (Scoped).
// CmsDbContext queda "atrapado" en el Singleton para siempre.
// → Datos de diferentes peticiones se mezclan.
// → El DbContext nunca se libera (memory leak).
```

**Solución:** Ambos servicios deben tener el mismo lifetime:

```csharp
✅ CORRECTO:
services.AddScoped<IPostRepository, PostgresPostRepository>(); // Scoped
services.AddScoped<CmsDbContext>();                             // Scoped
```

Para detectar estos problemas en desarrollo, habilita `ValidateScopes`:

```csharp
builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateScopes = true;   // Lanza excepción si detecta Captive Dependency
    options.ValidateOnBuild = true;  // Valida todos los servicios al construir
});
```

---

## Resumen

- **Singleton** para repositorios en memoria (mantienen estado compartido).
- **Singleton** para validadores (no tienen estado).
- **Scoped** para servicios de aplicación (preparados para DbContext).
- El patrón `AddCmsServices()` encapsula toda la configuración de DI.
- El cambio futuro a PostgreSQL será transparente gracias al DIP.
