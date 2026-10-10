
# Lección 04 — Principio de Segregación de Interfaces (ISP)

---

## 1. Introducción

**¿Qué vamos a aprender?**

El **Principio de Segregación de Interfaces** (Interface Segregation Principle, ISP) es la "I" de SOLID. Fue formulado por Robert C. Martin y dice:

> "Ningún cliente debería verse obligado a depender de métodos que no usa."

En otras palabras: **las interfaces deben ser pequeñas, cohesivas y específicas para cada cliente**. No obligues a una clase a implementar métodos que no necesita.

**¿Por qué existe?**

Imagina una interfaz `IMultiFunctionPrinter` con métodos `Print()`, `Scan()`, `Fax()`, `Copy()` y `Staple()`. Una impresora básica que solo imprime está obligada a implementar `Scan()`, `Fax()`, `Copy()` y `Staple()`, aunque no tenga hardware para ello. El resultado: métodos que lanzan `NotImplementedException`, tests que necesitan mockear métodos irrelevantes, y dependencias innecesarias.

**¿Qué problema resuelve?**

- **Dependencias fantasma**: cuando una clase implementa métodos que no usa, cualquier cambio en la interfaz (aunque afecte a métodos que la clase no utiliza de verdad) obliga a recompilar, retestar y redesplegar esa clase.
- **Acoplamiento innecesario**: los clientes dependen de más cosas de las que realmente necesitan.
- **Interfaces frágiles**: cambiar una interfaz gorda afecta a todas las clases que la implementan, incluso a las que no usan la parte modificada.
- **Tests inflados**: necesitas mockear métodos que el SUT (System Under Test) nunca llama.

**Errores típicos**

- Crear "interfaces de conveniencia" que agrupan todo lo que una entidad podría necesitar.
- Pensar que una interfaz grande es mejor porque "así solo hay que inyectar una dependencia".
- Implementar interfaces parcialmente (dejando métodos vacíos o lanzando excepciones) en lugar de segregarlas.

---

## 2. Teoría

### El problema de la interfaz gorda

```
     ┌─────────────────────────────────┐
     │     IMultiFunctionPrinter       │
     │                                 │
     │  Print(Document)                │
     │  Scan() → byte[]                │
     │  Fax(number, Document)          │
     │  Copy()                         │
     │  Staple()                       │
     └────────────┬────────────────────┘
                  │
     ┌────────────┴───────────────────┐
     │                                │
     ▼                                ▼
┌─────────────────┐         ┌──────────────────┐
│ BasicPrinter    │         │ PremiumPrinter   │
│                 │         │                  │
│ Print() ✅      │         │ Print() ✅       │
│ Scan() → throw  │         │ Scan() ✅        │
│ Fax()  → throw  │         │ Fax() ✅         │
│ Copy() → throw  │         │ Copy() ✅        │
│ Staple()→ throw │         │ Staple() ✅      │
└─────────────────┘         └──────────────────┘
```

`BasicPrinter` tiene 4 métodos que lanzan excepción. Está obligada a **depender** de métodos que no usa. Esto es la violación de ISP.

### La solución: interfaces segregadas

```
┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐
│ IPrinter │  │ IScanner │  │  IFax    │  │ ICopier  │
│          │  │          │  │          │  │          │
│ Print()  │  │ Scan()   │  │ Fax()    │  │ Copy()   │
└────┬─────┘  └────┬─────┘  └────┬─────┘  └────┬─────┘
     │             │             │             │
     ▼             ▼             ▼             ▼
┌────────────┐ ┌──────────┐ ┌──────────────────────────┐
│BasicPrinter│ │ScannerOnly│ │    PremiumPrinter        │
│            │ │          │ │                          │
│ IPrinter   │ │ IScanner │ │ IPrinter, IScanner,      │
│            │ │          │ │ IFax, ICopier            │
└────────────┘ └──────────┘ └──────────────────────────┘
```

Cada clase implementa **solo** lo que necesita. `BasicPrinter` ya no tiene métodos que lancen excepción. El compilador impide que el código cliente llame a `Scan()` en una `BasicPrinter`.

### ISP y SRP: complementarios

- **SRP** (clases): "una clase, una razón de cambio".
- **ISP** (interfaces): "una interfaz, un cliente".

Una clase que cumple SRP tiende a implementar interfaces pequeñas y cohesivas. Una interfaz que cumple ISP tiende a ser implementada por clases con una sola responsabilidad. Ambos principios se refuerzan mutuamente.

### Herencia de interfaces para componer

