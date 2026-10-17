# Lección 6 — Delegados y Eventos

---

## 1. Introducción

Los **delegados** son el mecanismo de C# para tratar métodos como valores (*first-class functions*). Un delegado es un **tipo** que define la firma de un método (parámetros y tipo de retorno) y permite almacenar referencias a métodos que cumplan esa firma, pasarlas como argumento, devolverlas desde otros métodos e invocarlas dinámicamente.

Los **eventos** son la capa de protección sobre delegados para implementar el **patrón observer/publisher-subscriber**. Un evento es un delegado con restricciones de acceso: solo la clase propietaria puede invocarlo; desde fuera únicamente se permite suscribir (`+=`) y desuscribir (`-=`). Esto evita que código externo dispare el evento accidentalmente o sobreescriba la lista de suscriptores.

Son fundamentales en **todo el ecosistema .NET**:

| Ámbito | Uso de delegados y eventos |
|---|---|
| **UI** (WinForms, WPF, MAUI) | `Button.Click`, `TextBox.TextChanged` |
| **Callbacks asíncronos** | `Task.Run(Action)`, `Task.ContinueWith(Func<>)` |
| **LINQ** | `Where(Func<T, bool>)`, `Select(Func<T, TResult>)`, `OrderBy(Func<T, TKey>)` |
| **ASP.NET Core** | Middleware delegates, `IEndpointFilter`, `MapGet("/", Delegate)` |
| **Entity Framework** | `SaveChangesInterceptor` (eventos), `IQueryable` (delegados en expresiones) |
| **Background Jobs** | `BackgroundService.ExecuteAsync` devuelve `Task` (delegado implícito) |
| **SignalR** | `Hub.On("event", handler)` |

**Cuándo se utilizan**:
- **Delegados**: cuando necesitas pasar lógica como parámetro (callbacks, estrategias, filtros, transformaciones).
- **Eventos**: cuando necesitas notificar a otros componentes que algo ha ocurrido sin que el emisor conozca a los receptores.

**Errores típicos**:
- No desuscribirse de eventos y provocar **memory leaks** (el publisher mantiene viva la referencia al subscriber).
- Invocar un evento sin comprobar `null` → `NullReferenceException`.
- Usar `=` en lugar de `+=` al suscribirse (sobrescribe todos los suscriptores anteriores).
- Usar `async void` en handlers de eventos (no se puede esperar, las excepciones se pierden).

---

## 2. Teoría

### 2.1 Delegado: tipo que referencia métodos

Un **delegado** es un tipo que define la **firma** de un método (parámetros aceptados y tipo de retorno). Una vez declarado, puedes crear instancias del delegado y asignarles métodos que cumplan esa firma.

```csharp
// Declaración: define el TIPO "Operation"
// "Operation" puede referenciar cualquier método que reciba 2 ints y devuelva int
delegate int Operation(int a, int b);

// Métodos que cumplen la firma
int Add(int a, int b) => a + b;
int Multiply(int a, int b) => a * b;

// Instanciar y usar
Operation op = Add;           // op referencia al método Add
int result = op(5, 3);        // Invoca Add(5, 3) → 8
```

**Por dentro**: un delegado es una clase generada por el compilador que hereda de `System.MulticastDelegate`, la cual a su vez hereda de `System.Delegate`. Contiene:
- Un puntero al método (*method pointer*).
- Una referencia al objeto (*target*), si el método es de instancia. Si es estático, el target es `null`.
- Una *invocation list* (lista encadenada de delegados) para el comportamiento multicast.

```
┌──────────────────────────────────────────────────────────┐
│                Estructura interna de un delegado          │
│                                                          │
│  Operation op = Add;                                     │
│                                                          │
│  ┌─────────────────────┐                                 │
│  │  MulticastDelegate  │                                 │
│  │  ┌─────────────────┐│                                 │
│  │  │ _target = null   ││  ← null porque Add es estático │
│  │  │ _methodPtr = Add ││  ← puntero al método           │
│  │  │ _invocationList  ││  ← null (solo un método)       │
│  │  └─────────────────┘│                                 │
│  └─────────────────────┘                                 │
└──────────────────────────────────────────────────────────┘
```

### 2.2 Delegados multicast

Un delegado puede contener una **cadena de métodos** (*invocation list*). Se añaden con `+=` y se quitan con `-=`. Al invocar, se ejecutan **todos en orden**.

```csharp
Operation pipeline = Add;       // pipeline → [Add]
pipeline += Multiply;           // pipeline → [Add, Multiply]
pipeline += Subtract;           // pipeline → [Add, Multiply, Subtract]

int result = pipeline(10, 5);
// Ejecuta Add(10,5) → Multiply(10,5) → Subtract(10,5)
// result = 5 (valor del ÚLTIMO método ejecutado: Subtract)
```

**IMPORTANTE**: si el delegado tiene valor de retorno, solo se obtiene el valor del **último** método ejecutado. Los valores intermedios se descartan. Si necesitas todos los resultados, itera manualmente con `GetInvocationList()`.

**Estructura multicast**:

```
┌──────────────────────────────────────────────────────┐
│         Invocation List de un delegado multicast     │
│                                                      │
│  pipeline                                             │
│     │                                                │
│     ▼                                                │
│  ┌──────────┐    ┌──────────────┐    ┌───────────┐  │
│  │  Add     │───▶│  Multiply    │───▶│ Subtract  │  │
│  │ _prev=null│   │ _prev=Add    │   │_prev=Mult │  │
│  └──────────┘    └──────────────┘    └───────────┘  │
│                                                      │
│  Invocar pipeline(10,5):                             │
│    → Add(10,5)      → retorna 15 (se descarta)      │
│    → Multiply(10,5) → retorna 50 (se descarta)      │
│    → Subtract(10,5) → retorna 5  (valor final)      │
└──────────────────────────────────────────────────────┘
```

