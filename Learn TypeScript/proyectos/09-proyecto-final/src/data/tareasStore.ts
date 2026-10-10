import type { Tarea } from "../domain/modelo";

const CLAVE_TAREAS = "tareas";

export function leerTareas(): Tarea[] {
  const crudo = localStorage.getItem(CLAVE_TAREAS);
  return crudo ? (JSON.parse(crudo) as Tarea[]) : [];
}

export function guardarTareas(tareas: Tarea[]): void {
  localStorage.setItem(CLAVE_TAREAS, JSON.stringify(tareas));
}