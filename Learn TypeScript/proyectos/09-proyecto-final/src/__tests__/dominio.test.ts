import { describe, it, expect } from "vitest";
import {
  crearTarea,
  actualizarTarea,
  contarPendientes,
} from "../domain/operaciones";

describe("contarPendientes", () => {
  it("cuenta solo las pendientes", () => {
    const t1 = crearTarea({ titulo: "A" });
    const t2 = crearTarea({ titulo: "B" });
    const t3 = crearTarea({ titulo: "C" });

    const conEstados = actualizarTarea([t1, t2, t3], t2.id, {
      estado: "en-progreso",
    });
    const final = actualizarTarea(conEstados, t3.id, { estado: "completada" });

    expect(contarPendientes(final)).toBe(1);
  });

  it("devuelve 0 para un array vacio", () => {
    expect(contarPendientes([])).toBe(0);
  });
});