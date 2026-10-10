# Lección 03 — Pattern Matching en C#

## 1. Introducción

El **Pattern Matching** (coincidencia de patrones) es una característica del lenguaje C# que permite inspeccionar la forma y el contenido de un valor y tomar decisiones en función de ello. Introducido en **C# 7** y ampliado hasta **C# 12**, el pattern matching reemplaza progresivamente los `if-else` anidados, los casts redundantes y las largas cadenas de `switch` tradicionales.

**Por qué existe:** Antes del pattern matching, verificar tipos requería `is` + cast manual. Despachar por valores complejos requería `if-else` encadenados difíciles de mantener. El pattern matching unifica todas estas operaciones en una sintaxis declarativa.

**Cuándo se utiliza:**
- Despachar lógica según el tipo de un objeto (`is Type`)
- Evaluar múltiples condiciones sobre un mismo valor (`switch expression`)
- Validar propiedades de objetos complejos (property patterns)
- Parsear secuencias y listas (list patterns)
- Clasificar valores numéricos en rangos (relational patterns)

**Errores típicos:**
- Usar `is` sin declarar variable y luego hacer cast manual
- Olvidar que el `switch expression` es exhaustivo (necesitas `_` o el compilador avisa)
- Intentar usar list patterns en versiones anteriores a C# 11
- Usar switch expression donde un `if` simple es más legible

---

## 2. Teoría

### ¿Cómo funciona el pattern matching en el compilador?

Cuando escribes un patrón como `x is int numero`, el compilador de C# emite código IL que:
1. Verifica el tipo en tiempo de ejecución mediante `isinst` (instrucción IL)
2. Si la verificación es exitosa, asigna el valor a la variable declarada
3. Continúa la ejecución dentro del bloque `if` con la variable tipada

El pattern matching de C# es **sintaxis de alto nivel** — no introduce nuevas capacidades en el runtime, sino que genera el mismo IL que escribirías manualmente, pero con verificación de exhaustividad y menos errores humanos.

### Evolución del pattern matching en C#

| Versión C# | Funcionalidad |
|---|---|
| C# 7 | `is` con declaración de variable, `switch` con patrones, `when` |
| C# 8 | Switch expressions, property patterns, tuple patterns, positional patterns |
| C# 9 | `and`, `or`, `not`, relational patterns (`<`, `>`, etc.) |
| C# 10 | Property patterns extendidos (anidados) |
| C# 11 | List patterns (`[1, 2, ..]`), pattern matching sobre `Span<T>` |

### Comparación con otros lenguajes

- **Java:** Sin pattern matching nativo hasta Java 16 (`instanceof` con variable). Java 21 añadió `switch` con patrones, pero menos expresivo que C#.
- **Python:** No tiene pattern matching de tipos (es dinámico). Python 3.10 añadió `match/case` estructural similar al de C#.
- **JavaScript:** No tiene pattern matching nativo. Se usan `typeof`, `instanceof` y desestructuración manual.
- **F# / Rust / Scala:** El pattern matching es parte fundamental del lenguaje desde su creación. C# ha ido incorporando estas ideas progresivamente.

### Por qué usar pattern matching en lugar de `if-else`

```
// Sin pattern matching: verboso, propenso a errores
if (obj is string) {
    var s = (string)obj;  // cast redundante
    if (s.Length > 5) { ... }
}

// Con pattern matching: conciso, declarativo, seguro
if (obj is string s && s.Length > 5) { ... }
```

---

## 3. Ejemplo mínimo

```csharp
// El ejemplo más simple de pattern matching:
// Verificar tipo y declarar variable en un solo paso
object dato = "Hola .NET";

if (dato is string texto)
    Console.WriteLine(texto.ToUpper()); // "HOLA .NET"
```

Ejecutar con:
```bash
dotnet run --project src/Demo
```

---

## 4. Ejemplo real

Ver `src/Demo/Program.cs` para una demostración completa de todos los tipos de pattern matching.

El programa recorre:
1. `is` con verificación de tipo
2. `is` con declaración de variable
3. Switch expressions vs switch tradicional
4. Cláusula `when`
5. Property patterns `{ Prop: valor }`
6. List patterns `[1, 2, ..]`
7. Patrones relacionales `< > <= >=`
8. Patrones lógicos `and` / `or` / `not`
9. Tuple patterns `(x, y)`
10. Pattern matching sobre `Span<char>`

Ejemplo de un **property pattern** del programa:

