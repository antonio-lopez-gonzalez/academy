import { useState } from "react";

const initialPlayers = [
  { id: 101, name: "Nadia" },
  { id: 102, name: "Omar" },
  { id: 103, name: "Irene" },
];

// EXPERIMENTO DE KEYS (lección 02 del Módulo 2):
// El <input> de cada fila guarda su texto en el DOM (estado interno).
// Con key={player.id} el texto viaja con su jugadora al reordenar.
// Con key={index} el texto se queda pegado a la POSICIÓN: bug visible.
export default function PlayerList() {
  const [players, setPlayers] = useState(initialPlayers);
  const [useIndexKey, setUseIndexKey] = useState(false);

  function addPlayerFirst() {
    const nuevo = { id: crypto.randomUUID(), name: "Teo" };
    // Añadir AL PRINCIPIO: el peor enemigo de key={index}
    setPlayers([nuevo, ...players]);
  }

  return (
    <div className="player-experiment">
      <label className="player-experiment__toggle">
        <input
          type="checkbox"
          checked={useIndexKey}
          onChange={(e) => setUseIndexKey(e.target.checked)}
        />
        Usar key=índice (modo bug 🐛)
      </label>

      <button onClick={addPlayerFirst}>Añadir a Teo al principio</button>

      {players.map((player, index) => (
        // Cambia entre las dos keys con el checkbox y repite el experimento
        <div key={useIndexKey ? index : player.id} className="player-row">
          <strong>{player.name}</strong>
          <input placeholder="Apodo…" />
        </div>
      ))}
    </div>
  );
}
