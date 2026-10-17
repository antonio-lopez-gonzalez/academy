// Insignia de estado: verde si online, gris si no.
// La CLASE se calcula con un ternario (expresión en JSX).
export default function Badge({ online }) {
  return (
    <span className={online ? "badge badge--online" : "badge badge--offline"}>
      {online ? "🟢 Disponible" : "⚪ Ocupado"}
    </span>
  );
}
