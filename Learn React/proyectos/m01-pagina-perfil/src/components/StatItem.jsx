// Una estadística: número grande + etiqueta pequeña
export default function StatItem({ value, label }) {
  return (
    <div className="stat-item">
      <strong className="stat-item__value">{value}</strong>
      <span className="stat-item__label">{label}</span>
    </div>
  );
}
