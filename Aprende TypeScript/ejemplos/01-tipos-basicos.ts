// ============================================================
// 01-tipos-basicos.ts — primitivos, arrays, tuples, readonly
// ============================================================
// Ejecutar con:  node ejemplos/01-tipos-basicos.ts

// --- Primitivos e inferencia ---
let nombre: string = "Alan";      // anotacion explicita
const PI = 3.1416;                // inferido como literal 3.1416
let contador = 0;                 // inferido: number
contador += 1;                    // ok; el tipo no cambia

// --- Arrays ---
let puntuaciones: number[] = [10, 8, 9];
let tags: Array<string> = ["ts", "curso"];   // forma alternativa

// --- Tuplas: posiciones fijas ---
let par: [string, number] = ["edad", 36];
// par[0] = 9;                    // ✖ error: posicion 0 debe ser string

// --- readonly: inmutable a nivel de tipos ---
let fijas: readonly string[] = ["a", "b"];
// fijas.push("c");               // ✖ error: no se puede mutar

// --- const assertions: literales estrictos ---
const estados = ["pendiente", "hecho"] as const;
type Estado = (typeof estados)[number];   // "pendiente" | "hecho"

const miEstado: Estado = "hecho";
console.log({ nombre, contador, puntuaciones, tags, par, fijas, miEstado });

export {};