// ====================================================================
// Lección 03: Pattern Matching en C# 12
// Demostración completa de todas las formas de pattern matching
// disponibles en C# con .NET 8.
// ====================================================================

// ────────────────────────────────────────────────────────────────────
// 1. Patrón "is" con verificación de tipo (type check)
// ────────────────────────────────────────────────────────────────────
// El operador "is" verifica si un objeto es de un tipo específico.
// Es el pattern matching más simple y reemplaza el uso de "as" + null check.
Console.WriteLine("=== 1. Patrón 'is' con verificación de tipo ===");

object valor = 42;

// Verificación clásica sin pattern matching (antes de C# 7):
// if (valor is int) { int x = (int)valor; ... }  ← cast redundante

// Con pattern matching: "is" verifica el tipo en tiempo de ejecución
if (valor is int)
    Console.WriteLine($"  'valor' es un int: {valor}");

valor = "Hola .NET";

// Se puede encadenar con else if para despachar por tipo
if (valor is string)
    Console.WriteLine($"  'valor' es un string: {valor}");
else if (valor is int)
    Console.WriteLine($"  'valor' es un int: {valor}");

Console.WriteLine();

// ────────────────────────────────────────────────────────────────────
// 2. Patrón "is" con declaración de variable (type declaration)
// ────────────────────────────────────────────────────────────────────
// C# 7+ permite declarar la variable directamente en el "is",
// evitando el cast posterior.
Console.WriteLine("=== 2. Patrón 'is' con declaración de variable ===");

object dato = 3.14;

// "is double d" verifica el tipo Y declara la variable "d" en un solo paso
// La variable "d" solo existe dentro del ámbito del if
if (dato is double d)
    Console.WriteLine($"  dato es double con valor: {d}"); // d ya es double, sin cast

// También funciona con tipos de referencia
dato = "Patrones en C#";

if (dato is string s && s.Length > 5)
    Console.WriteLine($"  String detectado con {s.Length} caracteres: '{s}'");

// Patrón "not null" (C# 9+): forma concisa de verificar no-nulidad
object? nulo = null;
if (nulo is not null)
    Console.WriteLine("  No es null");
else
    Console.WriteLine("  Es null (patrón 'not null' detectó la nulidad)");

Console.WriteLine();

// ────────────────────────────────────────────────────────────────────
// 3. Switch tradicional vs Switch expression (C# 8+)
// ────────────────────────────────────────────────────────────────────
// El switch expression es más conciso, no necesita break,
// y devuelve un valor directamente.
Console.WriteLine("=== 3. Switch tradicional vs Switch expression ===");

int diaSemana = 3;

// --- Switch tradicional (antes de C# 8) ---
// Verboso, necesita break en cada case, no devuelve valor directamente
string nombreDiaTradicional;
switch (diaSemana)
{
    case 1: nombreDiaTradicional = "Lunes"; break;
    case 2: nombreDiaTradicional = "Martes"; break;
    case 3: nombreDiaTradicional = "Miércoles"; break;
    case 4: nombreDiaTradicional = "Jueves"; break;
    case 5: nombreDiaTradicional = "Viernes"; break;
    default: nombreDiaTradicional = "Fin de semana"; break;
}
Console.WriteLine($"  Switch tradicional: día {diaSemana} → {nombreDiaTradicional}");

// --- Switch expression (C# 8+) ---
// Más conciso: devuelve un valor, usa => y _ para default
var nombreDiaExpresion = diaSemana switch
{
    1 => "Lunes",
    2 => "Martes",
    3 => "Miércoles",
    4 => "Jueves",
    5 => "Viernes",
    _ => "Fin de semana" // _ es el patrón descarte (default)
};
Console.WriteLine($"  Switch expression: día {diaSemana} → {nombreDiaExpresion}");

// Switch expression con múltiples patrones (combinados con 'or')
int numero = 7;

var categoria = numero switch
{
    1 or 2 or 3 => "Bajo",      // Patrón lógico 'or' agrupa varios casos
    4 or 5 or 6 => "Medio",
    7 or 8 or 9 => "Alto",
    _ => "Fuera de rango"
};
Console.WriteLine($"  Número {numero} está en rango: {categoria}");

Console.WriteLine();

// ────────────────────────────────────────────────────────────────────
// 4. Cláusula "when" para patrones condicionales
// ────────────────────────────────────────────────────────────────────
// "when" añade una condición booleana adicional al patrón.
// Solo se ejecuta la rama si el patrón Y la condición se cumplen.
Console.WriteLine("=== 4. Cláusula 'when' ===");

int edad = 20;

