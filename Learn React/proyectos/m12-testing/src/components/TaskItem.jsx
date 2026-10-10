export default function TaskItem({ task, onToggle, onDelete }) {
  return (
    <li className={`task-item ${task.completed ? 'completed' : ''}`}>
      <input
        type="checkbox"
        checked={task.completed}
        onChange={() => onToggle(task.id)}
        aria-label={`Marcar ${task.title} como ${task.completed ? 'incompleta' : 'completa'}`}
      />
      <div className="task-content">
        <span className="task-title">{task.title}</span>
        <span className="task-date">
          {new Date(task.createdAt).toLocaleDateString('es-ES')}
        </span>
      </div>
      <button 
        onClick={() => onDelete(task.id)} 
        aria-label="Eliminar tarea"
        className="delete-btn"
      >
        ×
      </button>
    </li>
  )
}
