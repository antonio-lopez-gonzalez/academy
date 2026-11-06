# document-store — almacén de documentos con MinIO

Proyecto del curso **Aprende MinIO**. Implementa un almacén de documentos con:

- bucket versionado y **lifecycle** para temporales y versiones antiguas;
- **presigned uploads**: el navegador sube directo a MinIO sin credenciales;
- **catálogo** de metadatos (simplificado en JSON para el curso);
- validación de tenant, nombre, content-type y tamaño;
- descarga mediante **presigned GET**;
- **delete marker** al borrar (recuperable gracias al versionado);
- webhook de evento al completar una subida;
- receptor de ejemplo para notificaciones reales de MinIO;
- pruebas unitarias de claves y catálogo.

## Requisitos

- Node.js 22+
- Docker + Docker Compose

## Arranque rápido

```bash
# 1. Servidor MinIO
docker compose up -d

# 2. Dependencias
npm install

# 3. API (crea el bucket, versionado y lifecycle al arrancar)
npm run api
```

La API escucha en `http://localhost:7072`. La API S3 de MinIO, en
`http://localhost:9000`; la consola, en `http://localhost:9001`
(`minioadmin` / `minioadmin123`).

> Cambia las credenciales root en `docker-compose.yml` antes de usar esto fuera
> de tu máquina. La aplicación, en un entorno real, no debe usar root.

## Flujo de subida

### 1. Crear el documento y pedir una URL firmada

```bash
curl -X POST http://localhost:7072/documents \
  -H "Content-Type: application/json" \
  -d '{"tenant":"acme","filename":"informe.pdf","contentType":"application/pdf","size":204800}'
```

Respuesta:

```json
{
  "id": "7f2c…",
  "key": "acme/2026/11/7f2c…-informe.pdf",
  "uploadUrl": "http://127.0.0.1:9000/documentos/…?X-Amz-…",
  "expiresIn": 900
}
```

### 2. Subir el archivo directamente a MinIO

```bash
curl -X PUT "<uploadUrl>" \
  -H "Content-Type: application/pdf" \
  --data-binary "@informe.pdf"
```

La URL caduca en 15 minutos y solo permite `PUT` sobre esa clave.

### 3. Confirmar y registrar metadatos

```bash
curl -X POST http://localhost:7072/documents/<id>/complete
```

La API hace `statObject`, comprueba el tamaño real y guarda `etag` y `size`. Si
`WEBHOOK_URL` está definida, envía un evento `document.stored`.

### 4. Descargar

```bash
curl http://localhost:7072/documents/<id>
```

Devuelve la ficha y una `downloadUrl` firmada.

### 5. Borrar (recuperable)

```bash
curl -X DELETE http://localhost:7072/documents/<id>
```

Con versionado activo, MinIO crea un **delete marker**. La versión anterior sigue
en el bucket y se puede recuperar desde `mc` o la consola.

## Notificaciones reales de MinIO

El endpoint `POST /webhooks/minio` recibe eventos de MinIO. Para conectar un
destino webhook, MinIO debe arrancar con las variables
`MINIO_NOTIFY_WEBHOOK_ENABLE_*` y el endpoint correspondiente; después:

```bash
mc event add local/documentos arn:minio:sqs::primary:webhook --event put --suffix ".pdf"
mc event list local/documentos
```

La API de este proyecto también emite un webhook de aplicación al completar la
subida, para que el flujo se pueda probar sin configurar destinos de MinIO.

## Pruebas

```bash
npm test
```

No necesitan MinIO: cubren la generación de claves, la lista blanca de tipos y el
catálogo. La prueba de integración se salta si no defines `BASE_URL`; con el
servidor levantado:

```bash
BASE_URL=http://localhost:7072 node --test test/integration.test.mjs
```

## Configuración

| Variable | Default | Uso |
|---|---|---|
| `MINIO_ENDPOINT` | `127.0.0.1` | Host del servidor |
| `MINIO_PORT` | `9000` | Puerto S3 |
| `MINIO_USE_SSL` | `false` | TLS |
| `MINIO_ACCESS_KEY` | `minioadmin` | Credencial (solo desarrollo) |
| `MINIO_SECRET_KEY` | `minioadmin123` | Secreto (solo desarrollo) |
| `MINIO_BUCKET` | `documentos` | Bucket del almacén |
| `API_PORT` | `7072` | Puerto de la API |
| `MAX_UPLOAD_BYTES` | `26214400` | Límite de 25 MiB |
| `PRESIGN_EXPIRY_SECONDS` | `900` | Caducidad de las URLs |
| `WEBHOOK_URL` | vacío | Webhook de aplicación (`document.stored`) |
| `MINIO_WEBHOOK_TOKEN` | `dev-token` | Token que debe enviar MinIO en `Authorization` |
| `CATALOG_PATH` | `./data/catalog.json` | Catálogo de metadatos |

## Estructura

```text
document-store/
├── docker-compose.yml
├── package.json
├── src/
│   ├── config.mjs     variables y defaults
│   ├── keys.mjs       validación y generación de claves (testeable)
│   ├── catalog.mjs    catálogo de documentos en JSON
│   ├── storage.mjs    SDK de MinIO, bucket, versionado y lifecycle
│   ├── bootstrap.mjs  prepara el bucket y muestra su estado
│   └── api.mjs        API HTTP y webhooks
└── test/
    ├── keys.test.mjs
    └── catalog.test.mjs
```

## Notas para producción

- El catálogo JSON es didáctico: usa PostgreSQL (u otro) para consultas reales.
- No uses credenciales root en la aplicación: crea un usuario con política
  acotada al prefijo del tenant.
- Activa TLS y sirve la API detrás de HTTPS.
- Acorta la expiración de las presigned URLs y no las registres en logs.
- Prueba la restauración de versiones y el lifecycle; no solo la subida.
- Para un clúster distribuido, usa Kubernetes Operator o baremetal, no Compose.