// Sin "when": switch por rangos necesitaba if/else o código en el case
// Con "when": la condición va directamente en el case
var etapa = edad switch
{
    < 0 => "Edad inválida",
    >= 0 and < 13 when DateTime.Now.Year > 2025 => "Niño (con when: año actual > 2025)", // when añade condición extra
    >= 0 and < 13 => "Niño",
    >= 13 and < 18 => "Adolescente",
    >= 18 and < 65 => "Adulto",
    _ => "Senior"
};
Console.WriteLine($"  Edad {edad}: {etapa}");

// "when" en un case tradicional también funciona
object objeto = "hola";

switch (objeto)
{
    case string str when str.Length > 3:
        Console.WriteLine($"  String con más de 3 caracteres: '{str}' ({str.Length})");
        break;
    case string str:
        Console.WriteLine($"  String corto: '{str}'");
        break;
    default:
        Console.WriteLine("  Otro tipo");
        break;
}

Console.WriteLine();

// ────────────────────────────────────────────────────────────────────
// 5. Patrones de propiedad (Property patterns)
// ────────────────────────────────────────────────────────────────────
// Permiten hacer match sobre las propiedades de un objeto.
// Se usa la sintaxis { Propiedad: patrón }.
Console.WriteLine("=== 5. Patrones de propiedad ===");

// Definimos un record para demostrar property patterns
var persona1 = new Persona("Ana", 25);
var persona2 = new Persona("Carlos", 17);
var persona3 = new Persona("Ana", 15);

// Property pattern: verifica propiedades dentro de un objeto
var descripcion = persona1 switch
{
    // { Nombre: "Ana", Edad: > 18 } → patrón anidado dentro de propiedades
    { Nombre: "Ana", Edad: > 18 } => "Ana es mayor de edad",
    { Nombre: "Ana" } => "Ana (sin importar la edad)",
    { Edad: > 18 } => "Mayor de edad (cualquier nombre)",
    _ => "No cumple ningún criterio"
};
Console.WriteLine($"  persona1 (Ana, 25): {descripcion}");

descripcion = persona2 switch
{
    { Nombre: "Ana", Edad: > 18 } => "Ana es mayor de edad",
    { Edad: > 18 } => "Mayor de edad (cualquier nombre)",
    _ => "No cumple ningún criterio"
};
Console.WriteLine($"  persona2 (Carlos, 17): {descripcion}");

descripcion = persona3 switch
{
    { Nombre: "Ana", Edad: > 18 } => "Ana es mayor de edad",
    { Nombre: "Ana" } => "Ana (sin importar la edad)",
    _ => "No cumple ningún criterio"
};
Console.WriteLine($"  persona3 (Ana, 15): {descripcion}");

// Property pattern anidado: propiedades dentro de propiedades
var direccion = new Direccion("Madrid", "España");
var cliente = new Cliente("Juan", direccion);

var infoCliente = cliente switch
{
    // Patrón anidado: accede a Direccion.Ciudad dentro de Cliente
    { Direccion: { Ciudad: "Madrid" } } => "Cliente de Madrid",
    { Direccion: { Pais: "España" } } => "Cliente de España (otra ciudad)",
    _ => "Cliente internacional"
};
Console.WriteLine($"  Cliente Juan (Madrid, España): {infoCliente}");

Console.WriteLine();

// ────────────────────────────────────────────────────────────────────
// 6. Patrones de lista (List patterns) — C# 11+
// ────────────────────────────────────────────────────────────────────
// Permiten hacer match sobre arrays, List<T>, Span<T> y otros tipos
// indexables con patrones posicionales.
Console.WriteLine("=== 6. Patrones de lista (List patterns) ===");

int[] numeros = { 1, 2, 3 };

// Patrón de lista con longitud fija: coincide si tiene exactamente esos elementos
var resultadoLista = numeros switch
{
    [1, 2, 3] => "Array exacto [1, 2, 3]",
    [1, 2] => "Array [1, 2]",
    _ => "Otro array"
};
Console.WriteLine($"  [1, 2, 3]: {resultadoLista}");

numeros = new[] { 1, 2, 3, 4, 5 };

// Patrón de lista con slice (..): coincide con cero o más elementos en esa posición
resultadoLista = numeros switch
{
    [1, ..] => "Empieza con 1 (el slice '..' captura el resto)", // .. captura 0 o más elementos
    [.., 5] => "Termina en 5",
    _ => "Otra secuencia"
};
Console.WriteLine($"  [1,2,3,4,5]: {resultadoLista}");

// Patrón con descarte (_) y captura de variable en lista
string[] frutas = { "manzana", "naranja", "plátano" };

var descripcionFrutas = frutas switch
{
    // _ descarta el primer elemento, var segunda captura el segundo
    [_, var segunda, ..] => $"Segunda fruta: {segunda}, resto con slice",
    [] => "Lista vacía",
    _ => "Otra lista"
};
Console.WriteLine($"  ['manzana','naranja','plátano']: {descripcionFrutas}");

