<div align="center">

# 🚀 Curso Completo de .NET 8 con C#

## De Cero a Arquitecto usando .NET 8 + C# 12

Curso en español · Código en inglés · 19 módulos · Un proyecto CMS real de principio a fin

</div>

---

## 📊 El curso en números

| | |
|---|---|
| 📚 **19 módulos** | Del CLR al despliegue en producción |
| 🧩 **73 lecciones** con código ejecutable | Cada una con teoría, ejemplos, ejercicios y retos |
| 🏗️ **6 proyectos CMS incrementales** | El CMS crece contigo en cada módulo |
| 🧪 **12 proyectos de tests** | xUnit + Moq + FluentAssertions + Coverlet |
| 💻 **400+ archivos C#** | Todo compila, todo se ejecuta |
| 🌐 **Web estática del curso** | 100+ páginas HTML con teoría completa |

---

## 🎯 ¿Qué es este proyecto?

Un curso completo de **.NET 8** pensado para desarrolladores que ya saben programar
(JavaScript, Python, Java, etc.) pero que **nunca han aprendido .NET correctamente**.

No es un tutorial más de «hola mundo»: cada lección explica **el porqué** de cada decisión
de diseño — cómo funciona el CLR, el Garbage Collector, los principios SOLID aplicados,
la inyección de dependencias por dentro, y las mejores prácticas profesionales.

> Cada lección sigue **10 secciones**: introducción, teoría en profundidad, ejemplo mínimo,
> ejemplo real, buenas prácticas, errores habituales, ejercicio guiado, reto, resumen y preguntas frecuentes.

---

## 🛤️ Ruta de aprendizaje

