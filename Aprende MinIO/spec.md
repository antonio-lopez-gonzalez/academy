# Curso Completo de MinIO

## Almacenamiento de objetos S3: de un bucket a producción

---

# 1. Objetivo

Actúa como un ingeniero de plataforma que diseña almacenamiento de objetos para aplicaciones reales. El objetivo es crear **un curso completo de MinIO**, pensado para quien **ya sabe programar** y quiere dominar el almacenamiento de objetos: no solo subir archivos, sino **modelar datos**, **asegurar el acceso**, **automatizar el ciclo de vida** y **operar el servicio** sin perder datos.

El curso enseña el **por qué**:

- Un objeto no es un archivo en una carpeta: es **datos + metadatos + clave**, y eso cambia cómo se consulta y se versiona.
- S3 es una **API**, no un producto: MinIO la implementa y el mismo código sirve para la nube.
- Las **presigned URLs** delegan la subida sin dar credenciales; son la base de las subidas directas.
- El ciclo de vida y la réplica son **políticas declarativas**, no scripts que alguien recuerda ejecutar.
- La durabilidad no es «tener discos»: es **erasure coding**, réplica y una operación que se prueba.

El hilo conductor es un **almacén de documentos** para un copiloto: subida directa, metadatos, versionado, lifecycle, notificaciones de eventos y pruebas.

> Stack fijado: **MinIO Community (AGPLv3)** con la imagen `quay.io/minio/minio`, **`mc`** y **Node.js 22 + minio-js**. Se enseña la compatibilidad con **AWS SDK v3** para no acoplar la aplicación a un proveedor.

---

# 2. Audiencia y requisitos previos

- **Saber programar** y usar una terminal.
- Node.js 22+ y Docker (Docker Desktop o Docker Engine + Compose).
- No se asume experiencia previa con S3 ni con almacenamiento de objetos. Se agradece `Aprende Docker` y `Aprende Cloud`.
- Ganas de entender seguridad, ciclo de vida y operación, no solo `putObject`.

---

# 3. Hilo conductor: el almacén de documentos

Un almacén que crece con el curso:

1. **Un bucket** `documentos` con prefijos por tenant, año y mes.
2. **Metadatos** de negocio (`tenant`, `content-type`, `checksum`) y subida directa desde el navegador.
3. **Versionado** y protección contra borrados accidentales.
4. **Lifecycle**: mover a almacenamiento frío y expirar temporales.
5. **Eventos**: avisar a un webhook cuando llega un documento (enlace con `Aprende RabbitMQ`).
6. **Réplica** y backup del bucket.
7. **Proyecto final**: el **document store** con API, presigned uploads, metadatos y pruebas.

El proyecto ejecutable vive en `proyectos/document-store/` y se levanta con Docker Compose + Node.js.

> Relación con otros cursos: la notificación de eventos se puede conectar a una cola de RabbitMQ, pero el proyecto de MinIO **no depende** del curso de RabbitMQ para funcionar.

---

# 4. Tecnologías

## Utilizar SIEMPRE

- **MinIO Community** (AGPLv3) con la imagen oficial `quay.io/minio/minio`.
- **Compose de un solo nodo** para desarrollo; el modo distribuido se explica con el **Kubernetes Operator** (MinIO no soporta Compose para clústeres distribuidos).
- **`mc`** para administración y automatización.
- **Node.js 22 LTS**, ESM, `minio-js` y **AWS SDK v3** en la lección de compatibilidad.
- **Presigned URLs** para subidas/descargas directas.
- **Políticas IAM de mínimo privilegio** y credenciales root solo para el arranque.
- **TLS** en cualquier entorno que no sea local.
- **Pruebas** con `node --test`; integración con el servidor levantado por Compose.

## No utilizar

- Credenciales root en la aplicación o en el repositorio.
- Buckets públicos por comodidad.
- `mc` como única vía de configuración: las políticas deben poder versionarse.
- Docker Compose para un clúster distribuido (no está soportado).
- Depender de una consola web para operar en producción.
- Suponer que MinIO es «solo un S3 barato»: se explican sus límites y su licencia.

---

# 5. Forma de enseñar

Cada lección sigue EXACTAMENTE las 10 secciones del contrato Academy:

1. **Introducción** — qué, por qué existe, qué problema resuelve, cuándo se usa, errores típicos.
2. **Teoría** — object storage, S3, consistencia, seguridad, lifecycle; diagramas ASCII.
3. **Ejemplo mínimo** — el comando o código más pequeño posible, explicado línea a línea.
4. **Ejemplo real** — el almacén de documentos o un escenario profesional.
5. **Buenas prácticas** — qué hacer / qué evitar / por qué (tabla cuando aplique).
6. **Errores habituales** — incorrecto, por qué está mal y versión correcta.
7. **Ejercicio guiado** — paso a paso, resuelto, con resultado esperado.
8. **Reto** — reto adicional **sin resolver** (solo pista).
9. **Resumen** — bullets + conceptos clave.
10. **Preguntas frecuentes** — FAQ (3–5 `<details>`).

