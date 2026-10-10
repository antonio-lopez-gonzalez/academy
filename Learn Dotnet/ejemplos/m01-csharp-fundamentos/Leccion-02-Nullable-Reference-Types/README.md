# Lección 02: Nullable Reference Types (NRT)

---

## 1. Introducción

Los **Nullable Reference Types** (NRT) son una característica de C# 8+ que convierte uno de los errores más frecuentes en producción —la `NullReferenceException`— en una **advertencia de compilación**.

Antes de NRT, cualquier variable de tipo referencia (`string`, `object`, `List<T>`) podía ser `null` en cualquier momento y el compilador no avisaba. El resultado: errores que explotaban en producción, no en desarrollo.

**¿Qué problema resuelve?** Detecta posibles desreferencias de `null` en tiempo de compilación, forzando al desarrollador a manejar el caso `null` explícitamente.

**¿Cuándo se utiliza?** Siempre. A partir de .NET 6, `<Nullable>enable</Nullable>` viene activado por defecto en nuevos proyectos. Es la configuración recomendada para cualquier proyecto nuevo.

**Errores típicos:** Usar el operador `!` para silenciar advertencias sin entender por qué ocurren. Asumir que `string` y `string?` son lo mismo.

---

## 2. Teoría

### El problema de los mil millones de dólares

Tony Hoare, creador del concepto de referencia nula, lo llamó su "error de mil millones de dólares". En C# antes de 8.0, cualquier tipo de referencia podía ser `null` sin que el compilador avisara:

```csharp
string nombre = null; // Compila sin problemas en C# < 8.0
Console.WriteLine(nombre.Length); // NullReferenceException en runtime
```

### Cómo funciona NRT

Con `<Nullable>enable</Nullable>`, el compilador distingue dos categorías de tipos de referencia:

| Declaración | Significado | Puede ser null | Advertencia si se desreferencia sin comprobar |
|---|---|---|---|
| `string` | Tipo de referencia no anulable | No (según el compilador) | No |
| `string?` | Tipo de referencia anulable | Sí | Sí |

```
┌────────────────────────────────────────────────────┐
│              ANÁLISIS DE FLUJO DEL COMPILADOR      │
│                                                    │
│  string? name = GetName();  ← puede ser null       │
│                                                    │
│  if (name != null)                                 │
│  {                                                 │
│      name.Length;  ← seguro: el compilador sabe    │
│  }                    que aquí NO es null           │
│                                                    │
│  name.Length;  ← ⚠️ CS8602: posible null ref       │
│                                                    │
└────────────────────────────────────────────────────┘
```

### Atributos de diagnóstico

C# usa atributos en `System.Diagnostics.CodeAnalysis` para informar al compilador sobre nulabilidad en escenarios complejos:

- `[NotNullWhen(true)]` — si retorna `true`, el parámetro `out` no es null (patrón Try)
- `[MaybeNullWhen(false)]` — si retorna `false`, el parámetro `out` puede ser null
- `[MemberNotNull]` — tras llamar al método, ciertos miembros no son null
- `[MaybeNull]` — un parámetro `out T` puede contener null a la salida

### Comparación con otros lenguajes

| Característica | C# NRT | Java | TypeScript |
|---|---|---|---|
| Nativo del lenguaje | Sí (.NET 6+) | No (anotaciones externas) | Sí (strictNullChecks) |
| Runtime involucrado | Sí (BCL anotada) | No | No (solo compilación) |
| Estándar único | Sí | No (múltiples sabores) | Sí |
| null vs undefined | Solo null | Solo null | Ambos |

### Warning waves

Las "olas de advertencias" son versiones incrementales del análisis de nulabilidad:

- **Wave 5** (.NET 5): análisis básico de flujo, tracking de campos
- **Wave 6** (.NET 6): guard clauses mejoradas
- **Wave 7** (.NET 7): análisis de atributos mejorado, genéricos
- **Wave 8** (.NET 8): análisis inter-procedural más preciso

Se configuran en `.csproj` con `<AnalysisLevel>` y se pueden tratar como errores con `<WarningsAsErrors>nullable</WarningsAsErrors>`.

---

## 3. Ejemplo mínimo

```csharp
// Con <Nullable>enable</Nullable> en .csproj

string? maybeNull = null;           // ✅ Sin advertencia: acepta null explícitamente
string notNull = "Hola";            // ✅ string normal

// notNull = null;                  // ⚠️ CS8600: no se puede asignar null a string

int length = maybeNull?.Length ?? 0; // ?. evita NullRef, ?? da valor por defecto
Console.WriteLine($"Longitud: {length}"); // 0
```

Ejecutar con `dotnet run`.

---

## 4. Ejemplo real

Servicio de procesamiento de pedidos con NRT aplicado correctamente:

```csharp
public class OrderService
{
    /// <summary>
    /// Busca un pedido. Puede retornar null si no existe.
    /// </summary>
    public Order? FindOrder(int id)
    {
        // Simula búsqueda en BD
        return id > 0 ? new Order { Id = id } : null;
    }

    /// <summary>
    /// Procesa un pedido. Lanza si el pedido es null.
    /// </summary>
    public void ProcessOrder(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);
        // Procesar...
    }

    /// <summary>
    /// Intenta cancelar un pedido. Retorna true si se pudo cancelar.
    /// </summary>
    public bool TryCancelOrder(
        int id,
        [NotNullWhen(true)] out Order? cancelledOrder)
    {
        var order = FindOrder(id);
        if (order is null)
        {
            cancelledOrder = null;
            return false;
        }

        cancelledOrder = order;
        return true;
    }
}
```

---

## 5. Buenas prácticas