| # | Módulo | Estado |
|---|---|---|
| 00 | [Preparación: ¿Qué es .NET? Historia, CLI, estructura](./web/modulos/00-preparacion/01-que-es-dotnet.html) | ✅ Completado |
| 01 | [Fundamentos de C#](./web/modulos/01-fundamentos/) | ✅ Completado |
| 02 | [POO en C# en profundidad](./web/modulos/02-poo/) | ✅ Completado |
| 03 | [SOLID en C#](./web/modulos/03-solid/) | ✅ Completado |
| 04 | [C# Avanzado](./web/modulos/04-csharp-avanzado/) | ✅ Completado |
| 05 | [Inyección de Dependencias](./web/modulos/05-di/) | ✅ Completado |
| 06 | [Arquitectura y organización](./web/modulos/06-arquitectura/) | ✅ Completado |
| 07 | [Entity Framework Core](./web/modulos/07-ef-core/) | ✅ Completado |
| 08 | [ASP.NET Core Web API](./web/modulos/08-web-api/) | ✅ Completado |
| 09 | [Validación y mapeo](./web/modulos/09-validacion-mapeo/) | ✅ Completado |
| 10 | [Middleware y pipelines](./web/modulos/10-middleware/) | ✅ Completado |
| 11 | [Autenticación y autorización](./web/modulos/11-auth/) | ✅ Completado |
| 12 | [Testing](./web/modulos/12-testing/) | ✅ Completado |
| 13 | [Caching y rendimiento](./web/modulos/13-caching/) | ✅ Completado |
| 14 | [Tiempo real (SignalR)](./web/modulos/14-tiempo-real/) | ✅ Completado |
| 15 | [Background Jobs](./web/modulos/15-background-jobs/) | ✅ Completado |
| 16 | [Docker y despliegue](./web/modulos/16-docker/) | ✅ Completado |
| 17 | [Temas avanzados y producción](./web/modulos/17-avanzados/) | ✅ Completado |
| 18 | [Seguridad en APIs (OWASP Top 10)](./web/modulos/18-seguridad/) | ✅ Completado |

---

## 🏗️ El proyecto: CMS Headless

Durante todo el curso construimos un **Sistema de Gestión de Contenidos (CMS)** como API headless,
similar a WordPress pero solo backend. Cada módulo añade funcionalidades nuevas:

| Módulo | Funcionalidad añadida |
|---|---|
| M00 | Setup del proyecto vacío |
| M01 | Modelos de dominio: `Post`, `Category`, `Tag`, `User` |
| M02 | Servicios con interfaces: `IPostService`, `ICategoryService` |
| M03 | Refactor SOLID de servicios |
| M04 | Source Generator de DTOs |
| M05 | Configuración de DI |
| M06 | Clean Architecture + CQRS + MediatR |
| M07 | DbContext + repositorios + migraciones |
| M08 | Endpoints CRUD REST |
| M09 | Validación con FluentValidation |
| M10 | Middleware: logging, correlation ID, rate limiting |
| M11 | JWT + roles: Admin, Editor, Author |
| M12 | Suite completa de tests |
| M13 | Caching con Redis + Output Caching |
| M14 | Notificaciones en tiempo real con SignalR |
| M15 | Jobs programados: publicación automática + emails |
| M16 | Docker Compose + CI/CD |
| M17 | gRPC + OpenTelemetry + Polly + Feature Flags |
| M18 | Auditoría de seguridad OWASP Top 10 |

---

## 🧰 Stack tecnológico

| Categoría | Tecnologías |
|---|---|
| Plataforma | **.NET 8 (LTS)** + **C# 12** |
| API | ASP.NET Core (Minimal APIs y Controller-based) |
| Datos | Entity Framework Core 8 + PostgreSQL (Npgsql) |
| Testing | xUnit · Moq · FluentAssertions · Coverlet · FsCheck |
| Observabilidad | Serilog + OpenTelemetry |
| Arquitectura | Clean Architecture · CQRS · MediatR · Vertical Slices · DDD táctico |
| Infraestructura | Docker / Docker Compose · Redis · SignalR + Redis Backplane |
| Seguridad | JWT / Identity · OWASP Top 10 |
| Otros | FluentValidation · Polly · Hangfire/Quartz · BenchmarkDotNet · Swagger/Scalar |

---

## 📁 Estructura del repositorio

```text
curso-dotnet/
├── AGENTS.md                          ← Guía completa del curso (para la IA)
├── README.md                          ← Este archivo
├── ejemplos/                          ← 73 lecciones con código ejecutable
│   ├── m00-preparacion/
│   │   └── Leccion-01-Que-es-DotNet/
│   │       ├── src/                   ← Código ejecutable
│   │       ├── tests/                 ← Tests unitarios
│   │       └── README.md              ← Lección completa (10 secciones)
│   ├── m01-csharp-fundamentos/
│   └── ...
├── proyectos/                         ← Proyecto CMS incremental
│   ├── m00-cms-setup/
│   └── ...
├── web/                               ← Curso estático (HTML + CSS + JS)
│   ├── modulos/                       ← 00-preparacion … 18-seguridad
│   └── recursos/                      ← Cheatsheet, FAQ, Glosario
└── proyecto-final/                    ← CMS completo al final del curso
    ├── src/                           ← Cms.Domain / Application / Infrastructure / Api
    ├── tests/
    ├── docker-compose.yml
    └── .github/workflows/
```

---

## 🚀 Empezar

### Requisitos

- **Saber programar** en cualquier lenguaje (JavaScript, Python, Java, etc.)
- **.NET 8 SDK** (8.0.x) — [Descargar](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- **Visual Studio Code** con extensiones: C# Dev Kit, NuGet Gallery
- **Docker Desktop** (a partir del Módulo 16)
- **PostgreSQL** local o vía Docker
- **Redis** local o vía Docker

### Instalación rápida

```bash
# Verificar instalación
dotnet --version  # Debe mostrar 8.0.x

# Clonar el repositorio (o abrir la carpeta)
cd curso-dotnet

# Build de todos los proyectos de ejemplo
dotnet build ejemplos/**/*.csproj

# Ejecutar tests
dotnet test ejemplos/**/*.csproj
```

### Ver el curso en tu navegador

```bash
# Abre la web estática del curso
explorer web/index.html
```

---

## 🛠️ Comandos útiles

```bash
# Build de un proyecto específico
dotnet build ejemplos/m00-preparacion/Leccion-01-Que-es-DotNet

# Ejecutar un ejemplo
dotnet run --project ejemplos/m00-preparacion/Leccion-01-Que-es-DotNet/src/IntroduccionDotNet

# Ejecutar tests
dotnet test ejemplos/m00-preparacion/Leccion-01-Que-es-DotNet

# Formatear código
dotnet format

# Verificar formato sin cambios
dotnet format --verify-no-changes
```

---

## ✨ Convenciones

- **Código:** identificadores en inglés. Comentarios en español.
- **C# 12** con file-scoped namespaces, primary constructors, collection expressions.
- **Nullable Reference Types** activados en todos los proyectos.
- **Implicit Usings** activados en todos los proyectos.
- **Todos los ejemplos compilan y se ejecutan.** Si no compila, la lección no está terminada.

---

<div align="center">

**Happy coding!** 🎉

</div>
