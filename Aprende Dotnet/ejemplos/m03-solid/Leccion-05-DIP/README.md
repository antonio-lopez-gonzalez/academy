
# Lección 05 — Principio de Inversión de Dependencias (DIP)

---

## 1. Introducción

**¿Qué vamos a aprender?**

El **Principio de Inversión de Dependencias** (Dependency Inversion Principle, DIP) es la "D" de SOLID. Fue formulado por Robert C. Martin y establece dos reglas:

> 1. Los módulos de alto nivel **no deben depender** de módulos de bajo nivel. Ambos deben depender de **abstracciones**.
>
> 2. Las abstracciones **no deben depender** de los detalles. Los **detalles** deben depender de las abstracciones.

**¿Por qué existe?**

En una arquitectura tradicional sin DIP, el flujo de dependencias va de arriba hacia abajo:

```
┌─────────────────────┐
│  NotificationService │  ← Módulo de alto nivel (lógica de negocio)
│  (alto nivel)        │
└──────────┬──────────┘
           │ depende de
           ▼
┌─────────────────────┐
│    EmailSender       │  ← Módulo de bajo nivel (infraestructura)
│  (bajo nivel)        │
└─────────────────────┘
```

El problema: la lógica de negocio más importante del sistema está **acoplada** a un detalle de infraestructura (cómo se envían emails). Si mañana quieres usar SMS, Push o Slack, tienes que MODIFICAR la lógica de negocio. Esto viola OCP y hace que el código sea imposible de testear sin un servidor SMTP real.

**¿Qué problema resuelve?**

- **Acoplamiento a infraestructura**: la lógica de negocio no debería saber si los datos se guardan en SQL Server, PostgreSQL o un archivo de texto.
- **Imposibilidad de testear**: sin DIP, necesitas bases de datos, servidores SMTP y APIs externas reales para ejecutar un test unitario.
- **Cambios en cascada**: cambiar un detalle de bajo nivel (ej: migrar de SQL Server a PostgreSQL) obliga a modificar todas las capas superiores que dependen de él.
- **Violación de OCP**: sin abstracciones, añadir un nuevo canal de notificación requiere modificar código existente.

**¿Cuándo se utiliza?**

SIEMPRE. DIP es la base sobre la que se construye la Inyección de Dependencias, los tests unitarios, la arquitectura limpia y la capacidad de evolucionar un sistema sin romperlo.

**Errores típicos**

- Confundir DIP (el principio) con DI (el patrón) o con IoC (el mecanismo). DIP es el QUÉ, DI es el CÓMO.
- Crear interfaces que son un calco de la implementación concreta (eso no invierte la dependencia, solo añade una capa innecesaria).
- Hacer que las abstracciones dependan de detalles (ej: interfaz `IEmailSender` que expone `SmtpPort`).

---

## 2. Teoría

### El flujo tradicional de dependencias (incorrecto)

```
Flujo de control = Flujo de dependencias (COPIA EXACTA)

Lógica de negocio
       │
       │ importa / usa
       ▼
Infraestructura (BD, email, archivos)
```

El módulo de alto nivel **importa** y **conoce** el módulo de bajo nivel. La flecha de dependencia en el código fuente apunta hacia abajo.

### Con DIP: la inversión

```
Flujo de control ≠ Flujo de dependencias (SE INVIERTE)

┌──────────────────────────┐
│  NotificationService     │  ← Módulo de alto nivel
│                          │
│  - INotificationSender ──│──► Define la abstracción
└──────────────────────────┘
            ▲
            │ implementa
            │
┌───────────┴───────────┐
│    EmailSenderDIP     │  ← Módulo de bajo nivel
│    : INotificationSender│
└───────────────────────┘
```

Ahora el módulo de alto nivel **define** la abstracción (`INotificationSender`) y el módulo de bajo nivel la **implementa**. La flecha de dependencia en el código fuente apunta **hacia arriba**, hacia la abstracción. Los detalles dependen de la abstracción, no al revés.

### Propiedad de la abstracción

Una abstracción **pertenece** a la capa que la usa, no a la capa que la implementa. `INotificationSender` debe vivir en el mismo paquete/proyecto que `NotificationService` (la capa de lógica de negocio), NO en el paquete de infraestructura. Esto es crucial: la capa de negocio no debe tener dependencia de compilación hacia la capa de infraestructura.

