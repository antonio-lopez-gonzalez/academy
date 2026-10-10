
# Lección 02 — Principio de Abierto/Cerrado (OCP)

---

## 1. Introducción

**¿Qué vamos a aprender?**

El **Principio de Abierto/Cerrado** (Open/Closed Principle, OCP) es la "O" de SOLID. Fue formulado por Bertrand Meyer en 1988 y dice:

> "Las entidades de software (clases, módulos, funciones) deben estar **abiertas a extensión** pero **cerradas a modificación**."

En otras palabras: debes poder añadir nuevo comportamiento a un sistema sin tener que modificar el código existente.

**¿Por qué existe?**

Imagina un sistema de cálculo de áreas que usa un `switch` enorme para cada tipo de figura geométrica. Cada vez que añades una nueva figura (hexágono, trapecio, elipse), tienes que modificar esa función central. Cada modificación es una oportunidad de introducir un bug en código que ya funcionaba. OCP existe para eliminar ese riesgo.

**¿Qué problema resuelve?**

- **Fragilidad ante cambios**: modificar código existente para añadir funcionalidad nueva es peligroso. Puedes romper lo que ya funciona.
- **Efecto dominó**: un cambio en un módulo "cerrado" puede propagarse a todos sus dependientes.
- **Imposibilidad de extender sin acceso al código fuente**: si el código está en una librería de terceros, no puedes modificarlo. OCP permite extenderlo mediante herencia o composición.
- **Violación en cadena**: si una clase no cumple OCP, obliga a sus clientes a modificarse también cuando se añaden nuevos casos.

**¿Cuándo se utiliza?**

Siempre que preveas que un sistema va a crecer con nuevos tipos, nuevas reglas o nuevos comportamientos. En la práctica: casi siempre. Los requisitos cambian constantemente.

**Errores típicos**

- Creer que OCP significa "nunca modificar código existente". El código se modifica para corregir bugs. OCP aplica a la extensión de funcionalidad, no a la corrección de errores.
- Aplicar OCP prematuramente a código que nunca va a cambiar. Abstraer por abstraer añade complejidad innecesaria.
- Usar herencia cuando composición o interfaces serían mejores.

---

## 2. Teoría

### El problema del código "cerrado a secas"

Cuando el comportamiento está codificado con estructuras de control fijas (`if`/`else`, `switch`), añadir un nuevo caso requiere modificar el código fuente:

```
                    ┌─────────────────────┐
                    │   AreaCalculator    │
                    │                     │
                    │ if (Circle)  ...    │  ← Hay que MODIFICAR esta clase
                    │ else if (Rect) ...  │    cada vez que aparece una
                    │ else if (Tri)  ...  │    nueva forma geométrica
                    │ else throw          │
                    └─────────────────────┘
```

### La solución OCP: abstracción + polimorfismo

Definimos una abstracción (interfaz `IShape`) que cada forma implementa. El `AreaCalculator` depende solo de la abstracción, no de las formas concretas:

```
     ┌──────────┐                                      ┌──────────┐
     │  IShape  │◄─────────────────────────────────────│  Circle  │
     │          │                                       │          │
     │ Area()   │◄─────────────────────────────────────│Rectangle │
     └────┬─────┘                                       └──────────┘
          │
          │ usa
          │
┌─────────┴───────────┐
│  AreaCalculator     │
│                     │
│ TotalArea(IEnumerable<IShape>) │  ← NUNCA se modifica.
│                     │              Nuevas formas = nuevas clases.
└─────────────────────┘
```

Ahora, añadir un `Hexagon` solo requiere crear la clase `Hexagon : IShape`. El `AreaCalculator` no se toca. Está **abierto a extensión** (nuevas clases que implementan `IShape`) y **cerrado a modificación** (no se modifica su código fuente).

### El Strategy Pattern como aplicación de OCP

El patrón Strategy encapsula **algoritmos intercambiables** detrás de una interfaz común. Es OCP aplicado a comportamiento:

```
          ┌─────────────────┐
          │ ITaxCalculator  │  ← Abstracción (contrato)
          │ CalculateTax()  │
          └────────┬────────┘
                   │
      ┌────────────┼────────────────┐
      │            │                │
      ▼            ▼                ▼
┌───────────┐ ┌──────────┐ ┌──────────────┐
│ SpainTax  │ │  UsTax   │ │  ExportTax   │  ← Estrategias concretas
│ (21%)     │ │  (8%)    │ │  (0%)        │
└───────────┘ └──────────┘ └──────────────┘
```

El `TaxService` consume `ITaxCalculator` sin saber qué país es. Añadir Reino Unido es crear `UkTaxCalculator` y ya está. `TaxService` no se modifica.

### Comparación con otros lenguajes

| Concepto | Java | JavaScript/TypeScript | Python |
|---|---|---|---|
| OCP mediante interfaz | `interface` + `implements` | `interface` en TS, duck typing en JS | Protocolos / ABC |
| Strategy Pattern | Mismo patrón | Funciones de orden superior | Funciones como objetos |
| Herencia vs composición | Clases abstractas | Mixins, composición | Herencia múltiple (mixin) |

### ¿Cuándo NO aplicar OCP?

No abstraigas prematuramente. Si tienes solo 2-3 casos que no van a crecer, un `switch` es más simple y legible que una jerarquía de interfaces. Aplica OCP cuando:
- El número de variantes crece.
- Las variantes cambian con frecuencia.
- Distintos equipos mantienen distintas variantes.
- Necesitas añadir variantes sin acceso al código fuente original.

---

## 3. Ejemplo mínimo

```csharp
// ─── VIOLACIÓN OCP: switch que crece sin control ───
public class BadLogger
{
    public void Log(string message, string format)
    {
        switch (format)
        {
            case "console":
                Console.WriteLine($"[Console] {message}");
                break;
            case "file":
                File.AppendAllText("log.txt", message + "\n");
                break;
            case "json":
                Console.WriteLine($"{{'message': '{message}'}}");
                break;
            default:
                throw new ArgumentException($"Formato no soportado: {format}");
        }
        // Cada nuevo formato obliga a AÑADIR un case aquí → OCP violado
    }
}

// ─── CORRECTO OCP: abstracción con interfaz ───
public interface ILogger
{
    void Log(string message);
}

public class ConsoleLogger : ILogger
{
    public void Log(string message) => Console.WriteLine($"[Console] {message}");
}

public class FileLogger : ILogger
{
    public void Log(string message) => File.AppendAllText("log.txt", message + "\n");
}

public class JsonLogger : ILogger
{
    public void Log(string message) => Console.WriteLine($"{{'message': '{message}'}}");
}

// El consumidor depende de la abstracción, no de los formatos concretos.
public class AppService
{
    private readonly ILogger _logger;

    public AppService(ILogger logger)
    {
        _logger = logger;
    }

    public void DoWork()
    {
        _logger.Log("Tarea completada.");
        // No sabe ni le importa qué tipo de logger es.
    }
}
```

Para ejecutar el ejemplo completo:

```bash
dotnet run --project ejemplos/m03-solid/Leccion-02-OCP/src/SolidOCP
```

---

## 4. Ejemplo real

El código completo en `src/SolidOCP/Program.cs` demuestra OCP con dos escenarios reales:

### Escenario 1: Calculadora de áreas (líneas 17-448)

**Violación: `BadAreaCalculator`** (línea 52)
- Usa `if/else if` con `is` para discriminar tipos.
- Cada nueva forma geométrica requiere añadir una rama.
- El compilador no avisa si olvidas un tipo.
- Método `CalculateArea(object shape)` — el `object` es señal de que no hay abstracción.

**Solución: `GoodAreaCalculator` + `IShape`** (línea 110-188)
- Interfaz `IShape` con método `CalculateArea()`.
- Cada forma (`Circle`, `Rectangle`, `Triangle`, `Hexagon`) implementa `IShape`.
- `GoodAreaCalculator.TotalArea(IEnumerable<IShape>)` — solo conoce la abstracción.
- Añadir `Hexagon` fue crear una clase nueva. `GoodAreaCalculator` no se tocó.

