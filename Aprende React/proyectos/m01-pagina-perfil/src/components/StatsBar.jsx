import StatItem from "./StatItem.jsx";

// Fila de estadísticas: recibe el ARRAY y delega cada item.
// (map() se estudia a fondo en el Módulo 2; aquí es un avance.)
export default function StatsBar({ stats }) {
  return (
    <div className="stats-bar">
      {stats.map((stat) => (
        <StatItem key={stat.label} value={stat.value} label={stat.label} />
      ))}
    </div>
  );
}
