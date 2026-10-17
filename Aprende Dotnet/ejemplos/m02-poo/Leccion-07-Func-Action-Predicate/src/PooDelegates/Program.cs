// =============================================================================
// Módulo 2 — POO en C# en profundidad
// Lección 07 — Func, Action, Predicate (delegados predefinidos)
// =============================================================================
// En esta lección aprenderás:
//   - Qué son Func<T>, Action<T> y Predicate<T>
//   - Cómo usarlos para evitar declarar delegados personalizados
//   - Cuándo usar cada uno
//   - Cuándo SÍ conviene declarar tu propio delegado
//   - Ejemplos reales: pipelines de datos, validación, mapeo, callbacks
// =============================================================================

using System.Text;

// =============================================================================
// NOTA: Las instrucciones top-level DEBEN ir antes de cualquier tipo.
// =============================================================================

PrintSeparator("1. ACTION<T> — Callbacks sin retorno");

// Action sin parámetros: ejecuta una acción simple
Action sayHello = () => Console.WriteLine("  ¡Hola desde Action!");
sayHello();

// Action<T> con 1 parámetro: recibe un string y lo imprime
Action<string> print = message => Console.WriteLine($"  {message}");
print("Action<string> recibe un parámetro y no devuelve nada.");

// Action<T1, T2> con 2 parámetros: repite un texto n veces
Action<string, int> repeat = (text, times) =>
{
    for (int i = 0; i < times; i++)
    {
        Console.WriteLine($"    [{i + 1}] {text}");
    }
};
Console.WriteLine("  Action<string, int>:");
repeat("Línea", 2);

// ---- FileProcessor con Action<int> como callback de progreso ----
Console.WriteLine("\n  --- FileProcessor con Action<int> (progreso) ---");
FileProcessor fileProcessor = new FileProcessor();

// Definimos el callback que dibuja una barra de progreso en consola
Action<int> progressBar = percent =>
{
    int filled = percent / 10;
    string bar = new string('#', filled) + new string('-', 10 - filled);
    Console.WriteLine($"    [{bar}] {percent}%");
};

fileProcessor.Process("documento.pdf", progressBar);

// ---- FileProcessor con Action sin parámetros (backup) ----
Console.WriteLine("\n  --- FileProcessor con Action (backup) ---");
fileProcessor.RunBackup(() => Console.WriteLine("    [OK] Backup completado."));

// ---- FileProcessor con Action<string, string> (log) ----
Console.WriteLine("\n  --- FileProcessor con Action<string, string> (log) ---");
fileProcessor.ProcessWithLog("datos.csv", (level, msg) =>
    Console.WriteLine($"    [{level}] {msg}"));

// =============================================================================

PrintSeparator("2. FUNC<T, TResult> — Funciones con retorno");

// Func<TResult>: sin parámetros, devuelve un valor
Func<int> rollDice = () => Random.Shared.Next(1, 7); // Dado de 6 caras
Console.WriteLine($"  Tirada de dado: {rollDice()}");

// Func<T, TResult>: recibe un string, devuelve su longitud
Func<string, int> length = s => s.Length;
Console.WriteLine($"  Longitud de 'Hola Mundo': {length("Hola Mundo")}");

// Func<T1, T2, TResult>: recibe dos ints, devuelve int
Func<int, int, int> add = (a, b) => a + b;
Func<int, int, int> multiply = (a, b) => a * b;
Console.WriteLine($"  Suma 3 + 7 = {add(3, 7)}");
Console.WriteLine($"  Producto 3 * 7 = {multiply(3, 7)}");

// ---- Mapper: transformación de datos con Func ----
Console.WriteLine("\n  --- Mapper: transformar lista con Func ---");
Mapper mapper = new Mapper();
List<int> sourceNumbers = new List<int> { 1, 2, 3, 4, 5 };