```
Clean Architecture (Robert C. Martin):

       ┌──────────────────────────────┐
       │        Dominio / Core        │
       │  Entidades + Interfaces      │  ← Define IRepository, INotificationSender
       └──────────────┬───────────────┘
                      │
       ┌──────────────┴───────────────┐
       │       Aplicación / UseCases  │
       │  NotificationService          │  ← Consume las interfaces
       │  UserService                 │
       └──────────────┬───────────────┘
                      │
       ┌──────────────┴───────────────┐
       │       Infraestructura        │
       │  EmailSender : INotificationSender │  ← Implementa las interfaces
       │  SqlServerDb : IDatabase     │
       └──────────────────────────────┘
```

La regla de dependencia: todas las flechas de dependencia en código fuente apuntan **hacia adentro** (hacia el dominio).

### DIP → IoC → DI: la jerarquía de conceptos

| Concepto | Significado | Analogía |
|---|---|---|
| **DIP** (Principio) | "Depende de abstracciones, no de concreciones" | La norma de tráfico |
| **IoC** (Mecanismo) | Inversión de Control: "no me llames, yo te llamo" | El semáforo |
| **DI** (Patrón) | Inyección de Dependencias: constructor injection | El motor del coche que respeta la norma |
| **Contenedor DI** | Automatiza la creación e inyección de objetos | La fábrica de coches |

**Hollywood Principle**: "No me llames, yo te llamo". En el código tradicional, tu clase llama a `new EmailSender()`. Con IoC/DI, el framework crea `EmailSender` y te lo pasa por constructor. El flujo de control está **invertido**.

### Poor Man's DI

Es la DI manual, sin contenedor. Es el "punto de composición" (composition root) donde se ensambla todo el grafo de dependencias a mano con `new`:

```csharp
// Composition root: la entrada de la aplicación
INotificationSender sender = new EmailSenderDIP(); // Decisión: Email
IDatabase database = new PostgreSqlDatabaseDIP();     // Decisión: PostgreSQL

NotificationService notifService = new(sender);
UserService userService = new(database);
```

El contenedor DI de .NET (`IServiceCollection`) hace exactamente esto, pero automatizado.

---

## 3. Ejemplo mínimo

```csharp
// ─── VIOLACIÓN DIP: dependencia directa de clase concreta ───
public class ConsoleLogger
{
    public void Log(string message) => Console.WriteLine($"[LOG] {message}");
}

public class BadService
{
    // ❌ Dependencia HARDCODEADA. BadService "sabe" que existe ConsoleLogger.
    private readonly ConsoleLogger _logger = new ConsoleLogger();

    public void DoWork()
    {
        _logger.Log("Trabajo realizado.");
        // No puedo cambiar el logger sin modificar BadService.
        // No puedo testear sin escribir en consola.
    }
}

// ─── CORRECTO DIP: dependencia de abstracción ───
public interface ILogger
{
    void Log(string message);
}

public class ConsoleLoggerDIP : ILogger
{
    public void Log(string message) => Console.WriteLine($"[LOG] {message}");
}

public class FileLoggerDIP : ILogger
{
    public void Log(string message) => File.AppendAllText("log.txt", message + "\n");
}

public class GoodService
{
    // ✅ Dependencia de ILogger (abstracción), no de ConsoleLogger (concreción).
    private readonly ILogger _logger;

    public GoodService(ILogger logger) // Inyección por constructor
    {
        _logger = logger;
    }

    public void DoWork()
    {
        _logger.Log("Trabajo realizado.");
    }
}

// Composición manual (Poor Man's DI):
ILogger fileLogger = new FileLoggerDIP();
GoodService service = new GoodService(fileLogger);
service.DoWork(); // Escribe en archivo sin que GoodService lo sepa
```

Para ejecutar el ejemplo completo:

```bash
dotnet run --project ejemplos/m03-solid/Leccion-05-DIP/src/SolidDIP
```

---

## 4. Ejemplo real

El código en `src/SolidDIP/Program.cs` demuestra DIP con 6 secciones:

### Sección 1 (línea 76): Violación DIP en notificaciones

- `NotificationServiceViolation` crea `new EmailSender()` internamente.
- `UserServiceViolation` crea `new SqlServerDatabase()` internamente.
- Ambos están **casados de por vida** con sus dependencias concretas.

### Sección 2 (línea 126): Violación DIP en acceso a datos

- `UserServiceViolation` depende de `SqlServerDatabase` concreto.
- Migrar a PostgreSQL implicaría modificar la clase de negocio.

### Sección 3 (línea 171): Solución DIP

- Abstracciones: `INotificationSender` y `IDatabase`.
- Implementaciones: `EmailSenderDIP`, `SmsSenderDIP`, `SqlServerDatabaseDIP`, `PostgreSqlDatabaseDIP`.
- `NotificationService` y `UserService` reciben sus dependencias por constructor.
- La lógica de negocio no sabe qué implementación concreta está usando.

### Sección 4 (línea 319): Mocks para testing

- `MockNotificationSender` implementa `INotificationSender` sin enviar nada real.
- `MockDatabase` implementa `IDatabase` con almacenamiento en memoria.
- Esto permite testear la lógica de negocio AISLADA de infraestructura.

### Sección 5 (línea 366): Demostración completa

- **Demo 1**: muestra la violación y sus problemas.
- **Demo 2**: `NotificationService` con `EmailSenderDIP` y luego con `SmsSenderDIP` — mismo servicio, distinto canal. El servicio NO se modificó.
- **Demo 3**: `UserService` con `SqlServerDatabaseDIP` y luego con `PostgreSqlDatabaseDIP` — mismo servicio, distinto motor.
- **Demo 4**: Poor Man's DI — composición manual en el entry point.
- **Demo 5**: Testing con mocks — se verifica que `NotificationService` llama a `INotificationSender.Send()` con los parámetros correctos.
- **Demo 6**: IoC, Hollywood Principle y resumen gráfico.

---

## 5. Buenas prácticas

### ✅ QUÉ HACER

1. **Las abstracciones pertenecen a quien las USA**: `INotificationSender` vive en la capa de negocio, no en infraestructura.
2. **Inyecta dependencias por constructor**: es la forma más explícita y testable de DI.
3. **El punto de composición (composition root) en la entrada de la app**: en `Program.cs` o `Startup.cs`. Solo ahí se usa `new` con clases concretas.
4. **Crea mocks que implementen las mismas interfaces que tus servicios reales**: gracias a DIP, tus tests solo conocen interfaces.
5. **Usa `IHttpClientFactory` en lugar de `new HttpClient()`**: es DIP aplicado a HTTP.

### ❌ QUÉ EVITAR

1. **`new ClaseConcreta()` dentro de una clase de negocio**: es la violación más directa de DIP.
2. **Interfaces que calcan la implementación**: `ISqlServerDatabase` con método `OpenSqlConnection()`. La abstracción debe ser genérica (`IDatabase`), no específica de una tecnología.
3. **Interfaces con detalles de infraestructura**: `IEmailSender` no debería exponer `SmtpHost` o `Port`. Eso pertenece a la implementación, no al contrato.
4. **Service Locator como sustituto de DI**: `var service = ServiceLocator.Resolve<IService>()` es un anti-patrón que oculta las dependencias reales.
5. **Propiedades o setters para inyección**: `public ILogger Logger { get; set; }` — las dependencias obligatorias van en el constructor.

### ¿Por qué?

DIP es la pieza que faltaba para cerrar el círculo de SOLID:
- **Sin DIP no hay OCP**: no puedes extender sin modificar si dependes de implementaciones concretas.
- **Sin DIP no hay tests aislados**: necesitarías infraestructura real para probar lógica de negocio.
- **Sin DIP no hay Clean Architecture**: las dependencias en código fuente deben apuntar hacia el dominio, y eso solo se consigue con DIP.

---

## 6. Errores habituales

### Error 1: Crear interfaces que no invierten la dependencia

