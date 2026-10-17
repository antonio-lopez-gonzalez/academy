
# Lección 01 — Principio de Responsabilidad Única (SRP)

---

## 1. Introducción

**¿Qué vamos a aprender?**

El **Principio de Responsabilidad Única** (Single Responsibility Principle, SRP) es la "S" de SOLID. Fue formulado por Robert C. Martin (Uncle Bob) y dice:

> "Una clase debe tener una, y solo una, razón para cambiar."

Dicho de otra forma: una clase debe hacer **una sola cosa**, y debe haber **un solo motivo** que te obligue a modificar su código fuente.

**¿Por qué existe?**

Imagina que tienes una navaja suiza. Sirve para cortar, abrir latas, destornillar, limar uñas... Es muy versátil, pero si se rompe la hoja, pierdes TODAS las funcionalidades mientras la reparas. Lo mismo pasa con una clase que hace demasiadas cosas: cualquier cambio en una de sus responsabilidades te obliga a tocar (y potencialmente romper) todo el código que convive en esa clase.

**¿Qué problema resuelve?**

- **Acoplamiento accidental**: cuando juntas lógica de negocio, persistencia, notificaciones e infraestructura en una misma clase, cualquier cambio en una de esas áreas fuerza modificar un archivo que también contiene código de las otras.
- **Fragilidad**: modificar una clase multi-responsabilidad para arreglar un bug de validación puede romper el envío de emails sin que te des cuenta.
- **Imposibilidad de testear de forma aislada**: si la clase que valida también conecta con la base de datos, no puedes probar la validación sin una base de datos real.
- **Dificultad para reutilizar**: no puedes reutilizar solo la parte de validación en otro contexto porque está inseparablemente unida al resto.

**¿Cuándo se utiliza?**

SIEMPRE. SRP es el principio más fundamental de SOLID. No es una opción, es la base sobre la que se construyen los otros cuatro principios. Cada clase que escribas debe tener **una sola razón de cambio**.

**Errores típicos**

- Pensar que SRP significa "una clase, un método". No: una clase puede tener varios métodos siempre que todos sirvan al mismo propósito.
- Crear clases con nombres vagos como `Manager`, `Helper`, `Processor` o `Utils`. Si no sabes nombrarla, probablemente estás violando SRP.
- Confundir SRP con "clases pequeñas". El tamaño es un síntoma, no la causa.

---

## 2. Teoría

### El concepto de "razón de cambio"

Una "razón de cambio" es un **actor** o un **interesado** (stakeholder) que puede pedirte que modifiques el comportamiento de la clase. En una organización típica:

| Actor | Podría pedir cambios en... |
|---|---|
| Negocio / Producto | Reglas de validación (longitud mínima, formato) |
| DBA / Arquitecto de datos | Cómo se persisten los datos (SQL Server → PostgreSQL) |
| Operaciones / DevOps | Proveedor de email (SMTP → SendGrid → Mailgun) |
| Seguridad / Auditoría | Formato y destino de logs (archivo → ElasticSearch) |

Cada uno de estos actores es una "razón de cambio" distinta. Si tu clase atiende a más de uno de estos actores, violas SRP.

### Comparación con otros lenguajes

| Concepto | Java | JavaScript/TypeScript | Python |
|---|---|---|---|
| SRP | Mismo principio (SOLID es agnóstico del lenguaje) | Clases o funciones con una responsabilidad | Mismo principio |
| Mecanismo de separación | Clases e interfaces | Clases, funciones puras, módulos | Clases, funciones, módulos |
| Inyección de dependencias | Spring, Guice, manual | Inversify, tsyringe, manual | Dependencias manuales, decorators |

### Síntomas de violación de SRP (heurísticas)

