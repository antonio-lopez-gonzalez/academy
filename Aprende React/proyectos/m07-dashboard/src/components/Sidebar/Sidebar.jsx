import styles from "./Sidebar.module.css";

const menuItems = [
  { icon: "🏠", label: "Inicio" },
  { icon: "📊", label: "Estadísticas" },
  { icon: "👥", label: "Usuarios" },
  { icon: "⚙️", label: "Configuración" },
];

export default function Sidebar({ open, onClose }) {
  return (
    <>
      {open && <div className={styles.overlay} onClick={onClose} />}
      <aside className={`${styles.sidebar} ${open ? styles.open : ""}`}>
        <nav className={styles.nav}>
          {menuItems.map((item) => (
            <a key={item.label} href="#" className={styles.link}>
              <span className={styles.icon}>{item.icon}</span>
              {item.label}
            </a>
          ))}
        </nav>
      </aside>
    </>
  );
}
