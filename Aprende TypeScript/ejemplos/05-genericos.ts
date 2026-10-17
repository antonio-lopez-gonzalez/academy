// ============================================================
// 05-genericos.ts — genericos, extends, keyof y conditional types
// ============================================================
// Ejecutar con:  node ejemplos/05-genericos.ts

// --- Generico basico: relacion entre entrada y salida ---
function primero<T>(arr: T[]): T | undefined {
  return arr[0];
}
const a = primero(["x", "y"]);   // inferido: string
const b = primero([1, 2, 3]);    // inferido: number

// --- Restriccion extends: T tiene al menos lo base ---
interface ConId {
  id: string;
}
function porId<T extends ConId>(items: T[], id: string): T | undefined {
  return items.find((i) => i.id === id);
}

// --- keyof y indexed access ---
type ClavesDeUsuario = keyof { id: string; nombre: string }; // "id" | "nombre"
type Solo = Pick<{ id: number; a: string; b: boolean }, "id" | "a">;

// --- Conditional type con infer ---
type ElementoDePromesa<T> = T extends Promise<infer E> ? E : T;
type Lib = ElementoDePromesa<Promise<number>>; // number

const item = porId([{ id: "x", nombre: "prueba" }], "x");
console.log(a, b, item?.nombre);

export {};