// Lista vacía
int[] vacia = Array.Empty<int>();
resultadoLista = vacia switch
{
    [] => "Lista vacía detectada", // [] coincide con lista sin elementos
    [var unico] => $"Lista con un elemento: {unico}",
    [var primero, var segundo] => $"Dos elementos: {primero}, {segundo}",
    _ => "Lista con 3 o más elementos"
};
Console.WriteLine($"  Array.Empty<int>(): {resultadoLista}");

Console.WriteLine();

// ────────────────────────────────────────────────────────────────────
// 7. Patrones relacionales: <, >, <=, >=
// ────────────────────────────────────────────────────────────────────
// Comparan valores numéricos (o cualquier tipo con operadores de comparación)
// directamente en el patrón.
Console.WriteLine("=== 7. Patrones relacionales ===");

int puntuacion = 85;

// Los operadores relacionales se pueden usar directamente en patrones
var calificacion = puntuacion switch
{
    >= 90 => "Sobresaliente",
    >= 80 and < 90 => "Notable",   // Combinación de relacionales con 'and'
    >= 70 and < 80 => "Bien",
    >= 60 and < 70 => "Suficiente",
    >= 0 and < 60 => "Insuficiente",
    < 0 => "Puntuación inválida",
};
Console.WriteLine($"  Puntuación {puntuacion}: {calificacion}");

// También se pueden usar con 'is' y con tipos que implementan operadores
double temperatura = 38.5;

var estadoTemperatura = temperatura switch
{
    <= 0 => "Congelación",
    > 0 and <= 15 => "Frío",
    > 15 and <= 25 => "Templado",
    > 25 and <= 35 => "Calor",
    > 35 => "Mucho calor",
    _ => "Indefinida"  // Captura double.NaN y otros valores no cubiertos
};
Console.WriteLine($"  Temperatura {temperatura}°C: {estadoTemperatura}");

Console.WriteLine();

// ────────────────────────────────────────────────────────────────────
// 8. Patrones lógicos: "and", "or", "not"
// ────────────────────────────────────────────────────────────────────
// Combinan patrones con operadores lógicos para mayor expresividad.
Console.WriteLine("=== 8. Patrones lógicos: and, or, not ===");

char letra = 'E';

// 'and': ambos patrones deben cumplirse
// 'or': al menos un patrón debe cumplirse
// 'not': el patrón NO debe cumplirse
var tipoLetra = letra switch
{
    >= 'A' and <= 'Z' => "Mayúscula",             // 'and' combina dos patrones relacionales
    >= 'a' and <= 'z' => "Minúscula",
    >= '0' and <= '9' => "Dígito",
    _ => "Otro carácter"
};
Console.WriteLine($"  Letra '{letra}': {tipoLetra}");

// 'or' para agrupar múltiples casos
int mes = 7;

var estacion = mes switch
{
    12 or 1 or 2 => "Invierno",     // 'or' agrupa varios valores
    3 or 4 or 5 => "Primavera",
    6 or 7 or 8 => "Verano",
    9 or 10 or 11 => "Otoño",
    _ => "Mes inválido"
};
Console.WriteLine($"  Mes {mes}: {estacion}");

// 'not' para negar un patrón
object? valorNulo = null;

var esNulo = valorNulo switch
{
    not null => "Tiene valor",      // 'not null' equivale a "no es null"
    null => "Es null"
};
Console.WriteLine($"  valorNulo: {esNulo}");

// 'not' también puede negar patrones compuestos
int valorNumerico = 42;

var esEspecial = valorNumerico switch
{
    not (>= 0 and <= 100) => "Fuera del rango 0-100",
    _ => "Dentro del rango 0-100"
};
Console.WriteLine($"  {valorNumerico}: {esEspecial}");

Console.WriteLine();

// ────────────────────────────────────────────────────────────────────
// 9. Patrones de tupla (Tuple patterns)
// ────────────────────────────────────────────────────────────────────
// Permiten hacer match sobre tuplas, comparando cada elemento
// con un patrón independiente.
Console.WriteLine("=== 9. Patrones de tupla ===");

// Patrones en tuplas: cada posición se compara con un patrón
(int X, int Y) punto = (0, 0);

var posicion = punto switch
{
    (0, 0) => "Origen",                     // Ambos elementos son 0
    (0, _) => "Sobre el eje Y",             // X=0, Y vale cualquier cosa (descartado)
    (_, 0) => "Sobre el eje X",             // X cualquier cosa, Y=0
    ( > 0, > 0) => "Cuadrante I",          // X>0, Y>0
    ( < 0, > 0) => "Cuadrante II",         // X<0, Y>0
    ( < 0, < 0) => "Cuadrante III",        // X<0, Y<0
    ( > 0, < 0) => "Cuadrante IV",         // X>0, Y<0
};
Console.WriteLine($"  Punto {punto}: {posicion}");

