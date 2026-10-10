import { Link } from "react-router-dom";
import { TASK_STATUS } from "../../types/task";
import styles from "./TaskCard.module.css";

const PRIORITY_STYLES = {
  high: styles.priorityHigh,
  medium: styles.priorityMedium,
  low: styles.priorityLow,
};

export default function TaskCard({ task, onMove }) {
  return (
    <article className={styles.card}>
      <div className={styles.header}>
        <span
          className={`${styles.priority} ${PRIORITY_STYLES[task.priority]}`}
        >
          {task.priority}
        </span>
      </div>

      <h4>
        <Link to={`/tasks/${task.id}`}>{task.title}</Link>
      </h4>
      <p>{task.description}</p>

      {task.labels?.length > 0 && (
        <div className={styles.labels}>
          {task.labels.map((label) => (
            <span key={label} className={styles.label}>
              {label}
            </span>
          ))}
        </div>
      )}

      <div className={styles.actions}>
        {task.status !== TASK_STATUS.TODO && (
          <button onClick={() => onMove(task.id, TASK_STATUS.TODO)}>←</button>
        )}
        {task.status !== TASK_STATUS.IN_PROGRESS && (
          <button onClick={() => onMove(task.id, TASK_STATUS.IN_PROGRESS)}>
            →
          </button>
        )}
        {task.status !== TASK_STATUS.DONE && (
          <button onClick={() => onMove(task.id, TASK_STATUS.DONE)}>✓</button>
        )}
      </div>
    </article>
  );
}
