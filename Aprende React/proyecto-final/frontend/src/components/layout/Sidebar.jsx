import { Link, useLocation } from "react-router-dom";
import { useProjects } from "../../hooks/useProjects";
import styles from "./Sidebar.module.css";

export default function Sidebar() {
  const location = useLocation();
  const { projects } = useProjects();

  return (
    <aside className={styles.sidebar}>
      <nav>
        <Link
          to="/"
          className={location.pathname === "/" ? styles.active : ""}
        >
          Dashboard
        </Link>

        <div className={styles.section}>
          <h3>Proyectos</h3>
          <ul>
            {projects?.map((project) => (
              <li key={project.id}>
                <Link
                  to={`/projects/${project.id}`}
                  className={
                    location.pathname.includes(project.id) ? styles.active : ""
                  }
                >
                  {project.name}
                </Link>
              </li>
            ))}
          </ul>
        </div>
      </nav>
    </aside>
  );
}
