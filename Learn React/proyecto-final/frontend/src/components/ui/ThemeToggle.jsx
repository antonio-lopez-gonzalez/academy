import { useTheme } from "../../contexts/ThemeContext";
import styles from "./ThemeToggle.module.css";

export default function ThemeToggle() {
  const { theme, toggleTheme } = useTheme();

  return (
    <button onClick={toggleTheme} className={styles.toggle}>
      {theme === "dark" ? "🌙" : "☀️"} {theme === "dark" ? "Oscuro" : "Claro"}
    </button>
  );
}
