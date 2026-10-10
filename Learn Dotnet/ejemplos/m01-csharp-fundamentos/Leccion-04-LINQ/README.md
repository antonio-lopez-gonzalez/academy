# Módulo 1 — Lección 04: LINQ (Language Integrated Query)

---

## 1. Introducción

Aprendemos LINQ, el lenguaje de consultas integrado en C#. Permite escribir consultas declarativas sobre colecciones (`IEnumerable<T>`) y bases de datos (`IQueryable<T>`) usando la misma sintaxis. Existe desde C# 3.0 (.NET Framework 3.5) y reemplaza bucles `foreach` anidados, filtros manuales y ordenaciones verbosas que escribías en Java, Python o JavaScript vanilla.

---

## 2. Teoría

LINQ se basa en **métodos de extensión** definidos en `System.Linq`. Cada método recibe un delegado (`Func<T, bool>`, `Func<T, TResult>`, etc.) y devuelve un `IEnumerable<T>` o un valor escalar.

| Concepto | Explicación |
|---|---|
| **Sintaxis de método** | `collection.Where(x => x > 5).Select(x => x * 2)` |
| **Sintaxis de consulta** | `from x in collection where x > 5 select x * 2` |
| **Ejecución diferida** | La consulta no se ejecuta al definirla, solo al iterarla (`foreach`, `ToList`, `Count`, etc.) |
| **Ejecución inmediata** | `ToList()`, `ToArray()`, `ToDictionary()` fuerzan la ejecución y materializan resultados |
| **Árbol de expresión** | `IQueryable<T>` acumula la consulta como `Expression<T>` para que el proveedor (EF Core, LINQ to SQL) la traduzca a SQL |
| **Delegado compilado** | `IEnumerable<T>` recibe delegados C# compilados, ejecuta todo en memoria |

**Cuándo usar cada sintaxis:**
- **Método:** más conciso para `Where`, `Select`, `OrderBy`
- **Consulta:** más legible para `Join`, `GroupBy`, `let`, múltiples `from`

**Comparación con otros lenguajes:**
- **Java:** Streams API (`list.stream().filter().map().collect()`) — mismo concepto, sintaxis distinta
- **Python:** list comprehensions, `filter()`, `map()` — menos tipado, sin equivalente a `IQueryable`
- **JavaScript:** `Array.prototype.filter().map().reduce()` — similar a method syntax, pero sin ejecución diferida real

---

## 3. Ejemplo mínimo

```csharp
var numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

// Method syntax
var evenSquares = numbers
    .Where(n => n % 2 == 0)      // Filtrar pares: [2, 4, 6, 8, 10]
    .Select(n => n * n);          // Elevar al cuadrado: [4, 16, 36, 64, 100]

// Query syntax (equivalente)
var evenSquaresQuery =
    from n in numbers
    where n % 2 == 0
    select n * n;

// Nada se ejecuta hasta que iteremos con foreach o ToList()
Console.WriteLine(string.Join(", ", evenSquares)); // 4, 16, 36, 64, 100
```

---

## 4. Ejemplo real

```bash
dotnet run --project ejemplos/m01-csharp-fundamentos/Leccion-04-LINQ/src/Demo
```

Cubre: `Where`, `Select`, `SelectMany`, `OrderBy`/`ThenBy`, `GroupBy`, `Join`/`GroupJoin`, `Any`/`All`/`Count`/`Sum`/`Min`/`Max`/`Average`/`Aggregate`, `First`/`FirstOrDefault`/`Single`/`SingleOrDefault`/`Last`/`LastOrDefault`, `Skip`/`Take`, `Distinct`/`Union`/`Intersect`/`Except`, sintaxis de consulta vs método, ejecución diferida vs inmediata con `ToList`/`ToDictionary`, `IEnumerable<T>` vs `IQueryable<T>`, y proyecciones a tipos anónimos y tuplas.

---

## 5. Buenas prácticas

- **Usa `var`** para el resultado de LINQ: `var result = people.Where(...)`
- **No llames `ToList()` antes de tiempo** — pierdes ejecución diferida y gastas memoria
- **Usa tipos anónimos** en proyecciones intermedias: `Select(p => new { p.Name, p.Age })`
- **Encadena operaciones** en lugar de anidar bucles: `Where().Select().OrderBy()`
- **Prefiere `Any()` sobre `Count() > 0`** — `Any()` se detiene al primer elemento, `Count()` recorre toda la secuencia
- **Usa `FirstOrDefault()`** cuando no estés seguro de que exista:
  ```csharp
  // Bien: no lanza si no hay coincidencia
  var person = people.FirstOrDefault(p => p.Id == id);
  if (person is null) return NotFound();

  // Mal: lanza InvalidOperationException si no existe
  var person = people.First(p => p.Id == id);
  ```
- **`Single()` solo** cuando esperes EXACTAMENTE un elemento (ej: buscar por clave única)

---

## 6. Errores habituales

