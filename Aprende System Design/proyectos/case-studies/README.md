# Cuaderno de arquitectura (System Design)

Documentos de **diseño de sistemas** del curso [`Aprende System Design`](../../spec.md).
No es código: son diseños con **requisitos, estimaciones, diagramas y trade-offs**, versionados.

## Contenido

```text
case-studies/
├── url-shortener/         ← diseño de ejemplo completo
│   └── README.md
└── diseno-final/          ← plantilla para tu proyecto final
    ├── README.md          ← plantilla de diseño
    └── adr/
        └── 0001-plantilla.md
```

## Método (6 pasos)

1. **Requisitos** — funcionales y no funcionales; supuestos y no-objetivos.
2. **Estimación** — QPS, almacenamiento, ancho de banda (con supuestos).
3. **Diseño de alto nivel** — componentes y flujos (diagrama ASCII).
4. **Profundización** — datos, caché, particionado, consistencia.
5. **Trade-offs** — qué se gana y qué se pierde (ADRs).
6. **Operación** — SLOs, resiliencia, observabilidad, coste.

Cada decisión relevante se documenta en un **ADR** (`adr/NNNN-titulo.md`).
