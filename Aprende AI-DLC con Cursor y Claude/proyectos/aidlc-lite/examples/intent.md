# Intención: API de inventario

## Contexto

El equipo de operaciones necesita consultar y reservar stock desde una API
REST. Hoy lo hace con una hoja de cálculo compartida que se desactualiza y
provoca ventas de unidades que ya no existen.

## Requisitos

- Listar stock por almacén y producto.
- Reservar unidades durante quince minutos.
- Confirmar o liberar una reserva.
- Devolver errores claros cuando no hay stock.

## Riesgos

- Dos reservas concurrentes podrían vender la misma unidad.
- Los datos de almacén pueden llegar con retraso.
- La API no debe convertirse en la única fuente de verdad del stock.
