// ============================================================
// 03-funciones.ts — tipado de parametros, retorno y callbacks
// ============================================================
// Ejecutar con:  node ejemplos/03-funciones.ts

// --- Parametros y retorno tipados ---
function sumar(a: number, b: number = 10): number {
  return a + b;
}

// --- Function type: la firma como tipo ---
type Operacion = (x: number, y: number) => number;
const multiplicar: Operacion = (a, b) => a * b;

// --- Callbacks tipados ---
type Resultado = { ok: boolean; dato?: string };
function procesar(datos: string[], alTerminar: (r: Resultado) => void): void {
  const limpio = datos.filter((d) => d.trim().length > 0);
  alTerminar({ ok: true, dato: limpio.join(", ") });
}

procesar(["a", "  ", "b"], (r) => {
  console.log("Callback:", r);
});

// --- Parametros rest tipados ---
function juntar(separador: string, ...partes: string[]): string {
  return partes.join(separador);
}

console.log(sumar(5), sumar(5, 20), multiplicar(3, 4), juntar("-", "a", "b", "c"));

export {};