// Transforma cada número a su representación en texto
List<string> numberTexts = mapper.Map(sourceNumbers, n => $"Número {n}");
Console.WriteLine("  Números originales: " + string.Join(", ", sourceNumbers));
Console.WriteLine("  Transformados: " + string.Join(", ", numberTexts));

// Transforma cada número a su cuadrado
List<int> squares = mapper.Map(sourceNumbers, n => n * n);
Console.WriteLine("  Cuadrados: " + string.Join(", ", squares));

// ---- Calculator: operaciones como Func ----
Console.WriteLine("\n  --- Calculator: operaciones como parámetro ---");
Calculator calculator = new Calculator();

int sumResult = calculator.Execute(15, 27, (a, b) => a + b);
int multiplyResult = calculator.Execute(15, 27, (a, b) => a * b);
int maxResult = calculator.Execute(15, 27, (a, b) => a > b ? a : b);

Console.WriteLine($"  15 + 27 = {sumResult}");
Console.WriteLine($"  15 * 27 = {multiplyResult}");
Console.WriteLine($"  Max(15, 27) = {maxResult}");

// Transform con Func<int, int>
int squared = calculator.Transform(9, x => x * x);
int doubled = calculator.Transform(9, x => x * 2);
Console.WriteLine($"  9 al cuadrado = {squared}");
Console.WriteLine($"  9 duplicado = {doubled}");

// Create con Func<TResult>: fábrica sin parámetros
string guid = calculator.Create(() => Guid.NewGuid().ToString("N")[..8]);
Console.WriteLine($"  ID generado: {guid}");

// =============================================================================

PrintSeparator("3. PREDICATE<T> — Condiciones (Func<T, bool>)");

Predicate<int> isPositive = n => n > 0;
Predicate<int> isEven = n => n % 2 == 0;
Predicate<string> isNotEmpty = s => !string.IsNullOrWhiteSpace(s);

Console.WriteLine($"  ¿5 es positivo? {isPositive(5)}");
Console.WriteLine($"  ¿-3 es positivo? {isPositive(-3)}");
Console.WriteLine($"  ¿4 es par? {isEven(4)}");
Console.WriteLine($"  ¿'Hola' no está vacío? {isNotEmpty("Hola")}");
Console.WriteLine($"  ¿'' no está vacío? {isNotEmpty("")}");

// ---- FilterService con Predicate<T> ----
Console.WriteLine("\n  --- FilterService: filtrar con Predicate ---");
FilterService filterService = new FilterService();
List<int> allNumbers = new List<int> { -10, -5, 0, 3, 8, 15, 22, 37, 50, 64, 71, 99 };

List<int> onlyPositives = filterService.Filter(allNumbers, n => n > 0);
Console.WriteLine("  Solo positivos: " + string.Join(", ", onlyPositives));

List<int> multiplesOf5 = filterService.Filter(allNumbers, n => n % 5 == 0);
Console.WriteLine("  Múltiplos de 5: " + string.Join(", ", multiplesOf5));

// ---- Métodos nativos de List<T> con Predicate ----
Console.WriteLine("\n  --- Métodos nativos de List<T> con Predicate ---");
filterService.DemonstrateBuiltInPredicates(allNumbers);

// =============================================================================

PrintSeparator("4. DELEGADO CUSTOM vs FUNC/ACTION");

// Ambos enfoques funcionan, pero el delegado personalizado
// comunica mejor la INTENCIÓN del código.
OrderService orderService = new OrderService();
Order sampleOrder = new Order(Guid.NewGuid(), 99.99m, "Pending");

// ---- Con delegado personalizado (más expresivo) ----
Console.WriteLine("  Usando delegado personalizado OrderProcessedHandler:");
OrderProcessedHandler customHandler = order =>
    Console.WriteLine($"    [Custom] Orden {order.Id} procesada. Estado: {order.Status}");

orderService.ProcessOrderCustom(sampleOrder, customHandler);

