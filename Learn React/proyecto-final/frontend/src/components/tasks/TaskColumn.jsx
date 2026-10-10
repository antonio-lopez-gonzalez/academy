import { useState } from "react";
import TaskCard from "./TaskCard";
import CreateTaskModal from "./CreateTaskModal";
import styles from "./TaskColumn.module.css";

export default function TaskColumn({ title, status, tasks, onMove, onCreate }) {
  const [showModal, setShowModal] = useState(false);

  return (
    <div className={styles.column}>
      <div className={styles.header}>
        <h3>{title}</h3>
        <span className={styles.count}>{tasks.length}</span>
      </div>

      <div className={styles.list}>
        {tasks.map((task) => (
          <TaskCard key={task.id} task={task} onMove={onMove} />
        ))}
      </div>

      <button className={styles.addButton} onClick={() => setShowModal(true)}>
        + Añadir tarea
      </button>

      {showModal && (
        <CreateTaskModal
          status={status}
          onClose={() => setShowModal(false)}
          onSubmit={async (data) => {
            await onCreate({ ...data, status });
            setShowModal(false);
          }}
        />
      )}
    </div>
  );
}
