# Lección 7 — Func, Action, Predicate (delegados predefinidos)

---

## 1. Introducción

Cada vez que necesitas un delegado en C#, el primer instinto de muchos desarrolladores es declarar un `delegate` personalizado. Pero en el 90% de los casos **no hace falta**: el BCL incluye `Func`, `Action` y `Predicate`, tres familias de delegados genéricos predefinidos en `System` que cubren prácticamente todos los escenarios comunes.

- **`Action`** encapsula métodos **sin retorno** (`void`). Hasta 16 parámetros de entrada.
- **`Func`** encapsula métodos **con retorno**. Hasta 16 parámetros de entrada + 1 de retorno.
- **`Predicate<T>`** encapsula métodos que devuelven `bool`. Es semánticamente equivalente a `Func<T, bool>`, pero comunica que se trata de una **condición o test**.

**¿Por qué existen?** Para evitar la proliferación de delegados personalizados. Antes de .NET 2.0, cada callback requería su propio `delegate`. Con `Func` y `Action`, el BCL y tu propio código pueden expresar cualquier firma de método sin declarar nada nuevo.

**¿Cuándo se utilizan?** Son **ubicuos** en el ecosistema .NET:

| Contexto | Delegado usado |
|---|---|
| LINQ (`Where`, `Select`, `GroupBy`) | `Func<T, bool>`, `Func<T, TResult>` |
| `Task.Run`, `ThreadPool.QueueUserWorkItem` | `Action` |
| `List<T>.FindAll`, `RemoveAll`, `Exists` | `Predicate<T>` |
| Inyección de dependencias (factories) | `Func<T>` |
| Middleware, pipelines, eventos | `Action<T>`, `Func<T, Task>` |

**Errores típicos**:
- Usar `Predicate<T>` donde LINQ espera `Func<T, bool>` (son compatibles en asignación, pero `Where` pide `Func`).
- Olvidar que el **último** parámetro de tipo de `Func` es **siempre el retorno**.
- Pasar un `Action` donde se espera un `Func` (no compila: `Action` no devuelve valor).
- No entender el comportamiento de **closures**: las lambdas capturan variables, no valores.

---

## 2. Teoría

### 2.1 Action — Métodos sin retorno

```csharp
// Action sin parámetros: ejecuta algo y no devuelve nada
public delegate void Action();

// Action<T>: recibe un parámetro, no devuelve nada
public delegate void Action<in T>(T obj);

// Action<T1, T2, ... T16>: hasta 16 parámetros, sin retorno
public delegate void Action<in T1, in T2, in T3>(T1 arg1, T2 arg2, T3 arg3);
```

`Action` está marcado como **contravariante** (`in T`) en todos sus parámetros. Esto significa que un `Action<object>` puede tratarse como un `Action<string>` porque lo que acepta `object` también acepta `string`.

**Uso típico**: callbacks de progreso, logging, notificaciones, eventos, cualquier método que se ejecuta por su **efecto secundario** y no produce un resultado.

```
┌──────────────────────────────────────────────────────────┐
│                      Action<T>                           │
│                                                          │
│   Invocas el Action ──► Se ejecuta el método ──► Nada    │
│   (pasas datos)         (efecto secundario)    (void)    │
│                                                          │
│   Ejemplo: Action<int> progress = p => Console.Write(p); │
│            progress(50); // Imprime "50", no devuelve    │
│            nada                                          │
└──────────────────────────────────────────────────────────┘
```

### 2.2 Func — Métodos con retorno

```csharp
// Func<TResult>: sin parámetros, devuelve TResult
public delegate TResult Func<out TResult>();

// Func<T, TResult>: recibe T, devuelve TResult
public delegate TResult Func<in T, out TResult>(T arg);

// Func<T1, T2, ..., T16, TResult>: hasta 16 entradas + 1 retorno
public delegate TResult Func<in T1, in T2, out TResult>(T1 arg1, T2 arg2);
```

**Regla de oro**: en `Func<T1, T2, T3, ..., T16, TResult>`, los primeros N tipos son parámetros de entrada y el **último siempre es el tipo de retorno**.

`Func` combina **contravarianza** en las entradas (`in T`) con **covarianza** en la salida (`out TResult`). Esto permite que `Func<string, object>` sea asignable desde `Func<string, string>` (la salida `string` es más específica que `object`, covarianza) y que `Func<object, int>` sea asignable a `Func<string, int>` (la entrada `object` es más general que `string`, contravarianza).

```
┌──────────────────────────────────────────────────────────┐
│                      Func<T, TResult>                    │
│                                                          │
│   Invocas el Func ──► Se ejecuta la función ──► TResult  │
│   (pasas datos)         (transformación)       (retorno) │
│                                                          │
│   Ejemplo: Func<int, int> square = x => x * x;           │
│            int result = square(5); // 25                  │
└──────────────────────────────────────────────────────────┘
```

### 2.3 Predicate<T> — Condiciones (test lógico)

```csharp
// Predicate<T>: recibe T, devuelve bool
public delegate bool Predicate<in T>(T obj);
```

`Predicate<T>` es **idéntico** a `Func<T, bool>` en cuanto a firma. La diferencia es puramente **semántica**: `Predicate<T>` indica "esto es una condición, un test, un filtro". `Func<T, bool>` podría ser cualquier cosa que devuelva `bool`.

