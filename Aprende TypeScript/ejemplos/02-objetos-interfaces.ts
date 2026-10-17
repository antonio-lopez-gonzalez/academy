// ============================================================
// 02-objetos-interfaces.ts — interfaces, unions e index signatures
// ============================================================
// Ejecutar con:  node ejemplos/02-objetos-interfaces.ts

interface Usuario {
  readonly id: string;   // no se puede reasignar (a nivel de tipos)
  nombre: string;
  email?: string;        // opcional (puede faltar)
}

const ada: Usuario = { id: "u-1", nombre: "Ada Lovelace" };

// Union discriminada: el discriminante "tipo"
type Evento =
  | { tipo: "clic"; x: number; y: number }
  | { tipo: "tecla"; codigo: string };

function describir(e: Evento): string {
  switch (e.tipo) {
    case "clic":
      return `Clic en (${e.x}, ${e.y})`;   // aqui e tiene x e y
    case "tecla":
      return `Tecla ${e.codigo}`;          // aqui e tiene codigo
  }
}

// Index signature: objeto con claves dinamicas
const contadorPorPalabra: Record<string, number> = {};
contadorPorPalabra["typescript"] = 3;

console.log(ada, describir({ tipo: "clic", x: 1, y: 2 }), contadorPorPalabra);

export {};