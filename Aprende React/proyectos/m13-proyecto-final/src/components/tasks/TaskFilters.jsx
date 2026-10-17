import { TASK_STATUS, TASK_PRIORITY } from "../../types/task";
import styles from "./TaskFilters.module.css";

export default function TaskFilters({
  filters,
  availableLabels,
  onSearchChange,
  onStatusChange,
  onPriorityChange,
  onLabelChange,
}) {
  return (
    <div className={styles.filters}>
      <input
        type="search"
        placeholder="Buscar tareas..."
        value={filters.search}
        onChange={(e) => onSearchChange(e.target.value)}
      />

      <select
        value={filters.status}
        onChange={(e) => onStatusChange(e.target.value)}
      >
        <option value="all">Todos los estados</option>
        <option value={TASK_STATUS.TODO}>Por hacer</option>
        <option value={TASK_STATUS.IN_PROGRESS}>En progreso</option>
        <option value={TASK_STATUS.DONE}>Completadas</option>
      </select>

      <select
        value={filters.priority}
        onChange={(e) => onPriorityChange(e.target.value)}
      >
        <option value="all">Todas las prioridades</option>
        <option value={TASK_PRIORITY.LOW}>Baja</option>
        <option value={TASK_PRIORITY.MEDIUM}>Media</option>
        <option value={TASK_PRIORITY.HIGH}>Alta</option>
      </select>

      <select
        value={filters.label}
        onChange={(e) => onLabelChange(e.target.value)}
      >
        <option value="all">Todas las etiquetas</option>
        {availableLabels.map((label) => (
          <option key={label} value={label}>
            {label}
          </option>
        ))}
      </select>
    </div>
  );
}
