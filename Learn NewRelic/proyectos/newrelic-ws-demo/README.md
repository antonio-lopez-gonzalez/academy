# newrelic-ws-demo

Aplicación de ejemplo para el curso **New Relic para .NET**: un servidor
**WebSocket** + **Minimal API** en .NET 8, instrumentado con el agente de
New Relic (API `NewRelic.Agent.Api`) y configurable sin licencia.

## Qué hace

- `GET /api/status` — estado + atributo `version`.
- `POST /api/broadcast` — difusión simulada + métrica `Custom/BroadcastMs`.
- `GET /healthz` — health check ignorado por el agente.
- `GET /agent` — diagnóstico de configuración (licencia/app configuradas + hint; no usa null-check: la API de New Relic devuelve handle no-op sin profiler).
- `WS /ws` — recibe mensajes JSON con `type` / `timestamp` / `messageId` /
  contexto W3C (`traceparent`/`tracestate`) / `payload`.

Cada mensaje se convierte en una **transacción** (OtherTransaction) con:

- atributos de negocio (`messageType`, `messageId`, `queueLatencyMs`),
- aceptación del contexto W3C (`AcceptDistributedTraceHeaders`),
- spans de fases `[Trace]` (Validate / Apply; ping),
- errores esperados vs. no esperados (`NoticeError`),
- métricas custom (`Custom/Messages/*`, `Custom/QueueLatencyMs`, `Custom/TradeProcessingMs`),
- un custom event de negocio (`OrderPlaced`),
- logs en contexto via `ILogger` (el agente añade `trace.id`/`span.id`).

## Cómo ejecutar

### Sin New Relic (todo funciona; la API es no-op)

```bash
dotnet run --project src/NewRelicWsDemo
```

Prueba rápida:

```bash
curl http://localhost:5000/api/status
# con WebSocket: conecta a ws://localhost:5000/ws
#  {"type":"ping","timestamp":"2026-08-31T10:00:00Z","messageId":"1"}
#  {"type":"trade","timestamp":"2026-08-31T10:15:00Z","messageId":"887",
#   "traceparent":"00-<32hex>-<16hex>-01","payload":{"symbol":"EURUSDD","amount":1000}}
```

### Con New Relic APM

El proyecto incluye `NewRelic.Agent`, `newrelic.config` y
`Properties/launchSettings.json` (perfil `NewRelicWsDemo` con `CORECLR_*` +
`NEW_RELIC_*`).

**Arranque recomendado** (calcula rutas del profiler y lee la licencia de
`appsettings.json`):

```powershell
pwsh ./scripts/run-with-agent.ps1
```

O con el perfil de Visual Studio / `dotnet run`:

```bash
dotnet run --project src/NewRelicWsDemo --launch-profile NewRelicWsDemo
```

Comprueba el adjunto:

```bash
curl http://localhost:5000/agent
# profilingEnabled=true  → el profiler está cargado
```

Logs del agente: `bin/Debug/net8.0/newrelic/logs/stdout`. Busca `Connected`.
Si ves `401 Unauthorized`, la license key no es válida (o no corresponde a la
región EU): renueva la **Ingest - License** en New Relic (EU) y actualízala en
`appsettings.json`, `newrelic.config` y `Properties/launchSettings.json`.

En ~1-2 minutos (con clave válida) aparece **APM & services** →
`NotificadorWebSocket`.

## Tests

```bash
dotnet test proyectos/newrelic-ws-demo/tests/NewRelicWsDemo.Tests
```

## Estructura

```text
src/NewRelicWsDemo/
├── Program.cs                       # endpoint + config del agente por env
├── WsServer.cs                      # transporte del socket (espera fuera de txn)
├── Messages/MessageEnvelope.cs      # envelope JSON + parse
├── Handlers/
│   ├── MessageDispatcher.cs         # [Transaction] por mensaje + W3C + errores
│   ├── TradeProcessor.cs            # [Trace] Validate/Apply + OrderPlaced
│   └── PingProcessor.cs             # [Trace] ping + métrica
├── Services/PriceSimulator.cs       # simula llamada externa con latencia
└── newrelic.config.example          # config XML de referencia (no usada por defecto)
```