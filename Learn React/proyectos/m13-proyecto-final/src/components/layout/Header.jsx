import { useAuth } from "../../contexts/AuthContext";
import { useTheme } from "../../contexts/ThemeContext";
import ThemeToggle from "../ui/ThemeToggle";
import { useNavigate } from "react-router-dom";
import styles from "./Header.module.css";

export default function Header() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  function handleLogout() {
    logout();
    navigate("/login");
  }

  return (
    <header className={styles.header}>
      <div className={styles.userInfo}>
        <span>Hola, {user?.name}</span>
      </div>
      <div>
        <ThemeToggle />
        <button onClick={handleLogout} className={styles.logoutBtn}>
          Cerrar Sesión
        </button>
      </div>
    </header>
  );
}
