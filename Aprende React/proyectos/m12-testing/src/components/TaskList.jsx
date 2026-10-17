import TaskItem from './TaskItem'

export default function TaskList({ tasks, onToggle, onDelete, onClearCompleted }) {
  if (tasks.length === 0) {
    return (
      <div className="empty-state">
        <p>No hay tareas</p>
      </div>
    )
  }

  const completedCount = tasks.filter(t => t.completed).length

  return (
    <div className="task-list-container">
      <ul className="task-list">
        {tasks.map(task => (
          <TaskItem
            key={task.id}
            task={task}
            onToggle={onToggle}
            onDelete={onDelete}
          />
        ))}
      </ul>
      {completedCount > 0 && (
        <button 
          onClick={onClearCompleted}
          className="clear-completed"
        >
          Limpiar completadas ({completedCount})
        </button>
      )}
    </div>
  )
}