punto = (-3, 7);
posicion = punto switch
{
    (0, 0) => "Origen",
    (0, _) => "Sobre el eje Y",
    (_, 0) => "Sobre el eje X",
    ( > 0, > 0) => "Cuadrante I",
    ( < 0, > 0) => "Cuadrante II",
    ( < 0, < 0) => "Cuadrante III",
    ( > 0, < 0) => "Cuadrante IV",
};
Console.WriteLine($"  Punto {punto}: {posicion}");

// Tupla de tres elementos
(int A, int B, int C) terna = (1, 2, 3);

var descripcionTerna = terna switch
{
    (1, 2, 3) => "Secuencia 1-2-3",
    (var x, var y, var z) when x < y && y < z => $"Creciente: {x} < {y} < {z}",
    _ => "Sin patrón específico"
};
Console.WriteLine($"  Terna {terna}: {descripcionTerna}");

Console.WriteLine();

// ────────────────────────────────────────────────────────────────────
// 10. Pattern matching sobre Span<char> y ReadOnlySpan<char>
// ────────────────────────────────────────────────────────────────────
// C# 11+ permite pattern matching sobre Span<T> y ReadOnlySpan<T>,
// lo cual es muy útil para parsear strings sin asignaciones en heap.
Console.WriteLine("=== 10. Pattern matching sobre Span<char> ===");

string texto = "Hola Mundo";

// Convertir a ReadOnlySpan<char> (stack-friendly, sin heap allocation)
ReadOnlySpan<char> span = texto.AsSpan();

// El pattern matching sobre Span es eficiente: no crea substrings
var resultadoSpan = span switch
{
    // Verifica si el span empieza con 'H'
    ['H', ..] => "Empieza con 'H'",
    _ => "No empieza con 'H'"
};
Console.WriteLine($"  Span \"{texto}\": {resultadoSpan}");

// Parseo de un identificador de versión con Span
ReadOnlySpan<char> versionSpan = "v2.1.0".AsSpan();

var versionInfo = versionSpan switch
{
    // ['v', .. var resto, '.', ..] — pero Span no funciona así exactamente
    // En su lugar, usamos la posición y longitud
    ['v', ..] => "Prefijo de versión detectado (v)",
    _ => "Sin prefijo de versión"
};
Console.WriteLine($"  Span \"v2.1.0\": {versionInfo}");

// Demostración de eficiencia: Span no aloca en heap
// Mientras que string.Substring() crea un nuevo string en el heap,
// Span.Slice() solo crea una vista (ventana) sobre la memoria existente.
Span<char> stackSpan = stackalloc char[5] { 'H', 'e', 'l', 'l', 'o' };

var saludoCheck = stackSpan switch
{
    ['H', 'e', 'l', 'l', 'o'] => "¡Coincidencia exacta! Hello detectado",
    _ => "No coincide"
};
Console.WriteLine($"  Span stackalloc \"Hello\": {saludoCheck}");

Console.WriteLine();

// ────────────────────────────────────────────────────────────────────
// Resumen final
// ────────────────────────────────────────────────────────────────────
Console.WriteLine("=== Resumen ===");
Console.WriteLine("""
    Pattern Matching en C# moderno permite escribir código más:
    - Declarativo: describes QUÉ quieres, no CÓMO comprobarlo
    - Conciso: menos líneas, menos casts, menos if-else anidados
    - Seguro: el compilador verifica exhaustividad en switch expressions
    - Eficiente: Span evita asignaciones en heap

    Formas de pattern matching cubiertas:
    1.  'is' con verificación de tipo
    2.  'is' con declaración de variable
    3.  Switch expression (vs switch tradicional)
    4.  Cláusula 'when'
    5.  Patrones de propiedad { Prop: patrón }
    6.  Patrones de lista [1, 2, ..]
    7.  Patrones relacionales < > <= >=
    8.  Patrones lógicos and / or / not
    9.  Patrones de tupla (x, y)
    10. Pattern matching sobre Span<char>
    """);

// ────────────────────────────────────────────────────────────────────
// Tipos auxiliares usados en los ejemplos de property patterns
// ────────────────────────────────────────────────────────────────────

// Record: tipo inmutable ideal para pattern matching
// Los records generan Deconstruct() automáticamente, lo que
// permite usarlos en patrones posicionales.
internal record Persona(string Nombre, int Edad);

// Clase con propiedad de tipo record (para patrones anidados)
internal record Direccion(string Ciudad, string Pais);

internal record Cliente(string Nombre, Direccion Direccion);