Históricamente, `Predicate<T>` existe desde .NET 2.0 y es el tipo que usan los métodos de `List<T>` (`FindAll`, `RemoveAll`, `Exists`, `TrueForAll`, `Find`). LINQ, introducido en .NET 3.5, decidió usar `Func<T, bool>` en su lugar (porque `Func` es más general y ya estaba definido).

**Conversión entre Predicate y Func**:

```csharp
Predicate<int> isEven = n => n % 2 == 0;
Func<int, bool> isEvenFunc = isEven; // ✅ Conversión implícita

// Pero ojo: List<int>.Where() espera Func<int, bool>, no Predicate<int>
List<int> numbers = new List<int> { 1, 2, 3, 4 };
var evensLinq = numbers.Where(n => n % 2 == 0);  // Func<int, bool>
var evensList = numbers.FindAll(n => n % 2 == 0); // Predicate<int>
```

### 2.4 Delegado custom vs Func/Action

Aunque `Func` y `Action` cubren casi todo, hay casos donde un **delegado personalizado** es mejor:

| Situación | Usar |
|---|---|
| El nombre del delegado **aporta semántica de dominio** (`AgeValidator`, `OrderProcessedHandler`) | Delegado custom |
| Los parámetros deben tener **nombres significativos** que documenten el contrato | Delegado custom |
| Necesitas parámetros `ref` o `out` (no soportados por `Func`/`Action`) | Delegado custom |
| Es una firma genérica que se repite en muchos sitios (selectores, filtros) | `Func` o `Action` |

```csharp
// Delegado custom: el nombre AgeValidator expresa claramente el propósito.
// El nombre del parámetro "age" documenta qué se espera.
public delegate bool AgeValidator(int age);

// Func<int, bool>: técnicamente idéntico, pero no comunica qué se valida.
Func<int, bool> isAdult = age => age >= 18;
```

### 2.5 Lambdas y closures

Las lambdas son la forma más común de crear `Func`, `Action` y `Predicate`:

```csharp
// Lambda con expresión (cuerpo simple, retorno implícito)
Func<int, int> square = x => x * x;

// Lambda con bloque (múltiples sentencias, retorno explícito)
Func<int, int> factorial = n =>
{
    int result = 1;
    for (int i = 2; i <= n; i++)
        result *= i;
    return result;
};

// Lambda sin parámetros
Action greet = () => Console.WriteLine("Hola");

// Lambda con múltiples parámetros
Func<int, int, int> add = (a, b) => a + b;
```

**Closures**: una lambda captura las variables del scope externo **por referencia**, no por valor. Esto significa que si la variable cambia después de definir la lambda, la lambda ve el valor actualizado:

```csharp
int factor = 10;
Func<int, int> multiply = x => x * factor; // Captura 'factor' por referencia
factor = 20;
Console.WriteLine(multiply(5)); // 100 (5 * 20), NO 50 (5 * 10)
```

---

## 3. Ejemplo mínimo

```csharp
// Action<string>: recibe un string, lo imprime, no devuelve nada
Action<string> print = msg => Console.WriteLine(msg);

// Func<int, int, int>: recibe dos ints, devuelve un int
// El ÚLTIMO parámetro de tipo (int) es SIEMPRE el retorno
Func<int, int, int> add = (a, b) => a + b;

// Predicate<int>: recibe un int, devuelve bool (condición)
Predicate<int> isEven = n => n % 2 == 0;

// ─── Uso ───
print("Hola");                   // Action: efecto secundario
int result = add(3, 5);        // Func: produce un valor (8)
bool check = isEven(4);        // Predicate: test lógico (true)

Console.WriteLine($"Suma: {result}, ¿Es par?: {check}");
```

**Explicación línea por línea**:

1. `Action<string> print = msg => Console.WriteLine(msg);` — Declara un `Action<string>` llamado `print`. La lambda `msg => Console.WriteLine(msg)` recibe un `string` y lo imprime. No hay `return` porque `Action` es `void`.
2. `Func<int, int, int> add = (a, b) => a + b;` — Declara un `Func` que recibe dos `int` y devuelve `int`. Los dos primeros `int` son los parámetros de entrada (`a` y `b`), el tercer `int` es el tipo de retorno.
3. `Predicate<int> isEven = n => n % 2 == 0;` — Declara un `Predicate<int>`: recibe `int`, devuelve `bool`. La expresión `n % 2 == 0` evalúa a `true` si el número es par.
4. `print("Hola");` — Invoca el `Action`: ejecuta `Console.WriteLine("Hola")`.
5. `int result = add(3, 5);` — Invoca el `Func`: ejecuta `3 + 5` y devuelve `8`.
6. `bool check = isEven(4);` — Invoca el `Predicate`: ejecuta `4 % 2 == 0` y devuelve `true`.

---

## 4. Ejemplo real

Consulta el código completo en `src/PooDelegates/Program.cs`. Este programa demuestra 7 escenarios reales de uso:

### 4.1 `FileProcessor` — Action como callback de progreso