## Reglas de contenido

- Distingue siempre **bucket**, **objeto** y **prefijo**; un prefijo no es una carpeta.
- Toda operación sensible (borrado, política, lifecycle) se explica con su **reversibilidad**.
- Los ejemplos usan variables de entorno y `.env.example`; nunca credenciales reales.
- La compatibilidad S3 se demuestra con AWS SDK cuando aporta valor.
- El reto nunca se resuelve en el HTML.

---

# 6. Multiplataforma

- **Windows:** Docker Desktop + PowerShell. Shell canónico: **bash**, con callout cuando PowerShell cambie variables o rutas.
- **macOS / Linux:** Docker Engine + Compose.
- `mc` se instala con el binario oficial o se ejecuta en un contenedor; se documentan ambas vías.
- Puertos por defecto: `9000` (API S3) y `9001` (consola). Si están ocupados, se documenta cómo cambiarlos.

---

# 7. Índice del curso

## Módulo 0 — Fundamentos
- **01-que-es-object-storage.html** — Objeto vs archivo vs bloque; cuándo usar object storage.
- **02-s3-api-y-minio.html** — La API S3, compatibilidad y el modelo de MinIO.
- **03-instalacion-y-console.html** — Docker, consola web y credenciales de arranque.
- **04-mc-y-primer-bucket.html** — `mc alias`, `mb`, `cp`, `ls` y el primer objeto.

## Módulo 1 — Objetos y buckets
- **01-buckets-y-prefijos.html** — Nombres, regiones y organización por prefijos.
- **02-put-get-y-metadatos.html** — Objetos, content-type, metadatos de usuario y ETag.
- **03-multipart-y-streaming.html** — Objetos grandes, multipart y streaming.
- **04-versionado-y-object-locking.html** — Versiones, borrados y WORM.

## Módulo 2 — SDK y aplicación
- **01-cliente-node.html** — Cliente `minio-js`, configuración y operaciones básicas.
- **02-subidas-directas.html** — Presigned PUT/POST y subida desde el navegador.
- **03-errores-reintentos-y-checksums.html** — Errores S3, reintentos y verificación de integridad.
- **04-descargas-y-transformaciones.html** — Streams, rangos, thumbnails y procesado.

## Módulo 3 — Seguridad
- **01-credenciales-root.html** — Root solo para arranque; rotación y buenas prácticas.
- **02-usuarios-y-politicas.html** — Usuarios, grupos, políticas y mínimo privilegio.
- **03-presigned-urls.html** — Firma, expiración, alcance y riesgos.
- **04-tls-y-cifrado.html** — TLS, cifrado en reposo y gestión de claves.

## Módulo 4 — Ciclo de vida
- **01-lifecycle-e-ilm.html** — Reglas de lifecycle, transiciones y expiración.
- **02-replicacion.html** — Réplica de bucket y entre sitios.
- **03-notificaciones-eventos.html** — Eventos a webhook/cola; enlace con RabbitMQ.
- **04-backups-y-retencion.html** — Copias, restauración y retención.

## Módulo 5 — Operación
- **01-erasure-coding.html** — Códigos de borrado, paridad y tolerancia a fallos.
- **02-modo-distribuido.html** — Clúster, pools y requisitos reales.
- **03-monitorizacion-y-alertas.html** — Métricas, health y alertas.
- **04-capacidad-y-actualizaciones.html** — Capacidad, crecimiento y upgrades sin sorpresas.

## Módulo 6 — Integración
- **01-s3-compatible-aws-sdk.html** — AWS SDK v3 contra MinIO; portabilidad.
- **02-kubernetes-operator.html** — Operator, tenants y despliegue declarativo.
- **03-procesado-y-antivirus.html** — Escaneo, cuarentena y pipelines de procesado.
- **04-cdn-y-cache.html** — CDN, caché y entrega de objetos.

## Módulo 7 — Proyecto final
- **01-arquitectura-document-store.html** — Diseño del almacén y modelo de claves. *(project)*
- **02-api-y-presigned-uploads.html** — API, presigned uploads y validación. *(project)*
- **03-metadatos-versionado-y-lifecycle.html** — Metadatos, versiones y políticas. *(project)*
- **04-eventos-y-pruebas.html** — Eventos, webhook y pruebas. *(project)*

