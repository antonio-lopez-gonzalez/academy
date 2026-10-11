---
title: "Islands: menos JavaScript, más velocidad"
description: "Por qué Astro envía cero JavaScript por defecto y cuándo hidratar."
pubDate: 2026-02-02
tags: ["islands", "rendimiento"]
---

Astro genera HTML y **no hidrata nada** salvo que se lo pidas con `client:*`.

## Regla práctica

- Estático por defecto.
- `client:visible` para lo que está por debajo del pliegue.
- `client:load` solo para lo crítico e interactivo.

Ese es el corazón de la arquitectura **islands**.
