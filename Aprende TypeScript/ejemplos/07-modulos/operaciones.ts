// ============================================================
// 07-modulos/operaciones.ts — logica (values) sobre los tipos
// ============================================================
import type { Producto } from "./modelo.ts";

// import type: los tipos se borran al ejecutar, ni rastro en el bundle.
export function total(lista: Producto[]): number {
  return lista.reduce((acum, p) => acum + p.precio, 0);
}

export function descuento(lista: Producto[], pct: number): number {
  const bruto = total(lista);
  return Math.round(bruto * (1 - pct) * 100) / 100;
}