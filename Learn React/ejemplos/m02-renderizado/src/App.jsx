import OrderTable from "./components/OrderTable.jsx";
import PlayerList from "./components/PlayerList.jsx";

// Ejemplo del Módulo 2: renderizado de listas, keys y condicionales
export default function App() {
  return (
    <main className="page">
      <h1>Módulo 2 — Renderizado</h1>

      <section>
        <h2>1 · Tabla de pedidos (filter → map, diccionario de estados)</h2>
        <OrderTable />
      </section>

      <section>
        <h2>2 · Experimento de keys (índice vs id)</h2>
        <p className="hint">
          Escribe un apodo en la fila de <strong>Nadia</strong>, pulsa el botón
          «Añadir al principio» y observa dónde acaba el texto con cada tipo
          de key. Usa el checkbox para cambiar entre key=id y key=índice.
        </p>
        <PlayerList />
      </section>
    </main>
  );
}
