import { useState } from "react";

// Formulario controlado: el input es un espejo del estado local "text".
// Al enviar, avisa al padre (onAdd) y se limpia.
//
// Patrón: el formulario tiene estado LOCAL (el texto que escribes),
// pero la tarea añadida vive en el padre. Separación de responsabilidades.
export default function TaskForm({ onAdd }) {
  const [text, setText] = useState("");

  function handleSubmit(e) {
    e.preventDefault(); // evita que el navegador recargue la página
    const trimmed = text.trim();
    if (trimmed === "") return; // guard clause: no añadimos tareas vacías
    onAdd(trimmed); // el padre decide cómo se añade (recibe solo el texto)
    setText(""); // limpiamos el input local
  }

  return (
    <form className="task-form" onSubmit={handleSubmit}>
      <input
        type="text"
        value={text}
        onChange={(e) => setText(e.target.value)}
        placeholder="¿Qué hay que hacer?"
        aria-label="Nueva tarea"
        autoFocus
      />
      <button type="submit" disabled={text.trim() === ""}>
        Añadir
      </button>
    </form>
  );
}
