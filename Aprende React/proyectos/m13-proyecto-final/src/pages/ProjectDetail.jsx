import { useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useProject } from "../hooks/useProject";
import TaskBoard from "../components/tasks/TaskBoard";
import EditProjectModal from "../components/projects/EditProjectModal";
import styles from "./Detail.module.css";

export default function ProjectDetail() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { project, loading, error, updateProject, deleteProject } =
    useProject(id);
  const [showEdit, setShowEdit] = useState(false);

  async function handleDelete() {
    if (window.confirm("¿Eliminar este proyecto?")) {
      await deleteProject();
      navigate("/");
    }
  }

  if (loading) return <div className={styles.loading}>Cargando...</div>;
  if (error) return <div className={styles.error}>{error}</div>;
  if (!project) return <div>Proyecto no encontrado</div>;

  return (
    <div className={styles.detail}>
      <div className={styles.header}>
        <div>
          <h1>{project.name}</h1>
          <p>{project.description}</p>
        </div>
        <div>
          <button onClick={() => setShowEdit(true)}>Editar</button>
          <button onClick={handleDelete} className={styles.deleteBtn}>
            Eliminar
          </button>
        </div>
      </div>

      <TaskBoard projectId={id} />

      {showEdit && (
        <EditProjectModal
          project={project}
          onClose={() => setShowEdit(false)}
          onSubmit={async (data) => {
            await updateProject(data);
            setShowEdit(false);
          }}
        />
      )}
    </div>
  );
}
