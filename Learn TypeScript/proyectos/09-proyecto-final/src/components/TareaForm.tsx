import { useState } from "react";
import type { FormEvent } from "react";
import type { NuevaTarea } from "../domain/operaciones";

interface TareaFormProps {
  onCrear: (nueva: NuevaTarea) => void;
}

export function TareaForm({ onCrear }: TareaFormProps) {
  const [titulo, setTitulo] = useState("");
  const [descripcion, setDescripcion] = useState("");

  function enviar(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    const limpio = titulo.trim();
    if (!limpio) return;
    onCrear({ titulo: limpio, descripcion: descripcion.trim() || undefined });
    setTitulo("");
    setDescripcion("");
  }

  return (
    <form className="tarea-form" onSubmit={enviar}>
      <input
        value={titulo}
        onChange={(e) => setTitulo(e.target.value)}
        placeholder="Titulo de la tarea"
        aria-label="Titulo"
      />
      <input
        value={descripcion}
        onChange={(e) => setDescripcion(e.target.value)}
        placeholder="Descripcion (opcional)"
        aria-label="Descripcion"
      />
      <button type="submit">Anadir</button>
    </form>
  );
}