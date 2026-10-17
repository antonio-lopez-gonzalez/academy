// Barra de estadísticas y filtros.
// Todo son valores DERIVADOS que llegan por props: ningún estado propio.
//
// Renderizado condicional: el botón "Limpiar completadas" solo aparece
// si hay tareas completadas (done > 0).
export default function TaskStats({
  total,
  done,
  pending,
  filter,
  onFilterChange,
  onClearCompleted,
}) {
  return (
    <div className="task-stats">
      <div className="task-stats__info">
        <span className="stat">
          <strong>{pending}</strong> pendientes
        </span>
        <span className="stat-separator">·</span>
        <span className="stat">
          <strong>{done}</strong> de {total} completadas
        </span>
      </div>

      <div className="filters" role="group" aria-label="Filtros de tareas">
        <button
          className={filter === "all" ? "filter-btn active" : "filter-btn"}
          onClick={() => onFilterChange("all")}
        >
          Todas
        </button>
        <button
          className={filter === "pending" ? "filter-btn active" : "filter-btn"}
          onClick={() => onFilterChange("pending")}
        >
          Pendientes
        </button>
        <button
          className={filter === "done" ? "filter-btn active" : "filter-btn"}
          onClick={() => onFilterChange("done")}
        >
          Completadas
        </button>
      </div>

      {done > 0 && (
        <button className="clear-btn" onClick={onClearCompleted}>
          Limpiar completadas
        </button>
      )}
    </div>
  );
}
