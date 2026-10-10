import type { Tarea, EstadoTarea } from "./modelo";

export interface NuevaTarea {
  titulo: string;
  descripcion?: string;
}

export type CambioTarea = Partial<
  Pick<Tarea, "titulo" | "descripcion" | "estado">
>;

export type Filtro = "todas" | "pendientes" | "completadas";

export function crearTarea(nueva: NuevaTarea): Tarea {
  return {
    id: crypto.randomUUID(),
    titulo: nueva.titulo,
    descripcion: nueva.descripcion ?? "",
    estado: "pendiente",
    creadaEn: new Date().toISOString(),
  };
}

export function cambiarEstado(tarea: Tarea, estado: EstadoTarea): Tarea {
  return { ...tarea, estado };
}

export function porEstado(tareas: Tarea[], estado: EstadoTarea): Tarea[] {
  return tareas.filter((t) => t.estado === estado);
}

export function findById(tareas: Tarea[], id: string): Tarea | undefined {
  return tareas.find((t) => t.id === id);
}

export function actualizarTarea(
  tareas: Tarea[],
  id: string,
  cambios: CambioTarea
): Tarea[] {
  return tareas.map((t) => (t.id === id ? { ...t, ...cambios } : t));
}

export function eliminarTarea(tareas: Tarea[], id: string): Tarea[] {
  return tareas.filter((t) => t.id !== id);
}

export function aplicarFiltro(tareas: Tarea[], filtro: Filtro): Tarea[] {
  switch (filtro) {
    case "todas":
      return tareas;
    case "pendientes":
      return porEstado(tareas, "pendiente");
    case "completadas":
      return porEstado(tareas, "completada");
  }
}

export function contarPendientes(tareas: Tarea[]): number {
  return porEstado(tareas, "pendiente").length;
}