```csharp
// ❌ MAL: Interfaz que calca la implementación concreta.
// Esto NO es DIP, es solo un envoltorio innecesario.
public interface ISqlConnection
{
    void OpenSqlConnection(string connectionString);
    SqlDataReader ExecuteSqlQuery(string sql);
}

// La interfaz tiene "Sql" en el nombre y en los métodos.
// Si migras a PostgreSQL, tienes que cambiar la interfaz.
// Eso significa que la abstracción depende del detalle. DIP violado.
```

**Versión correcta**:

```csharp
// ✅ BIEN: Abstracción genérica, independiente de la tecnología
public interface IDatabase
{
    void Save(string data);
    string Get(int id);
}
```

### Error 2: La interfaz vive en el proyecto equivocado

```csharp
// ❌ MAL: INotificationSender definido en el proyecto de infraestructura.
// La capa de negocio tiene que REFERENCIAR infraestructura para usar la interfaz.
// Eso invierte la dirección de dependencia al revés de lo que debe ser.

// Proyecto: Cms.Infrastructure
public interface INotificationSender { void Send(string to, string message); }
public class EmailSender : INotificationSender { /* ... */ }

// Proyecto: Cms.Application (depende de Cms.Infrastructure → ❌ DIRECCIÓN INVERTIDA)
public class NotificationService
{
    public NotificationService(Cms.Infrastructure.INotificationSender sender) { }
}
```

**Versión correcta**:

```csharp
// ✅ BIEN: La interfaz vive donde se CONSUME (Application/Domain)

// Proyecto: Cms.Application
public interface INotificationSender { void Send(string to, string message); }
public class NotificationService
{
    public NotificationService(INotificationSender sender) { }
    // Application NO referencia Infrastructure
}

// Proyecto: Cms.Infrastructure (depende de Cms.Application → ✅ DIRECCIÓN CORRECTA)
public class EmailSender : INotificationSender { /* ... */ }
```

### Error 3: Crear interfaces para absolutamente todo

```csharp
// ❌ MAL: Interfaz para una clase que nunca tendrá otra implementación
public interface IStringHelper
{
    string Capitalize(string input);
}

public class StringHelper : IStringHelper
{
    public string Capitalize(string input) =>
        input.Length > 0 ? char.ToUpper(input[0]) + input[1..] : input;
}
```

**Por qué está mal**: DIP no obliga a crear una interfaz para cada clase. Si `StringHelper` es una utilidad pura sin dependencias externas, no necesita interfaz. Solo abstrae cuando hay una dependencia externa que quieres poder sustituir (BD, API, sistema de archivos, reloj).

---

## 7. Ejercicio guiado

### Enunciado

Tienes un `PaymentService` que procesa pagos usando directamente las APIs de Stripe y PayPal. El código actual viola DIP porque la lógica de negocio conoce los detalles de cada pasarela:

```csharp
public class PaymentService
{
    // Hardcodeado: el servicio conoce las clases concretas de Stripe y PayPal
    private readonly StripeGateway _stripe = new StripeGateway("sk_live_12345");
    private readonly PayPalGateway _paypal = new PayPalGateway("client_id_67890");

    public void ProcessPayment(Payment payment)
    {
        switch (payment.Method)
        {
            case PaymentMethod.CreditCard:
                _stripe.Charge(payment.Amount, payment.Currency);
                break;

            case PaymentMethod.PayPal:
                _paypal.SendPayment(payment.Amount, payment.Currency);
                break;

            default:
                throw new ArgumentException($"Método de pago no soportado.");
        }

        // Además, los métodos de cada gateway tienen firmas DIFERENTES:
        // Stripe: Charge(decimal amount, string currency)
        // PayPal: SendPayment(decimal amount, string currencyCode)
        // Esto obliga al servicio a conocer los detalles de cada API.
    }
}
```

### Objetivo

1. Crea una abstracción `IPaymentGateway` con una interfaz unificada.
2. Implementa `StripePaymentGateway` y `PayPalPaymentGateway`.
3. Refactoriza `PaymentService` para que dependa de `IEnumerable<IPaymentGateway>` y seleccione el adecuado dinámicamente.
4. Aplica Poor Man's DI para componer el sistema.
5. Demuestra cómo añadir una tercera pasarela (MercadoPago) sin modificar `PaymentService`.

### Solución paso a paso

#### Paso 1: Definir la abstracción