```csharp
var persona = new Persona("Ana", 25);

var descripcion = persona switch
{
    { Nombre: "Ana", Edad: > 18 } => "Ana es mayor de edad",
    { Nombre: "Ana" }             => "Ana (cualquier edad)",
    { Edad: > 18 }                => "Mayor de edad",
    _                              => "No cumple criterio"
};
```

---

## 5. Buenas prácticas

### Qué hacer

- **Usa `is` con declaración** siempre que necesites verificar tipo y usar el valor: `if (obj is string s)` en lugar de `if (obj is string) { var s = (string)obj; }`
- **Prefiere switch expression** sobre switch tradicional cuando necesites devolver un valor
- **Usa `_` como descarte** en vez de `default` en switch expressions
- **Usa `not null`** en lugar de `!= null` en contextos de pattern matching
- **Usa property patterns** para validar DTOs y modelos: más legible que `if (obj.Prop == algo && obj.Otro == ...)`
- **Usa list patterns** para validar arrays y colecciones de longitud conocida
- **Comprueba la exhaustividad:** el compilador avisa si un switch expression no cubre todos los casos

### Qué evitar

- **No abuses del pattern matching:** si un `if` simple es más legible, úsalo. El pattern matching es más expresivo, pero no siempre más simple.
- **No uses switch expression para efectos secundarios** (solo para devolver valores)
- **No anides excesivamente:** property patterns anidados > 2 niveles pueden ser ilegibles
- **No uses `var` en patrones** si no necesitas la variable: prefiere `_` (descartar)
- **No olvides el caso exhaustivo `_`** en switch expressions: el compilador genera warning CS8509

---

## 6. Errores habituales

### Error 1: Hacer cast manual después de `is`

```csharp
// ❌ Incorrecto: cast redundante
if (obj is string)
{
    var texto = (string)obj;  // Ya verificaste que es string, ¿por qué el cast?
    Console.WriteLine(texto.Length);
}

// ✅ Correcto: declaración de variable en el patrón
if (obj is string texto)
{
    Console.WriteLine(texto.Length);  // texto ya es string tipado
}
```

### Error 2: Olvidar la exhaustividad en switch expression

```csharp
// ❌ Incorrecto: falta el caso exhaustivo
var resultado = valor switch
{
    1 => "Uno",
    2 => "Dos"
    // CS8509: The switch expression does not handle all possible values
};

// ✅ Correcto: incluir descarte _
var resultado = valor switch
{
    1 => "Uno",
    2 => "Dos",
    _ => "Otro"
};
```

### Error 3: Confundir `or` de patrones con `||` de condiciones

```csharp
// ❌ Incorrecto: || no es válido en patrones
valor switch { 1 || 2 || 3 => "bajo" }

// ✅ Correcto: 'or' es el operador de patrones
valor switch { 1 or 2 or 3 => "bajo" }
```

### Error 4: Asumir que list patterns funcionan en .NET 6

```csharp
// ❌ No compila en .NET 6 / C# 10
numeros switch { [1, 2, 3] => "exacto" }
// Error: List patterns requieren C# 11 (.NET 7+)

// ✅ Verificar TargetFramework: net7.0 o net8.0
```

---

## 7. Ejercicio guiado

Crea un método que clasifique una **figura geométrica** usando pattern matching:

```csharp
// Paso 1: Define los tipos de figuras
record Circulo(double Radio);
record Rectangulo(double Ancho, double Alto);
record Triangulo(double Base, double Altura);

// Paso 2: Crea el método de clasificación
static string ClasificarFigura(object figura) =>
    figura switch
    {
        Circulo { Radio: > 100 }        => "Círculo grande",
        Circulo { Radio: > 0 }          => "Círculo pequeño",
        Rectangulo { Ancho: var a, Alto: var h } when a == h
                                         => "Cuadrado",
        Rectangulo { Ancho: > 100, Alto: > 100 }
                                         => "Rectángulo grande",
        Rectangulo                       => "Rectángulo",
        Triangulo                        => "Triángulo",
        null                             => "Figura nula",
        _                                => "Tipo desconocido"
    };

// Paso 3: Prueba con diferentes figuras
Console.WriteLine(ClasificarFigura(new Circulo(150)));     // Círculo grande
Console.WriteLine(ClasificarFigura(new Rectangulo(50, 50))); // Cuadrado
Console.WriteLine(ClasificarFigura(new Triangulo(10, 20)));  // Triángulo
```

### Explicación paso a paso