```csharp
public class FileProcessor
{
    // Action<int>: el callback recibe el porcentaje (0-100) y lo reporta
    public void Process(string path, Action<int> onProgress)
    {
        for (int step = 1; step <= 5; step++)
        {
            Thread.Sleep(100);
            onProgress(step * 20); // Notifica 20%, 40%, 60%, 80%, 100%
        }
    }

    // Action sin parámetros: simplemente notifica que terminó
    public void RunBackup(Action onComplete)
    {
        Thread.Sleep(150);
        onComplete(); // "Backup completado"
    }

    // Action<string, string>: callback de log con nivel y mensaje
    public void ProcessWithLog(string path, Action<string, string> logCallback)
    {
        logCallback("INFO", $"Iniciando {path}");
        Thread.Sleep(100);
        logCallback("INFO", $"Finalizado {path}");
    }
}
```

El `FileProcessor` no sabe ni le importa **cómo** se reporta el progreso. El consumidor decide: puede imprimir una barra en consola, actualizar una UI, enviar un evento por SignalR o escribir en un archivo de log.

### 4.2 `Mapper` — Func para transformación de listas

```csharp
public class Mapper
{
    // Func<TSource, TDestination>: transforma cada elemento
    public List<TDestination> Map<TSource, TDestination>(
        List<TSource> source,
        Func<TSource, TDestination> transform)
    {
        List<TDestination> result = new List<TDestination>(source.Count);
        foreach (TSource item in source)
            result.Add(transform(item));
        return result;
    }
}

// Uso:
List<int> numbers = new List<int> { 1, 2, 3, 4, 5 };
List<string> texts = mapper.Map(numbers, n => $"Número {n}");
List<int> squares = mapper.Map(numbers, n => n * n);
```

Este patrón es exactamente lo que hace LINQ con `Select()`. La diferencia es que aquí ves la implementación por dentro. `Func<TSource, TDestination>` es la firma canónica de cualquier transformación de datos.

### 4.3 `Calculator` — Func como patrón Strategy

```csharp
public class Calculator
{
    // La operación es un parámetro más: Strategy Pattern sin clases
    public int Execute(int a, int b, Func<int, int, int> operation)
    {
        return operation(a, b);
    }

    // Transformación unaria
    public int Transform(int value, Func<int, int> transformation)
    {
        return transformation(value);
    }

    // Fábrica sin parámetros: Func<TResult> sin entradas
    public T Create<T>(Func<T> factory)
    {
        return factory();
    }
}

// Uso: pasas la operación como lambda, sin necesidad de clases Strategy
int sum = calculator.Execute(15, 27, (a, b) => a + b);
int max = calculator.Execute(15, 27, (a, b) => a > b ? a : b);
int squared = calculator.Transform(9, x => x * x);
string guid = calculator.Create(() => Guid.NewGuid().ToString("N")[..8]);
```

`Func<int, int, int>` reemplaza lo que en un diseño clásico OOP requeriría una interfaz `IOperation` con múltiples implementaciones (`AddOperation`, `MultiplyOperation`, `MaxOperation`). Para operaciones simples, los delegados son más ligeros y expresivos.

### 4.4 `FilterService` — Predicate para filtros y métodos nativos de List<T>

```csharp
public class FilterService
{
    public List<int> Filter(List<int> numbers, Predicate<int> condition)
    {
        List<int> result = new List<int>();
        foreach (int number in numbers)
            if (condition(number))
                result.Add(number);
        return result;
    }

    // Métodos nativos de List<T> que usan Predicate<T>
    public void DemonstrateBuiltInPredicates(List<int> numbers)
    {
        List<int> evens = numbers.FindAll(n => n % 2 == 0);    // Predicate<int>
        bool hasNegative = numbers.Exists(n => n < 0);         // Predicate<int>
        int firstLarge = numbers.Find(n => n > 50);            // Predicate<int>
        bool allPositive = numbers.TrueForAll(n => n > 0);     // Predicate<int>
    }
}
```

`List<T>` tiene 7 métodos que aceptan `Predicate<T>`: `Find`, `FindLast`, `FindAll`, `FindIndex`, `FindLastIndex`, `Exists`, `TrueForAll`, `RemoveAll`. Todos expresan condiciones.

### 4.5 `DataPipeline` — Combinando Predicate + Func + Action

```csharp
public class DataPipeline
{
    public List<int> Process(
        List<int> data,
        Predicate<int> filter,     // Paso 1: decidir qué procesar
        Func<int, int> transform,  // Paso 2: transformar cada elemento
        Action<int> onEach)        // Paso 3: notificar cada resultado
    {
        List<int> results = new List<int>();
        foreach (int item in data)
        {
            if (!filter(item)) continue;        // Predicate: filtrar
            int transformed = transform(item);   // Func: transformar
            onEach(transformed);                 // Action: notificar
            results.Add(transformed);
        }
        return results;
    }
}

// Uso: filtrar pares, elevar al cuadrado, imprimir cada uno
var result = pipeline.Process(
    data: new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
    filter: n => n % 2 == 0,       // Predicate<int>: solo pares
    transform: n => n * n,         // Func<int, int>: elevar al cuadrado
    onEach: n => Console.WriteLine($"Procesado: {n}") // Action<int>
);
// Resultado: [4, 16, 36, 64, 100]
```

