import { useState } from "react";
import { calculate } from "../logic/calculate.js";
import Display from "./Display.jsx";
import CalcButton from "./CalcButton.jsx";

// Definición del teclado: datos, no JSX repetido a mano.
// El layout se controla desde este array (orden = orden visual).
const KEYS = [
  { label: "7" }, { label: "8" }, { label: "9" }, { label: "÷", variant: "operator" },
  { label: "4" }, { label: "5" }, { label: "6" }, { label: "×", variant: "operator" },
  { label: "1" }, { label: "2" }, { label: "3" }, { label: "−", variant: "operator" },
  { label: "0" }, { label: "." }, { label: "=", variant: "equals" }, { label: "+", variant: "operator" },
];

const OPERATORS = ["+", "−", "×", "÷"];

export default function Calculator() {
  // ── Estado (dosis mínima anticipada del Módulo 4) ──────────────
  const [display, setDisplay] = useState("0"); // lo que se ve
  const [previous, setPrevious] = useState(null); // primer operando
  const [operator, setOperator] = useState(null); // operación pendiente

  // ── Manejadores: UNO por tipo de acción ────────────────────────

  // Dígito o punto: lo añade a la pantalla
  function handleDigit(digit) {
    // Si la pantalla muestra "0" (inicio) o "Error", empezamos de nuevo
    setDisplay(display === "0" || display === "Error" ? digit : display + digit);
  }

  // Operador: guarda el primer número y el operador; espera el segundo
  function handleOperator(op) {
    setPrevious(display);
    setOperator(op);
    setDisplay("0");
  }

  // Igual: calcula con la función pura y muestra el resultado
  function handleEquals() {
    // Guard clause: sin operación pendiente, no hay nada que hacer
    if (previous === null || operator === null) return;
    setDisplay(String(calculate(previous, operator, display)));
    setPrevious(null);
    setOperator(null);
  }

  // Limpiar: vuelve todo al estado inicial
  function handleClear() {
    setDisplay("0");
    setPrevious(null);
    setOperator(null);
  }

  // Despachador central: decide qué hacer con cada tecla por su etiqueta
  function handleKeyPress(label) {
    if (label === "=") return handleEquals();
    if (OPERATORS.includes(label)) return handleOperator(label);
    return handleDigit(label);
  }

  return (
    <div className="calculator">
      <Display value={display} />

      <div className="calc-keypad">
        {KEYS.map((key) => (
          <CalcButton
            key={key.label}
            label={key.label}
            variant={key.variant}
            onPress={handleKeyPress}
          />
        ))}
        {/* El botón C ocupa todo el ancho restante */}
        <button className="calc-btn calc-btn--clear" onClick={handleClear}>
          C
        </button>
      </div>
    </div>
  );
}
