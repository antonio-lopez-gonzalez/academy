# Diseño: <Nombre del sistema> (plantilla)

> Copia esta plantilla para tu proyecto final y rellena cada sección. Sin supuestos
> numéricos no hay estimación; sin trade-offs no hay diseño.

## 1. Requisitos

**Funcionales**
- …

**No funcionales** (medibles)
- Latencia: …
- Disponibilidad: …
- Consistencia: …
- Coste: …

**Supuestos y no-objetivos**
- … (qué asumimos explícitamente)
- … (qué queda fuera de alcance)

## 2. Estimación de capacidad

```text
QPS:            …
Almacenamiento: …
Ancho de banda: …
Crecimiento:    …
```

## 3. Diseño de alto nivel

```text
Cliente ─► …  ─► …  ─► …
```

Componentes y responsabilidad de cada uno.

## 4. Profundización

- Datos (motor, esquema, particiones).
- Caché (dónde, qué, invalidación).
- Consistencia y particionado.
- Mensajería/asincronía (si aplica).

## 5. Trade-offs (ADRs)

- ADR 000N — <decisión>: alternativas, decisión, consecuencias.

## 6. Operación

- SLOs y error budget.
- Resiliencia (timeouts, retries, circuit breaker, degradación).
- Observabilidad (métricas/trazas/logs).
- Coste y su control.
