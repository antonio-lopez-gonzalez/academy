import StatCard from "./StatCard";
import styles from "./StatsGrid.module.css";

const stats = [
  { label: "Usuarios", value: "1,234", trend: "+12%", trendUp: true },
  { label: "Ingresos", value: "€45,678", trend: "+8%", trendUp: true },
  { label: "Pedidos", value: "567", trend: "-3%", trendUp: false },
  { label: "Conversión", value: "3.2%", trend: "+1.5%", trendUp: true },
];

export default function StatsGrid() {
  return (
    <div className={styles.grid}>
      {stats.map((stat) => (
        <StatCard key={stat.label} {...stat} />
      ))}
    </div>
  );
}
