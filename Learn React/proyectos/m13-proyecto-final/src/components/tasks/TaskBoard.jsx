import { TASK_STATUS } from "../../types/task";
import { useTasks } from "../../hooks/useTasks";
import { useWebSocket } from "../../hooks/useWebSocket";
import TaskColumn from "./TaskColumn";
import styles from "./TaskBoard.module.css";

const COLUMNS = [
  { key: TASK_STATUS.TODO, title: "Por hacer" },
  { key: TASK_STATUS.IN_PROGRESS, title: "En progreso" },
  { key: TASK_STATUS.DONE, title: "Completadas" },
];

export default function TaskBoard({ projectId }) {
  const { tasks, setTasks, loading, moveTask, createTask } =
    useTasks(projectId);

  useWebSocket(projectId, (message) => {
    setTasks((prev) => {
      switch (message.type) {
        case "created":
          return [...prev, message.task];
        case "updated":
        case "moved":
          return prev.map((t) => (t.id === message.task.id ? message.task : t));
        case "deleted":
          return prev.filter((t) => t.id !== message.taskId);
        default:
          return prev;
      }
    });
  });

  if (loading) return <div>Cargando tareas...</div>;

  return (
    <div className={styles.board}>
      {COLUMNS.map((column) => (
        <TaskColumn
          key={column.key}
          title={column.title}
          status={column.key}
          tasks={tasks.filter((t) => t.status === column.key)}
          onMove={moveTask}
          onCreate={createTask}
        />
      ))}
    </div>
  );
}
