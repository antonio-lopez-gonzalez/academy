import { useState, useEffect } from "react";

interface FetchEstado<T> {
  datos: T | null;
  cargando: boolean;
  error: string | null;
  recargar: () => void;
}

// Hook generico: se reutiliza para posts, comentarios y usuarios.
export function useFetch<T>(url: string): FetchEstado<T> {
  const [datos, setDatos] = useState<T | null>(null);
  const [cargando, setCargando] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [intento, setIntento] = useState(0);

  useEffect(() => {
    let cancelado = false;
    async function cargar() {
      setCargando(true);
      setError(null);
      try {
        const res = await fetch(url);
        if (!res.ok) throw new Error(`HTTP ${res.status}`);
        const json = (await res.json()) as T;
        if (!cancelado) setDatos(json);
      } catch (e) {
        if (!cancelado && e instanceof Error) setError(e.message);
      } finally {
        if (!cancelado) setCargando(false);
      }
    }
    cargar();
    return () => {
      cancelado = true;
    };
  }, [url, intento]);

  return { datos, cargando, error, recargar: () => setIntento((i) => i + 1) };
}