```csharp
/// <summary>
/// Abstracción: cualquier pasarela de pago debe poder procesar un pago
/// e indicar qué método de pago soporta.
/// </summary>
public interface IPaymentGateway
{
    /// <summary>El método de pago que esta pasarela maneja.</summary>
    PaymentMethod SupportedMethod { get; }

    /// <summary>Procesa un pago y devuelve un resultado.</summary>
    PaymentResult Process(Payment payment);
}

// Modelos de dominio
public enum PaymentMethod { CreditCard, PayPal, MercadoPago }

public record Payment(decimal Amount, string Currency, PaymentMethod Method);

public record PaymentResult(bool Success, string TransactionId, string Message);
```

#### Paso 2: Implementar cada pasarela

```csharp
// Implementación para Stripe (tarjeta de crédito)
public class StripePaymentGateway : IPaymentGateway
{
    public PaymentMethod SupportedMethod => PaymentMethod.CreditCard;

    private readonly string _apiKey;

    public StripePaymentGateway(string apiKey)
    {
        _apiKey = apiKey;
    }

    public PaymentResult Process(Payment payment)
    {
        Console.WriteLine($"[Stripe] Cargando {payment.Amount} {payment.Currency}...");
        Console.WriteLine($"[Stripe] Usando API key: {_apiKey[..8]}...");

        // En producción: StripeClient.Charge(...)
        // Simulamos respuesta exitosa
        return new PaymentResult(
            Success: true,
            TransactionId: $"stripe_tx_{Guid.NewGuid():N}"[..12],
            Message: "Pago con tarjeta procesado correctamente.");
    }
}

// Implementación para PayPal
public class PayPalPaymentGateway : IPaymentGateway
{
    public PaymentMethod SupportedMethod => PaymentMethod.PayPal;

    private readonly string _clientId;

    public PayPalPaymentGateway(string clientId)
    {
        _clientId = clientId;
    }

    public PaymentResult Process(Payment payment)
    {
        Console.WriteLine($"[PayPal] Enviando {payment.Amount} {payment.Currency}...");
        Console.WriteLine($"[PayPal] Usando Client ID: {_clientId[..8]}...");

        return new PaymentResult(
            Success: true,
            TransactionId: $"paypal_tx_{Guid.NewGuid():N}"[..12],
            Message: "Pago con PayPal procesado correctamente.");
    }
}
```

#### Paso 3: Refactorizar PaymentService

```csharp
/// <summary>
/// ✅ DIP: PaymentService solo conoce IPaymentGateway.
/// No sabe si el pago es con Stripe, PayPal o cualquier otra pasarela futura.
/// </summary>
public class PaymentService
{
    private readonly Dictionary<PaymentMethod, IPaymentGateway> _gateways;

    /// <summary>
    /// Recibe TODAS las pasarelas disponibles y las indexa por método de pago.
    /// El servicio no sabe cuántas ni cuáles son.
    /// </summary>
    public PaymentService(IEnumerable<IPaymentGateway> gateways)
    {
        // Indexamos por método soportado para búsqueda O(1)
        _gateways = gateways.ToDictionary(g => g.SupportedMethod);
    }

    public PaymentResult ProcessPayment(Payment payment)
    {
        if (!_gateways.TryGetValue(payment.Method, out IPaymentGateway? gateway))
        {
            return new PaymentResult(
                Success: false,
                TransactionId: string.Empty,
                Message: $"No hay pasarela configurada para {payment.Method}.");
        }

        return gateway.Process(payment);
    }
}
```

#### Paso 4: Composición manual (Poor Man's DI)