**Comportamiento de `-=`**: quitar un método de la cadena recorre la lista buscando una coincidencia. Si el método aparece varias veces, solo elimina la **última** ocurrencia (en realidad, la implementación de `MulticastDelegate` elimina la última coincidencia al reconstruir la lista). Si tras quitar no queda ningún método, el delegado se convierte en `null`.

```csharp
pipeline -= Multiply;  // pipeline → [Add, Subtract]
// Si quitas todos: pipeline -= Add; pipeline -= Subtract; → pipeline == null
```

### 2.3 Delegados predefinidos: `Func`, `Action`, `Predicate`

El BCL incluye delegados genéricos que cubren la mayoría de casos. Rara vez necesitas declarar tus propios delegados:

| Delegado | Firma | Retorno | Uso típico |
|---|---|---|---|
| `Action` | `void()` | `void` | Método sin parámetros ni retorno |
| `Action<T>` | `void(T)` | `void` | Callback que no devuelve nada |
| `Action<T1, T2>` | `void(T1, T2)` | `void` | Hasta 16 parámetros |
| `Func<TResult>` | `TResult()` | `TResult` | Factory, lazy evaluation |
| `Func<T, TResult>` | `TResult(T)` | `TResult` | Selector, transformación |
| `Func<T1, T2, TResult>` | `TResult(T1, T2)` | `TResult` | Hasta 16 parámetros |
| `Predicate<T>` | `bool(T)` | `bool` | Filtro, condición |

```csharp
// Equivalencias:
Func<int, int, int> funcSum = (a, b) => a + b;
Action<string, int> logAction = (msg, n) => Console.WriteLine($"{msg}: {n}");
Predicate<int> isEven = n => n % 2 == 0;

// Sin delegados predefinidos tendrías que declarar:
delegate int IntOperation(int a, int b);
delegate void LogDelegate(string msg, int n);
delegate bool IntPredicate(int n);
```

### 2.4 Evento: un delegado protegido

Un **evento** es un miembro de una clase que encapsula un delegado con dos restricciones fundamentales:

1. **Solo la clase propietaria puede invocarlo** (dispararlo).
2. **Desde fuera solo se puede suscribir** (`+=`) **y desuscribir** (`-=`).

Se declara con la palabra clave `event`:

```csharp
class Button
{
    // 'event' + tipo delegado + nombre
    // Desde fuera: button.Clicked += myHandler;  ✅
    // Desde fuera: button.Clicked(...);           ❌ No compila
    // Desde fuera: button.Clicked = myHandler;    ❌ No compila
    public event EventHandler? Clicked;

    protected virtual void OnClicked(EventArgs e)
    {
        Clicked?.Invoke(this, e);
    }

    public void Press()
    {
        OnClicked(EventArgs.Empty); // Solo Button puede disparar
    }
}
```

**¿Por qué existe `event`?** Sin la keyword `event`, un delegado público sería completamente vulnerable:

```csharp
// ❌ SIN 'event': cualquier código externo puede hacer esto
button.Clicked = null;           // Borrar todos los suscriptores
button.Clicked += malicioso;     // Sobrescribir (si usas = en vez de +=)
button.Clicked.Invoke(...);      // DISPARAR el evento desde fuera
```

Con `event`, el compilador genera un *wrapper* que solo expone `add_Clicked` y `remove_Clicked` en el IL, impidiendo la invocación y la asignación directa desde fuera.

### 2.5 Patrón estándar de eventos en .NET

El ecosistema .NET sigue un **patrón consistente** para declarar eventos:

```
┌────────────────────────────────────────────────────────────────┐
│                  PATRÓN ESTÁNDAR DE EVENTOS                    │
│                                                                │
│  1. EventArgs personalizados heredan de EventArgs             │
│     class OrderEventArgs : EventArgs { }                       │
│                                                                │
│  2. El evento usa EventHandler<TEventArgs>                    │
│     public event EventHandler<OrderEventArgs>? OrderPlaced;    │
│                                                                │
│  3. Método disparador protected virtual OnXxx()               │
│     protected virtual void OnOrderPlaced(OrderEventArgs e)     │
│     {                                                          │
│         OrderPlaced?.Invoke(this, e);                          │
│     }                                                          │
│                                                                │
│  4. sender = this (quién disparó)                              │
│  5. e = EventArgs con los datos del evento                     │
│  6. ?.Invoke: seguro frente a null (sin suscriptores)          │
└────────────────────────────────────────────────────────────────┘
```

**Ventajas de `protected virtual`**: permite que una subclase intercepte el evento antes de que se notifique a los suscriptores externos:

```csharp
class LoggingButton : Button
{
    protected override void OnClicked(EventArgs e)
    {
        Console.WriteLine($"[{DateTime.Now}] Botón clickeado");
        base.OnClicked(e); // Llama a la implementación base que dispara el evento
    }
}
```

### 2.6 Comparación con otros lenguajes

| Concepto | C# | Java | JavaScript |
|---|---|---|---|
| Callback como valor | Delegado (`delegate`, `Func<>`, `Action<>`) | Interfaz funcional (`Consumer<T>`, `Function<T,R>`) | Función como valor (first-class) |
| Observer / Pub-Sub | `event` + `EventHandler<T>` | `EventListener` + interfaces (`ActionListener`) | `EventEmitter` / `addEventListener` |
| Multicast | Nativo con `+=` / `-=` | No nativo (listas manuales) | Nativo con `addEventListener` |
| Type safety en callbacks | ✅ Total (el delegado es un tipo) | ✅ Con interfaz funcional | ❌ No tiene (las funciones son `Function`) |
| Invocación restringida | ✅ `event` impide invocación externa | ❌ Cualquiera puede llamar a `fireEvent()` | ✅ Similar con `.emit()` privado |