1. **Regla de la "y"**: si al describir tu clase dices "esta clase valida **Y** persiste **Y** envía emails **Y** loguea", cada "y" es una responsabilidad distinta.
2. **Métodos que cambian por motivos distintos**: si cuando cambian las reglas de validación tocas el mismo archivo que cuando migras de base de datos, hay un problema.
3. **Clase con más de ~200 líneas**: no es una regla estricta, pero una clase con una sola responsabilidad rara vez necesita tanto código.
4. **Constructor con muchas dependencias** (+7 parámetros): sugiere que la clase orquesta demasiadas cosas, posiblemente porque no ha delegado lo suficiente.
5. **Dificultad para nombrar la clase**: si solo se te ocurren nombres como `Manager`, `Helper` o `Processor`, la clase probablemente tiene responsabilidades difusas.

### Diagrama ASCII: antes y después de SRP

```
VIOLACIÓN (BadPostService):
┌──────────────────────────────────────────┐
│         BadPostService                   │
│                                          │
│  · Validar título, contenido, email      │  ← Reglas de negocio
│  · Insertar en base de datos             │  ← Persistencia
│  · Conectar con SMTP y enviar email      │  ← Infraestructura
│  · Abrir archivo y escribir log          │  ← Infraestructura
│                                          │
│  4 razones para cambiar ⇒ SRP violado    │
└──────────────────────────────────────────┘

REFACTOR (con SRP):
┌──────────────┐  ┌────────────────┐  ┌──────────────┐  ┌──────────┐
│ PostValidator│  │ PostRepository │  │ EmailNotifier│  │  Logger  │
│              │  │                │  │              │  │          │
│  Validar()   │  │   Save()       │  │  Notify()    │  │  Log()   │
└──────┬───────┘  └───────┬────────┘  └──────┬───────┘  └────┬─────┘
       │                  │                  │               │
       └──────────────────┼──────────────────┼───────────────┘
                          │                  │
                  ┌───────┴──────────────────┴───────┐
                  │         PostService              │
                  │                                 │
                  │  Publish(post):                 │
                  │    1. _validator.Validate(post) │
                  │    2. _repository.Save(post)    │
                  │    3. _notifier.Notify(post)    │
                  │    4. _logger.Log(...)          │
                  │                                 │
                  │  1 sola razón para cambiar:     │
                  │  el FLUJO de publicación.       │
                  └─────────────────────────────────┘
```

---

## 3. Ejemplo mínimo

El ejemplo más simple posible que demuestra SRP: separar la validación de la persistencia.

```csharp
// ─── VIOLACIÓN: una clase hace validación Y persistencia ───
public class BadUserService
{
    public void CreateUser(string name, string email)
    {
        // Responsabilidad 1: validar
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre es obligatorio.");
        if (!email.Contains('@'))
            throw new ArgumentException("Email inválido.");

        // Responsabilidad 2: guardar
        Console.WriteLine($"[BD] Guardando usuario: {name} ({email})");
    }
}

// ─── CORRECTO (SRP): separamos en dos clases ───
public class UserValidator
{
    public void Validate(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre es obligatorio.");
        if (!email.Contains('@'))
            throw new ArgumentException("Email inválido.");
    }
}

public class UserRepository
{
    public void Save(string name, string email)
    {
        Console.WriteLine($"[BD] Guardando usuario: {name} ({email})");
    }
}

public class UserService
{
    private readonly UserValidator _validator;
    private readonly UserRepository _repository;

    public UserService(UserValidator validator, UserRepository repository)
    {
        _validator = validator;
        _repository = repository;
    }

    public void CreateUser(string name, string email)
    {
        _validator.Validate(name, email);
        _repository.Save(name, email);
    }
}
```

Para ejecutar el ejemplo completo con la demo de posts:

```bash
dotnet run --project ejemplos/m03-solid/Leccion-01-SRP/src/SolidSRP
```

---

## 4. Ejemplo real

El código completo en `src/SolidSRP/Program.cs` contiene una demo de un sistema de publicación de posts en un CMS. Vamos paso a paso:

### La violación: `BadPostService` (línea 225 del Program.cs)

Esta clase contiene **4 responsabilidades** distintas en un solo método `Publish()`:

1. **Validación**: comprueba longitud del título, formato de email.
2. **Persistencia**: añade el post a una lista en memoria (simulando BD).
3. **Notificación**: "envía" un email al autor.
4. **Logging**: escribe una entrada en un archivo de log.

