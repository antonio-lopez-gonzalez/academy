# Curso Completo de FastAPI

## APIs modernas en Python — de un endpoint a una API en producción

---

# 1. Objetivo

Actúa como un ingeniero senior de backend Python con años construyendo APIs en producción. El objetivo es crear **un curso completo de FastAPI**, pensado para un **desarrollador que ya programa** (sobre todo Python, pero también .NET/Node) y quiere dominar el framework de APIs más usado del ecosistema Python: de `@app.get("/")` a una **API real con base de datos, autenticación, tests y despliegue**.

El curso es extremadamente didáctico y orientado al **por qué**:

- **cómo funciona** FastAPI por dentro (ASGI, Pydantic, inyección de dependencias, validación);
- **qué problema resuelve** cada pieza (modelos, dependencias, `lifespan`, alembic, OAuth2);
- **cómo se escribe una API** que sea correcta, testeable y mantenible (repositorios, errores consistentes, OpenAPI);
- **cómo se lleva a producción** (CORS, rate limiting, logging, contenedores).

El hilo conductor es **Copiloto API**: el asistente con RAG del curso `Learn IA`, expuesto ahora como una API HTTP real.

---

# 2. Audiencia y requisitos previos

- **Saber programar** en Python (el curso usa Python 3.12+ y `uv`). No se asume FastAPI previo.
- Ganas de tocar la terminal y Git. Ganas de conocer Pydantic, ASGI y `async/await`.
- Opcional: Docker para el módulo de despliegue.

---

# 3. Hilo conductor: Copiloto API

**Copiloto API** — la API del Copiloto (curso `Learn IA`):

1. Expone endpoints (`GET /health`, `GET /modules`, `POST /ask`).
2. Valida todo con **Pydantic** y devuelve **Problem Details** en los errores.
3. Usa **dependencias** para la configuración y el "motor" de RAG.
4. Persiste un histórico de consultas en **SQLite/SQLModel**.
5. Protege los endpoints con **JWT** y **roles**.
6. Se **testea** con `TestClient`/`httpx` y se **despliega** con Docker.

Evolución:

- Módulos 0-3: se construye la API: rutas, validación, respuestas, dependencias.
- Módulo 4: se añade persistencia (SQLModel + Alembic).
- Módulo 5: seguridad (OAuth2/JWT, roles, CORS, rate limiting).
- Módulos 6-7: testing, docs y proyecto final end-to-end + despliegue.

---

# 4. Tecnologías

## Utilizar SIEMPRE

- **Python 3.12+** con **uv** y `pyproject.toml`.
- **FastAPI** + **Uvicorn** (ASGI) y **Pydantic v2** (modelos y settings).
- **SQLModel** (SQLAlchemy 2.0 + Pydantic) y **Alembic** (migraciones).
- **python-jose** o **PyJWT** + `passlib[bcrypt]` para auth (JWT).
- **pytest** + `httpx`/`TestClient` para tests.
- **ruff** + **mypy** para calidad.
- Logging estructurado y `openapi` como contrato.

## No utilizar

- Frameworks "todo en uno" tipo Django como sustituto (se menciona como comparación).
- `requests` dentro de endpoints async (usar `httpx`/clientes async).
- ORMs "mágicos" sin entender el SQL que generan (se explica el SQL).
- Claves de API o secretos en el repositorio (variables de entorno / `.env`).

---

# 5. Forma de enseñar

Cada lección sigue EXACTAMENTE esta estructura.

## 1. Introducción
Qué vamos a aprender. Por qué existe. Qué problema resuelve. Cuándo se usa. Errores típicos.

## 2. Teoría
Con mucho detalle, sin asumir FastAPI previo. Comparaciones del mundo real (una API como un mostrador de atención; Pydantic como un formulario con validación). Explicar el **por qué** de cada decisión. Diagramas ASCII (petición HTTP → middleware → ruta → dependencia → respuesta).

## 3. Ejemplo mínimo
El más pequeño posible, explicado línea por línea. Ejecutable con `uv run`.

## 4. Ejemplo real
Aplicado a **Copiloto API** o a un escenario profesional. Explicar decisiones.

## 5. Buenas prácticas
Qué hacer, qué evitar, por qué. Tabla cuando aplique.

## 6. Errores habituales
Ejemplos incorrectos, por qué están mal, versión correcta.

## 7. Ejercicio guiado
Paso a paso, resuelto al detalle, con resultado esperado.

## 8. Reto
Reto adicional **sin resolver** (solo pista).

## 9. Resumen
Bullets + conceptos clave. Tabla "concepto → código/flag" cuando aplique.

## 10. Preguntas frecuentes
FAQ (3-5 `<details>`).

