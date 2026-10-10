import { useState, useMemo } from "react";

const ITEMS = Array.from({ length: 5000 }, (_, i) => ({
  id: i,
  name: `Item ${i}`,
  value: Math.floor(Math.random() * 100),
}));

export default function UseMemoDemo() {
  const [filter, setFilter] = useState(50);
  const [count, setCount] = useState(0);

  const filtered = useMemo(() => {
    console.log("Filtrando…");
    return ITEMS.filter((item) => item.value >= filter);
  }, [filter]);

  return (
    <div>
      <label>
        Valor mínimo: {filter}
        <input
          type="range"
          min={0}
          max={100}
          value={filter}
          onChange={(e) => setFilter(Number(e.target.value))}
        />
      </label>
      <p>Resultados: {filtered.length}</p>
      <p>Contador: {count}</p>
      <button onClick={() => setCount(count + 1)}>
        Incrementar (no recalcula filtro)
      </button>
    </div>
  );
}