---

## 3. Ejemplo mínimo

```csharp
// ─── Declaración del delegado ───
// Define un tipo que referencia métodos que reciben un string y devuelven un string
delegate string Greeter(string name);

// ─── Uso con lambda ───
Greeter greet = name => $"¡Hola, {name}!";
string result = greet("Antonio");

Console.WriteLine(result); // "¡Hola, Antonio!"
```

**Explicación línea por línea**:

1. `delegate string Greeter(string name);` — Declara un **tipo** llamado `Greeter`. Cualquier variable de tipo `Greeter` puede almacenar una referencia a un método que reciba `string` y devuelva `string`.
2. `Greeter greet = name => $"¡Hola, {name}!";` — Crea una instancia del delegado `Greeter` y le asigna una **lambda** (función anónima) que cumple la firma. La lambda recibe `name` (string) y devuelve un string interpolado.
3. `string result = greet("Antonio");` — Invoca el delegado como si fuera el método original. Internamente, el CLR llama a `greet.Invoke("Antonio")`.
4. El resultado es `"¡Hola, Antonio!"`.

**Equivalente con método nombrado**:

```csharp
// Método tradicional
static string SpanishGreet(string name)
{
    return $"¡Hola, {name}!";
}

Greeter greet = SpanishGreet;  // Asignación directa (grupo de métodos)
string result = greet("Ana");  // "¡Hola, Ana!"
```

---

## 4. Ejemplo real

Consulta el código en `src/PooEvents/Program.cs`. Este programa demuestra delegados y eventos en varios escenarios progresivos:

### 4.1 Delegado multicast con métodos matemáticos

Se declara un delegado personalizado `Operation` (`delegate int Operation(int a, int b)`) y se implementan tres métodos: `Add`, `Multiply` y `Subtract`.

```csharp
// Delegado personalizado — convención: delegados se declaran en ámbito de namespace
delegate int Operation(int a, int b);

// Cadena multicast
Operation pipeline = Add;
pipeline += Multiply;     // pipeline → [Add, Multiply]
pipeline += Subtract;     // pipeline → [Add, Multiply, Subtract]

int final = pipeline(10, 5);
// Se ejecutan Add(10,5)=15, Multiply(10,5)=50, Subtract(10,5)=5
// final = 5 (solo se obtiene el return del último método)

// Quitar un método de la cadena
pipeline -= Multiply;     // pipeline → [Add, Subtract]
```

### 4.2 Métodos anónimos y lambdas

El programa muestra la evolución histórica:

| Versión C# | Sintaxis | Ejemplo |
|---|---|---|
| C# 1.0 | Delegado con método nombrado | `Operation op = new Operation(Add);` |
| C# 2.0 | Método anónimo | `delegate(int a, int b) { return a + b; }` |
| C# 3.0+ | Lambda de expresión | `(a, b) => a + b` |
| C# 3.0+ | Lambda de bloque | `(a, b) => { return a + b; }` |

También se usan delegados genéricos predefinidos:

```csharp
Func<int, int, int> funcSum = (a, b) => a + b;
Action<string, int> logAction = (msg, n) => Console.WriteLine($"{msg}: {n}");
Predicate<int> isEven = n => n % 2 == 0;
```

### 4.3 `Button` con evento `Clicked`

La clase `Button` simula un botón de UI y expone el evento `Clicked` siguiendo el patrón estándar:

| Elemento | Implementación |
|---|---|
| **Evento** | `public event EventHandler? Clicked;` — `EventHandler` es `delegate void EventHandler(object? sender, EventArgs e)` |
| **Disparador** | `protected virtual void OnClicked(EventArgs e)` — `protected virtual` permite que subclases lo intercepten |
| **Invocación segura** | `Clicked?.Invoke(this, e)` — el operador `?.` evita `NullReferenceException` si no hay suscriptores |
| **Acción pública** | `public void Click()` → llama a `OnClicked(EventArgs.Empty)` |

**Suscriptores**:
- `Display.OnButtonClicked()` — muestra feedback visual en consola.
- `Logger.OnButtonClicked()` — registra el evento con timestamp.

### 4.4 `Downloader` con eventos de progreso y finalización

La clase `Downloader` simula la descarga de un archivo con dos eventos:

| Evento | Tipo | Datos |
|---|---|---|
| `ProgressChanged` | `EventHandler<DownloadEventArgs>` | Progreso (%), bytes descargados, total bytes, nombre de archivo |
| `DownloadCompleted` | `EventHandler` | `EventArgs.Empty` (sin datos adicionales) |