```csharp
// Fragmento del código real (simplificado)
public class BadPostService
{
    public void Publish(Post post)
    {
        // Validación (reglas de negocio)
        if (post.Title.Length < 5)
            throw new InvalidOperationException("El título debe tener al menos 5 caracteres.");

        // Persistencia (infraestructura)
        Database.Add(post);

        // Notificación por email (infraestructura)
        Console.WriteLine($"Enviando email a {post.AuthorEmail}...");

        // Logging (infraestructura)
        File.AppendAllText(_logFilePath, $"Post publicado: {post.Title}");
    }
}
```

### El refactor: 5 clases con una responsabilidad cada una

1. **`PostValidator`** (línea 316) — solo sabe validar posts. Recibe el mínimo de longitud de título por constructor para que las reglas puedan cambiar sin modificar la clase.

2. **`PostRepository`** (línea 389) — solo sabe guardar y recuperar posts. Si cambia el motor de BD, solo se toca esta clase.

3. **`EmailNotifier`** (línea 413) — solo sabe enviar notificaciones. Si cambia el proveedor SMTP, solo se toca esta clase.

4. **`Logger`** (línea 460) — solo sabe escribir logs. Si cambia el destino, solo se toca esta clase.

5. **`PostService`** (línea 511) — **orquestador**. No valida, no guarda, no notifica, no loguea. Solo coordina a las otras cuatro clases en el orden correcto. Su única razón de cambio es que cambie el FLUJO de publicación.

### La demo muestra el impacto de un cambio real

En el `Main()` (a partir de línea 19), el programa:
1. Ejecuta la versión incorrecta (con violación SRP).
2. Ejecuta la versión correcta (con SRP).
3. Simula un cambio de requisito: el título ahora debe tener mínimo 10 caracteres en lugar de 5.
4. Con la versión SRP, solo se modifica PostValidator. Las otras 4 clases permanecen intactas.

---

## 5. Buenas prácticas

### ✅ QUÉ HACER

