const orders = [
  { id: "A-001", customer: "Ana", total: 129.9, status: "paid" },
  { id: "A-002", customer: "Luis", total: 59.5, status: "pending" },
  { id: "A-003", customer: "Marta", total: 210.0, status: "paid" },
  { id: "A-004", customer: "Pablo", total: 15.99, status: "cancelled" },
];

// Diccionario valor-dato → presentación (mejor que ternarios en cadena)
const STATUS_LABELS = {
  paid: "✅ Pagado",
  pending: "⏳ Pendiente",
  cancelled: "❌ Cancelado",
};

export default function OrderTable() {
  // Preparamos las filas FUERA del JSX: cadena filter → map
  const visibleRows = orders
    .filter((order) => order.status !== "cancelled") // ocultamos cancelados
    .map((order) => (
      <tr key={order.id}>
        <td>{order.id}</td>
        <td>{order.customer}</td>
        <td>{order.total.toFixed(2)} €</td>
        <td>{STATUS_LABELS[order.status]}</td>
      </tr>
    ));

  // El JSX se lee como una plantilla limpia
  return (
    <table className="order-table">
      <thead>
        <tr>
          <th>Pedido</th>
          <th>Cliente</th>
          <th>Total</th>
          <th>Estado</th>
        </tr>
      </thead>
      <tbody>{visibleRows}</tbody>
    </table>
  );
}
