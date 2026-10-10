import { beforeEach, describe, it, expect } from "vitest";
import { guardarTareas, leerTareas } from "../data/tareasStore";
import { crearTarea } from "../domain/operaciones";
import type { Tarea } from "../domain/modelo";

// Asercion de tipos: solo compila si la igualdad es correcta.
type Expect<T extends true> = T;
type Equal<A, B> = (<T>() => T extends A ? 1 : 2) extends (
  <T>() => T extends B ? 1 : 2
)
  ? true
  : false;

const resultado = leerTareas();
const chequeo: Expect<Equal<typeof resultado, Tarea[]>> = true;
void chequeo;

describe("tareasStore", () => {
  beforeEach(() => localStorage.clear());

  it("guarda y recupera tareas de localStorage", () => {
    const tarea = crearTarea({ titulo: "Recuperar" });
    guardarTareas([tarea]);

    const leidas = leerTareas();
    expect(leidas).toHaveLength(1);
    expect(leidas[0].titulo).toBe("Recuperar");
  });

  it("devuelve array vacio si no hay nada", () => {
    expect(leerTareas()).toEqual([]);
  });
});