```csharp
// Composition root: punto de entrada de la aplicación
// Aquí se decide QUÉ implementaciones concretas usar.

List<IPaymentGateway> gateways = new()
{
    new StripePaymentGateway("sk_live_abc123xyz"),
    new PayPalPaymentGateway("client_id_def456uvw"),
    // Añadir MercadoPago — solo esta línea cambia, PaymentService no se toca:
    new MercadoPagoPaymentGateway("access_token_ghi789"),
};

PaymentService paymentService = new PaymentService(gateways);

// Procesar pagos con distintos métodos
Payment cardPayment = new(Amount: 99.99m, Currency: "EUR", Method: PaymentMethod.CreditCard);
PaymentResult result1 = paymentService.ProcessPayment(cardPayment);
Console.WriteLine($"Resultado: {result1.Message}\n");

Payment paypalPayment = new(Amount: 49.99m, Currency: "USD", Method: PaymentMethod.PayPal);
PaymentResult result2 = paymentService.ProcessPayment(paypalPayment);
Console.WriteLine($"Resultado: {result2.Message}\n");

Payment mercadoPagoPayment = new(Amount: 29.99m, Currency: "ARS", Method: PaymentMethod.MercadoPago);
PaymentResult result3 = paymentService.ProcessPayment(mercadoPagoPayment);
Console.WriteLine($"Resultado: {result3.Message}\n");
```

#### Paso 5: Añadir MercadoPago sin tocar PaymentService

```csharp
// Nueva pasarela: solo se añade esta clase. PaymentService NO se modifica.
public class MercadoPagoPaymentGateway : IPaymentGateway
{
    public PaymentMethod SupportedMethod => PaymentMethod.MercadoPago;

    private readonly string _accessToken;

    public MercadoPagoPaymentGateway(string accessToken)
    {
        _accessToken = accessToken;
    }

    public PaymentResult Process(Payment payment)
    {
        Console.WriteLine($"[MercadoPago] Procesando {payment.Amount} {payment.Currency}...");
        return new PaymentResult(
            Success: true,
            TransactionId: $"mp_tx_{Guid.NewGuid():N}"[..12],
            Message: "Pago con MercadoPago procesado correctamente.");
    }
}
```

### ¿Qué hemos ganado con DIP?

- `PaymentService` no conoce Stripe, PayPal ni MercadoPago. Solo conoce `IPaymentGateway`.
- Añadir una nueva pasarela es crear una clase e inyectarla. `PaymentService` no se toca (OCP cumplido gracias a DIP).
- Podemos testear `PaymentService` con un `MockPaymentGateway` que no haga llamadas reales a APIs externas.
- Si la API de Stripe cambia, solo se modifica `StripePaymentGateway`. La lógica de negocio permanece intacta.
- Cada pasarela se puede desarrollar y testear de forma independiente.

---

## 8. Reto

**NO lo resuelvas todavía. Inténtalo por tu cuenta antes de mirar la solución.**

Tienes un `ReportService` que genera informes y los exporta. Actualmente está totalmente acoplado a implementaciones concretas:

```csharp
public class ReportService
{
    private readonly SqlConnection _connection;

    public ReportService()
    {
        _connection = new SqlConnection("Server=localhost;Database=CmsDb;...");
    }

    public void GenerateMonthlyReport(int year, int month)
    {
        // 1. Obtener datos de SQL Server
        _connection.Open();
        SqlCommand cmd = new SqlCommand($"SELECT * FROM Posts WHERE Year={year} AND Month={month}", _connection);
        SqlDataReader reader = cmd.ExecuteReader();
        // ... procesar datos ...
        _connection.Close();

        // 2. Exportar a PDF con librería concreta
        PdfWriter pdfWriter = new PdfWriter("report.pdf");
        // ... escribir PDF ...

        // 3. Exportar a Excel con librería concreta
        ExcelPackage excel = new ExcelPackage("report.xlsx");
        // ... escribir Excel ...

        // 4. Enviar por email con SMTP concreto
        SmtpClient smtp = new SmtpClient("smtp.empresa.com", 587);
        MailMessage mail = new MailMessage("reports@cms.com", "admin@cms.com");
        smtp.Send(mail);

        // 5. Guardar registro de auditoría en archivo
        File.AppendAllText("audit.log", $"Report {year}-{month} generated.\n");
    }
}
```

**Reto**: Refactoriza `ReportService` aplicando DIP:

1. Crea abstracciones para cada dependencia externa: `IDataProvider`, `IPdfExporter`, `IExcelExporter`, `IEmailSender`, `IAuditLogger`.
2. Crea implementaciones concretas para cada una (SQL Server, librería PDF, etc.).
3. `ReportService` debe recibir TODAS sus dependencias por constructor.
4. Demuestra cómo cambiar de SQL Server a PostgreSQL, del PDF actual a otra librería, y de SMTP a SendGrid sin tocar `ReportService`.
5. Crea mocks para todas las dependencias y escribe un "test" (simulado) que verifique que `ReportService` orquesta correctamente el flujo.

