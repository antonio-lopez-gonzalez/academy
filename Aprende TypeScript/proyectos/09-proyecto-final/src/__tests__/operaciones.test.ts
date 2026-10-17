import { describe, it, expect } from "vitest";
import {
  crearTarea,
  actualizarTarea,
  eliminarTarea,
  aplicarFiltro,
} from "../domain/operaciones";
import type { Tarea } from "../domain/modelo";

function construir(): Tarea[] {
  const a = crearTarea({ titulo: "Backlog" });
  const b = crearTarea({ titulo: "En progreso" });
  const c = crearTarea({ titulo: "Hecho" });
  const enProgreso = actualizarTarea([a, b, c], b.id, { estado: "en-progreso" });
  const completado = actualizarTarea(enProgreso, c.id, { estado: "completada" });
  return completado;
}

describe("operaciones del dominio", () => {
  it("crea tareas con ids unicos", () => {
    const [x, y] = [crearTarea({ titulo: "X" }), crearTarea({ titulo: "Y" })];
    expect(x.id).not.toBe(y.id);
  });

  it("crea una tarea pendiente con formato UUID", () => {
    const tarea = crearTarea({ titulo: "Comprar pan" });
    expect(tarea.titulo).toBe("Comprar pan");
    expect(tarea.estado).toBe("pendiente");
    expect(tarea.id).toMatch(/^[0-9a-f-]{36}$/);
  });

  it("aplica el filtro correctamente", () => {
    const tareas = construir();
    expect(aplicarFiltro(tareas, "pendientes")).toHaveLength(1);
    expect(aplicarFiltro(tareas, "completadas")).toHaveLength(1);
    expect(aplicarFiltro(tareas, "todas")).toHaveLength(3);
  });

  it("actualiza respetando la inmutabilidad", () => {
    const tareas = construir();
    const antes = JSON.stringify(tareas);
    const modificadas = actualizarTarea(tareas, tareas[0].id, {
      titulo: "Cambiado",
    });
    expect(JSON.stringify(tareas)).toBe(antes);
    expect(modificadas[0].titulo).toBe("Cambiado");
  });

  it("elimina por id sin mutar el original", () => {
    const tareas = construir();
    const antes = JSON.stringify(tareas);
    const sinPrimera = eliminarTarea(tareas, tareas[0].id);
    expect(JSON.stringify(tareas)).toBe(antes);
    expect(sinPrimera.find((t) => t.id === tareas[0].id)).toBeUndefined();
  });
});