import { useState } from "react";
import type { Tarea } from "../domain/modelo";
import { leerTareas, guardarTareas } from "../data/tareasStore";
import {
  crearTarea,
  actualizarTarea,
  eliminarTarea,
} from "../domain/operaciones";
import type { NuevaTarea, CambioTarea } from "../domain/operaciones";

export function useTareas() {
  const [tareas, setTareas] = useState<Tarea[]>(leerTareas);

  function crear(nueva: NuevaTarea): void {
    const siguientes = [...tareas, crearTarea(nueva)];
    setTareas(siguientes);
    guardarTareas(siguientes);
  }

  function actualizar(id: string, cambios: CambioTarea): void {
    const siguientes = actualizarTarea(tareas, id, cambios);
    setTareas(siguientes);
    guardarTareas(siguientes);
  }

  function eliminar(id: string): void {
    const siguientes = eliminarTarea(tareas, id);
    setTareas(siguientes);
    guardarTareas(siguientes);
  }

  return { tareas, crear, actualizar, eliminar };
}