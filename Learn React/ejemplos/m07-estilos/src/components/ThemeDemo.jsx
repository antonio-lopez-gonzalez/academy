import { useState, useEffect } from "react";
import styles from "./ThemeDemo.module.css";

export default function ThemeDemo() {
  const [theme, setTheme] = useState(() => {
    return localStorage.getItem("demo-theme") || "light";
  });

  useEffect(() => {
    document.documentElement.dataset.theme = theme;
    localStorage.setItem("demo-theme", theme);
  }, [theme]);

  function toggleTheme() {
    setTheme((t) => (t === "light" ? "dark" : "light"));
  }

  return (
    <div className={styles.container}>
      <button onClick={toggleTheme} className={styles.button}>
        Cambiar a tema {theme === "light" ? "oscuro" : "claro"}
      </button>
      <div className={styles.card}>
        <h3>Tarjeta de ejemplo</h3>
        <p>Los colores cambian según el tema activo.</p>
      </div>
    </div>
  );
}
