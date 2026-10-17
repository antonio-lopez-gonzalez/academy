import { useState } from "react";

export function useLocalStorage<T>(clave: string, inicial: T) {
  const [valor, setValor] = useState<T>(() => {
    const crudo = localStorage.getItem(clave);
    if (crudo === null) return inicial;
    try {
      return JSON.parse(crudo) as T;
    } catch {
      return inicial;
    }
  });

  const guardar = (nuevo: T) => {
    setValor(nuevo);
    localStorage.setItem(clave, JSON.stringify(nuevo));
  };

  return [valor, guardar] as const;
}