Este pipeline demuestra cómo los tres delegados trabajan juntos formando un patrón **Filtrar → Transformar → Reportar** sin acoplar el pipeline a ninguna implementación concreta.

### 4.6 `Validator<T>` — Sistema de validación con Predicate<T> params

```csharp
public class Validator<T>
{
    // params Predicate<T>[]: cualquier número de reglas
    // TODAS deben cumplirse (AND lógico)
    public bool Validate(T item, params Predicate<T>[] rules)
    {
        foreach (Predicate<T> rule in rules)
        {
            if (!rule(item))
                return false; // Basta con que una regla falle
        }
        return true;
    }
}

// Uso: validar strings contra múltiples reglas
Validator<string> stringValidator = new Validator<string>();

bool isValid = stringValidator.Validate(
    "Hola",
    s => !string.IsNullOrWhiteSpace(s),  // No vacío
    s => s.Length >= 3,                  // Mínimo 3 caracteres
    s => s.Length <= 20,                 // Máximo 20 caracteres
    s => !s.Contains(' '),              // Sin espacios
    s => char.IsLetter(s[0])            // Empieza con letra
);
```

Cada regla es un `Predicate<T>` independiente. Añadir o quitar reglas es trivial. No necesitas una jerarquía de clases `IValidationRule`.

### 4.7 Delegado custom vs Func/Action — `OrderService` y `AgeValidator`

```csharp
// Delegado personalizado: el nombre y el tipo del parámetro comunican la intención
public delegate void OrderProcessedHandler(Order order);
public delegate bool AgeValidator(int age);

public class OrderService
{
    // Versión con delegado personalizado: semántica clara
    public void ProcessOrderCustom(Order order, OrderProcessedHandler onProcessed)
    {
        onProcessed(order);
    }

    // Versión con Action<Order>: técnicamente idéntica, menos expresiva
    public void ProcessOrderAction(Order order, Action<Order> onProcessed)
    {
        onProcessed(order);
    }

    // Validación con delegado custom: AgeValidator expresa el dominio
    public bool IsValidAge(int age, AgeValidator validator) => validator(age);

    // Validación con Func<int, bool>: ¿qué valida esto exactamente?
    public bool IsValidAgeFunc(int age, Func<int, bool> validator) => validator(age);
}
```

**Conclusión**: ambos compilan y funcionan. La diferencia está en la **legibilidad** para quien lee el código. `OrderProcessedHandler` grita "esto maneja órdenes procesadas". `Action<Order>` dice "esto es un método que recibe una Order y no devuelve nada".

### Ejecución del ejemplo

```bash
dotnet run --project ejemplos/m02-poo/Leccion-07-Func-Action-Predicate/src/PooDelegates
```

---

## 5. Buenas prácticas

### ✅ Hacer

1. **Usa `Action` para callbacks sin retorno** (progreso, logging, notificaciones):
   ```csharp
   // Bien: Action<int> para reportar progreso
   void ProcessFile(string path, Action<int> onProgress);

   // Mal: forzar un retorno innecesario
   void ProcessFile(string path, Func<int, bool> onProgress); // ¿Qué significa el bool?
   ```

2. **Usa `Func` para transformaciones puras** (sin efectos secundarios):
   ```csharp
   // Bien: transformación pura, misma entrada → misma salida
   Func<int, int> square = x => x * x;

   // Mal: Func con efectos secundarios escondidos
   Func<int, int> broken = x => { _logger.Log(x); return x * x; }; // ¡tiene side effect!
   ```

3. **Usa `Predicate` para condiciones y filtros** cuando trabajes con `List<T>`:
   ```csharp
   // Bien para List<T>.FindAll, RemoveAll, Exists...
   Predicate<User> isActive = u => u.IsActive;
   var activeUsers = allUsers.FindAll(isActive);

   // Para LINQ, usa Func<T, bool> (aunque Predicate es compatible por asignación)
   var activeUsersLinq = allUsers.Where(u => u.IsActive); // Func<User, bool>
   ```

4. **Prefiere delegados custom cuando el nombre aporta significado del dominio**:
   ```csharp
   // Bien: el nombre del delegado documenta el contrato
   public delegate bool CreditScoreValidator(int score);
   public delegate void PaymentProcessedHandler(Payment payment);

   // Menos expresivo: ¿qué validan exactamente estos Func?
   Func<int, bool> validateScore;
   Action<Payment> onPayment;
   ```

5. **Entiende las closures**: la lambda captura la **variable**, no el valor:
   ```csharp
   // Si iteras y capturas la variable de iteración, usa una copia local
   for (int i = 0; i < 5; i++)
   {
       int copy = i; // Copia local — cada lambda captura su propia variable
       actions.Add(() => Console.WriteLine(copy)); // 0, 1, 2, 3, 4
   }
   ```

6. **Usa `Func<T>` para factories** (inyección de dependencias, creación perezosa):
   ```csharp
   // En lugar de pasar el objeto ya creado, pasa la fábrica
   // El consumidor decide CUÁNDO crear la instancia
   void RegisterFactory(Func<IService> factory);
   ```

### ❌ Evitar

