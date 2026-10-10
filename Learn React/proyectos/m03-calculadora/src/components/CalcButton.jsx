// Botón genérico de la calculadora.
// Recibe: label (qué muestra) y onPress (a QUÉ avisa al pulsarse).
// "variant" configura el aspecto: default | operator | equals | clear
export default function CalcButton({ label, onPress, variant = "default" }) {
  return (
    <button
      className={`calc-btn calc-btn--${variant}`}
      // La arrow envuelve la llamada: avisa al padre CON su etiqueta
      // (patrón de la lección 03: paso de parámetros a manejadores)
      onClick={() => onPress(label)}
    >
      {label}
    </button>
  );
}