1. **Nombrar las clases según su única responsabilidad**: `PostValidator`, `EmailNotifier`, `PostRepository`. Si el nombre contiene "y" o "o", mal.
2. **Inyectar dependencias por constructor**: cada clase recibe lo que necesita, no lo crea. Esto es Dependency Injection manual (Poor Man's DI).
3. **Usar el patrón Orquestador**: una clase coordina el flujo, otras implementan cada paso. El orquestador delega, no ejecuta.
4. **Separar por capas lógicas**: validación (reglas de negocio), persistencia (acceso a datos), notificación (infraestructura), logging (infraestructura transversal).
5. **Aplicar la regla del "solo un motivo para cambiar" en cada code review**.

### ❌ QUÉ EVITAR

1. **Clases "Manager" o "Helper"**: si una clase tiene ese nombre, probablemente estás metiendo cosas que no van juntas.
2. **Métodos que hacen más de una cosa**: un método `Publish()` no debería validar, guardar, notificar y loguear. Debe orquestar llamadas a otras clases.
3. **`new` dentro de clases de lógica de negocio**: crear dependencias dentro de la clase (`new EmailSender()`) impide sustituirlas.
4. **Copiar y pegar lógica de validación entre servicios**: si dos servicios validan emails igual, extrae un `EmailValidator` compartido.

### ¿Por qué?

Porque cada clase que cumple SRP es:
- **Testeable de forma aislada**: puedes probar `PostValidator` sin base de datos, sin email, sin archivos.
- **Reemplazable**: puedes cambiar `EmailNotifier` por `SmsNotifier` sin tocar nada más.
- **Comprensible**: leer una clase de 40 líneas que hace una cosa es inmediato. Leer una de 400 líneas que hace 4 cosas requiere concentración.
- **Paralelizable**: dos desarrolladores pueden trabajar en `PostValidator` y `PostRepository` simultáneamente sin conflictos.

---

## 6. Errores habituales

### Error 1: Sobresegregar (SRP extremo)

```csharp
// ❌ MAL: Demasiadas clases para una tarea trivial
public class TitleValidator
{
    public void ValidateTitle(string title) { /* ... */ }
}

public class ContentValidator
{
    public void ValidateContent(string content) { /* ... */ }
}

public class EmailValidator
{
    public void ValidateEmail(string email) { /* ... */ }
}
```

**Por qué está mal**: SRP no significa "una clase por validación". Las tres validaciones pertenecen al mismo actor (negocio) y cambian por el mismo motivo. Deben estar juntas en `PostValidator`.

**Versión correcta**:

```csharp
// ✅ BIEN: Una clase agrupa todas las validaciones de posts
public class PostValidator
{
    public void Validate(Post post)
    {
        ValidateTitle(post.Title);
        ValidateContent(post.Content);
        ValidateEmail(post.AuthorEmail);
    }

    private void ValidateTitle(string title) { /* ... */ }
    private void ValidateContent(string content) { /* ... */ }
    private void ValidateEmail(string email) { /* ... */ }
}
```

### Error 2: "Ya lo separaré después"

```csharp
// ❌ MAL: Dejar para luego lo que deberías hacer ahora
public class PostService
{
    public void Publish(Post post)
    {
        // TODO: Mover esto a un validador
        if (string.IsNullOrWhiteSpace(post.Title))
            throw new Exception("Título inválido");

        // TODO: Mover esto a un repositorio
        File.WriteAllText($"posts/{post.Title}.json", post.Content);

        // TODO: Mover esto a un notificador
        SendEmail(post.AuthorEmail, "Post publicado");
    }
}
```

**Por qué está mal**: el "después" nunca llega. La deuda técnica se acumula y refactorizar una clase de 2000 líneas es mucho más difícil que diseñar bien desde el principio.

### Error 3: Poner la lógica de negocio en el orquestador

```csharp
// ❌ MAL: El orquestador contiene lógica de validación
public class PostService
{
    public void Publish(Post post)
    {
        // Esto NO debería estar aquí. Esto es responsabilidad del validador.
        if (post.Title.Length < 5)
            throw new Exception("Título demasiado corto.");

        _repository.Save(post);
    }
}
```

**Versión correcta**:

```csharp
// ✅ BIEN: El orquestador solo orquesta
public class PostService
{
    public void Publish(Post post)
    {
        _validator.Validate(post);   // Delega completamente
        _repository.Save(post);      // Delega completamente
    }
}
```

---

## 7. Ejercicio guiado

### Enunciado

Tienes una clase `ReportGenerator` que hace demasiadas cosas:

1. Obtiene datos de ventas desde una "base de datos" (simulada).
2. Genera un informe en PDF.
3. Genera un informe en Excel.
4. Envía el informe por email al destinatario.

La clase actual (violación de SRP):

```csharp
public class ReportGenerator
{
    public void GenerateAndSend(string recipient)
    {
        // 1. Obtener datos
        var data = GetSalesData();

        // 2. Generar PDF
        var pdf = GeneratePdfReport(data);

        // 3. Generar Excel
        var excel = GenerateExcelReport(data);

        // 4. Enviar por email
        SendEmail(recipient, pdf, excel);
    }

    private List<Sale> GetSalesData() { /* ... */ }
    private byte[] GeneratePdfReport(List<Sale> data) { /* ... */ }
    private byte[] GenerateExcelReport(List<Sale> data) { /* ... */ }
    private void SendEmail(string to, byte[] pdf, byte[] excel) { /* ... */ }
}
```

### Objetivo

Refactoriza `ReportGenerator` aplicando SRP. El resultado debe tener **al menos 4 clases** separadas con una responsabilidad cada una y una clase orquestadora.

### Solución paso a paso

#### Paso 1: Identificar las responsabilidades

Leyendo la clase, detectamos 4 responsabilidades distintas:
- **Obtener datos**: `GetSalesData()` → `SalesDataProvider`
- **Generar PDF**: `GeneratePdfReport()` → `PdfReportGenerator`
- **Generar Excel**: `GenerateExcelReport()` → `ExcelReportGenerator`
- **Enviar email**: `SendEmail()` → `EmailReportSender`

#### Paso 2: Extraer cada responsabilidad a su propia clase

```csharp
// Responsabilidad 1: obtener datos de ventas
public class SalesDataProvider
{
    public List<Sale> GetSalesData()
    {
        // En producción: consulta a base de datos, API, etc.
        return new List<Sale>
        {
            new Sale { Product = "Laptop", Amount = 1200m, Date = DateTime.Today },
            new Sale { Product = "Mouse", Amount = 25m, Date = DateTime.Today },
            new Sale { Product = "Teclado", Amount = 80m, Date = DateTime.Today },
        };
    }
}

// Responsabilidad 2: generar informes en PDF
public class PdfReportGenerator
{
    public byte[] Generate(List<Sale> data)
    {
        Console.WriteLine("[PDF] Generando informe PDF...");
        // En producción: usaría una librería como QuestPDF, iTextSharp, etc.
        return new byte[] { 0x25, 0x50, 0x44, 0x46 }; // Simulación
    }
}

// Responsabilidad 3: generar informes en Excel
public class ExcelReportGenerator
{
    public byte[] Generate(List<Sale> data)
    {
        Console.WriteLine("[Excel] Generando informe Excel...");
        // En producción: usaría ClosedXML, EPPlus, etc.
        return new byte[] { 0x50, 0x4B, 0x03, 0x04 }; // Simulación
    }
}

// Responsabilidad 4: enviar informes por email
public class EmailReportSender
{
    public void Send(string recipient, byte[] pdfData, byte[] excelData)
    {
        Console.WriteLine($"[Email] Enviando informes a {recipient}...");
        Console.WriteLine($"  Adjunto 1: report.pdf ({pdfData.Length} bytes)");
        Console.WriteLine($"  Adjunto 2: report.xlsx ({excelData.Length} bytes)");
        Console.WriteLine("[Email] Enviado.");
        // En producción: usaría SmtpClient, MailKit, SendGrid, etc.
    }
}
```

#### Paso 3: Crear la clase orquestadora

```csharp
// Orquestador: una sola responsabilidad = coordinar el flujo
public class ReportService
{
    private readonly SalesDataProvider _dataProvider;
    private readonly PdfReportGenerator _pdfGenerator;
    private readonly ExcelReportGenerator _excelGenerator;
    private readonly EmailReportSender _emailSender;

    public ReportService(
        SalesDataProvider dataProvider,
        PdfReportGenerator pdfGenerator,
        ExcelReportGenerator excelGenerator,
        EmailReportSender emailSender)
    {
        _dataProvider = dataProvider;
        _pdfGenerator = pdfGenerator;
        _excelGenerator = excelGenerator;
        _emailSender = emailSender;
    }

    public void GenerateAndSendReport(string recipient)
    {
        // 1. Obtener datos
        List<Sale> data = _dataProvider.GetSalesData();

        // 2. Generar PDF
        byte[] pdf = _pdfGenerator.Generate(data);

        // 3. Generar Excel
        byte[] excel = _excelGenerator.Generate(data);

        // 4. Enviar
        _emailSender.Send(recipient, pdf, excel);
    }
}
```

#### Paso 4: Composición manual (entry point)

```csharp
// Programa principal: punto de composición
SalesDataProvider dataProvider = new SalesDataProvider();
PdfReportGenerator pdfGenerator = new PdfReportGenerator();
ExcelReportGenerator excelGenerator = new ExcelReportGenerator();
EmailReportSender emailSender = new EmailReportSender();

ReportService reportService = new ReportService(
    dataProvider, pdfGenerator, excelGenerator, emailSender);

reportService.GenerateAndSendReport("gerente@empresa.com");
```

### ¿Qué hemos ganado?

- Si cambia la fuente de datos (API en lugar de BD), solo modificas `SalesDataProvider`.
- Si cambia la librería de PDF, solo modificas `PdfReportGenerator`.
- Si cambia el proveedor de email, solo modificas `EmailReportSender`.
- Si cambia el flujo (ej: generar solo Excel y no PDF), solo modificas `ReportService`.
- Puedes testear cada clase por separado con datos simulados.

---

## 8. Reto

**NO lo resuelvas todavía. Inténtalo por tu cuenta antes de mirar la solución.**

Tienes esta clase que gestiona pedidos en un e-commerce:

```csharp
public class OrderProcessor
{
    public void Process(Order order)
    {
        // Validar stock
        if (order.Quantity > 10)
            throw new Exception("No hay suficiente stock.");

        // Calcular total con descuento
        decimal discount = 0;
        if (order.CustomerType == "VIP")
            discount = 0.15m;
        else if (order.CouponCode == "BLACKFRIDAY")
            discount = 0.20m;
        order.Total = order.UnitPrice * order.Quantity * (1 - discount);

        // Guardar en BD
        Console.WriteLine($"[BD] Guardando pedido {order.Id}...");

        // Actualizar inventario
        Console.WriteLine($"[Inventario] Reduciendo stock de {order.ProductName} en {order.Quantity}...");

        // Enviar confirmación al cliente
        Console.WriteLine($"[Email] Confirmación enviada a {order.CustomerEmail}");

        // Registrar auditoría
        File.AppendAllText("audit.log", $"Pedido {order.Id} procesado: {order.Total:C}\n");
    }
}
```

**Reto**: Refactoriza `OrderProcessor` aplicando SRP. Identifica todas las responsabilidades y extráelas a clases separadas con una sola razón de cambio. Crea un orquestador que coordine el flujo.

Pista: hay **6 responsabilidades** distintas en esa clase. ¿Puedes encontrarlas todas?

---

## 9. Resumen

| Concepto | Descripción |
|---|---|
| **SRP** | Una clase debe tener una sola razón de cambio. |
| **Razón de cambio** | Un actor o stakeholder que puede pedir modificar la clase. |
| **Orquestador** | Clase que coordina el flujo delegando en otras clases especializadas. |
| **Heurística de la "y"** | Si describes la clase con "y", cada conjunción es una responsabilidad. |
| **Cohesión** | SRP maximiza la cohesión: todo lo que está junto pertenece junto. |

**Conceptos importantes**:
- SRP es el principio fundamental de SOLID. Sin él, los otros cuatro pierden su base.
- Separar responsabilidades no es crear más clases porque sí: es asignar cada responsabilidad al actor correcto.
- El orquestador tiene su propia responsabilidad (el flujo), que es distinta de la implementación de cada paso.
- SRP aplicado correctamente conduce naturalmente a Dependency Injection y facilita el testing.

---

## 10. Preguntas frecuentes

**P: ¿Cuántos métodos puede tener una clase que cumple SRP?**

R: Los que necesite para cumplir su responsabilidad. Una clase `PostValidator` puede tener 10 métodos de validación. El límite no es numérico, es conceptual: todos los métodos deben servir al mismo propósito.

**P: ¿SRP significa que cada clase debe tener solo un método público?**

R: No. SRP habla de responsabilidades (razones de cambio), no de métodos. Una clase con un solo método público pero que mezcla validación, persistencia y logging sigue violando SRP.

**P: ¿No es excesivo crear 5 clases para una tarea simple?**

R: Para una demo puede parecer excesivo. En un sistema real con 50 reglas de validación, 3 motores de BD, 2 proveedores de email y 4 destinos de log, tener cada cosa en su sitio es lo que evita el caos. El coste de NO separar se paga con creces en mantenimiento.

**P: ¿Cómo sé si dos métodos pertenecen a la misma responsabilidad?**

R: Pregúntate: "¿Cambiarían estos dos métodos por el mismo motivo?" Si la respuesta es no, pertenecen a responsabilidades distintas.

**P: ¿Qué diferencia hay entre SRP y el Principio de Segregación de Interfaces (ISP)?**

R: SRP aplica a clases ("una clase, una razón de cambio"). ISP aplica a interfaces ("un cliente no debe depender de métodos que no usa"). Son complementarios: una clase que cumple SRP tiende a implementar interfaces pequeñas (ISP), y viceversa.
