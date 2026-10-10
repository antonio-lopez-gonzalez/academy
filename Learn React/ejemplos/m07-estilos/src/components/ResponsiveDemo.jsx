import styles from "./ResponsiveDemo.module.css";

export default function ResponsiveDemo() {
  const items = Array.from({ length: 6 }, (_, i) => `Item ${i + 1}`);

  return (
    <div className={styles.container}>
      <p>Redimensiona la ventana para ver el cambio de layout.</p>
      <div className={styles.grid}>
        {items.map((item, i) => (
          <div key={i} className={styles.card}>
            {item}
          </div>
        ))}
      </div>
    </div>
  );
}