### Escenario 2: Calculadora de impuestos con Strategy Pattern (líneas 194-285)

- Interfaz `ITaxCalculator` con `CalculateTax(Order)`.
- Estrategias concretas: `SpainTaxCalculator` (21%), `UsTaxCalculator` (8%), `ExportTaxCalculator` (0%), `UkTaxCalculator` (20%).
- `TaxService` recibe `ITaxCalculator` por constructor.
- Añadir Reino Unido solo requirió crear `UkTaxCalculator`. `TaxService` permaneció intacto.

### La demo lo demuestra lado a lado

El `Main()` (línea 293) ejecuta 4 demos secuenciales:
1. Muestra la versión mala con `BadAreaCalculator`.
2. Muestra la versión buena con `GoodAreaCalculator`.
3. Demuestra el Strategy Pattern con impuestos de varios países.
4. Compara explícitamente qué hay que tocar para añadir un Hexágono en cada versión.

---

## 5. Buenas prácticas

### ✅ QUÉ HACER

1. **Programar contra interfaces, no contra implementaciones**: `IEnumerable<IShape>` en lugar de `List<Circle>`.
2. **Usar el Strategy Pattern** para comportamientos que varían (impuestos, descuentos, loggers, validadores).
3. **Preferir composición de objetos sobre herencia de clases**: inyectar una estrategia es más flexible que heredar y sobrescribir.
4. **Aplicar OCP donde realmente hay variabilidad**: no crees 30 interfaces para 2 casos que nunca cambiarán.
5. **Delegar en el compilador**: si usas `switch` sobre un `enum`, el compilador no te avisa si añades un valor al enum y olvidas actualizar el switch. Con interfaces, si no implementas el método, no compila.

### ❌ QUÉ EVITAR

1. **`switch`/`if-else` sobre tipos concretos**: es la señal más clara de violación de OCP.
2. **Parámetros de tipo `object` o `string` mágica**: `CalculateArea(object shape)` es una trampa. Usa una interfaz.
3. **Modificar clases existentes para añadir funcionalidad nueva**: si la funcionalidad es una variante más, debería ser una clase nueva.
4. **Abstracciones que filtran por nombre de tipo**: `if (shape is Circle)` dentro de una clase que debería ser genérica respecto a formas.

### ¿Por qué?

OCP reduce el riesgo de regresiones. Cuando solo añades código (nuevas clases) sin modificar código existente, lo que ya funcionaba sigue funcionando. Es el principio que permite que los sistemas crezcan sin volverse frágiles.

---

## 6. Errores habituales

### Error 1: Abstraer demasiado pronto

```csharp
// ❌ MAL: Abstracción prematura para algo que no va a crecer
public interface INumberFormatter
{
    string Format(int number);
}

public class SpanishFormatter : INumberFormatter
{
    public string Format(int number) => number.ToString("N0"); // 1.234
}

public class EnglishFormatter : INumberFormatter
{
    public string Format(int number) => number.ToString("N0"); // 1,234
}

// Si solo tienes DOS formatos y no van a aparecer más,
// esta abstracción añade complejidad sin beneficio real.
```

**Por qué está mal**: no todo necesita una interfaz. Si el sistema solo maneja español e inglés y eso no va a cambiar, un simple `if (culture == "es")` es más legible y más fácil de mantener.

**Regla práctica**: espera a tener al menos 3 implementaciones o una previsión clara de crecimiento antes de abstraer.

### Error 2: Interfaces demasiado genéricas

```csharp
// ❌ MAL: Interfaz genérica que obliga al consumidor a hacer casting
public interface IShape
{
    object GetArea(); // ¿Devuelve decimal? ¿double? ¿int?
}
```

**Versión correcta**:

```csharp
// ✅ BIEN: Interfaz con tipo de retorno concreto
public interface IShape
{
    decimal CalculateArea();
}
```

### Error 3: Violar OCP modificando el switch sin añadir abstracción