## Recursos
- **cheatsheet.html** — `mc`, SDK y políticas por intención.
- **glosario.html** — Bucket, objeto, prefijo, ETag, presigned, ILM, erasure coding…
- **faq.html** — FAQ general del curso.

**Total: 8 módulos (0-7), 32 lecciones + 3 recursos.**

---

# 8. Estructura

```text
Aprende MinIO/
├── spec.md
├── AGENTS.md
├── package.json / pnpm-workspace.yaml
├── .gitattributes
├── web/
│   ├── index.html
│   ├── assets/{css,js,fonts,favicon.svg}
│   ├── modulos/00-fundamentos/ … 07-proyecto-final/
│   ├── recursos/{cheatsheet,glosario,faq}.html
│   └── descargas/Aprende-MinIO.zip
├── scripts/serve-web.mjs
└── proyectos/document-store/  ← API + presigned uploads + Compose
```

---

# 9. Formato de entrega

Web estática sin build. Claves: `minio-theme`, `minio-progress`. `SITE_BASE` según el contrato (`./`, `../`, `../../`).

- Bloques de código: `language-javascript` (Node/SDK), `language-bash` (`mc`, Docker), `language-json`, `language-yaml`.
- Salidas de `mc` y diagramas de buckets: `language-text`.
- El proyecto usa Node.js ESM y `node --test`; no requiere build.

---

# 10. Diseño y marca

- Paleta propia: **rojo MinIO** (`#C72C48`) con acento **menta de objeto** (`#3DDC97`) sobre fondo oscuro.
- Firma de portada: una consola `mc` que sube un PDF, lo lista y genera una **presigned URL**.
- Logo del topbar y favicon: **icono oficial de MinIO** (Simple Icons).
- Tema oscuro por defecto, toggle a claro, `prefers-reduced-motion` respetado.

---

# 11. Convenciones

1. Contenido, comentarios y UI en español; identificadores y comandos en inglés.
2. Bucket, objeto y prefijo se usan con precisión; «carpeta» solo como analogía explícita.
3. Toda operación destructiva se explica con su reversibilidad (versionado, object locking, réplica).
4. Proyecto conductor: `proyectos/document-store/`.
5. Cada lección usa SIEMPRE las 10 secciones; el reto no se resuelve.
6. Sin secretos en el repositorio: `.env.example` y variables de entorno.

---

# 12. Documentación actualizada

Consultar la documentación oficial de MinIO, `mc`, `minio-js` y AWS S3 vía Context7 antes de afirmar APIs, flags o comportamientos. Preferir la versión estable más reciente. Señalar explícitamente que la edición Community es AGPLv3 y que el despliegue distribuido soportado es Kubernetes/baremetal, no Docker Compose.

---

# 13. Página principal (`web/index.html`)

Hero con eyebrow, firma animada, lead, botones (empezar, índice, cheatsheet, descarga del ZIP), estadísticas calculadas por `site.js`, callouts de «cómo funciona» e `<div id="module-list">`.

---

# 14. Checklist de implementación

## Fase 0 — Spec y AGENTS ✅
- [x] `spec.md` (este documento).
- [x] `AGENTS.md` (puntero corto).

## Fase 1 — Scaffold ✅
- [x] `package.json`, `pnpm-workspace.yaml`, `.gitattributes`, `scripts/serve-web.mjs`.
- [x] Assets re-tematizados, `course-data.js` completo, favicon oficial.
- [x] Portada `web/index.html` + firma.

## Fase 2 — Lecciones ✅
- [x] 00-fundamentos … 07-proyecto-final (32 lecciones).

## Fase 3 — Recursos ✅
- [x] `web/recursos/{cheatsheet,glosario,faq}.html`.

## Fase 4 — Proyecto document-store ✅
- [x] `proyectos/document-store/` con Compose, API, presigned uploads, metadatos, lifecycle y tests.

## Fase 5 — Academy ✅
- [x] Tarjeta, contadores, filas en `specs.md` y `README.md`, ZIP.

---

# 15. Reglas importantes

1. No des por sabido object storage: explica objeto, bucket y prefijo desde cero.
2. Toda operación destructiva se acompaña de su mecanismo de recuperación.
3. Root solo para arrancar; aplicación con usuario de mínimo privilegio.
4. Las presigned URLs tienen alcance y expiración; nunca se generan con credenciales root.
5. Lifecycle, réplica y políticas se declaran y se versionan.
6. Sin secretos en el repositorio; `.env.example` siempre.
7. No presentes Compose como modo distribuido soportado.
8. Si un concepto depende de otro, enlaza la lección correspondiente.
9. Prioriza comprensión y operación frente a memorizar la consola web.
10. El proyecto debe arrancar con un solo `docker compose up` y pasar sus pruebas.