- **Activar NRT siempre** (`<Nullable>enable</Nullable>`). No hay razón para desactivarlo en proyectos nuevos.
- **Preferir `?.` y `??` sobre comprobaciones manuales** de null. Son más legibles y concisos.
- **Usar `ArgumentNullException.ThrowIfNull()`** en lugar del patrón `if (x is null) throw new...`.
- **No abusar del operador `!`**. Si necesitas `!` más de un par de veces en tu código, algo está mal diseñado.
- **Tratar advertencias como errores** en CI/CD (`<WarningsAsErrors>nullable</WarningsAsErrors>`).
- **Inicializar propiedades non-nullable** en el constructor o con inicializadores (`= ""`).
- **Evitar `default!`** para silenciar advertencias en campos no inicializados. Usa `[MemberNotNull]` o inicialización en constructor.

---

## 6. Errores habituales

### ❌ Abusar del operador `!`

```csharp
string? name = GetName();
Console.WriteLine(name!.Length); // Silencia la advertencia, pero explota si GetName() retorna null
```

### ✅ Versión correcta

```csharp
string? name = GetName();
Console.WriteLine(name?.Length ?? 0);
// O comprobar antes:
if (name is not null)
    Console.WriteLine(name.Length);
```

### ❌ Asumir que los DTOs de serialización no necesitan `?`

```csharp
public class CreateUserRequest
{
    public string Name { get; set; } // Si el JSON no incluye "name", será null en runtime
}
```

### ✅ Versión correcta

```csharp
public class CreateUserRequest
{
    public required string Name { get; set; } // C# 11+: obliga a inicializar
    // O:
    public string Name { get; set; } = "";    // Valor por defecto seguro
}
```

### ❌ Ignorar advertencias con `#pragma warning disable`

```csharp
#pragma warning disable CS8602
Console.WriteLine(name.Length); // Peligro oculto
#pragma warning restore CS8602
```

---

## 7. Ejercicio guiado

Crea un método `TryGetFirstWord` que analice un `string?` y extraiga la primera palabra:

```csharp
// Requisitos:
// 1. Si el input es null o vacío, retorna false y out "".  
// 2. Si el input tiene texto, retorna true y out la primera palabra (hasta el primer espacio).
// 3. Usa [NotNullWhen(true)] para informar al compilador.

public static bool TryGetFirstWord(
    [NotNullWhen(true)] string? input,
    [NotNullWhen(true)] out string firstWord)
{
    if (string.IsNullOrWhiteSpace(input))
    {
        firstWord = "";  // Nunca null gracias al atributo
        return false;
    }

    int spaceIndex = input.IndexOf(' ');
    firstWord = spaceIndex == -1 ? input : input[..spaceIndex];
    return true;
}

// Prueba:
if (TryGetFirstWord("Hola mundo .NET", out string word))
    Console.WriteLine($"Primera palabra: {word}"); // "Hola"
```

---

## 8. Reto

Implementa una clase `Cache<T>` con carga perezosa (`Lazy<T>`) y NRT correcto:

1. El método `GetOrAdd(string key, Func<T> factory)` debe retornar `T` (non-nullable).
2. Si `factory` es `null`, debe lanzar `ArgumentNullException` usando `ThrowIfNull`.
3. Usa `??=` para inicializar el diccionario interno de forma perezosa.
4. Marca el campo del diccionario con `[MemberNotNull]` si usas inicialización en dos fases.
5. El método `TryGet(string key, out T? value)` retorna `true` si la clave existe.

Intenta resolverlo sin mirar la solución.

---

## 9. Resumen

| Concepto | Descripción |
|---|---|
| `<Nullable>enable</Nullable>` | Activa NRT en el proyecto |
| `string` | Tipo no anulable (el compilador asume que nunca es null) |
| `string?` | Tipo anulable (debe comprobarse antes de usar) |
| `?.` | Acceso condicional: retorna null si el operando es null |
| `??` | Coalescencia: valor por defecto si el operando es null |
| `??=` | Asignación condicional: asigna solo si la variable es null |
| `!` | Null-forgiving: suprime advertencia (usar con extrema precaución) |
| `ThrowIfNull()` | Lanza `ArgumentNullException` si el argumento es null (.NET 6+) |
| `[NotNullWhen]` | Informa al compilador que un out param no es null |
| `[MemberNotNull]` | Informa que un campo se inicializa en un método |

---

## 10. Preguntas frecuentes

**P: ¿Puedo migrar un proyecto legacy a NRT gradualmente?**
R: Sí. Puedes activar `<Nullable>enable</Nullable>` y usar un `.editorconfig` para reducir la severidad de las advertencias existentes (`dotnet_diagnostic.CS8602.severity = suggestion`). Luego ve resolviéndolas poco a poco.

**P: ¿NRT afecta al rendimiento en runtime?**
R: No. Los NRT son puramente una característica de compilación. No generan código adicional ni verificaciones en runtime. El IL generado es idéntico con o sin NRT.

**P: ¿Qué pasa si una librería externa no tiene anotaciones NRT?**
R: El compilador trata los tipos de librerías sin anotaciones como "oblivious" (ni nullable ni non-nullable). Puedes usar `!` con cuidado o crear un archivo de anotaciones externas.

**P: ¿Por qué `string.IsNullOrEmpty()` acepta `string?` sin advertencia?**
R: Porque la BCL de .NET está anotada con atributos NRT. El parámetro de `IsNullOrEmpty` está marcado como `[NotNullWhen(false)]`, indicando que cuando retorna `true` el valor no es null.

**P: ¿Debo usar `string?` en todos los parámetros que podrían ser null?**
R: Sí, esa es exactamente la idea. Cuanto más preciso seas con las anotaciones, más te ayudará el compilador a evitar errores.
