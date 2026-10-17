import { useState } from "react";
import { useTareas } from "./hooks/useTareas";
import { TareaForm } from "./components/TareaForm";
import { TareaList } from "./components/TareaList";
import { aplicarFiltro, contarPendientes } from "./domain/operaciones";
import type { Filtro } from "./domain/operaciones";
import type { EstadoTarea } from "./domain/modelo";

export default function App() {
  const { tareas, crear, actualizar, eliminar } = useTareas();
  const [filtro, setFiltro] = useState<Filtro>("todas");

  const visibles = aplicarFiltro(tareas, filtro);

  function cambiarEstado(id: string, estado: EstadoTarea) {
    actualizar(id, { estado });
  }

  return (
    <main className="contenedor">
      <header className="cabecera">
        <h1>Gestor de tareas</h1>
        <span className="badge-pendientes">{contarPendientes(tareas)} pendientes</span>
      </header>

      <TareaForm onCrear={crear} />

      <nav className="filtros" aria-label="Filtros">
        {(["todas", "pendientes", "completadas"] as const).map((f) => (
          <button
            key={f}
            className={f === filtro ? "activo" : ""}
            onClick={() => setFiltro(f)}
          >
            {f}
          </button>
        ))}
      </nav>

      <TareaList tareas={visibles} onCambiarEstado={cambiarEstado} onEliminar={eliminar} />
    </main>
  );
}