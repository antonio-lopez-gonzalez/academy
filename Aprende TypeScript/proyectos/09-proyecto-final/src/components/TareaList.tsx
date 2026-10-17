import type { Tarea, EstadoTarea } from "../domain/modelo";
import { TareaItem } from "./TareaItem";

interface TareaListProps {
  tareas: Tarea[];
  onCambiarEstado: (id: string, estado: EstadoTarea) => void;
  onEliminar: (id: string) => void;
}

export function TareaList({ tareas, onCambiarEstado, onEliminar }: TareaListProps) {
  if (tareas.length === 0) {
    return <p className="vacio">No hay tareas para mostrar.</p>;
  }

  return (
    <ul className="tarea-list">
      {tareas.map((tarea) => (
        <TareaItem
          key={tarea.id}
          tarea={tarea}
          onCambiarEstado={onCambiarEstado}
          onEliminar={onEliminar}
        />
      ))}
    </ul>
  );
}