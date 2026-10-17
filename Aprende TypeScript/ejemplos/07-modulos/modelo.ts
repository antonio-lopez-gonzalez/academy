// ============================================================
// 07-modulos/modelo.ts — tipos compartidos del demo de modulos
// ============================================================
// Este archivo SOLO declara tipos: no genera codigo en runtime.

export interface Producto {
  id: string;
  nombre: string;
  precio: number;
}

export type EstadoStock = "disponible" | "agotado";

// Importar varios tipos a la vez es posible:
//   import type { Producto, EstadoStock } from "./modelo.ts";