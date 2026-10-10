import { useState } from "react";

// Lección 3 · Estado con ARRAYS.
// Regla de oro: NUNCA mutar (push, splice, sort…). SIEMPRE crear
// un array NUEVO (spread, filter, map) para que React detecte el cambio
// comparando referencias.
let nextId = 4; // ids fuera del componente: no dependen del render

export default function ArrayStateDemo() {
  const [items, setItems] = useState([
    { id: 1, text: "Aprender useState" },
    { id: 2, text: "Entender el batching" },
    { id: 3, text: "Practicar con arrays" },
  ]);
  const [text, setText] = useState(""); // input controlado (M5 a fondo)

  function addItem(e) {
    e.preventDefault(); // el form no recarga la página
    const trimmed = text.trim();
    if (trimmed === "") return; // guard clause: nada vacío
    // AÑADIR sin mutar: array nuevo con spread + el elemento al final
    setItems([...items, { id: nextId++, text: trimmed }]);
    setText(""); // limpiamos el input
  }

  function removeItem(id) {
    // BORRAR sin mutar: filter devuelve SIEMPRE un array nuevo
    setItems(items.filter((item) => item.id !== id));
  }

  return (
    <div className="demo-box">
      <h3>Lista de la compra (array en estado)</h3>
      <form onSubmit={addItem}>
        <input
          value={text}
          onChange={(e) => setText(e.target.value)}
          placeholder="Nuevo elemento…"
        />
        <button type="submit">Añadir</button>
      </form>
      <ul>
        {/* key estable por id: la reconciliación del Módulo 2 */}
        {items.map((item) => (
          <li key={item.id}>
            {item.text}{" "}
            {/* Callback con parámetro: el padre recibe QUÉ borrar (M3) */}
            <button onClick={() => removeItem(item.id)}>🗑</button>
          </li>
        ))}
      </ul>
    </div>
  );
}
