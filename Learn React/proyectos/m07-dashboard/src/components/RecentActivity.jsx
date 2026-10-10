import styles from "./RecentActivity.module.css";

const activities = [
  { user: "Ana García", action: "completó la tarea", target: "Diseño de landing", time: "Hace 5 min" },
  { user: "Carlos López", action: "comentó en", target: "API de pagos", time: "Hace 15 min" },
  { user: "María Rodríguez", action: "creó el proyecto", target: "Móvil v2", time: "Hace 1 hora" },
  { user: "Juan Martínez", action: "subió archivo a", target: "Documentación", time: "Hace 2 horas" },
];

export default function RecentActivity() {
  return (
    <div className={styles.container}>
      <h2 className={styles.title}>Actividad reciente</h2>
      <div className={styles.list}>
        {activities.map((activity, i) => (
          <div key={i} className={styles.item}>
            <div className={styles.avatar}>
              {activity.user.charAt(0)}
            </div>
            <div className={styles.content}>
              <p className={styles.text}>
                <strong>{activity.user}</strong> {activity.action}{" "}
                <span className={styles.target}>{activity.target}</span>
              </p>
              <span className={styles.time}>{activity.time}</span>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