1. **No uses `async void` como `Action`**:
   ```csharp
   // ❌ Terrible: no se puede esperar, las excepciones matan el proceso
   Action doWork = async () => await SomeAsyncWork();

   // ✅ Correcto: usa Func<Task> para operaciones asíncronas
   Func<Task> doWork = async () => await SomeAsyncWork();
   await doWork(); // Ahora SÍ puedes esperar
   ```

2. **No pases `Action` donde se espera `Func`** (no compila):
   ```csharp
   // ❌ Error CS0029: no se puede convertir Action a Func
   Func<int, int> op = x => Console.WriteLine(x); // Action, no Func

   // ✅ Correcto: devuelve un valor
   Func<int, int> op = x => x * 2; // Toma int, devuelve int
   ```

3. **No asumas que `Predicate` es intercambiable con `Func<T, bool>` en todas partes**:
   ```csharp
   List<int> numbers = new List<int> { 1, 2, 3 };
   Predicate<int> isEven = n => n % 2 == 0;

   numbers.FindAll(isEven);              // ✅ Funciona (FindAll espera Predicate<int>)
   numbers.Where(isEven);                // ✅ Funciona (conversión implícita Predicate → Func)
   // Pero no al revés:
   Func<int, bool> isOdd = n => n % 2 != 0;
   // numbers.FindAll(isOdd);           // ⚠️ Compila pero no es idiomático
   ```

4. **No olvides el orden de los parámetros de tipo en `Func`**:
   ```csharp
   // ❌ Error: el último int no es el retorno aquí, es un parámetro de entrada
   Func<int, int, int, int> fourInputsOneOutput; // 3 entradas + 1 salida

   // ✅ Correcto: piensa "3 entradas, 1 salida"
   // Func<entrada1, entrada2, entrada3, salida>
   // Los primeros N-1 tipos son entradas, el último es la salida
   ```

5. **No crees delegados custom para todo**: si `Func<int, bool>` es suficientemente claro en el contexto, no necesitas `delegate bool NumberValidator(int n)`. La regla es: **si el nombre del delegado añade información que `Func` no comunica, crea el delegado. Si no, usa `Func`**.

---

## 6. Errores habituales

### Error 1 — Closure sobre la variable del bucle (pre-C# 5)

```csharp
// ❌ INCORRECTO en C# < 5 (hoy compila correctamente, pero entender el problema es importante)
List<Action> actions = new List<Action>();
for (int i = 0; i < 5; i++)
{
    // En C# < 5, todas las lambdas capturaban LA MISMA variable 'i'
    actions.Add(() => Console.WriteLine(i)); // Todas imprimen 5
}
foreach (var action in actions)
    action(); // Antes de C# 5: 5, 5, 5, 5, 5

// ✅ CORRECTO: captura una copia local (necesario en C# < 5, buena práctica siempre)
for (int i = 0; i < 5; i++)
{
    int copy = i; // Cada iteración crea una NUEVA variable
    actions.Add(() => Console.WriteLine(copy));
}
foreach (var action in actions)
    action(); // 0, 1, 2, 3, 4
```

**Por qué fallaba**: antes de C# 5, el compilador generaba una sola variable `i` para todo el bucle `for` y todas las lambdas capturaban esa misma variable. Al ejecutar las lambdas, `i` ya valía 5. C# 5 cambió el comportamiento para que cada iteración del `foreach` genere una variable nueva, pero `for` sigue compartiendo la variable (aunque muchos casos se optimizan).

### Error 2 — Asumir que Predicate se usa con LINQ

```csharp
// ❌ CONCEPTUALMENTE INCORRECTO (aunque compila)
Predicate<int> isEven = n => n % 2 == 0;
var evens = numbers.Where(isEven); // Where espera Func<int, bool>, no Predicate<int>

// ✅ CORRECTO: LINQ usa Func<T, bool>
Func<int, bool> isEvenFunc = n => n % 2 == 0;
var evens = numbers.Where(isEvenFunc);

// ✅ CORRECTO: métodos de List<T> usan Predicate<T>
var evensList = numbers.FindAll(n => n % 2 == 0); // FindAll espera Predicate<int>
```

**Por qué**: aunque la conversión implícita de `Predicate<int>` a `Func<int, bool>` existe y el código compila, LINQ se diseñó con `Func`. Predicate es específico de `List<T>` y `Array`.

### Error 3 — Pasar `Action` donde se espera `Func`

```csharp
// ❌ ERROR DE COMPILACIÓN CS0029
void Execute(Func<int, int> operation)
{
    int result = operation(5); // Espera un valor de retorno
}

Action<int> print = x => Console.WriteLine(x);
// Execute(print); // ❌ Action<int> no es Func<int, int>

// ✅ CORRECTO: si necesitas pasar un Action, cambia la firma
void ExecuteAction(Action<int> operation)
{
    operation(5); // No espera retorno
}
```

### Error 4 — Olvidar el orden de los tipos en Func

```csharp
// ❌ CONFUSIÓN COMÚN
// ¿Qué significa esto?
Func<int, string, bool> mystery;

// Significa: (int, string) → bool
// Parámetros: int y string. Retorno: bool.
// Piensa SIEMPRE: "los primeros N tipos son entrada, el ÚLTIMO es retorno"

// ✅ Para que quede claro, nombra los parámetros en el delegado:
Func<int, string, bool> hasLength = (length, text) => text.Length == length;
//                                   ^int    ^string  ^bool (retorno)
```

