import { useState } from "react";
import TaskForm from "./components/TaskForm.jsx";
import TaskList from "./components/TaskList.jsx";
import TaskStats from "./components/TaskStats.jsx";

// Proyecto del Módulo 4: Lista de tareas.
//
// Arquitectura:
//   App (estado + lógica)
//     ├── TaskForm (input controlado, avisa al padre)
//     ├── TaskStats (valores derivados, filtros)
//     └── TaskList (lista "tonta")
//           └── TaskItem × N (toggle, delete)
//
// El estado vive en App (el ancestro común) y los cambios se propagan
// hacia abajo por props. Flujo unidireccional del Módulo 0, aplicado.
export default function App() {
  // Estado principal: array de objetos { id, text, done }
  const [tasks, setTasks] = useState([
    { id: 1, text: "Repasar useState", done: true },
    { id: 2, text: "Terminar el proyecto del Módulo 4", done: false },
    { id: 3, text: "Leer sobre inmutabilidad", done: false },
  ]);

  // Estado de filtro: "all" | "pending" | "done"
  const [filter, setFilter] = useState("all");

  // ---- ACCIONES (todas INMUTABLES) ----

  function addTask(text) {
    // crypto.randomUUID() genera ids únicos y estables (nativo en navegadores modernos)
    const newTask = { id: crypto.randomUUID(), text, done: false };
    // Forma funcional: no dependemos del valor actual, pero es buena práctica
    setTasks((current) => [...current, newTask]);
  }

  function toggleTask(id) {
    // map + spread: actualizamos SOLO la tarea con ese id, sin mutar
    setTasks((current) =>
      current.map((task) =>
        task.id === id ? { ...task, done: !task.done } : task
      )
    );
  }

  function deleteTask(id) {
    // filter: devuelve un array nuevo sin la tarea borrada
    setTasks((current) => current.filter((task) => task.id !== id));
  }

  function clearCompleted() {
    // filter: mantiene solo las NO completadas
    setTasks((current) => current.filter((task) => !task.done));
  }

  // ---- VALORES DERIVADOS (no son estado: se calculan en cada render) ----

  const visibleTasks = tasks.filter((task) => {
    if (filter === "pending") return !task.done;
    if (filter === "done") return task.done;
    return true; // "all"
  });

  const doneCount = tasks.filter((t) => t.done).length;
  const pendingCount = tasks.length - doneCount;

  return (
    <main className="app">
      <header className="app-header">
        <h1>Lista de tareas</h1>
        <p className="app-subtitle">Proyecto del Módulo 4 · Estado en React</p>
      </header>

      <TaskForm onAdd={addTask} />

      <TaskStats
        total={tasks.length}
        done={doneCount}
        pending={pendingCount}
        filter={filter}
        onFilterChange={setFilter}
        onClearCompleted={clearCompleted}
      />

      <TaskList tasks={visibleTasks} onToggle={toggleTask} onDelete={deleteTask} />
    </main>
  );
}
