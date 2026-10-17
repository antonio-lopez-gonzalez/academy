// Lógica de cálculo PURA: sin React, sin DOM, sin efectos.
// Misma entrada → misma salida. Así es testeable (Módulo 12).
export function calculate(a, operator, b) {
  const x = Number(a);
  const y = Number(b);

  switch (operator) {
    case "+":
      return x + y;
    case "−": // signo menos tipográfico (el del botón)
      return x - y;
    case "×":
      return x * y;
    case "÷":
      // Decisión de negocio: dividir entre cero devuelve "Error"
      return y === 0 ? "Error" : x / y;
    default:
      return y;
  }
}