### Error 5 — `async void` en Action para eventos

```csharp
// ❌ MUY PELIGROSO: las excepciones en async void matan el proceso
button.Click += async (sender, args) =>
{
    await DoSomethingAsync(); // Si lanza excepción, el proceso CRASHEA
};

// ✅ CORRECTO: maneja excepciones dentro del async void
button.Click += async (sender, args) =>
{
    try
    {
        await DoSomethingAsync();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error en el manejador del evento");
    }
};

// ✅ MEJOR: usa Func<Task> y un wrapper que ejecute y maneje errores
Func<Task> safeHandler = async () =>
{
    await DoSomethingAsync();
};
```

---

## 7. Ejercicio guiado

Vamos a construir un **sistema de procesamiento de pedidos** que utiliza los tres delegados en conjunto.

### Paso 1 — Definir los modelos

```csharp
// Representa un pedido pendiente de procesar
public record Order(Guid Id, string Customer, decimal Total, string Status);

// Representa el recibo generado tras procesar un pedido
public record Receipt(Guid OrderId, string Customer, decimal Total, DateTime Timestamp);
```

### Paso 2 — El método ProcessOrders

```csharp
public static class OrderProcessor
{
    /// <summary>
    /// Procesa una lista de órdenes aplicando un pipeline:
    /// 1. filter (Predicate<Order>): decide qué órdenes procesar
    /// 2. transform (Func<Order, Receipt>): convierte Order → Receipt
    /// 3. onProcessed (Action<Receipt>): callback para cada recibo generado
    /// </summary>
    public static List<Receipt> ProcessOrders(
        List<Order> orders,
        Predicate<Order> filter,
        Func<Order, Receipt> transform,
        Action<Receipt> onProcessed)
    {
        List<Receipt> receipts = new List<Receipt>();

        foreach (Order order in orders)
        {
            // Paso 1: ¿Debemos procesar esta orden?
            if (!filter(order))
                continue;

            // Paso 2: Transformar la orden en un recibo
            Receipt receipt = transform(order);

            // Paso 3: Notificar que se generó el recibo
            onProcessed(receipt);

            receipts.Add(receipt);
        }

        return receipts;
    }
}
```

### Paso 3 — Datos de ejemplo y ejecución

```csharp
// Datos de ejemplo: órdenes con distintos estados
List<Order> orders = new List<Order>
{
    new Order(Guid.NewGuid(), "Alice",   150.00m, "Pending"),
    new Order(Guid.NewGuid(), "Bob",      75.50m, "Pending"),
    new Order(Guid.NewGuid(), "Charlie", 200.00m, "Shipped"),  // Ya enviado
    new Order(Guid.NewGuid(), "Diana",    50.00m, "Pending"),
    new Order(Guid.NewGuid(), "Eve",     300.00m, "Cancelled"), // Cancelado
    new Order(Guid.NewGuid(), "Frank",   120.00m, "Pending"),
};

// Paso 1 — Predicate<Order>: filtrar solo órdenes pendientes
Predicate<Order> onlyPending = order => order.Status == "Pending";

// Paso 2 — Func<Order, Receipt>: transformar orden → recibo
// Cada recibo tiene el OrderId, el nombre del cliente, el total y un timestamp
Func<Order, Receipt> toReceipt = order =>
    new Receipt(order.Id, order.Customer, order.Total, DateTime.Now);

// Paso 3 — Action<Receipt>: imprimir el recibo generado
Action<Receipt> printReceipt = receipt =>
    Console.WriteLine($"  [RECIBO] {receipt.Customer}: ${receipt.Total:F2} " +
                      $"({receipt.Timestamp:HH:mm:ss})");

// ─── Procesar ───
Console.WriteLine("=== Procesando pedidos pendientes ===");
List<Receipt> generatedReceipts = OrderProcessor.ProcessOrders(
    orders,
    onlyPending,
    toReceipt,
    printReceipt
);

Console.WriteLine($"\nTotal de recibos generados: {generatedReceipts.Count}");
// Resultado: 4 recibos (Alice, Bob, Diana, Frank)
// Charlie (Shipped) y Eve (Cancelled) fueron filtrados

// ─── Verificar ───
foreach (Receipt receipt in generatedReceipts)
{
    Console.WriteLine($"  {receipt.Customer}: ${receipt.Total:F2} - {receipt.Timestamp:yyyy-MM-dd HH:mm:ss}");
}
```

### Paso 4 — Variante: filtrar por monto mínimo

```csharp
// Cambiamos solo el Predicate, el resto del pipeline es idéntico
Predicate<Order> highValue = order => order.Total >= 100m;

Console.WriteLine("\n=== Procesando pedidos de alto valor ===");
List<Receipt> highValueReceipts = OrderProcessor.ProcessOrders(
    orders,
    highValue,       // Solo pedidos >= $100
    toReceipt,       // Misma transformación
    printReceipt     // Misma acción de notificación
);
```

