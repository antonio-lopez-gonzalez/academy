import { useState } from "react";

// Lección 1 · useState básico.
// El ciclo completo de React en un componente:
//   estado → UI → evento → setter → re-render (nueva UI)
export default function CounterDemo() {
  // count: valor actual (una CONSTANTE de este render)
  // setCount: ÚNICA vía para cambiarlo (además programa un re-render)
  // useState(0): el 0 solo se usa en el PRIMER render
  const [count, setCount] = useState(0);

  return (
    <div className="demo-box">
      <p>
        Contador: <strong>{count}</strong>
      </p>

      {/* Cada onClick llama al setter con el NUEVO valor */}
      <button onClick={() => setCount(count + 1)}>+1</button>{" "}
      {/* Math.max: regla de negocio "no bajar de 0" en el propio setter */}
      <button onClick={() => setCount((c) => Math.max(0, c - 1))}>−1</button>{" "}
      <button onClick={() => setCount(0)}>Reset</button>

      {/* Valor DERIVADO: no necesita estado propio,
          se calcula en cada render a partir de count */}
      <p>{count % 2 === 0 ? "Número par" : "Número impar"}</p>
    </div>
  );
}
