import type { Tarea, EstadoTarea } from "../domain/modelo";

interface TareaItemProps {
  tarea: Tarea;
  onCambiarEstado: (id: string, estado: EstadoTarea) => void;
  onEliminar: (id: string) => void;
}

function siguienteEstado(estado: EstadoTarea): EstadoTarea {
  switch (estado) {
    case "pendiente":
      return "en-progreso";
    case "en-progreso":
      return "completada";
    case "completada":
      return "pendiente";
  }
}

export function TareaItem({ tarea, onCambiarEstado, onEliminar }: TareaItemProps) {
  return (
    <li className={`tarea-item tarea-item--${tarea.estado}`}>
      <div className="tarea-info">
        <strong>{tarea.titulo}</strong>
        {tarea.descripcion ? <p>{tarea.descripcion}</p> : null}
        <span className={`etiqueta etiqueta--${tarea.estado}`}>{tarea.estado}</span>
      </div>
      <div className="acciones">
        <button onClick={() => onCambiarEstado(tarea.id, siguienteEstado(tarea.estado))}>
          Cambiar estado
        </button>
        <button className="peligro" onClick={() => onEliminar(tarea.id)}>
          Eliminar
        </button>
      </div>
    </li>
  );
}