// ---- Con Action<Order> (menos expresivo pero funcional) ----
Console.WriteLine("\n  Usando Action<Order> (menos semántico):");
Action<Order> actionHandler = order =>
    Console.WriteLine($"    [Action]  Orden {order.Id} procesada. Estado: {order.Status}");

orderService.ProcessOrderAction(sampleOrder, actionHandler);

// ---- Comparación con AgeValidator ----
Console.WriteLine("\n  --- Validación de edad con delegado custom vs Func ---");
// Delegado custom: el nombre AgeValidator expresa claramente el propósito
AgeValidator isAdultCustom = age => age >= 18;

// Func<int, bool>: técnicamente igual, pero no comunica qué valida
Func<int, bool> isAdultFunc = age => age >= 18;

bool resultCustom = orderService.IsValidAge(25, isAdultCustom);
bool resultFunc = orderService.IsValidAgeFunc(25, isAdultFunc);
Console.WriteLine($"  ¿25 es adulto? (custom): {resultCustom}");
Console.WriteLine($"  ¿25 es adulto? (Func):   {resultFunc}");

Console.WriteLine("\n  REGLA: Usa delegado custom cuando el NOMBRE aporta semántica de dominio.");
Console.WriteLine("  Usa Func/Action para operaciones genéricas (transformar, filtrar, callbacks).");

// =============================================================================

PrintSeparator("5. DATA PIPELINE — Combinando Predicate + Func + Action");

DataPipeline pipeline = new DataPipeline();
List<int> rawData = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

Console.WriteLine($"  Datos originales: {string.Join(", ", rawData)}");

// Pipeline completo:
//   - Filtra: solo números pares
//   - Transforma: eleva al cuadrado
//   - Reporta: imprime cada resultado procesado
List<int> pipelineResult = pipeline.Process(
    data: rawData,
    filter: n => n % 2 == 0,                           // Predicate<int>: solo pares
    transform: n => n * n,                              // Func<int, int>: elevar al cuadrado
    onEach: n => Console.WriteLine($"    -> Procesado: {n}") // Action<int>: reportar
);

Console.WriteLine($"  Resultado del pipeline: {string.Join(", ", pipelineResult)}");

// Otro pipeline: números impares, multiplicar por 10, acumular en silencio
List<int> pipeline2 = pipeline.Process(
    data: rawData,
    filter: n => n % 2 != 0,    // Solo impares
    transform: n => n * 10,     // Multiplicar por 10
    onEach: _ => { }            // Action<int> vacío: no reportar
);
Console.WriteLine($"  Pipeline 2 (impares x10): {string.Join(", ", pipeline2)}");

// =============================================================================

PrintSeparator("6. SISTEMA DE VALIDACIÓN — Validator<T> con Predicate");

Validator<string> stringValidator = new Validator<string>();

// Definimos reglas de validación como Predicate<string>
Predicate<string> notEmpty = s => !string.IsNullOrWhiteSpace(s);
Predicate<string> minLength3 = s => s.Length >= 3;
Predicate<string> maxLength20 = s => s.Length <= 20;
Predicate<string> noSpaces = s => !s.Contains(' ');
Predicate<string> startsWithLetter = s => char.IsLetter(s[0]);

// Probamos diferentes cadenas contra las reglas
string[] testStrings = { "Hola", "ab", "", "muy largoooooooooooooo", "con espacios", "123abc" };

foreach (string test in testStrings)
{
    bool isValid = stringValidator.Validate(
        test,
        notEmpty,
        minLength3,
        maxLength20,
        noSpaces,
        startsWithLetter);

    string display = string.IsNullOrEmpty(test) ? "(vacío)" : test;
    Console.WriteLine($"  '{display}' -> {(isValid ? "VÁLIDO" : "INVÁLIDO")}");
}

// Validator<T> también funciona con enteros
Validator<int> intValidator = new Validator<int>();
Predicate<int> positive = n => n > 0;
Predicate<int> lessThan100 = n => n < 100;
Predicate<int> even = n => n % 2 == 0;

