import CounterDemo from "./components/CounterDemo.jsx";
import BatchingDemo from "./components/BatchingDemo.jsx";
import ArrayStateDemo from "./components/ArrayStateDemo.jsx";
import ObjectStateDemo from "./components/ObjectStateDemo.jsx";
import MutationLab from "./components/MutationLab.jsx";

// Ejemplo del Módulo 4: useState, ciclo de renderizado (batching),
// estado con arrays/objetos e inmutabilidad.
// Cada <section> corresponde a una lección del módulo.
export default function App() {
  return (
    <main className="page">
      <h1>Módulo 4 · Estado</h1>

      <section>
        <h2>1 · useState: la memoria del componente</h2>
        <CounterDemo />
      </section>

      <section>
        <h2>2 · Batching y actualizaciones funcionales</h2>
        <BatchingDemo />
      </section>

      <section>
        <h2>3 · Estado con arrays y objetos</h2>
        <ArrayStateDemo />
        <ObjectStateDemo />
      </section>

      <section>
        <h2>4 · Laboratorio de mutación (inmutabilidad)</h2>
        <MutationLab />
      </section>
    </main>
  );
}
