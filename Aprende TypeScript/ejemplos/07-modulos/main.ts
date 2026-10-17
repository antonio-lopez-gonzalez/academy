// ============================================================
// 07-modulos/main.ts — punto de entrada del demo de modulos
// ============================================================
// Ejecutar con:  node ejemplos/07-modulos/main.ts
import { total, descuento } from "./operaciones.ts";
import type { Producto } from "./modelo.ts";

const carrito: Producto[] = [
  { id: "p1", nombre: "Teclado", precio: 45.0 },
  { id: "p2", nombre: "Raton", precio: 22.5 },
];

// total y descuento vienen de otro modulo: el tipado viaja con ellos.
console.log("Carrito:", carrito);
console.log("Total:", total(carrito));
console.log("Con 10%:", descuento(carrito, 0.1));