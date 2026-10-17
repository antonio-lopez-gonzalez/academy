import styles from "./StatsGrid.module.css";

export default function StatCard({ label, value, trend, trendUp }) {
  return (
    <div className={styles.card}>
      <div className={styles.label}>{label}</div>
      <div className={styles.value}>{value}</div>
      <div className={`${styles.trend} ${trendUp ? styles.trendUp : styles.trendDown}`}>
        {trendUp ? "↑" : "↓"} {trend}
      </div>
    </div>
  );
}