---

## 9. Resumen

| Concepto | Descripción |
|---|---|
| **DIP** | Módulos de alto nivel no dependen de bajo nivel. Ambos dependen de abstracciones. |
| **Abstracción** | Interfaz o clase abstracta que define QUÉ hace, no CÓMO. Pertenece al que la consume. |
| **Detalle** | Implementación concreta (SQL Server, SMTP, Stripe). Depende de la abstracción. |
| **Poor Man's DI** | Inyección de dependencias manual sin contenedor. Composición en el entry point. |
| **Composition Root** | Único punto de la aplicación donde se "conectan" las abstracciones con sus implementaciones. |
| **IoC** | Inversión de Control: el framework controla el flujo, no tu código. |
| **Hollywood Principle** | "No me llames, yo te llamo". Tus clases no crean dependencias, las reciben. |

**Conceptos importantes**:
- DIP es el principio que cierra el círculo de SOLID. Sin DIP, OCP es imposible y los tests aislados no existen.
- Las abstracciones deben definirse desde la perspectiva del consumidor, no del implementador.
- La Inyección de Dependencias (DI) es el patrón que implementa DIP. El contenedor DI de .NET automatiza la composición.
- DIP es la base teórica de Clean Architecture, Hexagonal Architecture y Onion Architecture: todas se basan en que las dependencias apuntan hacia el dominio.
- Regla de oro: si una clase tiene una dependencia que quieres poder sustituir (BD, email, HTTP, reloj, sistema de archivos), abstrae esa dependencia.

---

## 10. Preguntas frecuentes

**P: ¿DIP significa que debo crear una interfaz para CADA clase?**

R: No. Solo abstrae lo que es una dependencia externa que quieres poder sustituir o mockear. Una clase de utilidad pura, un DTO o un value object no necesitan interfaz. La regla práctica: si la clase habla con el mundo exterior (BD, red, archivos, reloj), abstrae. Si es lógica pura, no hace falta.

**P: ¿Qué diferencia hay entre DIP y Dependency Injection (DI)?**

R: DIP es el principio (el QUÉ): "depende de abstracciones". DI es el patrón (el CÓMO): "inyecta las dependencias por constructor en lugar de crearlas con new". Puedes cumplir DIP sin un contenedor DI (Poor Man's DI). No puedes hacer DI sin cumplir DIP.

**P: ¿Es lo mismo Inversión de Control (IoC) que Inyección de Dependencias?**

R: No. IoC es el concepto general de invertir el flujo de control. DI es una forma específica de IoC. Otras formas de IoC son: eventos, callbacks, el patrón Template Method, y los frameworks en general. "Don't call us, we'll call you."

**P: ¿Dónde defino las interfaces: en el proyecto de dominio o en el de infraestructura?**

R: En el proyecto que las CONSUME. Si `NotificationService` (aplicación) necesita `INotificationSender`, la interfaz debe vivir en el proyecto de aplicación. La infraestructura referencia aplicación (para implementar la interfaz), no al revés. Esta es la base de Clean Architecture.

**P: ¿Cómo se relaciona DIP con el contenedor DI de .NET?**

R: El contenedor DI (`IServiceCollection` / `ServiceProvider`) es la herramienta que automatiza DIP en .NET. En el `Program.cs` registras mapeos como `builder.Services.AddScoped<INotificationSender, EmailSenderDIP>()`, y el contenedor resuelve e inyecta las dependencias por ti. Todo lo que hemos hecho manualmente con Poor Man's DI, el contenedor lo hace automático.

**P: ¿Qué son los Keyed Services de .NET 8 y cómo se relacionan con DIP?**

R: Los Keyed Services permiten registrar múltiples implementaciones de la misma interfaz bajo una clave (`[FromKeyedServices("email")]`). Es la evolución natural de DIP + Strategy Pattern: puedes inyectar la implementación correcta según el contexto sin que el código cliente tenga que elegir manualmente.