Puedes combinar interfaces pequeñas mediante herencia para crear contratos más grandes sin violar ISP:

```csharp
// Interfaces base segregadas
public interface IReadOnlyRepository<T> { /* solo métodos de lectura */ }
public interface IWriteOnlyRepository<T> { /* solo métodos de escritura */ }

// Interfaz compuesta para quien necesita ambas
public interface IRepository<T> : IReadOnlyRepository<T>, IWriteOnlyRepository<T> { }
```

Esto permite que un servicio que solo lee dependa de `IReadOnlyRepository<T>`, mientras que uno que hace CRUD completo dependa de `IRepository<T>`. Ambos obtienen exactamente lo que necesitan, ni más ni menos.

### Comparación con otros lenguajes

| Concepto | Java | TypeScript | Python |
|---|---|---|---|
| Interfaces pequeñas | `interface` con pocos métodos | `interface` segregada | Protocolos / ABC |
| Herencia de interfaces | `extends` múltiple | `extends` múltiple | Herencia múltiple |
| ISP en la práctica | Igual que en C# | Duck typing relaja la necesidad | Protocolos implícitos |

---

## 3. Ejemplo mínimo

```csharp
// ─── VIOLACIÓN ISP: Interfaz gorda ───
public interface IWorker
{
    void Work();
    void Eat();
    void Sleep();
}

// Un robot trabaja pero NO come ni duerme.
// Está obligado a implementar Eat() y Sleep().
public class RobotViolation : IWorker
{
    public void Work() => Console.WriteLine("Robot trabajando...");

    public void Eat() =>
        throw new NotImplementedException("¡Un robot no come!");

    public void Sleep() =>
        throw new NotImplementedException("¡Un robot no duerme!");
}

// ─── CORRECTO ISP: Interfaces segregadas ───
public interface IWorkable { void Work(); }
public interface IEatable { void Eat(); }
public interface ISleepable { void Sleep(); }

// Humano: trabaja, come y duerme
public class Human : IWorkable, IEatable, ISleepable
{
    public void Work() => Console.WriteLine("Humano trabajando...");
    public void Eat() => Console.WriteLine("Humano comiendo...");
    public void Sleep() => Console.WriteLine("Humano durmiendo...");
}

// Robot: solo trabaja
public class Robot : IWorkable
{
    public void Work() => Console.WriteLine("Robot trabajando...");
    // No implementa IEatable ni ISleepable.
    // El compilador impide llamar a Eat() o Sleep() en un Robot.
}
```

Para ejecutar el ejemplo completo:

```bash
dotnet run --project ejemplos/m03-solid/Leccion-04-ISP/src/SolidISP
```

---

## 4. Ejemplo real

El código en `src/SolidISP/Program.cs` demuestra ISP con dos dominios distintos:

### Dominio 1: Impresoras (líneas 66-357)

**Violación: `IMultiFunctionPrinter`** (línea 76)
- Interfaz con 5 métodos: `Print`, `Scan`, `Fax`, `Copy`, `Staple`.
- `BasicPrinterViolation` solo imprime pero implementa los 5 métodos (4 lanzan `NotImplementedException`).

**Solución: interfaces segregadas** (líneas 277-295)
- `IPrinter`, `IScanner`, `IFax`, `ICopier` — cada una con un solo método cohesivo.
- `BasicPrinter : IPrinter` — solo `Print()`.
- `ScannerOnly : IScanner` — solo `Scan()`.
- `PremiumPrinter : IPrinter, IScanner, IFax, ICopier` — compone las 4 interfaces.

### Dominio 2: Repositorio CMS (líneas 359-621)

**Violación: `IRepositoryViolation<T>`** (línea 174)
- Mezcla métodos de lectura (`GetAll`, `GetById`, `Search`...) y escritura (`Add`, `Update`, `Delete`, `BulkInsert`).
- `ReadOnlyRepositoryViolation` implementa los métodos de escritura lanzando `NotImplementedException`.

**Solución: repositorios segregados** (líneas 363-399)
- `IReadOnlyRepository<T>` con solo métodos de consulta.
- `IWriteOnlyRepository<T>` con solo métodos de modificación.
- `IRepository<T> : IReadOnlyRepository<T>, IWriteOnlyRepository<T>` para quien necesita ambas.

**Demostración de composición**: `PublicPageService` depende solo de `IReadOnlyRepository<T>`, pero funciona tanto con `ReadOnlyPostRepository` como con `PostRepository` (que implementa `IRepository<T>` que hereda de `IReadOnlyRepository<T>`).

