# job-queue — job queue del taller

Proyecto del curso **Aprende RabbitMQ**. Implementa el diseño del módulo 7:

- API HTTP que persiste el job **antes** de publicarlo;
- publicación con **publisher confirms** y mensajes persistentes;
- cola de trabajo **quorum** con `x-delivery-limit`;
- **tres niveles de reintento** con TTL + DLX (`5s`, `1m`, `10m`);
- **DLQ** durable para fallos permanentes o intentos agotados;
- **lease** en el almacén para que un worker muerto no bloquee un job;
- estados `queued`, `running`, `retry_scheduled`, `succeeded`, `failed`;
- reconexión con backoff en API y worker;
- apagado ordenado que espera los handlers en vuelo;
- pruebas unitarias con `node --test`.

## Topología

```text
                       POST /jobs
                            |
                            v
                  +------------------+
                  |  API (Node)      |  guarda job (queued)
                  |  publish confirm |  publica job.run
                  +--------+---------+
                           |
                    taller.jobs (direct, durable)
                           | routing key "job.run"
                           v
                  +---------------------+
                  | taller.jobs.work    |  quorum, durable
                  | prefetch 10, ack    |  x-delivery-limit 5
                  +----+-----------+----+
                       |           |
             éxito     |           |  fallo transitorio
             ack       |           |  publish retry + ack
                       v           v
                 succeeded    taller.jobs.retry (direct)
                                   | "retry.5s" | "retry.1m" | "retry.10m"
                                   v            v             v
                            retry.5s        retry.1m      retry.10m
                            TTL 5s          TTL 1m        TTL 10m
                                   \            |            /
                                    v           v           v
                              dead-letter -> taller.jobs -> job.run

            fallo permanente o intentos agotados
                       |
                       v
                 taller.dlx (direct) --"job.dead"--> taller.jobs.dead (quorum)
                                                        |
                                                        v
                                                 inspección / replay
```

La cola de trabajo tiene `x-delivery-limit: 5` como red de seguridad ante
mensajes que se reentregan sin `ack`. Los reintentos de negocio se hacen con
las colas TTL, no con `nack(requeue=true)`.

## Requisitos

- Node.js 22+
- Docker + Docker Compose

## Arranque rápido

```bash
# 1. Broker (usuario taller-api, vhost taller)
docker compose up -d

# 2. Dependencias
npm install

# 3. En dos terminales
npm run api
npm run worker
```

La API escucha en `http://localhost:3000`. La management UI, en
`http://localhost:15672` (`taller-api` / `taller-api-secret`, vhost `taller`).

## Uso

### Encolar un job

```bash
curl -i -X POST http://localhost:3000/jobs \
  -H "Content-Type: application/json" \
  -H "X-Trace-Id: b3f1c0d2e4a5" \
  -d '{"type":"invoice.generate","payload":{"orderId":"PED-1042","customerId":"CLI-88"}}'
```

Respuesta:

```http
HTTP/1.1 202 Accepted
Location: /jobs/6f1c2a9e-4b7d-4c1e-9f30-2a7d5e8b1c44
Content-Type: application/json
```

```json
{ "id": "6f1c2a9e-…", "status": "queued" }
```

### Consultar el estado

```bash
curl http://localhost:3000/jobs/6f1c2a9e-…
```

Estados: `queued`, `running`, `retry_scheduled`, `succeeded`, `failed`.

### Forzar un fallo transitorio

```bash
curl -X POST http://localhost:3000/jobs \
  -H "Content-Type: application/json" \
  -d '{"type":"demo","payload":{"fail":true}}'
```

El job pasa por `retry_scheduled` (5 s, 1 min y 10 min) y termina en `failed`,
con el mensaje en `taller.jobs.dead`.

### Forzar un fallo permanente

```bash
curl -X POST http://localhost:3000/jobs \
  -H "Content-Type: application/json" \
  -d '{"type":"demo","payload":{"failPermanent":true}}'
```

Va directo a la DLQ sin reintentos.

### Inspeccionar las colas

```bash
docker exec academy-job-queue rabbitmqctl list_queues -p taller name type messages consumers
docker exec academy-job-queue rabbitmqctl list_bindings -p taller
```

## Pruebas

```bash
npm test
```

Las pruebas no necesitan broker: cubren la política pura `planRetry` y el
almacén con lease. La integración con un broker real se describe en la lección
`04-operacion-y-pruebas.html` y se puede añadir como `test/integration.test.js`
usando `RABBITMQ_URL` (si no está definida, se salta).

## Configuración

Copia `.env.example` a `.env` si quieres cambiar algo, o exporta las variables:

| Variable | Default | Uso |
|---|---|---|
| `RABBITMQ_URL` | `amqp://taller-api:taller-api-secret@localhost:5672/taller` | Conexión y vhost |
| `API_PORT` | `3000` | Puerto de la API |
| `PREFETCH` | `10` | Mensajes en vuelo por worker |
| `JOB_LEASE_MS` | `60000` | Lease de reclamación |
| `JOB_STORE` | `./data/jobs.json` | Estado compartido API/worker |

> El proyecto no carga `.env` automáticamente para no añadir dependencias.
> Usa `node --env-file=.env src/api.js` o exporta las variables en tu shell.

## Estructura

```text
job-queue/
├── docker-compose.yml
├── package.json
├── src/
│   ├── config.js         variables y defaults
│   ├── topology.js       exchanges, colas, bindings, TTLs y DLQ
│   ├── retry-policy.js   planRetry puro (testeable sin broker)
│   ├── store.js          estado, claim con lease y transiciones
│   ├── publisher.js      publicación con confirms y reconexión
│   ├── api.js            API HTTP, sweeper y healthchecks
│   └── worker.js         consumo, reintentos, DLQ y apagado ordenado
└── test/
    ├── retry-policy.test.js
    └── store.test.js
```

## Notas para producción

- El almacén JSON es didáctico: usa PostgreSQL y una reclamación atómica
  (`UPDATE … WHERE status IN (…)`) para varios workers.
- Añade una clave de idempotencia al `POST /jobs` si el cliente puede reintentar.
- Configura TLS (`amqps`) y credenciales por entorno; no uses `guest` ni root.
- Alerta sobre la profundidad de la DLQ y de las colas de reintento.
- Si cambias los argumentos de una cola, bórrala antes: son inmutables.
- Prueba el replay de la DLQ de forma manual y filtrada, nunca con un consumidor
  ciego que reenvíe todo.
