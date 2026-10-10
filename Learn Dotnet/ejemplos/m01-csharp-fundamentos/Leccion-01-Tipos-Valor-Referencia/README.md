# Módulo 1 — Lección 01: Tipos de valor y tipos de referencia

---

## 1. Introducción

Aprendemos la diferencia fundamental entre stack y heap, tipos de valor y referencia, `struct` vs `class`, `record`, boxing/unboxing, `ref struct`, `Span<T>` y los modificadores `in`/`ref`/`out`.

---

## 2. Teoría

| Concepto | Stack | Heap |
|---|---|---|
| Velocidad | Muy rápido | Más lento |
| Gestión | Automática (LIFO) | GC (no determinista) |
| Tamaño | ~1 MB/hilo | GB (límite SO) |
| Vive aquí | struct, int, Span<T> | class, arrays, strings |

| | struct | class | record | record struct |
|---|---|---|---|---|
| Vive en | Stack | Heap | Heap | Stack |
| Copia por | Valor | Referencia | Referencia | Valor |
| Igualdad | Valor | Referencia | Valor (estructural) | Valor |
| Inmutable | No por defecto | No | Sí | Sí |
| Herencia | No (solo interfaces) | Sí | Sí | No |

**Boxing:** convertir value type → object (va al heap, caro para el GC).  
**Span<T>:** vista mutable de memoria contigua sin copia (solo stack).  
**in:** paso por referencia solo lectura. **ref:** lectura/escritura. **out:** debe asignarse.

---

## 3. Ejemplo mínimo

```csharp
int a = 5;
int b = a;  // Copia: a y b son independientes
b = 10;     // a = 5, b = 10

var c1 = new Cliente("Ana");
var c2 = c1;    // Misma referencia
c2.Nombre = "Luis"; // c1.Nombre también es "Luis"
```

---

## 4. Ejemplo real

```bash
dotnet run --project ejemplos/m01-csharp-fundamentos/Leccion-01-Tipos-Valor-Referencia/src/TiposDeDatos
```

Cubre: stack vs heap, struct/class/record, boxing con ArrayList vs List<T>, Span<T>, stackalloc, in/ref/out.

---

## 5-10. Buenas prácticas, errores, ejercicios, reto, resumen, FAQ

**Buenas prácticas:**
- Usa `struct` para datos < 16 bytes, inmutables, efímeros
- Usa `record` para DTOs y value objects
- Prefiere `List<T>` sobre `ArrayList` (evita boxing)
- Usa `Span<T>` y `stackalloc` para buffers temporales

**Errores habituales:**
- Usar `struct` para objetos grandes (>16 bytes) → copias costosas
- Llamar `GC.Collect()` manualmente
- Usar `ArrayList` en código nuevo

**Reto:** Implementa un `readonly record struct Dinero(decimal Cantidad, string Moneda)` con método `Sumar` que devuelva nueva instancia.

---

**Siguiente:** Nullable Reference Types en profundidad
