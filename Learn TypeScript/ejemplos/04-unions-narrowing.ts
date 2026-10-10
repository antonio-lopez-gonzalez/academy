// ============================================================
// 04-unions-narrowing.ts — unions, type guards y satisfies
// ============================================================
// Ejecutar con:  node ejemplos/04-unions-narrowing.ts

type Resultado = string | number;

// Narrowing con typeof: TypeScript reduce el tipo en cada rama
function formatear(v: Resultado): string {
  if (typeof v === "string") return v.toUpperCase(); // aqui v: string
  return v.toFixed(2);                               // aqui v: number
}

// Type guard propio: predicado de tipo
function esMedio(x: unknown): x is "web" | "consola" {
  return x === "web" || x === "consola";
}

function definirPlataforma(x: unknown): string {
  if (esMedio(x)) return `Plataforma soportada: ${x}`; // narrowing por guard
  return "Plataforma desconocida";
}

// satisfies: valida el contrato sin perder los literales inferidos
const config = {
  nodo: "prod",
  reintentos: 3,
} satisfies Record<string, string | number>;

// --- Union exhaustiva con never ---
type Color = "rojo" | "verde" | "azul";
function hex(c: Color): string {
  switch (c) {
    case "rojo": return "#f00";
    case "verde": return "#0f0";
    case "azul": return "#00f";
  }
}

console.log(formatear("hola"), formatear(3.14159));
console.log(definirPlataforma("web"), definirPlataforma(42));
console.log(config, hex("azul"));

export {};