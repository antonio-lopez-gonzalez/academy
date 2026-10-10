import { useState } from "react";
import styles from "./CreateTaskModal.module.css";

export default function CreateTaskModal({ status, onClose, onSubmit }) {
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [priority, setPriority] = useState("medium");
  const [labels, setLabels] = useState("");
  const [dueDate, setDueDate] = useState("");

  async function handleSubmit(e) {
    e.preventDefault();
    await onSubmit({
      title,
      description,
      priority,
      dueDate,
      labels: labels
        .split(",")
        .map((l) => l.trim())
        .filter(Boolean),
    });
  }

  return (
    <div className={styles.overlay} onClick={onClose}>
      <div className={styles.modal} onClick={(e) => e.stopPropagation()}>
        <h2>Nueva Tarea</h2>
        <form onSubmit={handleSubmit}>
          <label>
            Título
            <input
              value={title}
              onChange={(e) => setTitle(e.target.value)}
              required
            />
          </label>

          <label>
            Descripción
            <textarea
              value={description}
              onChange={(e) => setDescription(e.target.value)}
            />
          </label>

          <label>
            Prioridad
            <select
              value={priority}
              onChange={(e) => setPriority(e.target.value)}
            >
              <option value="low">Baja</option>
              <option value="medium">Media</option>
              <option value="high">Alta</option>
            </select>
          </label>

          <label>
            Etiquetas (separadas por coma)
            <input value={labels} onChange={(e) => setLabels(e.target.value)} />
          </label>

          <label>
            Fecha límite
            <input
              type="date"
              value={dueDate}
              onChange={(e) => setDueDate(e.target.value)}
            />
          </label>

          <div className={styles.actions}>
            <button type="button" onClick={onClose}>
              Cancelar
            </button>
            <button type="submit">Crear</button>
          </div>
        </form>
      </div>
    </div>
  );
}
