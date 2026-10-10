import { useState } from "react";
import { useProjects } from "../hooks/useProjects";
import ProjectCard from "../components/projects/ProjectCard";
import CreateProjectModal from "../components/projects/CreateProjectModal";
import styles from "./Dashboard.module.css";

export default function Dashboard() {
  const { projects, loading, error, createProject } = useProjects();
  const [showModal, setShowModal] = useState(false);

  if (loading) return <div className={styles.loading}>Cargando...</div>;
  if (error) return <div className={styles.error}>{error}</div>;

  return (
    <div className={styles.dashboard}>
      <div className={styles.header}>
        <h1>Mis Proyectos</h1>
        <button onClick={() => setShowModal(true)}>+ Nuevo Proyecto</button>
      </div>

      {projects?.length === 0 ? (
        <div className={styles.empty}>
          <p>No tienes proyectos aún</p>
          <button onClick={() => setShowModal(true)}>
            Crear tu primer proyecto
          </button>
        </div>
      ) : (
        <div className={styles.grid}>
          {projects?.map((project) => (
            <ProjectCard key={project.id} project={project} />
          ))}
        </div>
      )}

      {showModal && (
        <CreateProjectModal
          onClose={() => setShowModal(false)}
          onSubmit={async (data) => {
            await createProject(data);
            setShowModal(false);
          }}
        />
      )}
    </div>
  );
}