| Error | Por qué está mal | Corrección |
|---|---|---|
| `people.Where(p => p.Age > 18).Count()` | `Where` + `Count` recorre dos veces. | `people.Count(p => p.Age > 18)` |
| Modificar la fuente mientras se itera | `foreach (var x in query) { list.Remove(x); }` → `InvalidOperationException` | Materializar antes: `foreach (var x in query.ToList())` |
| `First()` sin comprobar existencia | Lanza excepción si la secuencia está vacía. | `FirstOrDefault()` + null check |
| `Single()` con posibles duplicados | Si hay 0 o >1 elementos, lanza excepción. | Usa `FirstOrDefault()` o valida antes |
| `ToList()` en cada paso intermedio | Materializas múltiples veces, consumo de memoria innecesario. | Encadena operaciones LINQ, materializa una vez al final |
| Comparar `IQueryable` con datos en memoria | `IQueryable` necesita un proveedor (EF Core, etc.); `IEnumerable` ejecuta en cliente. Mezclarlos causa errores difíciles de depurar. | No llames `AsEnumerable()` antes de `Where` si trabajas con BD |
| Asumir que LINQ es más lento que un `foreach` | Para colecciones en memoria, `foreach` puede ser marginalmente más rápido, pero la diferencia rara vez importa. La legibilidad de LINQ gana. | Usa LINQ por defecto, optimiza con `foreach` solo si un benchmark lo demuestra |

---

## 7. Ejercicio guiado

**Objetivo:** dada una lista de estudiantes con nombre, nota y curso, obtener el top 3 de notas por curso.

```csharp
var students = new[]
{
    new { Name = "Ana",    Grade = 8.5, Course = "Mates" },
    new { Name = "Luis",   Grade = 9.2, Course = "Mates" },
    new { Name = "María",  Grade = 7.8, Course = "Física" },
    new { Name = "Carlos", Grade = 6.5, Course = "Mates" },
    new { Name = "Elena",  Grade = 9.5, Course = "Física" },
    new { Name = "Pedro",  Grade = 8.0, Course = "Mates" },
};

var topByCourse = students
    .GroupBy(s => s.Course)
    .Select(g => new
    {
        Course = g.Key,
        Top3 = g.OrderByDescending(s => s.Grade).Take(3).ToList()
    });

foreach (var course in topByCourse)
{
    Console.WriteLine($"{course.Course}:");
    foreach (var s in course.Top3)
        Console.WriteLine($"  {s.Name} — {s.Grade}");
}
```

---

## 8. Reto

Implementa un sistema de paginación genérico que reciba `IEnumerable<T>`, número de página y tamaño, y devuelva una tupla con los elementos de la página, el total de páginas y el total de elementos. El método `Paginate` debe:

- Ordenar por una clave proporcionada mediante delegado
- Soportar orden ascendente o descendente
- Devolver `(IEnumerable<T> Items, int TotalPages, int TotalItems)`
- Validar que página y tamaño sean > 0

Pista: usa `Skip`, `Take`, `Count` y `Math.Ceiling`.

---

## 9. Resumen

| Operador | Propósito |
|---|---|
| `Where` | Filtrar |
| `Select` | Proyectar / transformar |
| `SelectMany` | Aplanar colecciones anidadas |
| `OrderBy` / `ThenBy` | Ordenar |
| `GroupBy` | Agrupar por clave |
| `Join` / `GroupJoin` | Combinar colecciones |
| `Any` / `All` | Condiciones booleanas sobre toda la colección |
| `Count` / `Sum` / `Min` / `Max` / `Average` | Agregaciones numéricas |
| `Aggregate` | Acumulación personalizada |
| `First` / `Single` / `Last` | Obtener un elemento específico |
| `Skip` / `Take` | Paginación |
| `Distinct` / `Union` / `Intersect` / `Except` | Operaciones de conjunto |
| `ToList` / `ToArray` / `ToDictionary` | Ejecución inmediata (materializar) |

**Conceptos clave:** ejecución diferida, `IEnumerable<T>` (memoria) vs `IQueryable<T>` (BD/remoto), tipos anónimos y tuplas en proyecciones.

---

## 10. Preguntas frecuentes

**P: ¿Debo usar siempre LINQ en lugar de bucles `foreach`?**
R: Para filtrar, proyectar, ordenar y agrupar, sí. LINQ es declarativo y más legible. Para lógica con efectos secundarios (ej: enviar emails dentro del bucle), `foreach` es más apropiado.

**P: ¿Qué es más rápido, `foreach` o LINQ?**
R: Para colecciones en memoria, la diferencia es mínima (LINQ añade una indirección por delegado). Si el rendimiento es crítico, mide con BenchmarkDotNet. Para bases de datos, LINQ con EF Core compila a SQL optimizado.

**P: ¿Puedo usar LINQ con `async/await`?**
R: LINQ tradicional no soporta `async`. Para operaciones asíncronas sobre colecciones, usa `IAsyncEnumerable<T>` con `System.Linq.Async` (paquete NuGet) o `await foreach`. Para EF Core, usa `ToListAsync()`, `FirstOrDefaultAsync()`, etc.

**P: ¿Cuándo usar `IQueryable<T>` en lugar de `IEnumerable<T>`?**
R: `IQueryable<T>` cuando la fuente de datos es remota (base de datos, API externa) y quieres que el filtrado ocurra en el servidor, no en memoria. `IEnumerable<T>` para colecciones en memoria. Si conviertes un `IQueryable` a `IEnumerable` con `AsEnumerable()`, todo el resto de la consulta se ejecuta en el cliente (posible N+1 en EF Core).

---

**Siguiente:** Async/Await en profundidad
