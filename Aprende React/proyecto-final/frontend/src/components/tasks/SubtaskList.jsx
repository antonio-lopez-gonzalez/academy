import { useState } from "react";
import styles from "./DetailLists.module.css";

export default function SubtaskList({ subtasks, onAdd, onToggle, onDelete }) {
  const [content, setContent] = useState("");

  async function handleSubmit(e) {
    e.preventDefault();
    if (!content.trim()) return;
    await onAdd(content);
    setContent("");
  }

  return (
    <section className={styles.section}>
      <h3>Subtareas</h3>

      <form onSubmit={handleSubmit} className={styles.form}>
        <input
          value={content}
          onChange={(e) => setContent(e.target.value)}
          placeholder="Añadir subtarea"
        />
        <button type="submit">Añadir</button>
      </form>

      <ul className={styles.list}>
        {subtasks.map((subtask) => (
          <li
            key={subtask.id}
            className={subtask.done ? styles.done : ""}
          >
            <label>
              <input
                type="checkbox"
                checked={subtask.done}
                onChange={(e) => onToggle(subtask.id, e.target.checked)}
              />
              {subtask.content}
            </label>
            <button onClick={() => onDelete(subtask.id)}>Eliminar</button>
          </li>
        ))}
      </ul>
    </section>
  );
}
