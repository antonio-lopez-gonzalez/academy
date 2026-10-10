import { useState } from "react";

// Lección 4 · Laboratorio de MUTACIÓN (inmutabilidad).
// Experimento para VER el bug: un botón muta el array del estado
// (React no se entera: compara por referencia) y el otro crea un
// array nuevo (React re-renderiza al instante).
//
// Cómo probarlo:
//   1. Pulsa "Añadir mutando (✘)" varias veces → la pantalla NO cambia…
//      pero el array SÍ se está corrompiendo en silencio.
//   2. Pulsa "Añadir sin mutar (✔)" → aparecen DE GOLPE los números
//      ocultos que el botón malo había metido. Ese es el bug clásico.
export default function MutationLab() {
  const [numbers, setNumbers] = useState([1, 2, 3]);

  function nextValue() {
    return Math.max(...numbers) + 1;
  }

  function addMutating() {
    numbers.push(nextValue()); // ✘ MUTA el array actual
    setNumbers(numbers); // ✘ MISMA referencia → React no repinta
  }

  function addImmutable() {
    // ✔ Array NUEVO con spread → referencia distinta → re-render
    setNumbers([...numbers, nextValue()]);
  }

  return (
    <div className="demo-box">
      <p>
        Números: [{numbers.join(", ")}]
      </p>
      <button onClick={addMutating}>Añadir mutando (✘)</button>{" "}
      <button onClick={addImmutable}>Añadir sin mutar (✔)</button>{" "}
      <button onClick={() => setNumbers([1, 2, 3])}>Reset</button>
      <p>
        <small>
          Pista: pulsa el botón ✘ tres veces (parece roto) y luego el ✔:
          verás aparecer «de golpe» los valores corruptos.
        </small>
      </p>
    </div>
  );
}
