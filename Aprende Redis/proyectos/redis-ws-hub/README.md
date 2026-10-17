# redis-ws-hub

Hub de conexiones WebSocket **distribuido sobre Redis** para el curso **Redis para .NET**.
Servidor .NET 8 (Minimal API + WebSockets) con `StackExchange.Redis`: registro de conexiones (hash + TTL),
presencia global (sorted set), broadcast entre nodos (pub/sub) y resiliencia ante caídas.

## Arquitectura

```text
           +----------------------- REDIS -----------------------------+
           |  hub:conn:*  (hash)   hub:presence (zset)  hub:nodes (set) |
           |  hub:events (pub/sub) hub:direct  (pub/sub)                |
           +------------------------------▲-----------------------------+
                    subscribe + publish   │
   +-----------+               +-----------+               +-----------+
   | Nodo A    |               | Nodo B    |    (… más nodos…)           |
   | /ws       |               | /ws       |               | /ws       |
   +-----------+               +-----------+               +-----------+
```

- El socket vive en su nodo (diccionario local); Redis guarda «dónde está».
- `ConnectionRegistry`: hash por conexión + presencia (zset) + nodo vivo.
- `ConnectionHub`: backplane (broadcast `hub:events` / dirigido `hub:direct`) con filtro anti-loop.
- `HubRecoveryHostedService`: al restaurarse Redis, re-registra y re-suscribe.

## Requisitos

- .NET 8 SDK
- Docker (para Redis y RedisInsight)

## Cómo ejecutar

```bash
# 1. Redis + RedisInsight
docker compose up -d
#    RedisInsight en http://localhost:8001

# 2. Nodo 1
dotnet run --project src/RedisWsHub --urls http://localhost:5210

# 3. Nodo 2 (y opcionales), distinto NODE_ID y puerto
NODE_ID=node-2 dotnet run --project src/RedisWsHub --urls http://localhost:5211
```

## Endpoints

| Ruta | Descripción |
|------|-------------|
| `GET /node` | Identidad de este nodo |
| `GET /health/redis` | Estado de Redis (ping ms, hitRatio, clients) |
| `GET /api/online` | Usuarios online (ventana) |
| `WS /ws?userId=42` | WebSocket del hub |

### Mensajes del socket (envelope)

```json
{"type":"broadcast","payload":"hola a todos"}
{"type":"send","to":"<connectionId>","payload":"mensaje privado"}
```

### Probar con 2 nodos

Conecta un cliente WebSocket a cada nodo (por ejemplo con la consola del navegador):

```js
const ws = new WebSocket("ws://localhost:5210/ws?userId=100");
ws.onmessage = (e) => console.log("recibido:", e.data);
```

- **Broadcast**: envía `{"type":"broadcast","payload":"hola"}` desde el nodo-1 → ambos clientes lo reciben.
- **Dirigido**: envía `{"type":"send","to":"<connId>","payload":"privado"}` y solo el dueño responde.
  Con `curl http://localhost:5210/api/online` ves quién está conectado.

## Tests

```bash
dotnet test proyectos/redis-ws-hub/tests/RedisWsHub.Tests
```

Los tests cubren la lógica pura (convención de keys, serialización de DTOs) sin necesidad de Redis.