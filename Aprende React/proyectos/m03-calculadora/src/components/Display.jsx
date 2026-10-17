// Pantalla de la calculadora: solo muestra el valor que recibe.
// Componente "tonto" (presentacional): sin lógica, solo props.
export default function Display({ value }) {
  return <div className="calc-display">{value}</div>;
}
