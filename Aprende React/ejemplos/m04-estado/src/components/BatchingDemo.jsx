import { useState } from "react";

// Lección 2 · El ciclo de renderizado y el BATCHING.
//
// Dos descubrimientos que este demo hace VISIBLES (abre la consola):
//
//  a) BATCHING: React espera a que el manejador TERMINE y procesa
//     todos los setters en UN solo re-render.
//  b) El estado es una FOTO del render: "count" dentro del manejador
//     vale lo que valía cuando se pintó, no cambia entre llamadas.
let nextLogId = 0; // fuera del componente: no queremos que dependa del render

export default function BatchingDemo() {
  const [count, setCount] = useState(0);
  const [log, setLog] = useState([]); // mini-registro visible en pantalla

  // Este log se ejecuta en CADA render del componente.
  // Cuenta cuántos aparecen por click: ¡esa es la prueba del batching!
  console.log("RENDER con count =", count);

  function addLog(entry) {
    setLog((current) => [...current, { id: nextLogId++, entry }]);
  }

  function handleTripleNaive() {
    // Las TRES llamadas leen la MISMA foto del render (el "count" actual).
    // Equivalente a: setCount(0+1); setCount(0+1); setCount(0+1);
    // Resultado: sube 1, no 3. Y UN solo re-render (batching).
    setCount(count + 1);
    setCount(count + 1);
    setCount(count + 1);
    addLog("ingenuo: 3 × setCount(count + 1) → sube 1");
  }

  function handleTripleFunctional() {
    // La forma FUNCIONAL recibe el valor PENDIENTE de cada actualización,
    // encolado por React. Resultado: +3, también en UN solo re-render.
    setCount((c) => c + 1);
    setCount((c) => c + 1);
    setCount((c) => c + 1);
    addLog("funcional: 3 × setCount(c => c + 1) → sube 3");
  }

  function handleReset() {
    setCount(0);
    setLog([]); // dos setters, UN re-render: el batching también aquí
  }

  return (
    <div className="demo-box">
      <p>
        Contador: <strong>{count}</strong>{" "}
        <small>(consola: un «RENDER» por pintado, no por setter)</small>
      </p>
      <button onClick={handleTripleNaive}>+3 «ingenuo» (sube 1)</button>{" "}
      <button onClick={handleTripleFunctional}>+3 funcional (sube 3)</button>{" "}
      <button onClick={handleReset}>Reset</button>
      <ul>
        {log.map((line) => (
          <li key={line.id}>{line.entry}</li>
        ))}
      </ul>
    </div>
  );
}
