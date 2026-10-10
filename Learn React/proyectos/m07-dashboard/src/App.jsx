import { useState, useEffect } from "react";
import Header from "./components/Header/Header";
import Sidebar from "./components/Sidebar/Sidebar";
import StatsGrid from "./components/StatsGrid";
import RecentActivity from "./components/RecentActivity";
import styles from "./App.module.css";

export default function App() {
  const [theme, setTheme] = useState(() => {
    return localStorage.getItem("dashboard-theme") || "light";
  });
  const [sidebarOpen, setSidebarOpen] = useState(false);

  useEffect(() => {
    document.documentElement.dataset.theme = theme;
    localStorage.setItem("dashboard-theme", theme);
  }, [theme]);

  function toggleTheme() {
    setTheme((t) => (t === "light" ? "dark" : "light"));
  }

  return (
    <div className={styles.layout}>
      <Header
        theme={theme}
        onToggleTheme={toggleTheme}
        onMenuClick={() => setSidebarOpen(!sidebarOpen)}
      />
      <Sidebar open={sidebarOpen} onClose={() => setSidebarOpen(false)} />
      <main className={styles.main}>
        <h1 className={styles.title}>Dashboard</h1>
        <StatsGrid />
        <RecentActivity />
      </main>
    </div>
  );
}
