// ============================================================
// 06-utility-types.ts — Partial, Pick, Omit, Record, ReturnType
// ============================================================
// Ejecutar con:  node ejemplos/06-utility-types.ts

interface Tarea {
  id: string;
  titulo: string;
  estado: "pendiente" | "completada";
}

// --- Partial: todas las props opcionales ---
const cambios: Partial<Tarea> = { estado: "completada" };
console.log("Partial:", cambios);

// --- Pick / Omit: recortar el tipo ---
type Resumen = Pick<Tarea, "id" | "titulo">;
type SinEstado = Omit<Tarea, "estado">;

// --- Record: indice dinamico ---
const cache: Record<string, number> = {};
cache["visitas"] = 5;
console.log("Record:", cache);

// --- ReturnType / Awaited: extraer tipos de funciones ---
async function consultar(): Promise<string[]> {
  return ["a", "b"];
}
type Valores = Awaited<ReturnType<typeof consultar>>; // string[]

// --- Exclude / Extract: filtrar unions ---
type Base = "a" | "b" | "c";
type SinA = Exclude<Base, "a">;   // "b" | "c"
type SoloB = Extract<Base, "a" | "b">; // "a" | "b"

console.log("SinEstado:", ({} as SinEstado), (false as any).a);
console.log("Exclude/Extract:", ([] as unknown as SinA), ([] as unknown as SoloB));

export {};