Console.WriteLine("\n  --- Validación de enteros ---");
int[] testInts = { 5, -3, 150, 42, 0, 99 };

foreach (int n in testInts)
{
    bool isValid = intValidator.Validate(n, positive, lessThan100);
    Console.WriteLine($"  {n} -> {(isValid ? "VÁLIDO (>0 y <100)" : "INVÁLIDO")}");
}

// =============================================================================

PrintSeparator("7. RESUMEN — Cuándo usar cada uno");

Console.WriteLine("""
  ┌──────────────┬──────────────────────────────────────────────┐
  │ Action<T>    │ Métodos que NO devuelven valor (void)        │
  │              │ Callbacks, logging, notificaciones, eventos  │
  │              │ Hasta 16 parámetros de entrada               │
  ├──────────────┼──────────────────────────────────────────────┤
  │ Func<T,TR>   │ Métodos que SÍ devuelven valor               │
  │              │ Transformaciones, fábricas, consultas        │
  │              │ Hasta 16 parámetros de entrada + 1 retorno   │
  ├──────────────┼──────────────────────────────────────────────┤
  │ Predicate<T> │ Métodos que devuelven bool (condición)       │
  │              │ Equivalente a Func<T, bool>                  │
  │              │ List<T>.FindAll, RemoveAll, Exists, etc.     │
  ├──────────────┼──────────────────────────────────────────────┤
  │ Delegado     │ Cuando el NOMBRE aporta semántica de dominio │
  │ custom       │ Ej: delegate bool AgeValidator(int age);    │
  │              │ Mejor legibilidad que Func<int, bool>        │
  └──────────────┴──────────────────────────────────────────────┘
  """);

PrintSeparator("FIN DE LA LECCIÓN");

// =============================================================================
// Utilidad: función local para imprimir separadores en consola
// =============================================================================
static void PrintSeparator(string title)
{
    Console.WriteLine();
    Console.WriteLine(new string('=', 72));
    Console.WriteLine($"  {title}");
    Console.WriteLine(new string('=', 72));
}

// =============================================================================
// TIPOS — Declaraciones de clases, records y delegados
// (Deben ir DESPUÉS de las instrucciones top-level)
// =============================================================================

// Registro simple que representa una orden de compra.
// Se usa para demostrar delegados con tipos de dominio reales.
public record Order(Guid Id, decimal Total, string Status);

// =============================================================================
// 2. ACTION — Callbacks sin retorno
// =============================================================================

/// <summary>
/// FileProcessor recibe una ruta y un callback Action para reportar progreso.
/// Action<T> encapsula un método que recibe T y no devuelve nada (void).
/// Ideal para: callbacks de progreso, logging, notificaciones.
/// </summary>
public class FileProcessor
{
    /// <summary>
    /// Simula el procesamiento de un archivo informando el porcentaje de avance.
    /// onProgress: delegado que recibe el porcentaje (0-100) y lo reporta.
    /// </summary>
    public void Process(string path, Action<int> onProgress)
    {
        Console.WriteLine($"  [FileProcessor] Iniciando procesamiento de: {path}");

        // Simulamos 5 pasos de procesamiento
        for (int step = 1; step <= 5; step++)
        {
            Thread.Sleep(100); // Simula trabajo
            int percent = step * 20;
            onProgress(percent); // Invocamos el callback para notificar avance
        }

        Console.WriteLine($"  [FileProcessor] Procesamiento completado.");
    }

    /// <summary>
    /// Action sin parámetros: simplemente ejecuta algo, no recibe ni devuelve datos.
    /// </summary>
    public void RunBackup(Action onComplete)
    {
        Console.WriteLine("  [FileProcessor] Ejecutando backup...");
        Thread.Sleep(150);
        onComplete(); // Notifica que terminó
    }