**Puntos importantes**:
- El método `ProcessOrders` no sabe qué filtro se aplica ni cómo se genera el recibo. Recibe **comportamiento** como parámetro.
- Cambiar el filtro o la transformación no requiere modificar el método.
- Esto es **Open/Closed Principle** aplicado con delegados.

---

## 8. Reto

Implementa un `EventSourcedAggregator<TEvent>` que simule un sistema de **event sourcing** usando delegados.

### Requisitos

1. **Almacenar eventos**: recibe eventos mediante un `Action<TEvent>` como callback. Cada vez que la aplicación "emite" un evento, el callback lo almacena en una `List<TEvent>` interna.

2. **Filtrar eventos**: método `Query(Func<TEvent, bool> filter)` que devuelve los eventos almacenados que cumplen el filtro.

3. **Proyectar a estado**: método `Project<TState>(Func<List<TEvent>, TState> projector)` que aplica una función de proyección sobre todos los eventos y devuelve un estado agregado.

4. **Demostrar con `BankAccount`**:
   - Define eventos: `Deposit(decimal Amount)` y `Withdraw(decimal Amount)`.
   - Usa `EventSourcedAggregator<object>` para almacenar eventos.
   - Proyecta los eventos a un `decimal` que represente el balance actual (depósitos - retiros).
   - Filtra solo los depósitos y calcula el total depositado.

### Ejemplo de uso esperado

```csharp
var aggregator = new EventSourcedAggregator<object>();

// Emitir eventos mediante el callback
aggregator.Append(new Deposit(100m));
aggregator.Append(new Withdraw(30m));
aggregator.Append(new Deposit(50m));
aggregator.Append(new Withdraw(20m));
aggregator.Append(new Deposit(200m));

// Proyectar al balance actual
decimal balance = aggregator.Project(events =>
{
    decimal total = 0;
    foreach (var e in events)
    {
        if (e is Deposit d) total += d.Amount;
        else if (e is Withdraw w) total -= w.Amount;
    }
    return total;
});
Console.WriteLine($"Balance: ${balance}"); // 300 (100 - 30 + 50 - 20 + 200)

// Filtrar solo depósitos
var deposits = aggregator.Query(e => e is Deposit);
Console.WriteLine($"Cantidad de depósitos: {deposits.Count}"); // 3
```

### Pistas

- La lista interna debe ser privada. Usa `Action<TEvent>` como interfaz pública para añadir eventos.
- `Query` usa `Func<TEvent, bool>` como filtro (decisión de diseño: ¿por qué no `Predicate<TEvent>`?).
- `Project<TState>` usa `Func<List<TEvent>, TState>` para transformar la historia completa en un estado.
- Piensa en qué tipo concreto usar para `TEvent` si tienes `Deposit` y `Withdraw` como clases separadas.

¡Inténtalo antes de mirar la solución!

---

## 9. Resumen

### Conceptos clave

| Tipo | Firma | Retorno | Uso típico | Ejemplo |
|---|---|---|---|---|
| `Action` | `delegate void Action()` | `void` | Callback sin datos | `Action onComplete = () => Console.WriteLine("Hecho");` |
| `Action<T>` | `delegate void Action<in T>(T obj)` | `void` | Callback con un dato | `Action<int> progress = p => Console.Write($"{p}%");` |
| `Action<T1,T2>` | `delegate void Action<in T1, in T2>(T1 a, T2 b)` | `void` | Logging, eventos con múltiples datos | `Action<string,string> log = (lvl,msg) => ...;` |
| `Func<TResult>` | `delegate TResult Func<out TResult>()` | `TResult` | Fábrica, generador | `Func<Guid> newId = () => Guid.NewGuid();` |
| `Func<T, TResult>` | `delegate TResult Func<in T, out TResult>(T arg)` | `TResult` | Transformación, selector | `Func<int, string> toString = n => n.ToString();` |
| `Func<T1,T2,TResult>` | `delegate TResult Func<in T1, in T2, out TResult>(...)` | `TResult` | Operación binaria | `Func<int,int,int> add = (a,b) => a + b;` |
| `Predicate<T>` | `delegate bool Predicate<in T>(T obj)` | `bool` | Condición, test, filtro | `Predicate<int> isEven = n => n % 2 == 0;` |

### Tabla comparativa

```
┌──────────────┬──────────────────────────────────────────────────┐
│ Action<T>    │ Métodos que NO devuelven valor (void)            │
│              │ Callbacks, logging, notificaciones, eventos      │
│              │ Hasta 16 parámetros de entrada                   │
├──────────────┼──────────────────────────────────────────────────┤
│ Func<T,TR>   │ Métodos que SÍ devuelven valor                   │
│              │ Transformaciones, fábricas, consultas            │
│              │ Hasta 16 parámetros de entrada + 1 retorno       │
├──────────────┼──────────────────────────────────────────────────┤
│ Predicate<T> │ Métodos que devuelven bool (condición)           │
│              │ Equivalente a Func<T, bool>                      │
│              │ List<T>.FindAll, RemoveAll, Exists, etc.         │
├──────────────┼──────────────────────────────────────────────────┤
│ Delegado     │ Cuando el NOMBRE aporta semántica de dominio     │
│ custom       │ Ej: delegate bool AgeValidator(int age);        │
│              │ Mejor legibilidad que Func<int, bool>            │
└──────────────┴──────────────────────────────────────────────────┘
```