## Reglas de contenido

- Separar **sync vs async** con criterio (y explicar cuándo cada uno).
- Toda entrada se **valida**; toda salida se **modela**.
- Los errores tienen **forma consistente** (Problem Details).
- Mostrar el **contrato** (OpenAPI) como fuente de verdad del API.
- Nada de secretos en el repo.

---

# 6. Multiplataforma: Linux / macOS / Windows

Python, `uv` y uvicorn son multiplataforma. Shell canónico: **bash** (Linux/macOS/WSL2), con callouts de **PowerShell** (activación de entorno, variables) cuando cambie.

---

# 7. Índice del curso

## Módulo 0 — Preparación
- **01-que-es-fastapi.html** — Qué es FastAPI, ASGI y en qué se diferencia de Flask/Django.
- **02-entorno-uvicorn.html** — Entorno con `uv`; uvicorn y `--reload`; primer arranque.
- **03-primer-endpoint.html** — `@app.get`, la documentación automática (`/docs`) y por qué existe.
- **04-estructura-proyecto.html** — Estructura por capas, `pyproject.toml`, configuración base.

## Módulo 1 — Rutas y datos
- **01-rutas-path-query.html** — Path params, query params, tipado y validación.
- **02-body-pydantic.html** — Cuerpo de la petición con modelos Pydantic v2.
- **03-validacion-y-tipos.html** — Tipos, Field, validadores, constraints y errores 422.
- **04-status-codes.html** — Códigos de estado correctos y `status_code`/`responses`.

## Módulo 2 — Respuestas y errores
- **01-response-models.html** — `response_model`, filtrado y serialización segura.
- **02-errores-problem-details.html** — `HTTPException` y errores con forma (RFC 9457).
- **03-headers-cookies.html** — Cabeceras de respuesta, cookies y CORS básico.
- **04-paginacion-y-filtros.html** — Listados: paginación, orden y filtros.

## Módulo 3 — Dependencias y configuración
- **01-depends.html** — `Depends`: el corazón de FastAPI y por qué reemplaza al service locator.
- **02-subdependencias-y-clases.html** — Dependencias anidadas, clases y `yield`.
- **03-lifespan-y-recursos.html** — `lifespan`: abrir/cerrar recursos (motor, cliente) una vez.
- **04-settings.html** — Configuración con `pydantic-settings` y entornos.

## Módulo 4 — Datos y persistencia
- **01-sqlmodel.html** — Modelos de tabla con SQLModel y sesión.
- **02-sesiones-y-transacciones.html** — Dependencia de sesión, transacciones y N+1.
- **03-alembic-migraciones.html** — Migraciones con Alembic (init, revision, upgrade).
- **04-repositorios.html** — Patrón repositorio y separación de capas.

## Módulo 5 — Seguridad
- **01-oauth2-jwt.html** — OAuth2 password flow y JWT: login y token.
- **02-roles-permisos.html** — Dependencias de autorización, roles y scopes.
- **03-cors-headers.html** — CORS a fondo y cabeceras de seguridad.
- **04-rate-limiting.html** — Rate limiting y protecciones básicas.

## Módulo 6 — Testing y calidad
- **01-testclient-pytest.html** — `TestClient` y tests de endpoints.
- **02-async-httpx.html** — Tests async con `httpx.AsyncClient` y fixtures.
- **03-db-tests.html** — Tests con base de datos temporal y aislamiento.
- **04-openapi-y-calidad.html** — OpenAPI como contrato, ruff, mypy y cobertura.

## Módulo 7 — Proyecto final: Copiloto API
- **01-arquitectura.html** — Arquitectura de la API: capas, DI, configuración. *(project)*
- **02-endpoints-copiloto.html** — Endpoints del Copiloto (health, modules, ask) con RAG. *(project)*
- **03-persistencia-y-auth.html** — Histórico en BD y endpoints protegidos con JWT. *(project)*
- **04-testing-y-despliegue.html** — Tests, Dockerfile y puesta en producción. *(project)*

## Recursos
- **cheatsheet.html** — Decoradores, `Depends`, Pydantic, códigos de estado, comandos `uv`/alembic.
- **glosario.html** — ASGI, uvicorn, Pydantic, DI, lifespan, SQLModel, Alembic, JWT…
- **faq.html** — FAQ general del curso.

**Total: 8 módulos (0-7), 32 lecciones + 3 recursos.**

---

# 8. Estructura de repositorio

