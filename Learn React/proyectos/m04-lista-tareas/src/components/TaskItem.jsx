// Una tarea individual: checkbox para alternarla y botón para borrarla.
// Ambos AVISAN al padre pasando el id (parámetros en manejadores, M3).
//
// Clases condicionales: "task--done" se añade si task.done es true.
export default function TaskItem({ task, onToggle, onDelete }) {
  return (
    <li className={task.done ? "task task--done" : "task"}>
      <label className="task__label">
        <input
          type="checkbox"
          checked={task.done}
          onChange={() => onToggle(task.id)}
          aria-label={`Marcar "${task.text}" como ${task.done ? "pendiente" : "completada"}`}
        />
        <span className="task__text">{task.text}</span>
      </label>
      <button
        className="task__delete"
        onClick={() => onDelete(task.id)}
        aria-label={`Borrar "${task.text}"`}
      >
        🗑
      </button>
    </li>
  );
}