```csharp
// ❌ MAL: Seguir añadiendo casos al switch sin abstraer
public class PaymentProcessor
{
    public void Process(string method)
    {
        switch (method)
        {
            case "credit_card": /* ... */ break;
            case "paypal":      /* ... */ break;
            case "bank_transfer": /* ... */ break; // Añadido: modifiqué esta clase
            case "crypto":       /* ... */ break;  // Añadido: modifiqué esta clase Otra vez
        }
    }
}
```

Cada nueva pasarela de pago es un `case` más. Esto escala fatal. La solución es una interfaz `IPaymentMethod` con una implementación por pasarela.

---

## 7. Ejercicio guiado

### Enunciado

Tienes un sistema de descuentos para un e-commerce. Actualmente usa un `switch` para aplicar distintos tipos de descuento:

```csharp
public class DiscountCalculator
{
    public decimal Calculate(decimal price, string discountType)
    {
        switch (discountType)
        {
            case "seasonal":
                return price * 0.80m; // 20% descuento

            case "vip":
                return price * 0.85m; // 15% descuento

            case "coupon":
                return price > 100 ? price - 20 : price; // $20 off si > $100

            default:
                return price; // Sin descuento
        }
    }
}
```

### Objetivo

Refactoriza usando el Strategy Pattern. Crea una interfaz `IDiscountStrategy`, una implementación por cada tipo de descuento y una clase `DiscountService` que orqueste y reciba la estrategia por constructor.

### Solución paso a paso

#### Paso 1: Definir la abstracción

```csharp
/// <summary>
/// Contrato para cualquier estrategia de descuento.
/// Recibe el precio original y devuelve el precio con descuento aplicado.
/// </summary>
public interface IDiscountStrategy
{
    decimal Apply(decimal originalPrice);
    string Name { get; } // Para mostrar qué descuento se aplicó
}
```

#### Paso 2: Implementar cada estrategia

```csharp
// Descuento por temporada (20% off)
public class SeasonalDiscount : IDiscountStrategy
{
    public string Name => "Temporada (20%)";

    public decimal Apply(decimal originalPrice)
    {
        return originalPrice * 0.80m;
    }
}

// Descuento para cliente VIP (15% off)
public class VipDiscount : IDiscountStrategy
{
    public string Name => "VIP (15%)";

    public decimal Apply(decimal originalPrice)
    {
        return originalPrice * 0.85m;
    }
}

// Descuento por cupón ($20 off si el precio supera $100)
public class CouponDiscount : IDiscountStrategy
{
    public string Name => "Cupón ($20 off)";

    public decimal Apply(decimal originalPrice)
    {
        return originalPrice > 100 ? originalPrice - 20 : originalPrice;
    }
}

// Sin descuento (estrategia nula — evita comprobar null)
public class NoDiscount : IDiscountStrategy
{
    public string Name => "Sin descuento";

    public decimal Apply(decimal originalPrice)
    {
        return originalPrice;
    }
}
```

#### Paso 3: Crear el servicio que consume la estrategia

```csharp
public class DiscountService
{
    private readonly IDiscountStrategy _strategy;

    // La estrategia se inyecta — el servicio no sabe cuál es.
    public DiscountService(IDiscountStrategy strategy)
    {
        _strategy = strategy;
    }

    public void ApplyToCart(ShoppingCart cart)
    {
        Console.WriteLine($"\n  Descuento: {_strategy.Name}");
        Console.WriteLine($"  Precio original: {cart.Total:C}");

        decimal finalPrice = _strategy.Apply(cart.Total);

        Console.WriteLine($"  Precio final:    {finalPrice:C}");
        Console.WriteLine($"  Ahorro:          {cart.Total - finalPrice:C}");
    }
}
```

#### Paso 4: Composición y demostración

```csharp
ShoppingCart cart = new ShoppingCart { Total = 150m };

// Cada cliente recibe una estrategia distinta
DiscountService seasonalService = new DiscountService(new SeasonalDiscount());
seasonalService.ApplyToCart(cart);  // 20% off → $120

DiscountService vipService = new DiscountService(new VipDiscount());
vipService.ApplyToCart(cart);       // 15% off → $127.50

DiscountService couponService = new DiscountService(new CouponDiscount());
couponService.ApplyToCart(cart);    // $20 off → $130
```