    /// <summary>
    /// Action con 2 parámetros: útil para logs con nivel y mensaje.
    /// </summary>
    public void ProcessWithLog(
        string path,
        Action<string, string> logCallback)
    {
        logCallback("INFO", $"Iniciando {path}");
        Thread.Sleep(100);
        logCallback("INFO", $"Finalizado {path}");
    }
}

// =============================================================================
// 3. FUNC — Funciones con retorno
// =============================================================================

/// <summary>
/// Mapper: aplica una función de transformación a cada elemento de una lista.
/// Func<TSource, TDestination>: recibe TSource, devuelve TDestination.
/// El ÚLTIMO parámetro de tipo SIEMPRE es el tipo de retorno.
/// </summary>
public class Mapper
{
    /// <summary>
    /// Transforma cada elemento de la lista source usando la función transform.
    /// </summary>
    public List<TDestination> Map<TSource, TDestination>(
        List<TSource> source,
        Func<TSource, TDestination> transform)
    {
        List<TDestination> result = new List<TDestination>(source.Count);

        foreach (TSource item in source)
        {
            result.Add(transform(item));
        }

        return result;
    }
}

/// <summary>
/// Calculator: ejecuta operaciones matemáticas definidas como Func.
/// En lugar de tener un método por operación, recibe la operación como parámetro.
/// Esto es el Strategy Pattern implementado con delegados.
/// </summary>
public class Calculator
{
    /// <summary>
    /// Ejecuta una operación binaria sobre dos operandos y devuelve el resultado.
    /// </summary>
    public int Execute(int a, int b, Func<int, int, int> operation)
    {
        return operation(a, b);
    }

    /// <summary>
    /// Aplica una transformación unaria a un número.
    /// </summary>
    public int Transform(int value, Func<int, int> transformation)
    {
        return transformation(value);
    }

    /// <summary>
    /// Genera un valor usando una fábrica sin parámetros (Func<TResult> sin entradas).
    /// </summary>
    public T Create<T>(Func<T> factory)
    {
        return factory();
    }
}

// =============================================================================
// 4. PREDICATE — Condiciones que devuelven bool
// =============================================================================

/// <summary>
/// FilterService: demuestra el uso de Predicate<T> para filtrar colecciones.
/// Predicate<T> es equivalente a Func<T, bool>, pero semánticamente indica
/// que se trata de una condición / prueba lógica.
/// Se usa principalmente con List<T>.FindAll(), RemoveAll(), Exists(), etc.
/// </summary>
public class FilterService
{
    /// <summary>
    /// Filtra una lista usando un Predicate<T> (condición).
    /// </summary>
    public List<int> Filter(List<int> numbers, Predicate<int> condition)
    {
        List<int> result = new List<int>();

        foreach (int number in numbers)
        {
            if (condition(number))
            {
                result.Add(number);
            }
        }

        return result;
    }

    /// <summary>
    /// Demuestra los métodos nativos de List<T> que usan Predicate<T>.
    /// </summary>
    public void DemonstrateBuiltInPredicates(List<int> numbers)
    {
        Console.WriteLine("  Lista original: " + string.Join(", ", numbers));

        // FindAll devuelve todos los elementos que cumplen la condición
        List<int> evens = numbers.FindAll(n => n % 2 == 0);
        Console.WriteLine("  Pares (FindAll): " + string.Join(", ", evens));

        // Exists verifica si AL MENOS un elemento cumple la condición
        bool hasNegative = numbers.Exists(n => n < 0);
        Console.WriteLine($"  ¿Hay negativos? (Exists): {hasNegative}");

        // Find devuelve el PRIMER elemento que cumple la condición
        int firstGreaterThan50 = numbers.Find(n => n > 50);
        Console.WriteLine($"  Primer > 50 (Find): {firstGreaterThan50}");

        // TrueForAll verifica que TODOS los elementos cumplan la condición
        bool allPositive = numbers.TrueForAll(n => n > 0);
        Console.WriteLine($"  ¿Todos positivos? (TrueForAll): {allPositive}");
    }
}