1. **`Circulo { Radio: > 100 }`** — property pattern con patrón relacional anidado
2. **`Rectangulo { Ancho: var a, Alto: var h } when a == h`** — property pattern con variables capturadas + `when`
3. **`Rectangulo`** — type pattern sin condiciones adicionales
4. **`null`** — patrón constante para null
5. **`_`** — descarte exhaustivo

---

## 8. Reto

Crea un **parser de comandos** usando exclusivamente pattern matching que procese entrada del usuario en formato texto.

El programa debe aceptar entradas como:
- `"mover 10 20"` → mover a coordenadas (10, 20)
- `"saltar alto"` o `"saltar bajo"` → tipo de salto
- `"salir"` → terminar el programa
- `"color rojo"` o `"color #FF0000"` → cambiar color
- Cualquier otra entrada → "Comando no reconocido"

Requisitos:
- Usa **list patterns** para parsear el texto dividido en tokens
- Usa **tuples** para las coordenadas
- Usa **relational patterns** para validar coordenadas positivas
- Usa **`when`** para distinguir entre color por nombre y por código hexadecimal
- El programa debe ejecutarse en bucle hasta que el usuario escriba `salir`

No se proporciona la solución aquí. ¡Inténtalo primero!

---

## 9. Resumen

| Concepto | Sintaxis | Disponible desde |
|---|---|---|
| Type check | `obj is string` | C# 1 |
| Type declaration | `obj is string s` | C# 7 |
| Switch expression | `x switch { 1 => "a", _ => "b" }` | C# 8 |
| Property pattern | `{ Prop: valor }` | C# 8 |
| Tuple pattern | `(x, y)` | C# 8 |
| Relational pattern | `<`, `>`, `<=`, `>=` | C# 9 |
| Logical `and` | `> 0 and < 100` | C# 9 |
| Logical `or` | `1 or 2 or 3` | C# 9 |
| Logical `not` | `not null` | C# 9 |
| List pattern | `[1, 2, ..]` | C# 11 |
| Span pattern | `span is ['a', ..]` | C# 11 |
| `when` clause | `case int n when n > 0` | C# 7 |

**Ideas clave:**
- El pattern matching hace el código más **declarativo** y **seguro**
- La **exhaustividad** del compilador evita olvidar casos en switch expressions
- **Prefiere `is` con declaración** a `is` + cast manual
- Los **property patterns** reemplazan if-else encadenados sobre propiedades
- Los **list patterns** son ideales para validar secuencias sin bucles manuales
- Pattern matching sobre **Span** evita alocaciones en el heap

---

## 10. Preguntas frecuentes

**¿Puedo usar pattern matching con `switch` tradicional (no expression)?**
Sí. El switch tradicional también soporta patrones desde C# 7:
```csharp
switch (obj)
{
    case int n when n > 0: ...
    case string s: ...
    case null: ...
}
```

**¿Cuándo usar switch expression vs switch tradicional?**
- **Switch expression:** cuando necesitas devolver un valor (asignar a variable, retornar)
- **Switch tradicional:** cuando necesitas ejecutar múltiples statements por caso (efectos secundarios)

**¿Son los list patterns eficientes?**
Sí. El compilador genera código que usa `Length` y acceso por índice, sin crear colecciones intermedias. Sobre `Span<T>` es especialmente eficiente porque no hay alocaciones.

**¿Puedo combinar patrones en un `is`?**
Sí. Puedes combinar `is` con patrones relacionales, lógicos y de propiedad:
```csharp
if (numero is > 0 and <= 100) ...
if (persona is { Edad: >= 18, Nombre: not null }) ...
```

**¿Qué pasa si mi switch expression no es exhaustivo?**
El compilador genera un warning **CS8509** y el código lanza `SwitchExpressionException` en tiempo de ejecución si ningún caso coincide. Siempre incluye `_ =>` como caso por defecto.

**¿Funcionan los property patterns con clases normales (no records)?**
Sí, los property patterns funcionan con cualquier tipo que tenga propiedades públicas accesibles. Los records son convenientes porque generan `Deconstruct()` y son inmutables, pero no son obligatorios.

**¿Puedo usar pattern matching en `catch`?**
No directamente. Los bloques `catch` usan filtros de excepción con `when` (`catch (Exception ex) when (ex is HttpRequestException)`), pero no soportan la sintaxis completa de pattern matching dentro del `catch`.

**¿Se puede hacer pattern matching con `using`?**
No. `using` y pattern matching son independientes. Pero puedes combinar `is` con `using`:
```csharp
if (obj is IDisposable d)
{
    using (d) { ... }
}
```
