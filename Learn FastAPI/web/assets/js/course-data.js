/**
 * course-data.js — MAPA COMPLETO DEL CURSO FASTAPI
 *
 * Unica fuente de verdad para:
 *  - el sidebar (arbol de modulos y lecciones)
 *  - la navegacion anterior / siguiente
 *  - el indice de la portada
 *  - el calculo de progreso
 *
 * Cada leccion apunta a un archivo HTML real dentro de web/modulos/.
 * Si anades o renombras una leccion, edita SOLO este archivo.
 */
window.COURSE = {
  title: "Curso FastAPI",
  subtitle: "APIs modernas en Python — de un endpoint a produccion",

  resources: [
    { file: "recursos/cheatsheet.html", title: "Cheatsheet de FastAPI" },
    { file: "recursos/glosario.html", title: "Glosario de terminos" },
    { file: "recursos/faq.html", title: "FAQ general del curso" },
  ],

  modules: [
    /* ============================================================ MODULO 0 */
    {
      id: "00",
      dir: "modulos/00-preparacion",
      title: "Preparacion",
      description: "Que es FastAPI, ASGI y uvicorn, el primer endpoint y la estructura del proyecto.",
      lessons: [
        { file: "01-que-es-fastapi.html", title: "Que es FastAPI", minutes: 30 },
        { file: "02-entorno-uvicorn.html", title: "Entorno con uv y uvicorn", minutes: 30 },
        { file: "03-primer-endpoint.html", title: "Tu primer endpoint", minutes: 30 },
        { file: "04-estructura-proyecto.html", title: "Estructura del proyecto", minutes: 35 },
      ],
    },
    /* ============================================================ MODULO 1 */
    {
      id: "01",
      dir: "modulos/01-rutas-y-datos",
      title: "Rutas y datos",
      description: "Path y query params, cuerpo con Pydantic v2, validacion y codigos de estado.",
      lessons: [
        { file: "01-rutas-path-query.html", title: "Rutas: path y query", minutes: 30 },
        { file: "02-body-pydantic.html", title: "Cuerpo con Pydantic", minutes: 35 },
        { file: "03-validacion-y-tipos.html", title: "Validacion y tipos", minutes: 35 },
        { file: "04-status-codes.html", title: "Codigos de estado", minutes: 30 },
      ],
    },
    /* ============================================================ MODULO 2 */
    {
      id: "02",
      dir: "modulos/02-respuestas-errores",
      title: "Respuestas y errores",
      description: "response_model, errores con forma (Problem Details), cabeceras y listados con paginacion.",
      lessons: [
        { file: "01-response-models.html", title: "response_model y serializacion", minutes: 30 },
        { file: "02-errores-problem-details.html", title: "Errores y Problem Details", minutes: 35 },
        { file: "03-headers-cookies.html", title: "Cabeceras y cookies", minutes: 30 },
        { file: "04-paginacion-y-filtros.html", title: "Paginacion y filtros", minutes: 35 },
      ],
    },
    /* ============================================================ MODULO 3 */
    {
      id: "03",
      dir: "modulos/03-dependencias",
      title: "Dependencias y configuracion",
      description: "Depends, dependencias anidadas, lifespan para recursos y settings con pydantic-settings.",
      lessons: [
        { file: "01-depends.html", title: "Depends: el corazon de FastAPI", minutes: 35 },
        { file: "02-subdependencias-y-clases.html", title: "Sub-dependencias y clases", minutes: 30 },
        { file: "03-lifespan-y-recursos.html", title: "Lifespan y recursos", minutes: 35 },
        { file: "04-settings.html", title: "Configuracion y settings", minutes: 30 },
      ],
    },
    /* ============================================================ MODULO 4 */
    {
      id: "04",
      dir: "modulos/04-datos",
      title: "Datos y persistencia",
      description: "SQLModel, sesiones y transacciones, migraciones con Alembic y el patron repositorio.",
      lessons: [
        { file: "01-sqlmodel.html", title: "Modelos con SQLModel", minutes: 35 },
        { file: "02-sesiones-y-transacciones.html", title: "Sesiones y transacciones", minutes: 35 },
        { file: "03-alembic-migraciones.html", title: "Migraciones con Alembic", minutes: 40 },
        { file: "04-repositorios.html", title: "El patron repositorio", minutes: 35 },
      ],
    },
    /* ============================================================ MODULO 5 */
    {
      id: "05",
      dir: "modulos/05-seguridad",
      title: "Seguridad",
      description: "OAuth2 y JWT, roles y permisos, CORS/headers y rate limiting.",
      lessons: [
        { file: "01-oauth2-jwt.html", title: "OAuth2 y JWT", minutes: 40 },
        { file: "02-roles-permisos.html", title: "Roles y permisos", minutes: 35 },
        { file: "03-cors-headers.html", title: "CORS y cabeceras", minutes: 30 },
        { file: "04-rate-limiting.html", title: "Rate limiting", minutes: 30 },
      ],
    },
    /* ============================================================ MODULO 6 */
    {
      id: "06",
      dir: "modulos/06-testing",
      title: "Testing y calidad",
      description: "TestClient y pytest, pruebas async con httpx, tests con base de datos y OpenAPI/calidad.",
      lessons: [
        { file: "01-testclient-pytest.html", title: "TestClient y pytest", minutes: 35 },
        { file: "02-async-httpx.html", title: "Tests async con httpx", minutes: 35 },
        { file: "03-db-tests.html", title: "Tests con base de datos", minutes: 35 },
        { file: "04-openapi-y-calidad.html", title: "OpenAPI, ruff y mypy", minutes: 30 },
      ],
    },
    /* ============================================================ MODULO 7 */
    {
      id: "07",
      dir: "modulos/07-proyecto-final",
      title: "Proyecto final: Copiloto API",
      description: "Arquitectura de la API, endpoints del Copiloto con RAG, persistencia y auth, tests y despliegue.",
      lessons: [
        { file: "01-arquitectura.html", title: "Arquitectura de la API", minutes: 40, project: true },
        { file: "02-endpoints-copiloto.html", title: "Endpoints del Copiloto", minutes: 45, project: true },
        { file: "03-persistencia-y-auth.html", title: "Persistencia y auth", minutes: 45, project: true },
        { file: "04-testing-y-despliegue.html", title: "Testing y despliegue", minutes: 45, project: true },
      ],
    },
  ],
};
