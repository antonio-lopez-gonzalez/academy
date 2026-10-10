export default function TaskItem({ task, onToggle, onDelete }) {
  return (
    <li className={`task-item ${task.completed ? 'completed' : ''}`}>
      <input
        type="checkbox"
        checked={task.completed}
        onChange={() => onToggle(task.id)}
        aria-label={`Marcar ${task.title} como ${task.completed ? 'incompleta' : 'completa'}`}
      />
      <span>{task.title}</span>
      <button onClick={() => onDelete(task.id)} aria-label="Eliminar tarea">
        ×
      </button>
    </li>
  )
}