---

## 5. Buenas prácticas

### ✅ QUÉ HACER

1. **Una interfaz por rol de cliente**: si dos consumidores usan distintos subconjuntos de métodos, segrega.
2. **Interfaces de 3-5 métodos como máximo**: si una interfaz crece más, pregúntate si todos los implementadores realmente necesitan todo.
3. **Herencia de interfaces para componer**: `IRepository<T> : IReadOnlyRepository<T>, IWriteOnlyRepository<T>`.
4. **Declarar la dependencia mínima necesaria**: si un servicio solo lee, que pida `IReadOnlyRepository<T>`, no `IRepository<T>`.
5. **Usar el compilador como guardián**: si implementas una interfaz, debes implementar TODOS sus métodos. Si no puedes, la interfaz es demasiado grande.

### ❌ QUÉ EVITAR

1. **`throw new NotImplementedException()` en implementaciones de interfaz**: es la señal definitiva de ISP violado. Si no puedes implementar un método, no deberías implementar esa interfaz.
2. **Métodos vacíos**: `public void Scan() { }` — igual de malo. El código cliente cree que escanea pero no hace nada.
3. **Interfaces "por si acaso"**: no añadas métodos a una interfaz "por si alguien los necesita en el futuro". YAGNI (You Ain't Gonna Need It).
4. **Una única interfaz por clase**: una clase puede (y debe) implementar múltiples interfaces pequeñas. `PremiumPrinter : IPrinter, IScanner, IFax, ICopier` es mejor que `IPremiumPrinter` con 4 métodos.
5. **Depender de interfaces grandes cuando solo necesitas una parte**: `IReadOnlyRepository<T>` en lugar de `IRepository<T>` cuando solo consultas.

### ¿Por qué?

Cada dependencia innecesaria es un punto de acoplamiento. Si cambia `IRepository<T>` porque se añade un método de escritura `BulkUpdate`, todos los servicios que dependen de `IRepository<T>` deben recompilarse, aunque solo usen métodos de lectura. Con `IReadOnlyRepository<T>`, esos servicios ni se enteran.

---

## 6. Errores habituales

### Error 1: Segregar demasiado (atomización)

```csharp
// ❌ MAL: Demasiadas interfaces microscópicas
public interface IGetById<T> { T? GetById(int id); }
public interface IGetAll<T> { IEnumerable<T> GetAll(); }
public interface ICount<T> { int Count(); }
public interface IExists<T> { bool Exists(int id); }
```

**Por qué está mal**: estas 4 operaciones pertenecen al mismo rol de cliente (lectura) y cambian por los mismos motivos. Deben estar juntas en `IReadOnlyRepository<T>`. ISP no significa "un método por interfaz".

### Error 2: Creer que ISP es solo para reducir métodos

```csharp
// ❌ MAL: Interfaz con 7 métodos de solo lectura tampoco es ideal
public interface IPostQueries
{
    Post? GetById(int id);
    Post? GetBySlug(string slug);
    Post? GetLatest();
    IEnumerable<Post> GetByCategory(int categoryId);
    IEnumerable<Post> GetByAuthor(int authorId);
    IEnumerable<Post> Search(string term);
    (IEnumerable<Post>, int) GetPaged(int page, int size);
    int CountAll();
    int CountByCategory(int categoryId);
}
```

**Por qué está mal**: aunque todos son de lectura, hay dos roles de cliente distintos: consultas individuales (`GetById`, `GetBySlug`) y listados paginados (`GetPaged`, `Search`). Si un cliente solo necesita consultas individuales, debería existir una interfaz más pequeña.

### Error 3: Métodos con parámetros booleanos de "modo"

```csharp
// ❌ MAL: Parámetro bool para cambiar comportamiento — la interfaz esconde dos roles
public interface IUserRepository
{
    IEnumerable<User> GetUsers(bool includeDeleted);
    // includeDeleted=true es un requerimiento del admin
    // includeDeleted=false es un requerimiento de la página pública
}
```

**Versión correcta**:

```csharp
// ✅ BIEN: Dos métodos separados o dos interfaces
public interface IUserRepository
{
    IEnumerable<User> GetActiveUsers();
    IEnumerable<User> GetAllUsers(); // Admin: incluye eliminados
}
```

---

## 7. Ejercicio guiado

### Enunciado

Tienes esta interfaz `IWorker` que usan dos tipos de empleados en una fábrica:

```csharp
public interface IWorker
{
    void Work();
    void Eat();
    void Sleep();
    void TakeBreak();
    void AttendMeeting();
}
```

Tienes tres tipos de trabajadores:

1. **HumanWorker**: trabaja, come, duerme, toma descansos y asiste a reuniones.
2. **RobotWorker**: solo trabaja (no come, no duerme, no toma breaks, no va a meetings).
3. **ContractorWorker**: trabaja y asiste a reuniones, pero no está sujeto a los horarios de comida/descanso de la empresa.

Actualmente todos implementan `IWorker` y lanzan `NotImplementedException` en los métodos que no aplican.

### Objetivo

Segrega la interfaz `IWorker` en interfaces más pequeñas para que cada tipo de trabajador implemente solo lo que necesita. Cero `NotImplementedException`. El código cliente debe poder tratar a todos los trabajadores mediante las interfaces adecuadas.

### Solución paso a paso

#### Paso 1: Identificar los roles de cliente

Analizando quién usa qué:

| Método | HumanWorker | RobotWorker | ContractorWorker |
|---|---|---|---|
| Work() | ✅ | ✅ | ✅ |
| Eat() | ✅ | ❌ | ❌ |
| Sleep() | ✅ | ❌ | ❌ |
| TakeBreak() | ✅ | ❌ | ❌ |
| AttendMeeting() | ✅ | ❌ | ✅ |

Hay tres roles distintos:
- **Trabajador básico**: `Work()` — todos lo implementan.
- **Ser vivo**: `Eat()`, `Sleep()`, `TakeBreak()` — solo humanos.
- **Asistente a reuniones**: `AttendMeeting()` — humanos y contractors.

#### Paso 2: Crear interfaces segregadas

```csharp
// Rol 1: Todo trabajador puede trabajar
public interface IWorkable
{
    void Work();
}

// Rol 2: Solo los seres vivos tienen necesidades biológicas
public interface ILiving
{
    void Eat();
    void Sleep();
    void TakeBreak();
}

// Rol 3: Los que participan en reuniones
public interface IMeetingAttendee
{
    void AttendMeeting();
}
```

#### Paso 3: Implementar cada tipo con sus interfaces

```csharp
public class HumanWorker : IWorkable, ILiving, IMeetingAttendee
{
    public void Work() => Console.WriteLine("Humano: trabajando en la línea de montaje.");
    public void Eat() => Console.WriteLine("Humano: comiendo en la cafetería.");
    public void Sleep() => Console.WriteLine("Humano: durmiendo en casa.");
    public void TakeBreak() => Console.WriteLine("Humano: tomando un café.");
    public void AttendMeeting() => Console.WriteLine("Humano: asistiendo a la daily.");
}

public class RobotWorker : IWorkable
{
    public void Work() => Console.WriteLine("Robot: ensamblando piezas 24/7.");
    // Solo implementa IWorkable. Cero métodos innecesarios.
}

public class ContractorWorker : IWorkable, IMeetingAttendee
{
    public void Work() => Console.WriteLine("Contractor: desarrollando el módulo asignado.");
    public void AttendMeeting() => Console.WriteLine("Contractor: conectado a la sprint review.");
    // No implementa ILiving porque gestiona sus propios horarios.
}
```

#### Paso 4: Código cliente especializado

```csharp
// Función que usa la interfaz mínima necesaria
void StartShift(IWorkable worker)
{
    worker.Work(); // Funciona con HumanWorker, RobotWorker y ContractorWorker
}

void LunchBreak(ILiving living)
{
    living.Eat();    // Solo HumanWorker
    living.TakeBreak(); // Solo HumanWorker
}

void DailyStandup(IMeetingAttendee attendee)
{
    attendee.AttendMeeting(); // HumanWorker y ContractorWorker
}

// ─── Uso ───
IWorkable[] allWorkers = {
    new HumanWorker(),
    new RobotWorker(),
    new ContractorWorker(),
};

foreach (IWorkable w in allWorkers)
{
    StartShift(w); // ✅ Funciona con todos
}

ILiving[] livingBeings = { new HumanWorker() };
// RobotWorker y ContractorWorker NO están en esta lista (no implementan ILiving)

IMeetingAttendee[] attendees = {
    new HumanWorker(),
    new ContractorWorker(),
};
// RobotWorker NO está en esta lista (no implementa IMeetingAttendee)
```

---

## 8. Reto

**NO lo resuelvas todavía. Inténtalo por tu cuenta antes de mirar la solución.**

Tienes una interfaz de servicio de notificaciones en un CMS:

```csharp
public interface INotificationService
{
    void SendEmail(string to, string subject, string body);
    void SendSms(string phone, string message);
    void SendPushNotification(string deviceToken, string title, string body);
    void SendSlackMessage(string channel, string text);
    void ScheduleReminder(string recipient, string message, DateTime when);
    void SendBulkEmail(IEnumerable<string> recipients, string subject, string body);
    List<string> GetSentNotifications();
    void CancelScheduled(string reminderId);
}
```

El sistema tiene estos consumidores:

1. **`PostPublishedHandler`**: solo envía emails individuales cuando se publica un post.
2. **`WelcomeService`**: envía emails individuales y SMS de bienvenida a nuevos usuarios.
3. **`MarketingService`**: envía emails masivos, SMS masivos y mensajes de Slack.
4. **`ReminderService`**: programa recordatorios y los cancela.
5. **`AuditService`**: solo consulta el historial de notificaciones enviadas.

**Reto**: Segrega `INotificationService` en múltiples interfaces pequeñas. Cada consumidor debe depender SOLO de la interfaz con los métodos que realmente usa. Ninguna implementación debe lanzar `NotImplementedException`. Usa herencia de interfaces para componer donde tenga sentido.

---

## 9. Resumen

| Concepto | Descripción |
|---|---|
| **ISP** | Ningún cliente debe depender de métodos que no usa. |
| **Interfaz gorda** | Interfaz con demasiados métodos que obliga a implementaciones parciales. |
| **Rol de cliente** | Conjunto de métodos que un consumidor específico necesita. |
| **Segregación** | Dividir una interfaz grande en varias pequeñas y cohesivas. |
| **Herencia de interfaces** | Componer interfaces más grandes a partir de pequeñas (`IRepository : IReadOnly, IWriteOnly`). |
| **Composición de interfaces** | Una clase implementa múltiples interfaces pequeñas en lugar de una grande. |

**Conceptos importantes**:
- ISP protege a los clientes de cambios que no les afectan. Si cambia `IWriteOnlyRepository`, los clientes de `IReadOnlyRepository` ni se enteran.
- La señal más clara de violación de ISP es `throw new NotImplementedException()` en una implementación de interfaz.
- ISP y SRP son complementarios: clases con una responsabilidad tienden a implementar interfaces pequeñas.
- No segregues en exceso: varias operaciones que cambian por las mismas razones deben permanecer juntas.
- El compilador es tu aliado: con interfaces segregadas, el código que intenta llamar a un método no soportado simplemente no compila.

---

## 10. Preguntas frecuentes

**P: ¿Cuántos métodos debe tener una interfaz que cumple ISP?**

R: No hay un número mágico. La pregunta correcta es: "¿todos los implementadores necesitan todos los métodos?" Si la respuesta es no, segrega. En la práctica, las interfaces con 1-5 métodos altamente cohesivos suelen cumplir ISP.

**P: ¿ISP no genera demasiadas interfaces?**

R: Sí, genera más interfaces. Pero el coste de tener más interfaces es insignificante comparado con el coste de mantener código con dependencias fantasma. Las interfaces son baratas de crear y mantener.

**P: ¿Qué diferencia hay entre ISP y SRP?**

R: SRP aplica a **clases**: "una clase, una razón de cambio". ISP aplica a **interfaces**: "un cliente, una interfaz". Son complementarios: una clase que cumple SRP tiende a implementar interfaces que cumplen ISP, y viceversa.

**P: ¿Una clase puede implementar muchas interfaces y cumplir ISP?**

R: Sí, de hecho es lo ideal. `PremiumPrinter : IPrinter, IScanner, IFax, ICopier` cumple ISP perfectamente porque implementa 4 interfaces pequeñas, cada una con un solo método cohesivo. El código cliente elige contra cuál de ellas programa.

**P: ¿Cómo manejo el caso de una clase que necesita casi toda la interfaz pero no un método concreto?**

R: Ese método probablemente pertenece a un rol de cliente distinto. Extráelo a una interfaz separada. La clase original implementará la interfaz grande (sin ese método) y, si necesita el método extra, implementará también la interfaz segregada.

**P: ¿Qué relación hay entre ISP y el patrón Repository en EF Core?**

R: Muchísima. `IReadOnlyRepository<T>` y `IWriteOnlyRepository<T>` son aplicaciones directas de ISP. Un servicio de consulta pública depende de `IReadOnlyRepository<T>`, mientras que un servicio de administración depende de `IRepository<T>`. Esto evita exponer métodos de escritura donde no deberían estar disponibles.
