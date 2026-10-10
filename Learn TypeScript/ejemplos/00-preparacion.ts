// ============================================================
// 00-preparacion.ts — Que es TypeScript y el flujo de trabajo
// ============================================================
// Ejecutar con:  node ejemplos/00-preparacion.ts
// (Node.js 22.6+ ejecuta .ts borrando los tipos; sin build)

// 1) Un valor que JavaScript aceptaria... pero TypeScript vigila.
let edad: number = 30;
// edad = "treinta";   // ✖ descomentar esto NO compila

// 2) El modo strict obliga a pensar en null/undefined.
function saludar(nombre?: string): string {
  // El "?" anade undefined al tipo; lo manejamos explicitamente.
  return nombre ? `Hola, ${nombre}` : "Hola, anonimo";
}

// 3) El compilador es la "red de seguridad" (tsc o tsgo en TS 7).
const bienTipado: number = edad + 12;
console.log(saludar("Ada"));
console.log(saludar());
console.log("Comprobar errores sin emitir:", bienTipado);

// 4) Comprobaciones utiles durante el desarrollo:
//    npx tsc --noEmit ejemplos/00-preparacion.ts

export {};