### ¿Qué hemos ganado con OCP?

- Añadir un nuevo descuento (ej: "Black Friday 50%") es crear una clase `BlackFridayDiscount : IDiscountStrategy`. `DiscountService` no se toca.
- Cada estrategia se puede testear por separado.
- Las estrategias se pueden combinar con el patrón Decorator: `new VipDiscount(new SeasonalDiscount())`.

---

## 8. Reto

**NO lo resuelvas todavía. Inténtalo por tu cuenta antes de mirar la solución.**

Tienes un sistema de envío de notificaciones con este código:

```csharp
public class Notifier
{
    public void Send(string message, string channel)
    {
        switch (channel)
        {
            case "email":
                Console.WriteLine($"[Email] {message}");
                break;
            case "sms":
                Console.WriteLine($"[SMS] {message}");
                break;
            case "push":
                Console.WriteLine($"[Push] {message}");
                break;
            default:
                throw new ArgumentException($"Canal no soportado: {channel}");
        }
    }
}
```

**Reto**: Aplica el Strategy Pattern siguiendo OCP:

1. Crea una interfaz `INotificationChannel` con método `Send(string message)`.
2. Crea implementaciones para Email, SMS, Push y Slack.
3. Crea un `NotificationService` que reciba `INotificationChannel` por constructor.
4. Añade el canal "Slack" sin modificar `NotificationService`.
5. **Extra**: haz que `NotificationService` pueda enviar por MÚLTIPLES canales a la vez (recibir `IEnumerable<INotificationChannel>`).

---

## 9. Resumen

| Concepto | Descripción |
|---|---|
| **OCP** | Abierto a extensión, cerrado a modificación. |
| **Abstracción clave** | Interfaz que define el contrato común para todas las variantes. |
| **Strategy Pattern** | Encapsula algoritmos intercambiables detrás de una interfaz común. |
| **Polimorfismo** | El mecanismo que permite que el código cliente use la abstracción sin conocer los detalles. |
| **Dirección de dependencia** | Las clases concretas dependen de la abstracción, no al revés. |

**Conceptos importantes**:
- OCP se consigue mediante abstracciones (interfaces) + polimorfismo, no con `switch` o `if/else`.
- El Strategy Pattern es la materialización más común de OCP para comportamientos variables.
- OCP y DIP van de la mano: para cumplir OCP necesitas depender de abstracciones (DIP).
- No abstraigas prematuramente: espera a tener evidencia de que la variabilidad existe.

---

## 10. Preguntas frecuentes

**P: ¿OCP significa que nunca debo modificar una clase existente?**

R: No. Los bugs se corrigen modificando código. OCP aplica a la adición de nueva funcionalidad, no a la corrección de errores. Tampoco aplica cuando la abstracción misma necesita cambiar porque el contrato evoluciona.

**P: ¿Cuándo usar herencia vs composición para OCP?**

R: La composición (inyectar una interfaz) es casi siempre preferible. La herencia (clase base abstracta) es útil cuando el comportamiento común es significativo y las variantes solo cambian una parte concreta (Template Method Pattern).

**P: ¿Cómo sé si estoy aplicando OCP correctamente?**

R: Pregúntate: "¿Puedo añadir una nueva variante (nuevo país, nuevo descuento, nueva forma) sin modificar ninguna clase existente aparte de crear la nueva implementación?" Si la respuesta es sí, estás cumpliendo OCP.

**P: ¿OCP no genera demasiadas clases?**

R: Sí, genera más clases. Pero el coste de tener más clases es muchísimo menor que el coste de modificar código existente y arriesgar regresiones. Además, muchas clases pequeñas y enfocadas son más fáciles de entender, testear y mantener que pocas clases grandes llenas de `switch`.

**P: ¿Qué relación tiene OCP con el resto de SOLID?**

R: OCP usa polimorfismo (herencia bien aplicada = LSP), se apoya en abstracciones (DIP), y naturalmente lleva a interfaces pequeñas (ISP). SRP asegura que cada clase tiene un motivo para cambiar; OCP asegura que cuando cambia, lo hace por extensión, no por modificación.