### Regla de decisión

```
¿Necesitas un delegado?
│
├─ ¿Devuelve void?
│   └─ Usa Action / Action<T>
│
├─ ¿Devuelve bool como condición?
│   ├─ ¿Estás usando List<T>.FindAll/Exists/RemoveAll?
│   │   └─ Usa Predicate<T>
│   └─ ¿Estás usando LINQ o APIs generales?
│       └─ Usa Func<T, bool>
│
├─ ¿Devuelve otro tipo?
│   └─ Usa Func<T, TResult> / Func<T1, T2, TResult> / ...
│
└─ ¿El nombre del delegado añade semántica de dominio que Func/Action no comunica?
    └─ Crea un delegado custom (delegate bool AgeValidator(int age);)
```

---

## 10. Preguntas frecuentes

### ¿Cuál es la diferencia entre `Predicate<T>` y `Func<T, bool>`?

**Semántica**, no técnica. Ambos representan un método que recibe `T` y devuelve `bool`. La diferencia es de **intención**:

- `Predicate<T>` grita "esto es una condición, un test, un filtro". Es el tipo que usan los métodos de filtrado de `List<T>` (`FindAll`, `RemoveAll`, `Exists`).
- `Func<T, bool>` es más genérico: podría ser un predicado, una comprobación de base de datos, un validador, cualquier cosa que devuelva `bool`.

```csharp
// Ambos compilan y funcionan igual
Predicate<int> isEven = n => n % 2 == 0;
Func<int, bool> isEvenToo = isEven; // Conversión implícita

// Pero en APIs: List<T> usa Predicate, LINQ usa Func
numbers.FindAll(isEven);   // Predicate<int> — idiomático
numbers.Where(isEvenToo);  // Func<int, bool> — idiomático
```

### ¿Cuántos parámetros máximos puede tener un `Func`?

**16 parámetros de entrada + 1 de retorno = 17 tipos en total**. `Func<T1, T2, ..., T16, TResult>` es la firma más larga definida en el BCL. Si necesitas más de 16 parámetros, tu diseño probablemente tiene un problema más grave que la falta de un delegado: agrúpalos en una clase, un `record` o un `ValueTuple`.

```csharp
// Si llegas a esto, TUPLA o CLASE:
Func<int, int, int, int, int, int, int, int, int, int,
     int, int, int, int, int, int, int> locura; // 16 entradas + 1 salida
```

### ¿Puedo usar `ref` o `out` con `Func`/`Action`?

**No directamente**. `Func` y `Action` solo aceptan parámetros por valor. Si necesitas `ref` o `out`, debes declarar un **delegado personalizado**:

```csharp
// ❌ No compila
// Func<ref int, bool> impossible;

// ✅ Delegado custom
public delegate bool TryParser<T>(string input, out T result);

// Uso
TryParser<int> parseInt = (string s, out int r) => int.TryParse(s, out r);
if (parseInt("42", out int value))
    Console.WriteLine(value); // 42
```

### ¿Las lambdas siempre capturan variables por referencia?

**Sí**. Las closures en C# capturan la **variable**, no el valor en el momento de la captura. Esto es por diseño y es útil para compartir estado mutable, pero puede causar bugs si no lo esperas:

```csharp
int counter = 0;
Func<int> increment = () => ++counter; // Captura 'counter' por referencia

Console.WriteLine(increment()); // 1
Console.WriteLine(increment()); // 2 (la misma variable, ahora vale 2)
```

Si necesitas "congelar" el valor en el momento de la captura, haz una copia local:

```csharp
List<Func<int>> functions = new List<Func<int>>();
for (int i = 0; i < 3; i++)
{
    int captured = i; // Copia local: cada iteración crea una variable nueva
    functions.Add(() => captured);
}
// functions[0]() → 0, functions[1]() → 1, functions[2]() → 2
```

### ¿Por qué `async void` es peligroso con `Action`?

Porque `async void` no se puede esperar y las excepciones no capturadas terminan el proceso:

```csharp
// ❌ Si esto lanza excepción, el proceso muere
Action handler = async () =>
{
    await Task.Delay(100);
    throw new InvalidOperationException("¡BOOM!"); // Crash del proceso
};

// ✅ Usa Func<Task> para operaciones asíncronas
Func<Task> handler = async () =>
{
    await Task.Delay(100);
    throw new InvalidOperationException("Error controlado");
};

try
{
    await handler(); // Puedes esperar y capturar la excepción
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Error capturado: {ex.Message}");
}
```

### ¿Cuándo usar `Func<Task>` en lugar de `Action`?

Cuando la operación que encapsulas es **asíncrona** y necesitas **esperarla** o **capturar sus excepciones**:

| Necesitas... | Usa |
|---|---|
| Ejecutar y olvidar (fire-and-forget) síncrono | `Action` |
| Ejecutar y olvidar asíncrono (peligroso) | `async void` como `Action` |
| Ejecutar y esperar asíncrono | `Func<Task>` |
| Ejecutar, esperar y obtener resultado | `Func<Task<T>>` |
