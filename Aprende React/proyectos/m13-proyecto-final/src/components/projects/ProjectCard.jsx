import { Link } from "react-router-dom";
import styles from "./ProjectCard.module.css";

export default function ProjectCard({ project }) {
  return (
    <Link to={`/projects/${project.id}`} className={styles.card}>
      <h3>{project.name}</h3>
      <p>{project.description}</p>
      <div className={styles.stats}>
        <span>{project.tasksCount || 0} tareas</span>
        <span>{project.membersCount || 0} miembros</span>
      </div>
    </Link>
  );
}
