export default function StockNotice({ stock }) {
  // Tres estados: if clásico antes del return (legible y directo)
  if (stock === 0) {
    return <p className="stock stock--out">❌ Agotado</p>;
  }
  if (stock < 5) {
    return <p className="stock stock--low">🔥 ¡Solo quedan {stock}!</p>;
  }
  return <p className="stock stock--ok">✔ En stock</p>;
}