```text
Learn FastAPI/
├── spec.md                      ← ★ ESTE DOCUMENTO (fuente de verdad del curso)
├── AGENTS.md                    ← puntero corto a spec.md + reglas para agentes
├── package.json                 ← script serve
├── pnpm-workspace.yaml
├── .gitignore
├── .gitattributes               ← LF
│   (opencode.json y las skills viven en la raíz de la Academy)
├── web/                         ← ★ EL CURSO (web estática, sin build)
│   ├── index.html               ← portada + índice
│   ├── assets/{css,js,fonts,favicon.svg}
│   ├── modulos/00-preparacion/ … 07-proyecto-final/
│   └── recursos/                ← cheatsheet, glosario, faq
├── scripts/serve-web.mjs
└── proyectos/copiloto-api/      ← API real (FastAPI + SQLModel + auth + tests)
    ├── pyproject.toml
    ├── README.md
    └── app/{main,config,models,schemas,deps,routers/}.py
```

---

# 9. Formato de entrega

Web estática (HTML + CSS + JavaScript vanilla, sin Markdown ni build). `web/` contiene el curso navegable; el chrome lo inyecta `web/assets/js/site.js` desde `web/assets/js/course-data.js` (unica fuente de verdad). El proyecto `proyectos/copiloto-api/` es Python real y ejecutable con `uv`.

## Anatomia de una lección

Identica a la familia Academy (`data-module`/`data-lesson`, 10 secciones con ids exactos, `SITE_BASE`):

- `SITE_BASE` = `'../../'` en lecciones, `'./'` en portada, `'../'` en `web/recursos/`.
- Claves de localStorage: `fapi-theme`, `fapi-progress`.
- Bloques: `callout`, `challenge`, `faq`, `table`, `ol.steps`, `pre > code.language-*`.

---

# 10. Diseño y marca

- Tema oscuro por defecto, toggle a claro.
- Paleta propia: **teal FastAPI** (`#009688`) con acento verde (`#3ddc97`) sobre fondo profundo.
- Firma de portada: un **intercambio HTTP** que se escribe solo (petición → respuesta JSON).
- Identificadores en inglés; contenido, comentarios y UI en español.

---

# 11. Convenciones del curso

1. Idioma: contenido, comentarios y UI en español; identificadores/comandos en inglés.
2. Python 3.12+ y `uv` en todos los comandos.
3. Proyecto conductor: `proyectos/copiloto-api/`, paquete `app/`.
4. Cada lección usa SIEMPRE las 10 secciones.
5. Retos sin resolver en el HTML (solo pista).

---

# 12. Documentación actualizada

Consultar la documentación oficial (FastAPI, Pydantic v2, SQLModel, SQLAlchemy 2.0, Alembic, uvicorn) vía Context7 antes de afirmar APIs, flags o versiones. Preferir la versión estable más reciente.

---

# 13. Página principal (`web/index.html`)

Portada con hero, firma (intercambio HTTP que se escribe), estadísticas y `<div id="module-list">` alimentado por `course-data.js`.

---

# 14. Checklist de implementación

## Fase 0 — Spec y AGENTS ✅
- [x] `spec.md` (este documento).
- [x] `AGENTS.md` (puntero corto). Skills centralizadas en `Academy/.agents/skills/`.

## Fase 1 — Scaffold ✅
- [x] `package.json` (script `serve`), `pnpm-workspace.yaml`, `.gitignore`, `.gitattributes` (LF).
- [x] `scripts/serve-web.mjs`.
- [x] Assets re-tematizados (teal) y `course-data.js` completo.
- [x] Portada `web/index.html` + firma.

## Fase 2 — Lecciones (web/modulos/) ✅
- [x] 00-preparacion … 07-proyecto-final (32 lecciones).

## Fase 3 — Recursos ✅
- [x] `web/recursos/{cheatsheet,glosario,faq}.html`.

## Fase 4 — Proyecto Copiloto API (ejecutable) ✅
- [x] `proyectos/copiloto-api/` con `uv`, endpoints + auth + tests y README.

## Fase 5 — Academy ✅
- [x] Tarjeta en `Academy/index.html` + contador + fila en `specs.md` §3 y `README.md`.

---

# 15. Reglas importantes

1. Nunca des por sentado un concepto: ASGI, validación y DI se explican desde cero.
2. Explica siempre el "por qué" y la diferencia **sync vs async**.
3. Todo el código debe ejecutarse con Python 3.12+ y `uv`.
4. Valida la entrada, modela la salida, y da forma consistente a los errores.
5. El contrato OpenAPI es la fuente de verdad del API.
6. Sigue las buenas prácticas actuales de FastAPI y Pydantic v2.
7. No omitas pasos de configuración (uv, alembic, variables de entorno).
8. Si un concepto depende de otro, enlaza la lección correspondiente.
9. Prioriza la comprensión profunda frente a la velocidad.
10. No conviertas el curso en un tutorial de un único proveedor de base de datos.
