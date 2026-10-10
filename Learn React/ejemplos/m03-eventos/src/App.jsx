import DeleteButton from "./components/DeleteButton.jsx";
import EventPlayground from "./components/EventPlayground.jsx";

// Ejemplo del Módulo 3: eventos, Synthetic Events y paso de parámetros
export default function App() {
  return (
    <main className="page">
      <h1>Módulo 3 — Eventos</h1>

      <section>
        <h2>1 · onClick, burbuja y stopPropagation</h2>
        <EventPlayground />
      </section>

      <section>
        <h2>2 · Callback al padre (onDelete)</h2>
        <DeleteButton
          itemName="factura-2024.pdf"
          onDelete={() => alert("El padre decide qué significa borrar")}
        />
      </section>
    </main>
  );
}
