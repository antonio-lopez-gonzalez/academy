import TaskItem from "./TaskItem.jsx";

// Lista "tonta": recibe las tareas YA filtradas y los callbacks.
// No sabe nada de estado: solo pinta y avisa.
//
// Renderizado condicional: si no hay tareas, muestra un mensaje vacío.
export default function TaskList({ tasks, onToggle, onDelete }) {
  if (tasks.length === 0) {
    return (
      <div className="empty-state">
        <p className="empty-icon">✨</p>
        <p className="empty-text">No hay tareas aquí. ¡Añade la primera!</p>
      </div>
    );
  }

  return (
    <ul className="task-list">
      {tasks.map((task) => (
        <TaskItem
          key={task.id}
          task={task}
          onToggle={onToggle}
          onDelete={onDelete}
        />
      ))}
    </ul>
  );
}
