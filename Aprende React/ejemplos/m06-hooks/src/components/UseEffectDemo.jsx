import { useState, useEffect } from "react";

export default function UseEffectDemo() {
  const [seconds, setSeconds] = useState(0);
  const [isRunning, setIsRunning] = useState(true);

  useEffect(() => {
    if (!isRunning) return;

    const id = setInterval(() => {
      setSeconds((s) => s + 1);
    }, 1000);

    return () => clearInterval(id);
  }, [isRunning]);

  return (
    <div>
      <p>Tiempo: {seconds}s</p>
      <button onClick={() => setIsRunning(!isRunning)}>
        {isRunning ? "Pausar" : "Continuar"}
      </button>
      <button onClick={() => setSeconds(0)}>Reset</button>
    </div>
  );
}