`DownloadEventArgs` hereda de `EventArgs` e incluye propiedades con `init` accessor (C# 9+) para inmutabilidad tras construcción.

**Suscriptores**:
- `ProgressBar.OnProgressChanged()` — dibuja una barra de progreso ASCII: `[########------------] 40%`.
- `NotificationService.OnDownloadCompleted()` — simula el envío de una notificación push al finalizar.

**Flujo de ejecución**:

```
Downloader.DownloadAsync()
  │
  ├─▶ Task.Delay(100-400ms)  ← simula latencia de red
  ├─▶ OnProgressChanged(args) → ProgressChanged?.Invoke(this, args)
  │     ├─▶ ProgressBar.OnProgressChanged()    → "  [####--------------] 20% ..."
  │     └─▶ (otros suscriptores)
  │
  ... (se repite 10 veces: 10%, 20%, ..., 100%)
  │
  └─▶ OnDownloadCompleted(EventArgs.Empty) → DownloadCompleted?.Invoke(this, e)
        └─▶ NotificationService.OnDownloadCompleted() → "¡Descarga completada!"
```

### 4.5 Desuscripción de eventos (`-=`)

El programa demuestra que puedes quitar suscriptores en cualquier momento. Tras desuscribir al `Logger`, el segundo clic del botón solo es manejado por el `Display`.

### 4.6 Ejecución del ejemplo

```bash
dotnet run --project ejemplos/m02-poo/Leccion-06-Delegados-Eventos/src/PooEvents
```

---

## 5. Buenas prácticas

### ✅ Hacer

1. **Usar `EventHandler<T>` en lugar de delegados custom para eventos**:

   ```csharp
   // ✅ Bien: sigue la convención del ecosistema
   public event EventHandler<OrderEventArgs>? OrderPlaced;

   // ❌ Mal: delegado custom innecesario
   public delegate void OrderPlacedHandler(OrderEventArgs e);
   public event OrderPlacedHandler? OrderPlaced; // Fuera de convención
   ```

   `EventHandler<T>` es reconocido por herramientas, analizadores, y otros desarrolladores. Los delegados custom solo cuando el nombre comunica una intención de dominio muy específica.

2. **Siempre comprobar null antes de invocar**:

   ```csharp
   // ✅ Bien: null-conditional operator (C# 6+)
   MyEvent?.Invoke(this, args);

   // ✅ Bien (alternativa pre-C#6): guardia explícita
   var handler = MyEvent;
   if (handler is not null)
       handler(this, args);

   // ❌ Mal: NullReferenceException si no hay suscriptores
   MyEvent(this, args);
   ```

   La copia local (`var handler = MyEvent`) es importante en entornos multihilo pre-C#6 para evitar que otro hilo desuscriba el último handler entre la comprobación y la invocación.

3. **Crear clases de EventArgs propias que hereden de `EventArgs`**:

   ```csharp
   // ✅ Bien: datos del evento tipados y documentados
   class OrderEventArgs : EventArgs
   {
       public int OrderId { get; init; }
       public decimal Total { get; init; }
       public DateTime PlacedAt { get; init; }
   }

   // ❌ Mal: pasar datos sueltos rompe el patrón
   public event Action<int, decimal>? OrderPlaced; // ¿Qué es int? ¿Qué es decimal?
   ```

4. **Siempre pasar `this` como `sender`**:

   ```csharp
   // ✅ Bien: el receptor sabe quién disparó
   MyEvent?.Invoke(this, args);

   // ❌ Mal: sender nulo o incorrecto
   MyEvent?.Invoke(null, args); // El receptor no sabe quién disparó
   ```

5. **Hacer el método disparador `protected virtual`**:

   ```csharp
   // ✅ Bien: subclases pueden interceptar el evento
   protected virtual void OnOrderPlaced(OrderEventArgs e)
   {
       OrderPlaced?.Invoke(this, e);
   }

   // ❌ Mal: privado impide que subclases añadan comportamiento
   private void FireOrderPlaced(OrderEventArgs e)
   {
       OrderPlaced?.Invoke(this, e);
   }
   ```

6. **Desuscribirse de eventos cuando ya no se necesitan**:

   ```csharp
   // El publisher mantiene una referencia al subscriber.
   // Si el subscriber vive menos que el publisher → memory leak.
   class Subscriber : IDisposable
   {
       private readonly Publisher _publisher;

       public Subscriber(Publisher publisher)
       {
           _publisher = publisher;
           _publisher.SomethingHappened += HandleEvent;
       }

       public void Dispose()
       {
           _publisher.SomethingHappened -= HandleEvent; // Evita memory leak
       }
   }
   ```

7. **Usar delegados predefinidos (`Func`, `Action`) para callbacks simples**:

   ```csharp
   // ✅ Bien: para callbacks que no son eventos
   public Task ProcessAsync(Action<int> progressCallback)
   {
       // Func y Action son perfectos para parámetros callback
   }

   // Solo usa delegados custom cuando el nombre comunica dominio
   delegate bool DiscountEligibilityValidator(Customer customer, Order order);
   ```

### ❌ Evitar

1. **No usar `async void` en handlers de eventos salvo casos muy concretos**:

   ```csharp
   // ❌ Peligroso: excepciones se pierden, no se puede await
   button.Clicked += async (s, e) =>
   {
       await SaveToDatabaseAsync(); // Si falla, la excepción revienta el proceso
   };

   // ✅ Mejor: manejar la excepción explícitamente
   button.Clicked += async (s, e) =>
   {
       try
       {
           await SaveToDatabaseAsync();
       }
       catch (Exception ex)
       {
           _logger.LogError(ex, "Error al guardar tras clic");
       }
   };
   ```

   `async void` solo es aceptable en handlers de eventos de UI porque la firma `void` es obligatoria para delegados como `EventHandler`. En Minimal APIs o cualquier otro contexto, prefiere `async Task`.

2. **No exponer el delegado subyacente como campo público**:

   ```csharp
   // ❌ Mal: cualquiera puede invocarlo, asignarlo a null, etc.
   public EventHandler<EventArgs> SomethingHappened;

   // ✅ Bien: la keyword 'event' lo protege
   public event EventHandler<EventArgs>? SomethingHappened;
   ```

3. **No asumir el orden de ejecución de los handlers**: los suscriptores se ejecutan en orden de suscripción, pero **no debes depender de ello**. Si el orden importa, diseña un mecanismo explícito (pipeline, lista ordenada, prioridad).

4. **No lanzar excepciones en handlers sin capturarlas**: si un handler lanza una excepción, los handlers restantes en la *invocation list* **no se ejecutan**. Usa try/catch dentro de cada handler si la fiabilidad es crítica.

---

## 6. Errores habituales

### Error 1 — No desuscribirse de eventos (memory leak)

```csharp
// ❌ INCORRECTO: el subscriber vive más que el publisher, pero
//    el publisher mantiene viva la referencia del subscriber.
class Dashboard
{
    private readonly PriceService _priceService;

    public Dashboard(PriceService priceService)
    {
        _priceService = priceService;
        _priceService.PriceUpdated += OnPriceUpdated; // Suscripción
    }

    // Si Dashboard se deja de usar pero PriceService sigue vivo,
    // Dashboard NUNCA será garbage collected porque PriceService
    // tiene una referencia a OnPriceUpdated.
}

// ✅ CORRECTO: implementar IDisposable y desuscribirse
class Dashboard : IDisposable
{
    private readonly PriceService _priceService;

    public Dashboard(PriceService priceService)
    {
        _priceService = priceService;
        _priceService.PriceUpdated += OnPriceUpdated;
    }

    public void Dispose()
    {
        _priceService.PriceUpdated -= OnPriceUpdated;
    }
}
```

### Error 2 — Invocar evento sin comprobar null

```csharp
// ❌ INCORRECTO
public void DoSomething()
{
    SomethingHappened(this, EventArgs.Empty); // NullReferenceException si no hay suscriptores
}

// ✅ CORRECTO
public void DoSomething()
{
    SomethingHappened?.Invoke(this, EventArgs.Empty); // Seguro: no hace nada si es null
}
```

### Error 3 — Usar `=` en lugar de `+=` al suscribirse

```csharp
// ❌ INCORRECTO
button.Clicked = DisplayHandler;    // Sobrescribe: borra suscriptores anteriores
button.Clicked = LoggerHandler;     // Ahora solo está LoggerHandler

// ✅ CORRECTO
button.Clicked += DisplayHandler;   // Añade sin borrar
button.Clicked += LoggerHandler;    // Ambos están suscritos
```

### Error 4 — Pasar delegado nulo donde no se espera

```csharp
// ❌ INCORRECTO
Action<string>? callback = null;
callback("hola"); // NullReferenceException

// ✅ CORRECTO: usar null-conditional
callback?.Invoke("hola");

// ✅ CORRECTO: validar antes
if (callback is not null)
    callback("hola");
```

### Error 5 — Modificar la lista de suscriptores mientras se itera

```csharp
// ❌ INCORRECTO: modificar la invocation list durante la invocación
//    puede causar comportamientos impredecibles o excepciones
void OnSomethingHappened(EventArgs e)
{
    SomethingHappened?.Invoke(this, e);
    // Si un handler dentro de Invoke se desuscribe, la lista cambia
    // mientras se está iterando.
}

// ✅ CORRECTO: si necesitas modificar suscriptores desde un handler,
//    hazlo después de la invocación o usa una copia local.
void OnSomethingHappened(EventArgs e)
{
    var handler = SomethingHappened;
    handler?.Invoke(this, e); // Itera sobre una copia estable
}
```

---

## 7. Ejercicio guiado

Vamos a construir paso a paso un **sistema de notificaciones** para gestión de órdenes usando eventos.

### Paso 1 — Argumentos del evento

```csharp
// EventArgs personalizado que transporta los datos de la orden
class OrderEventArgs : EventArgs
{
    public Guid OrderId { get; init; }
    public string CustomerEmail { get; init; }
    public decimal Total { get; init; }
    public IReadOnlyList<string> Items { get; init; }
    public DateTime PlacedAt { get; init; }

    public OrderEventArgs(
        Guid orderId,
        string customerEmail,
        decimal total,
        IReadOnlyList<string> items)
    {
        OrderId = orderId;
        CustomerEmail = customerEmail;
        Total = total;
        Items = items;
        PlacedAt = DateTime.UtcNow;
    }
}
```

### Paso 2 — `OrderService` (publisher)

```csharp
class OrderService
{
    // Evento que se dispara cuando se crea una nueva orden
    // EventHandler<T> = delegate void EventHandler<TEventArgs>(object? sender, TEventArgs e)
    public event EventHandler<OrderEventArgs>? OrderPlaced;

    // Método público que la aplicación llama para crear órdenes
    public void PlaceOrder(string customerEmail, decimal total, List<string> items)
    {
        var orderId = Guid.NewGuid();
        var args = new OrderEventArgs(orderId, customerEmail, total, items.AsReadOnly());

        Console.WriteLine($"[OrderService] Orden {orderId} creada. Total: {total:C}");

        // Disparar el evento para notificar a todos los suscriptores
        OnOrderPlaced(args);
    }

    // Método disparador: protected virtual para permitir subclases
    protected virtual void OnOrderPlaced(OrderEventArgs e)
    {
        OrderPlaced?.Invoke(this, e);
    }
}
```

### Paso 3 — Servicios suscriptores

```csharp
class EmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    // Manejador de evento: envía email de confirmación
    public void OnOrderPlaced(object? sender, OrderEventArgs e)
    {
        Console.WriteLine(
            $"  [EmailService] Enviando confirmación a {e.CustomerEmail} " +
            $"para la orden {e.OrderId}...");

        // En producción: _emailSender.SendAsync(e.CustomerEmail, template, model);
        Console.WriteLine("  [EmailService] Email enviado.");
    }
}

class InventoryService
{
    public void OnOrderPlaced(object? sender, OrderEventArgs e)
    {
        Console.WriteLine($"  [InventoryService] Actualizando stock para {e.Items.Count} items...");

        foreach (var item in e.Items)
        {
            Console.WriteLine($"    - Descontando: {item}");
        }

        Console.WriteLine("  [InventoryService] Stock actualizado.");
    }
}

class LoggerService
{
    public void OnOrderPlaced(object? sender, OrderEventArgs e)
    {
        Console.WriteLine(
            $"  [LoggerService] [{e.PlacedAt:yyyy-MM-dd HH:mm:ss}] " +
            $"Orden={e.OrderId}, Cliente={e.CustomerEmail}, Total={e.Total:C}");
    }
}
```

### Paso 4 — Componer el sistema

```csharp
// Crear instancias
var orderService = new OrderService();
var emailService = new EmailService();
var inventoryService = new InventoryService();
var loggerService = new LoggerService();

// Suscribir servicios al evento
orderService.OrderPlaced += emailService.OnOrderPlaced;
orderService.OrderPlaced += inventoryService.OnOrderPlaced;
orderService.OrderPlaced += loggerService.OnOrderPlaced;

// Crear una orden
orderService.PlaceOrder(
    customerEmail: "cliente@example.com",
    total: 149.99m,
    items: ["Teclado mecánico", "Ratón ergonómico", "Alfombrilla XXL"]
);

// Salida:
// [OrderService] Orden abc-123 creada. Total: 149,99 €
//   [EmailService] Enviando confirmación a cliente@example.com...
//   [EmailService] Email enviado.
//   [InventoryService] Actualizando stock para 3 items...
//     - Descontando: Teclado mecánico
//     - Descontando: Ratón ergonómico
//     - Descontando: Alfombrilla XXL
//   [InventoryService] Stock actualizado.
//   [LoggerService] [2025-01-15 10:30:00] Orden=abc-123, Cliente=cliente@example.com, Total=149,99 €
```

### Paso 5 — Añadir validación al método disparador

```csharp
class OrderService
{
    public event EventHandler<OrderEventArgs>? OrderPlaced;

    public void PlaceOrder(string customerEmail, decimal total, List<string> items)
    {
        // Validación antes de disparar el evento
        if (string.IsNullOrWhiteSpace(customerEmail))
            throw new ArgumentException("Email del cliente requerido", nameof(customerEmail));

        if (total <= 0)
            throw new ArgumentException("Total debe ser positivo", nameof(total));

        if (items is null || items.Count == 0)
            throw new ArgumentException("La orden debe tener al menos un item", nameof(items));

        var args = new OrderEventArgs(Guid.NewGuid(), customerEmail, total, items.AsReadOnly());

        Console.WriteLine($"[OrderService] Orden {args.OrderId} creada. Total: {total:C}");
        OnOrderPlaced(args);
    }

    protected virtual void OnOrderPlaced(OrderEventArgs e)
    {
        OrderPlaced?.Invoke(this, e);
    }
}
```

**Puntos importantes**:
- El publisher (`OrderService`) no conoce a los subscribers. Solo expone el evento.
- Cada subscriber tiene **una única responsabilidad** (SRP): email, inventario, logging.
- Añadir un nuevo subscriber (ej. `AnalyticsService`) no requiere modificar el publisher (OCP).
- El patrón `protected virtual OnXxx` permite que una subclase como `AuditedOrderService` sobrescriba el método para añadir auditoría antes de disparar el evento.

---

## 8. Reto

Crea un `class EventAggregator` (patrón **Mediator** simplificado) que permita publicar y suscribirse a eventos por tipo, desacoplando completamente emisores y receptores.

### Requisitos

1. **API del EventAggregator**:

   ```csharp
   class EventAggregator
   {
       // Suscribirse a mensajes de tipo T
       // Devuelve un IDisposable para desuscribirse
       public IDisposable Subscribe<T>(Action<T> handler)
       {
           // Tu implementación
       }

       // Publicar un mensaje de tipo T
       // Todos los handlers suscritos a T se ejecutan
       public void Publish<T>(T message)
       {
           // Tu implementación
       }
   }
   ```

2. **Implementación interna**: usa un `Dictionary<Type, List<object>>` o similar para almacenar los handlers por tipo de mensaje. El método `Subscribe<T>` debe devolver algo que permita desuscribirse (un `IDisposable` que al hacer `Dispose()` elimine el handler).

3. **Clases de mensaje** (simples DTOs, sin comportamiento):

   ```csharp
   record OrderShippedMessage(Guid OrderId, string TrackingNumber);
   record PaymentProcessedMessage(Guid OrderId, decimal Amount);
   ```

4. **Servicios desacoplados** que se comunican exclusivamente a través del `EventAggregator`:

   - `BillingService`: cuando procesa un pago, publica `PaymentProcessedMessage`.
   - `ShippingService`: se suscribe a `PaymentProcessedMessage` y, al recibirlo, publica `OrderShippedMessage`.
   - `NotificationService`: se suscribe a `OrderShippedMessage` y envía un email con el tracking number.
   - `AuditService`: se suscribe a **ambos** mensajes y registra todo.

5. **Demostración**: en `Main`, crea el `EventAggregator`, instancia los servicios, suscríbelos y publica un mensaje inicial. Verifica que todos los servicios reaccionan correctamente **sin conocerse entre sí**.

**Pistas**:
- Usa un `ConcurrentDictionary<Type, List<object>>` para thread-safety.
- Para `Subscribe<T>`, crea una clase interna `Subscription : IDisposable` que contenga una referencia al diccionario, la clave (Type) y el handler. Al hacer `Dispose()`, elimina el handler de la lista.
- Al hacer `Publish<T>`, itera sobre los handlers. Como `List<object>` contiene `Action<T>`, necesitas un cast: `(handler as Action<T>)?.Invoke(message)`.
- Los handlers pueden lanzar excepciones. Envuélvelos en try/catch para que un handler roto no impida que los demás se ejecuten.

**Ejemplo de uso esperado**:

```csharp
var aggregator = new EventAggregator();

// Crear servicios (inyectándoles el aggregator)
var billing = new BillingService(aggregator);
var shipping = new ShippingService(aggregator);
var notifier = new NotificationService(aggregator);
var audit = new AuditService(aggregator);

// Los servicios se suscriben en su constructor
// (no necesitas hacerlo manualmente aquí)

// Simular: BillingService procesa un pago
billing.ProcessPayment(Guid.NewGuid(), 99.99m);

// Salida esperada (orden aproximado):
// [AuditService] Registrado pago: OrderId=..., Amount=99,99
// [ShippingService] Recibido pago para OrderId=... — preparando envío...
// [ShippingService] Orden enviada: Tracking=TRK-12345
// [AuditService] Registrado envío: OrderId=..., Tracking=TRK-12345
// [NotificationService] Enviando email con tracking TRK-12345...
```

¡Inténtalo antes de mirar la solución!

---

## 9. Resumen

### Conceptos clave

| Concepto | Propósito | Sintaxis |
|---|---|---|
| **Delegado** | Tipo que referencia métodos con una firma concreta | `delegate TResult Nombre(params);` |
| **Delegado multicast** | Encadenar varios métodos para ejecutarlos en orden | `del += método; del -= método;` |
| **Evento** | Delegado protegido: solo la clase dueña lo invoca | `public event EventHandler<T>? Nombre;` |
| **EventHandler\<T\>** | Delegado estándar para eventos con datos tipados | `EventHandler<OrderEventArgs>` |
| **EventArgs** | Clase base para transportar datos en un evento | `class MiEventArgs : EventArgs { }` |
| **Método anónimo** | Delegado inline sin nombre (C# 2.0) | `delegate(int a, int b) { return a + b; }` |
| **Lambda** | Sintaxis concisa para crear delegados (C# 3.0+) | `(a, b) => a + b` |
| **`Func<T, TResult>`** | Delegado genérico con retorno | `Func<int, int, int> f = (a, b) => a + b;` |
| **`Action<T>`** | Delegado genérico sin retorno (void) | `Action<string> log = msg => Console.WriteLine(msg);` |
| **`Predicate<T>`** | Delegado genérico que devuelve bool | `Predicate<int> isPositive = n => n > 0;` |
| **`?.Invoke`** | Invocación segura de delegados nulables | `MiEvento?.Invoke(this, args);` |
| **Patrón OnXxx** | Método disparador `protected virtual` | `protected virtual void OnClicked(EventArgs e)` |
| **`+=` / `-=`** | Suscribir y desuscribir handlers a eventos | `button.Clicked += handler;` |

### Cuadro comparativo: delegado vs evento

| Aspecto | Delegado | Evento |
|---|---|---|
| ¿Es un tipo? | Sí (`delegate int Op(int, int)`) | No, es un **miembro** de una clase |
| ¿Quién puede invocarlo? | Cualquiera con la referencia | Solo la clase propietaria |
| ¿Quién puede suscribir/desuscribir? | Cualquiera con la referencia | Cualquiera con la referencia |
| ¿Quién puede asignar con `=`? | Cualquiera con la referencia | Nadie desde fuera (solo `+=` y `-=`) |
| ¿Cuándo usarlo? | Callbacks, estrategias, LINQ | Notificaciones, patrón observer |
| Ejemplo | `Func<T, bool>` en `.Where()` | `Button.Click` en UI |

### Flujo completo del patrón de eventos .NET

```
┌─────────────────────────────────────────────────────────────────┐
│               FLUJO DEL PATRÓN DE EVENTOS .NET                  │
│                                                                 │
│  ┌──────────────┐   1. Suscripción (+=)    ┌──────────────────┐ │
│  │  Publisher   │◄────────────────────────│  Subscriber A    │ │
│  │              │                         │  (Display)       │ │
│  │  + event     │◄────────────────────────│                  │ │
│  │    Clicked   │                         └──────────────────┘ │
│  │              │                         ┌──────────────────┐ │
│  │  + Click()   │◄────────────────────────│  Subscriber B    │ │
│  │    │         │                         │  (Logger)        │ │
│  │    ▼         │                         │                  │ │
│  │  OnClicked() │                         └──────────────────┘ │
│  │    │         │                                               │
│  │    ▼         │  2. Invocación                               │
│  │  Clicked?    │  Clicked?.Invoke(this, EventArgs.Empty)      │
│  │  .Invoke()───┤───────────────────────────────────────────►  │
│  │              │         3. Ejecución en orden                │
│  └──────────────┘         Subscriber A → Subscriber B          │
└─────────────────────────────────────────────────────────────────┘
```

### Delegados predefinidos: cuándo usar cada uno

| Situación | Delegado |
|---|---|
| Callback sin retorno ni parámetros | `Action` |
| Callback sin retorno con 1 parámetro | `Action<T>` |
| Callback sin retorno con N parámetros | `Action<T1, T2, ...>` |
| Función que devuelve valor, sin parámetros | `Func<TResult>` |
| Función que devuelve valor, con 1 parámetro | `Func<T, TResult>` |
| Función que devuelve valor, con N parámetros | `Func<T1, T2, ..., TResult>` |
| Condición booleana sobre un tipo | `Predicate<T>` |
| Evento sin datos personalizados | `EventHandler` |
| Evento con datos personalizados | `EventHandler<TEventArgs>` |
| Comparación de dos valores | `Comparison<T>` (es `Func<T, T, int>`) |

---

## 10. Preguntas frecuentes

### ¿Cuál es la diferencia entre un delegado y un evento?

El **delegado es el tipo** (`delegate void Handler(int x)`). El **evento es la protección de acceso** sobre ese tipo. Es análogo a la diferencia entre un campo y una propiedad: el campo (delegado) es el almacenamiento, la propiedad (evento) controla el acceso.

```csharp
class Ejemplo
{
    public Action<int> DelegadoPublico;  // Cualquiera puede: invoke, assign, null
    public event Action<int>? Evento;    // Desde fuera solo: += y -=
}
```

**Regla práctica**: si expones un delegado como miembro público y no quieres que código externo lo invoque o lo ponga a `null`, usa `event`.

### ¿Puedo invocar un evento desde fuera de la clase que lo declara?

**No directamente**. El compilador genera un error CS0070: *"The event 'X' can only appear on the left hand side of += or -="*. Esto es por diseño: la keyword `event` protege la *invocation list* de manipulaciones externas.

```csharp
class MyClass
{
    public event EventHandler? SomethingHappened;
}

var obj = new MyClass();
obj.SomethingHappened += Handler;    // ✅ Válido
obj.SomethingHappened -= Handler;    // ✅ Válido
// obj.SomethingHappened?.Invoke();  // ❌ CS0070
// obj.SomethingHappened = null;     // ❌ CS0070
```

Si **necesitas** que código externo dispare el evento, expón un método público que lo haga:

```csharp
class MyClass
{
    public event EventHandler? SomethingHappened;

    public void RaiseSomethingHappened()
    {
        SomethingHappened?.Invoke(this, EventArgs.Empty);
    }
}
```

### ¿Cuándo usar un delegado custom vs `Func`/`Action`?

- **Usa `Func<>` / `Action<>`** para callbacks genéricos, parámetros de método, LINQ, etc. Son la opción por defecto.
- **Usa un delegado custom** cuando el **nombre del delegado comunica la intención del dominio**. Por ejemplo:
  ```csharp
  // Comunica mejor la intención que Func<Customer, Order, bool>
  delegate bool DiscountEligibilityValidator(Customer customer, Order order);
  ```

- Si necesitas parámetros `ref`, `in`, `out` o `params`, necesitas un delegado custom: `Func` y `Action` no los soportan.

### ¿Los eventos son síncronos o asíncronos?

**Por defecto son síncronos**. `EventHandler` es un delegado sincrono (`void`). Al invocar `Clicked?.Invoke(this, e)`, todos los handlers se ejecutan **secuencialmente** en el hilo que disparó el evento. El código que dispara el evento se bloquea hasta que todos los handlers terminen.

**Si un handler es lento** (ej. llama a una API externa), **bloquea a todos los demás handlers y al emisor**. Para evitarlo:

```csharp
// ❌ Peligroso: async void como handler
button.Clicked += async (s, e) =>
{
    await Task.Delay(5000); // Bloquea otros handlers solo si todo es sync,
                            // pero las excepciones se pierden
};

// ✅ Mejor: el handler despacha a un background task
button.Clicked += (s, e) =>
{
    Task.Run(async () =>
    {
        try
        {
            await HeavyAsyncWork();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en handler async");
        }
    });
};
```

La alternativa es usar delegados async personalizados (`Func<Task>`) pero el ecosistema de eventos de .NET (`EventHandler`) no los soporta nativamente y requerirías un sistema de eventos propio.

### ¿Por qué `sender` es `object?` en lugar del tipo concreto?

Es una decisión de diseño histórica para permitir que **cualquier objeto** pueda ser el emisor. Permite suscripciones genéricas:

```csharp
// Un handler puede manejar eventos de múltiples emisores
void GenericClickHandler(object? sender, EventArgs e)
{
    // Puede venir de un Button, un MenuItem, un Link...
    if (sender is Button btn)
        Console.WriteLine($"Botón: {btn.Text}");
    else if (sender is MenuItem menu)
        Console.WriteLine($"Menú: {menu.Text}");
}
```

Posteriormente se añadió `EventHandler<TEventArgs>` que genéricamente acepta `object? sender`, manteniendo la compatibilidad hacia atrás.

### ¿Cómo funciona internamente `+=` y `-=` en eventos?

El compilador transforma `+=` y `-=` en llamadas a los métodos `add_X` y `remove_X` del evento. Estos métodos son thread-safe: usan `CompareExchange` para modificar la *invocation list* de forma atómica:

```csharp
// Código que escribes:
button.Clicked += handler;

// Código que genera el compilador (simplificado):
button.add_Clicked(handler);
// Internamente: crea un nuevo MulticastDelegate combinando la lista existente
//               con el nuevo handler usando Delegate.Combine()
```

`Delegate.Combine` crea un **nuevo** delegado inmutable con la lista combinada. Los delegados son inmutables; `+=` no modifica el existente, crea uno nuevo y reasigna.

### ¿Qué pasa si desuscribo un handler que no estaba suscrito?

No pasa nada. `Delegate.Remove` devuelve la lista sin cambios si no encuentra el handler. No lanza excepción, simplemente no tiene efecto.

```csharp
button.Clicked += handlerA;
button.Clicked -= handlerB; // handlerB nunca estuvo suscrito → sin efecto
button.Clicked -= handlerA; // Correcto: elimina handlerA
button.Clicked -= handlerA; // handlerA ya fue eliminado → sin efecto
// Ahora Clicked es null
```