// =============================================================================
// 5. DELEGADO CUSTOM vs FUNC/ACTION
// =============================================================================

/// <summary>
/// Delegado personalizado para validación de edad.
/// Ventaja: el nombre del delegado (AgeValidator) y el nombre del parámetro (age)
/// comunican la intención mejor que Func<int, bool>.
/// </summary>
public delegate bool AgeValidator(int age);

/// <summary>
/// Delegado personalizado para manejar eventos de órdenes procesadas.
/// El nombre OrderProcessedHandler y el tipo Order dan contexto semántico.
/// Un Action<Order> sería técnicamente igual, pero menos legible.
/// </summary>
public delegate void OrderProcessedHandler(Order order);

/// <summary>
/// OrderService: demuestra la diferencia entre delegados personalizados y Func/Action.
/// </summary>
public class OrderService
{
    /// <summary>
    /// Versión con delegado personalizado: más expresiva.
    /// </summary>
    public void ProcessOrderCustom(Order order, OrderProcessedHandler onProcessed)
    {
        Console.WriteLine($"  Procesando orden {order.Id} por ${order.Total}...");
        Thread.Sleep(50);

        // El delegado personalizado deja claro qué representa el callback
        onProcessed(order);
    }

    /// <summary>
    /// Versión con Action<T>: técnicamente idéntica, menos semántica.
    /// </summary>
    public void ProcessOrderAction(Order order, Action<Order> onProcessed)
    {
        Console.WriteLine($"  Procesando orden {order.Id} por ${order.Total}...");
        Thread.Sleep(50);
        onProcessed(order);
    }

    /// <summary>
    /// Validación de edad con delegado personalizado AgeValidator.
    /// </summary>
    public bool IsValidAge(int age, AgeValidator validator)
    {
        return validator(age);
    }

    /// <summary>
    /// Validación de edad con Func<int, bool>: técnicamente idéntica.
    /// </summary>
    public bool IsValidAgeFunc(int age, Func<int, bool> validator)
    {
        return validator(age);
    }
}

// =============================================================================
// 6. EJEMPLO REAL — Pipeline de procesamiento de datos
// =============================================================================

/// <summary>
/// DataPipeline demuestra cómo combinar Predicate, Func y Action
/// para crear un pipeline de procesamiento de datos flexible.
/// - Predicate<int> filter: decide qué datos procesar
/// - Func<int, int> transform: transforma cada dato
/// - Action<int> onEach: callback que se ejecuta para cada elemento procesado
/// </summary>
public class DataPipeline
{
    public List<int> Process(
        List<int> data,
        Predicate<int> filter,
        Func<int, int> transform,
        Action<int> onEach)
    {
        List<int> results = new List<int>();

        foreach (int item in data)
        {
            // Paso 1: Filtrar — solo procesamos si cumple la condición
            if (!filter(item))
                continue;

            // Paso 2: Transformar — aplicamos la función de transformación
            int transformed = transform(item);

            // Paso 3: Notificar — ejecutamos el callback con el resultado
            onEach(transformed);

            results.Add(transformed);
        }

        return results;
    }
}

// =============================================================================
// 7. EJEMPLO REAL — Sistema de validación compuesto
// =============================================================================

/// <summary>
/// Validator<T> permite componer múltiples reglas de validación con Predicate<T>.
/// Cada regla es una condición que debe cumplirse.
/// El método Validate devuelve true solo si TODAS las reglas se cumplen.
/// </summary>
public class Validator<T>
{
    /// <summary>
    /// Valida un item contra un conjunto de reglas.
    /// params permite pasar cualquier cantidad de Predicate<T>.
    /// Devuelve true si TODAS las reglas se cumplen (AND lógico).
    /// </summary>
    public bool Validate(T item, params Predicate<T>[] rules)
    {
        foreach (Predicate<T> rule in rules)
        {
            if (!rule(item))
            {
                return false; // Basta con que una regla falle
            }
        }

        return true;
    }
}
