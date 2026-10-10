import { useState } from "react";

export default function ControlledInputDemo() {
  const [name, setName] = useState("");

  return (
    <div>
      <input
        value={name}
        onChange={(e) => setName(e.target.value)}
        placeholder="Escribe tu nombre…"
      />
      {name && <p>Hola, {name} 👋</p>}
    </div>
  );
}
