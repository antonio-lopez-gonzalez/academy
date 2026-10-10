import { useParams, useNavigate } from "react-router-dom";
import { useTaskDetail } from "../hooks/useTaskDetail";
import SubtaskList from "../components/tasks/SubtaskList";
import CommentList from "../components/tasks/CommentList";
import styles from "./TaskDetail.module.css";

export default function TaskDetail() {
  const { id } = useParams();
  const navigate = useNavigate();
  const {
    task,
    comments,
    subtasks,
    loading,
    progress,
    addComment,
    addSubtask,
    toggleSubtask,
    deleteSubtask,
  } = useTaskDetail(id);

  if (loading) return <div>Cargando...</div>;
  if (!task) return <div>Tarea no encontrada</div>;

  return (
    <div className={styles.detail}>
      <button onClick={() => navigate(-1)}>← Volver</button>

      <h1>{task.title}</h1>
      <p>{task.description}</p>

      <div className={styles.progress}>
        <label>Progreso: {progress}%</label>
        <progress value={progress} max={100} />
      </div>

      <SubtaskList
        subtasks={subtasks}
        onAdd={addSubtask}
        onToggle={toggleSubtask}
        onDelete={deleteSubtask}
      />

      <CommentList comments={comments} onAdd={addComment} />
    </div>
  );
}
