import styles from "./Header.module.css";

export default function Header({ theme, onToggleTheme, onMenuClick }) {
  return (
    <header className={styles.header}>
      <button className={styles.menuButton} onClick={onMenuClick}>
        ☰
      </button>
      <h2 className={styles.logo}>📊 Dashboard</h2>
      <button className={styles.themeButton} onClick={onToggleTheme}>
        {theme === "light" ? "🌙" : "☀️"}
      </button>
    </header>
  );
}
