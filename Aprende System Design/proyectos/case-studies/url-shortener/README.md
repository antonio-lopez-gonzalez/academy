# Diseño: URL shortener

> Ejemplo resuelto del método de 6 pasos. Sirve de referencia para el proyecto final.

## 1. Requisitos

**Funcionales**
- Acortar una URL larga → devolver un código corto.
- Redirigir un código corto → URL original.
- (Opcional) estadísticas de clics.

**No funcionales**
- Latencia de redirección p99 < 100 ms.
- Altísima disponibilidad de lectura (99.99 %).
- 100:1 lecturas/escrituras.

**Supuestos (con números)**
- 100 M nuevas URLs/día.
- Retención 5 años.

## 2. Estimación

```text
Escrituras:   100M/día  ≈ 1.160 r/s  (pico ×3 ≈ 3.500 r/s)
Lecturas:     100:1     ≈ 116.000 r/s (pico ×3 ≈ 350.000 r/s)
Almacenamiento: 100M/día · 365 · 5 años ≈ 1.8e11 filas
  ≈ 1.8e11 · ~500 B ≈ 90 TB
Ancho de banda: 350.000 r/s · ~500 B ≈ 175 MB/s (lecturas)
```

## 3. Diseño de alto nivel

```text
        escritura                          lectura
Cliente ──────► API ──► Generador ──► KV store      Cliente ─► Edge/CDN
                          (código)     (code→url)                 │ miss
                                                                 ▼
                                                     API ─► Caché ─► KV store
```

## 4. Profundización

- **Generación de código**: contador distribuido en `base62` (7 chars ≈ 3.5e12 combinaciones) o hash + colisión-check. El contador es más simple y sin colisiones.
- **Almacén**: KV particionado por `code` (hash) → distribución uniforme.
- **Caché**: cache-aside de `code→url` (lecturas 100:1), TTL largo + invalidación al borrar.
- **Redirección**: `302` (permite medir clics) vs `301` (cacheable por el navegador). Este sistema quiere métricas → **302**.

## 5. Trade-offs (ADRs)

- **ADR 0001 — Código por contador base62** (vs hash): sin colisiones, más simple; pierde "aleatoriedad" (codes enumerables) → mitigación: ofuscación opcional.
- **ADR 0002 — 302 temporal** (vs 301): gana analítica; pierde caché en el cliente (más carga). Aceptable por la CDN.

## 6. Operación

- **SLO**: redirección p99 < 100 ms, disponibilidad 99.99 %.
- **Resiliencia**: KV con réplicas por región; caché con single-flight contra stampede.
- **Observabilidad**: hit ratio de caché, p99 de redirección, QPS de escritura.
- **Coste**: almacenamiento dominante (~90 TB); compresión y TTL de estadísticas.
