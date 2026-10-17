import { useState, useEffect } from "react";

function useLocalStorage(key, initialValue) {
  const [value, setValue] = useState(() => {
    try {
      const item = localStorage.getItem(key);
      return item ? JSON.parse(item) : initialValue;
    } catch {
      return initialValue;
    }
  });

  useEffect(() => {
    try {
      localStorage.setItem(key, JSON.stringify(value));
    } catch (err) {
      console.error("Error al guardar en localStorage:", err);
    }
  }, [key, value]);

  return [value, setValue];
}

export default function CustomHookDemo() {
  const [name, setName] = useLocalStorage("demo-name", "");
  const [theme, setTheme] = useLocalStorage("demo-theme", "light");

  return (
    <div style={{ background: theme === "dark" ? "#333" : "#fff", color: theme === "dark" ? "#fff" : "#000", padding: "1rem" }}>
      <input value={name} onChange={(e) => setName(e.target.value)} placeholder="Tu nombre (persiste)" />
      <p>Hola, {name || "desconocido"}</p>
      <button onClick={() => setTheme(theme === "dark" ? "light" : "dark")}>
        Cambiar tema (actual: {theme})
      </button>
      <p>Recarga la página: los valores se mantienen.</p>
    </div>
  );
}
