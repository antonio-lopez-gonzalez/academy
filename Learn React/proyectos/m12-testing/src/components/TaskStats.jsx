export default function TaskStats({ tasks }) {
  const total = tasks.length
  const completed = tasks.filter(t => t.completed).length
  const pending = total - completed

  return (
    <div className="task-stats" aria-label="Estadísticas de tareas">
      <div className="stat">
        <span className="stat-number">{total}</span>
        <span className="stat-label">Total</span>
      </div>
      <div className="stat">
        <span className="stat-number">{completed}</span>
        <span className="stat-label">Completadas</span>
      </div>
      <div className="stat">
        <span className="stat-number">{pending}</span>
        <span className="stat-label">Pendientes</span>
      </div>
    </div>
  )
}
