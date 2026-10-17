# Lección 01 — Clases, Herencia, Interfaces y Polimorfismo

---

## 1. Introducción

En esta lección aprenderás los cuatro pilares fundamentales de la Programación Orientada a Objetos (POO) en C#: **clases**, **herencia**, **interfaces** y **polimorfismo**.

### ¿Por qué existen?

Cuando un programa crece, mantener cientos de funciones sueltas se vuelve insostenible. La POO permite organizar el código en **unidades cohesivas** (clases) que agrupan datos y comportamiento, modelando entidades del mundo real o conceptos del dominio de negocio.

La **herencia** evita duplicar código entre clases que comparten comportamiento común (`Employee` → `Manager`, `Developer`). Las **interfaces** permiten definir contratos sin imponer una jerarquía rígida: `IPayable` puede ser implementado por `Employee` y por `Freelancer`, sin que hereden de la misma clase. El **polimorfismo** permite escribir código que funciona con el tipo base (`Employee`) sin saber cuál es el tipo concreto, logrando que el sistema sea extensible sin modificar el código existente (principio Open/Closed de SOLID).

### Errores típicos de principiantes

- **Confundir herencia con composición:** heredar `Engine` de `Car` es incorrecto. Un coche **tiene** un motor (composición), no **es** un motor (herencia). La herencia modela relaciones "es un"; la composición modela "tiene un". Prefiere composición.
- **No entender virtual dispatch:** declarar un método en la clase derivada sin `virtual` en la base y sin `override` en la derivada produce **ocultación** (`new`), no polimorfismo. El comportamiento difiere según el tipo de la **variable**, no del objeto real.
- **Creer que C# tiene herencia múltiple de clases:** no la tiene. Una clase solo puede heredar de una clase base (para evitar el "diamond problem"), pero puede implementar múltiples interfaces.
- **Olvidar llamar a `base()`:** si la clase base no tiene constructor sin parámetros, toda clase derivada debe invocar explícitamente `base(...)` con los parámetros adecuados.

---

## 2. Teoría

### 2.1 Clases

Una clase es una **plantilla** (blueprint) a partir de la cual se crean objetos (instancias). Define:

| Elemento | Descripción | Ejemplo |
|---|---|---|
| **Campos** (`fields`) | Variables privadas que almacenan el estado interno del objeto | `private readonly Guid _id;` |
| **Propiedades** (`properties`) | Acceso controlado a los datos mediante `get`/`set` | `public string Name { get; set; }` |
| **Métodos** | Comportamiento que el objeto puede ejecutar | `public virtual decimal CalculateBonus()` |
| **Constructores** | Inicializan el objeto en un estado válido al crearlo | `public Employee(string name, decimal salary)` |

**Campo vs Propiedad:** un campo es una variable interna. Una propiedad es un miembro que encapsula un campo (o valor calculado) y permite añadir lógica de validación en `get`/`set`. Las propiedades son la interfaz pública; los campos son privados.

```
┌──────────────────────────────────┐
│           Employee               │
├──────────────────────────────────┤
│ - _id : Guid          ← campo   │
│ - _name : string      ← campo   │
├──────────────────────────────────┤
│ + Id : Guid           ← prop.   │
│ + Name : string       ← prop.   │
│ + Salary : decimal    ← prop.   │
├──────────────────────────────────┤
│ + Employee(name, salary)        │
│ + CalculateBonus() : decimal    │
│ + GetMonthlyPay() : decimal     │
│ + GenerateReport() : string     │
└──────────────────────────────────┘
```

### 2.2 Herencia

La herencia establece una relación **"es un"** entre clases. La **clase derivada** (subclase) hereda todos los miembros (campos, propiedades, métodos) de la **clase base** (superclase) y puede:

- **Extender:** añadir nuevos miembros.
- **Sobrescribir:** reemplazar métodos marcados como `virtual` mediante `override`.
- **Reutilizar:** invocar miembros de la clase base con `base.Miembro`.

En C#, la herencia de clases es **simple**: una clase solo puede tener una clase base directa. Todas las clases heredan implícitamente de `System.Object`.

```
         System.Object
              │
         Employee ◄── IPayable
              │         IReportable
     ┌────────┴────────┐
     │                  │
  Manager           Developer
```

La **cadena de constructores** funciona así: al instanciar `new Manager(...)`, el runtime primero ejecuta el constructor de `Employee` (vía `base(...)`) y luego el de `Manager`. La clase base debe estar completamente inicializada antes que la derivada.

### 2.3 Interfaces

Una interfaz (definida con la palabra clave `interface`) es un **contrato**: declara métodos, propiedades, eventos e indexadores que las clases implementadoras están **obligadas** a proporcionar. La interfaz no contiene implementación (en C# 8+ pueden tener métodos default, pero los veremos en otra lección).

```
interface IPayable            interface IReportable
{                             {
    decimal GetMonthlyPay();      string GenerateReport();
}                             }
```

C# no permite herencia múltiple de clases, pero sí **herencia múltiple de tipo** mediante interfaces. Una clase puede implementar tantas interfaces como necesite. Esto resuelve el "diamond problem" permitiendo polimorfismo múltiple sin ambigüedad de estado.

Comparación con Java: en C# no existe `implements`; se usa `:` tanto para herencia como para interfaces. Ejemplo: `class Employee : IPayable, IReportable`.

### 2.4 Polimorfismo

El polimorfismo ("muchas formas") permite tratar objetos de distintas clases derivadas a través de una referencia al tipo base, obteniendo el comportamiento específico de cada tipo real. Es la capacidad de que una misma llamada (`CalculateBonus()`) produzca resultados distintos según el tipo real del objeto.

**Virtual dispatch** es el mecanismo que lo hace posible:

1. Un método se marca como `virtual` en la clase base.
2. Las clases derivadas lo sobrescriben con `override`.
3. El CLR mantiene una **virtual method table (vtable)** por cada tipo: una tabla de punteros a las implementaciones reales de los métodos virtuales.
4. En tiempo de ejecución, una llamada a `emp.CalculateBonus()` consulta la vtable del **tipo real** del objeto (no el de la variable) y salta al método correcto.

```
Variable de tipo Employee ──► Objeto real: Manager
                                │
List<Employee>                  │  vtable de Manager:
  [0] ───────────────► Employee │    CalculateBonus → Manager.CalculateBonus
  [1] ───────────────► Manager  │    ToString       → Manager.ToString
  [2] ───────────────► Developer│
                                │
foreach (Employee emp in list)  │
    emp.CalculateBonus();  ─────┘  ← Virtual dispatch: se llama al método
                                      del tipo REAL, no de la variable
```

**Sin `virtual` no hay polimorfismo.** Si un método no es virtual, la llamada se resuelve en tiempo de compilación según el tipo de la variable (static dispatch).

### 2.5 Comparación con otros lenguajes

| Concepto | C# | Java | Python | JavaScript |
|---|---|---|---|---|
| **Clase** | `class` | `class` | `class` | `class` (sintaxis ES6, prototype por dentro) |
| **Herencia** | `:` (simple) | `extends` (simple) | `class A(B):` (múltiple, MRO-C3) | `extends` (prototype chain) |
| **Interfaz** | `interface` | `interface`, `implements` | No existe (ABC y duck typing) | No existe (duck typing) |
| **Método virtual** | Explícito (`virtual`) | Todos son virtuales por defecto | Todos son virtuales | Todos son virtuales |
| **Sobrescritura** | `override` | `@Override` | Mismo nombre | Mismo nombre |
| **Método sellado** | `sealed override` | `final` | No reimplementable | N/A |

**Diferencia clave con Java:** en Java todos los métodos son virtuales por defecto (salvo `final` o `static`). En C# los métodos NO son virtuales a menos que los marques explícitamente como `virtual`. Esto es una decisión de diseño deliberada: favorece el rendimiento (evita indirección de vtable cuando no es necesaria) y hace explícita la intención de permitir extensión.

**Diferencia clave con Python:** en Python no hay interfaces ni tipos estrictos en tiempo de compilación. El polimorfismo se basa en duck typing: "si camina como pato y suena como pato, es un pato". C# ofrece polimorfismo **estático** (interfaces verificadas en compilación) que detecta errores antes de ejecutar.

---

## 3. Ejemplo mínimo

El ejemplo más pequeño posible de polimorfismo con clases e interfaces:

```csharp
// ─── Clase base con método virtual ────────────────────────────────────
class Animal
{
    public string Name { get; }

    public Animal(string name) => Name = name;

    // `virtual` permite que las clases derivadas sobrescriban este método
    public virtual string MakeSound() => "Este animal no emite sonido";
}

// ─── Clases derivadas con override ────────────────────────────────────
class Dog : Animal
{
    public Dog(string name) : base(name) { }
    public override string MakeSound() => "¡Guau!";
}

class Cat : Animal
{
    public Cat(string name) : base(name) { }
    public override string MakeSound() => "¡Miau!";
}

// ─── Polimorfismo en acción ──────────────────────────────────────────
List<Animal> animals = new()
{
    new Dog("Rex"),
    new Cat("Luna"),
    new Animal("Desconocido") // Usa la implementación base
};

// Aunque la variable es de tipo Animal, cada objeto ejecuta su propio método
foreach (Animal animal in animals)
    Console.WriteLine($"{animal.Name}: {animal.MakeSound()}");

// Salida:
// Rex: ¡Guau!
// Luna: ¡Miau!
// Desconocido: Este animal no emite sonido
```

**Línea por línea:**

- `public virtual string MakeSound()` → el `virtual` habilita la sobrescritura. Sin él, el compilador no permitiría `override` en las subclases.
- `: base(name)` → encadena al constructor de `Animal`. Si `Animal` no tuviera constructor sin parámetros, esto sería obligatorio.
- `List<Animal>` → una colección del tipo base que contiene objetos de tipos derivados. Esto solo funciona porque `Dog` y `Cat` **son** `Animal`.
- `foreach (Animal animal in animals)` → en cada iteración se llama a `MakeSound()`, pero el CLR ejecuta la versión del tipo **real** del objeto (Dog, Cat o Animal), no la de la variable.

---

## 4. Ejemplo real

El código ejecutable de esta lección está en `src/PooFundamentos/Program.cs`. Modela un sistema de gestión de empleados con una jerarquía profesional:

```
┌─────────────────────────────────────────────┐
│  IPayable          IReportable              │
│  GetMonthlyPay()   GenerateReport()         │
└────────┬───────────────┬────────────────────┘
         │ (implementa)  │ (implementa)
         └───────┬───────┘
                 │
         ┌───────┴────────┐
         │   Employee     │ ← Clase base
         │  (virtual)     │
         │ CalculateBonus │ → 10% del salario
         └───────┬────────┘
                 │
     ┌───────────┴───────────┐
     │                       │
┌────┴─────┐          ┌──────┴──────┐
│ Manager  │          │  Developer  │
│ override │          │  override   │
│ Bonus    │          │  Bonus      │
│ = 20%    │          │  = 15%      │
│          │          │  +500€ si   │
│          │          │  .NET/C#    │
└──────────┘          └─────────────┘
```

El programa demuestra:

1. **Creación de objetos** con distintos constructores (Employee, Manager, Developer).
2. **Polimorfismo**: una `List<Employee>` donde cada elemento ejecuta su propia versión de `CalculateBonus()` mediante virtual dispatch.
3. **Upcasting y downcasting** con los operadores `is`, `as` y casting directo (con manejo de `InvalidCastException`).
4. **Switch expression sobre tipos** usando pattern matching de C#.
5. **Polimorfismo de interfaces**: tratar los mismos objetos como `IPayable` (pago mensual) y como `IReportable` (generación de informes).

Ejecuta el ejemplo con:

```bash
dotnet run --project "ejemplos/m02-poo/Leccion-01-Clases-Herencia-Interfaces-Polimorfismo/src/PooFundamentos"
```

---

## 5. Buenas prácticas

### ✅ Hacer

- **Preferir composición sobre herencia.** Si la relación no es claramente "es un", usa composición (una clase contiene una instancia de otra como campo privado). La composición es más flexible y produce menor acoplamiento.
- **Declarar `virtual` solo cuando sea necesario.** En C#, los métodos no son virtuales por defecto (a diferencia de Java). Esto es una ventaja: evitas que futuras clases derivadas rompan el contrato de la base inadvertidamente. El diseñador de la clase base debe decidir explícitamente qué puntos de extensión ofrece.
- **Diseñar interfaces pequeñas y cohesivas** (Interface Segregation Principle). Es mejor tener `IPrinter`, `IScanner` y `IFax` que una sola `IMultiFunctionPrinter` gigante. Los clientes no deben depender de métodos que no usan.
- **Documentar los métodos `virtual`** explicando qué pueden (y qué NO deben) modificar las clases derivadas. Indica precondiciones, postcondiciones e invariantes que las subclases deben respetar.
- **Usar `is` para downcasting** en lugar de `as` + null check. El pattern matching de C# es más expresivo y seguro: `if (emp is Manager m)`.
- **Declarar las clases como `sealed` si no están diseñadas para ser heredadas.** La herencia acopla fuertemente; sellar una clase comunica que no fue diseñada como base y evita que otros la extiendan incorrectamente.

### ❌ Evitar

- **No uses herencia solo para "reutilizar código".** Si dos clases comparten lógica pero no tienen relación "es un", extrae esa lógica a una clase auxiliar y compón. La herencia acopla permanentemente la subclase a la implementación de la base.
- **No escribas `new` para "sobrescribir" métodos.** `new` oculta el método base; no lo sobrescribe. El comportamiento será diferente según accedas al objeto por la variable base o por la derivada. Usa `override`.
- **No hagas downcasting sin comprobar el tipo.** Un `(Manager)emp` lanza `InvalidCastException` si `emp` no es realmente un `Manager`. Verifica con `is` o maneja la excepción.
- **No pongas lógica compleja en el constructor.** Los constructores deben inicializar estado, no ejecutar operaciones con efectos secundarios (llamadas a BD, red, etc.). Usa métodos fábrica o builders si necesitas lógica compleja.

---

## 6. Errores habituales

### Error 1: Usar `new` en lugar de `override`

```csharp
class Base
{
    public virtual void DoWork() => Console.WriteLine("Base");
}

class Derived : Base
{
    // ❌ MAL: Esto OCULTA el método, no lo sobrescribe
    // El compilador incluso emite un warning CS0108
    public new void DoWork() => Console.WriteLine("Derived");
}
```

```csharp
Base obj = new Derived();
obj.DoWork(); // ❌ Imprime "Base", no "Derived" — porque `new` NO participa en virtual dispatch
```

```csharp
class Derived : Base
{
    // ✅ BIEN: Sobrescribe el método virtual
    public override void DoWork() => Console.WriteLine("Derived");
}
```

### Error 2: Olvidar `base()` cuando el constructor base tiene parámetros

```csharp
class Employee
{
    public Employee(string name) { Name = name; }
    public string Name { get; }
}

class Manager : Employee
{
    // ❌ MAL: Error de compilación CS7036.
    // "There is no argument given that corresponds to the required parameter 'name' of 'Employee.Employee(string)'"
    public Manager(string name, string department)
    {
        Department = department;
    }
}
```

```csharp
class Manager : Employee
{
    // ✅ BIEN: Llamamos al constructor de la clase base con `base(name)`
    public Manager(string name, string department) : base(name)
    {
        Department = department;
    }
    public string Department { get; }
}
```

### Error 3: Confundir `abstract` con `virtual`

| `virtual` | `abstract` |
|---|---|
| Tiene implementación en la clase base | No tiene implementación (solo firma) |
| Las subclases PUEDEN sobrescribir | Las subclases DEBEN sobrescribir |
| Se puede instanciar la clase base | La clase base NO se puede instanciar |

```csharp
abstract class Shape
{
    // `abstract` no tiene cuerpo. Las subclases DEBEN implementarlo.
    public abstract double GetArea();
}

class Circle : Shape
{
    public override double GetArea() => Math.PI * Radius * Radius;
    public double Radius { get; set; }
}
```

### Error 4: Downcasting sin verificar tipo

```csharp
List<Employee> employees = new() { new Employee("Ana", 30000) };

// ❌ MAL: InvalidCastException en tiempo de ejecución
Manager m = (Manager)employees[0];
```

```csharp
// ✅ BIEN: Verificar con `is` antes de usar
if (employees[0] is Manager m)
    Console.WriteLine(m.Department);
```

### Error 5: No entender que las propiedades también son polimórficas

Las propiedades también pueden ser `virtual` y sobrescribirse con `override`:

```csharp
class Base
{
    public virtual string Description => "Soy Base";
}

class Derived : Base
{
    public override string Description => "Soy Derived";
}
```

---

## 7. Ejercicio guiado

**Objetivo:** crear un sistema de notificaciones que demuestre polimorfismo mediante interfaces.

### Paso 1: Crear la interfaz `INotificationChannel`

```csharp
// Define el contrato que todo canal de notificación debe cumplir
interface INotificationChannel
{
    // Envía un mensaje. Cada canal implementa el envío a su manera.
    void Send(string message);
}
```

### Paso 2: Implementar los canales concretos

```csharp
class EmailChannel : INotificationChannel
{
    public string EmailAddress { get; }

    public EmailChannel(string emailAddress) => EmailAddress = emailAddress;

    public void Send(string message)
    {
        Console.WriteLine($"[EMAIL] Enviando a {EmailAddress}: {message}");
        // En producción: SmtpClient.SendAsync(...)
    }
}

class SmsChannel : INotificationChannel
{
    public string PhoneNumber { get; }

    public SmsChannel(string phoneNumber) => PhoneNumber = phoneNumber;

    public void Send(string message)
    {
        Console.WriteLine($"[SMS] Enviando a {PhoneNumber}: {message}");
        // En producción: Twilio, Vonage, etc.
    }
}

class PushNotificationChannel : INotificationChannel
{
    public string DeviceToken { get; }

    public PushNotificationChannel(string deviceToken) => DeviceToken = deviceToken;

    public void Send(string message)
    {
        Console.WriteLine($"[PUSH] Enviando a dispositivo ({DeviceToken[..8]}...): {message}");
        // En producción: Firebase Cloud Messaging, APNs, etc.
    }
}
```

### Paso 3: Crear el servicio de notificaciones

```csharp
class NotificationService
{
    // Guardamos los canales como campo privado de solo lectura.
    // La colección se inyecta en el constructor.
    private readonly List<INotificationChannel> _channels;

    public NotificationService(List<INotificationChannel> channels)
    {
        _channels = channels ?? throw new ArgumentNullException(nameof(channels));
    }

    // POLIMORFISMO: No sabemos qué canales hay, solo que todos
    // implementan INotificationChannel. El dispatch se resuelve
    // en tiempo de ejecución según el tipo real de cada canal.
    public void Broadcast(string message)
    {
        foreach (INotificationChannel channel in _channels)
        {
            channel.Send(message); // Virtual dispatch vía interfaz
        }
    }
}
```

### Paso 4: Programa principal

```csharp
// Configuramos la lista de canales (composición)
List<INotificationChannel> channels = new()
{
    new EmailChannel("ana@empresa.com"),
    new SmsChannel("+34600111222"),
    new PushNotificationChannel("device-token-abc123"),
};

NotificationService notificationService = new(channels);

// Un solo método envía a todos los canales, sin saber cuáles son
notificationService.Broadcast("¡La build ha terminado correctamente!");
```

### Paso 5: Añadir un nuevo canal sin modificar nada existente

```csharp
// Open/Closed Principle en acción: extendemos sin modificar
class SlackChannel : INotificationChannel
{
    public string WebhookUrl { get; }

    public SlackChannel(string webhookUrl) => WebhookUrl = webhookUrl;

    public void Send(string message)
    {
        Console.WriteLine($"[SLACK] Enviando a webhook: {message}");
    }
}

// Simplemente añadimos el nuevo canal a la lista:
channels.Add(new SlackChannel("https://hooks.slack.com/services/..."));
notificationService.Broadcast("Nueva notificación para todos los canales");
```

**¿Qué has aprendido?**

- `INotificationChannel` define un contrato sin implementación.
- `EmailChannel`, `SmsChannel` y `PushNotificationChannel` son clases completamente distintas pero tratables de forma uniforme.
- `NotificationService` no conoce los tipos concretos; solo depende de la abstracción (`INotificationChannel`). Esto es el Principio de Inversión de Dependencias (DIP).
- Añadir `SlackChannel` no requirió cambiar `NotificationService` ni los demás canales. Esto es el Principio Open/Closed (OCP).

---

## 8. Reto

Crea un sistema de formas geométricas aplicando todos los conceptos de esta lección.

**Especificaciones:**

1. Define la interfaz `IShape` con:
   - `double GetArea()` — calcula y devuelve el área.
   - `double GetPerimeter()` — calcula y devuelve el perímetro.

2. Implementa las clases:
   - `Circle` (radio) — implementa `IShape`.
   - `Rectangle` (ancho, alto) — implementa `IShape`.
   - `Triangle` (ladoA, ladoB, ladoC) — implementa `IShape`. Usa la fórmula de Herón para el área.

3. Define la interfaz `IRotatable` con:
   - `void Rotate(double degrees)` — rota la forma.
   - Haz que `Rectangle` y `Triangle` implementen `IRotatable` (un círculo no necesita rotación).

4. Crea `ScalableShape : IShape` como un **decorador** que envuelve otra `IShape` y escala sus dimensiones por un factor dado. La nueva área será `original.GetArea() * factor²` y el nuevo perímetro `original.GetPerimeter() * factor`.

5. En `Main`, crea una `List<IShape>` con varias formas (incluyendo formas escaladas) y recórrela mostrando área y perímetro de cada una. Recorre las formas que también sean `IRotatable` y rótalas.

**Pistas:**

- Usa `is IRotatable rotatable` para filtrar formas rotables.
- Recuerda que `Math.PI`, `Math.Sqrt` y `Math.Pow` están en `System`.
- La fórmula de Herón: `s = (a + b + c) / 2`, área = `√(s(s-a)(s-b)(s-c))`.

Intenta resolverlo antes de mirar la solución. ¡Ánimo!

---

## 9. Resumen

En esta lección has aprendido los cuatro pilares de la POO en C#:

| Concepto | Definición | Keyword en C# |
|---|---|---|
| **Clase** | Plantilla que agrupa datos (campos, propiedades) y comportamiento (métodos) | `class` |
| **Herencia** | Relación "es un" donde una clase derivada reutiliza y extiende una clase base | `: BaseClass` |
| **Interfaz** | Contrato que declara métodos/propiedades sin implementarlos. Una clase puede implementar varias | `interface`, `: IInterfaz` |
| **Polimorfismo** | Capacidad de tratar objetos de tipos distintos a través de una referencia común, ejecutando el comportamiento del tipo real | `virtual` + `override` |

### Tabla resumen de keywords

| Keyword | Significado |
|---|---|
| `class` | Declara una clase (tipo por referencia) |
| `interface` | Declara una interfaz (contrato sin implementación) |
| `virtual` | Permite que un método/propiedad sea sobrescrito en clases derivadas. Marca un punto de extensión. |
| `override` | Sobrescribe (reemplaza) un método virtual de la clase base |
| `abstract` | Método sin implementación o clase que no se puede instanciar. Las subclases DEBEN implementar. |
| `base` | Accede a miembros de la clase base (constructor, métodos, propiedades) |
| `sealed` | Impide que una clase sea heredada o que un método sea sobrescrito de nuevo |
| `is` | Operador de pattern matching: comprueba tipo y convierte en un solo paso |
| `as` | Intenta conversión de tipo; devuelve `null` si falla (solo tipos por referencia) |
| `new` (en miembro) | Oculta un miembro de la clase base. NO sobrescribe. Evítalo salvo casos muy concretos. |

### Conceptos importantes

- En C# los métodos **no son virtuales por defecto** (diferencia clave con Java). Esto favorece el rendimiento y hace explícita la intención de diseño.
- La herencia de clases es **simple**: una clase solo puede heredar de una clase base.
- C# permite **herencia múltiple de tipo** mediante la implementación de múltiples interfaces.
- El **virtual dispatch** se resuelve en tiempo de ejecución consultando la vtable del tipo real del objeto.
- Prefiere **composición sobre herencia** cuando la relación no sea claramente "es un".
- Usa **pattern matching** (`is`, `switch`) para downcasting seguro y expresivo.

---

## 10. Preguntas frecuentes

### ¿Cuándo usar clase abstracta vs interfaz?

| Usa clase abstracta cuando... | Usa interfaz cuando... |
|---|---|
| Las clases comparten implementación parcial | Solo quieres definir un contrato (sin código común) |
| Quieres proporcionar métodos con implementación por defecto | Quieres que clases sin relación de herencia compartan un contrato |
| Los tipos derivados tienen una relación "es un" fuerte | Necesitas herencia múltiple de tipo |
| Quieres declarar campos o constructores protegidos | Quieres máxima flexibilidad y desacoplamiento |

Regla práctica: **prefiere interfaces**. Las clases abstractas acoplan más. Si necesitas compartir implementación, usa composición + interfaz antes que herencia.

### ¿Por qué C# no permite herencia múltiple de clases?

Para evitar el **"diamond problem"** (problema del diamante): si una clase D hereda de B y C, y ambas heredan de A, ¿cuál implementación de un método virtual de A debe usar D? Esta ambigüedad causa problemas de estado, constructores y dispatch.

```
        A
       / \
      B   C
       \ /
        D    ← ¿Qué versión de A usa D? ¿La de B o la de C?
```

C# resuelve esto permitiendo herencia múltiple solo a nivel de **tipo** (interfaces), no de **implementación** (clases). Las interfaces no contienen estado ni implementación, así que no hay ambigüedad posible. Esta es la misma decisión que tomó Java.

### ¿Qué es exactamente el "diamond problem"?

Es la ambigüedad que surge en herencia múltiple de clases. Por ejemplo:
- La clase `A` tiene el método virtual `DoWork()`.
- `B` y `C` sobrescriben `DoWork()` de forma distinta.
- `D` hereda de `B` y `C` (diamante).
- Cuando se llama a `d.DoWork()`, ¿se ejecuta `B.DoWork()` o `C.DoWork()`? ¿Y qué pasa si `D` también lo sobrescribe?

C++ permite herencia múltiple y resuelve esto con herencia virtual (`virtual` en la declaración de herencia), pero introduce complejidad significativa. C# y Java optaron por prohibir la herencia múltiple de clases y ofrecer interfaces como alternativa segura.

### ¿Se puede hacer override de un método no virtual?

**No.** Si un método no está marcado como `virtual` en la clase base, no puedes usar `override` en la clase derivada. El compilador emitirá el error CS0506.

Lo que SÍ puedes hacer es ocultarlo con `new`:

```csharp
class Base
{
    public void DoWork() => Console.WriteLine("Base");
}

class Derived : Base
{
    public new void DoWork() => Console.WriteLine("Derived"); // Oculta, no sobrescribe
}
```

Pero **`new` no es override**. El comportamiento depende del tipo de la variable:

```csharp
Derived d = new Derived();
d.DoWork(); // "Derived"

Base b = d;
b.DoWork(); // "Base" ← ¡Aquí está la trampa! No hay virtual dispatch.
```

En el 99% de los casos, usar `new` para ocultar métodos es un error de diseño. Si necesitas cambiar el comportamiento de un método de la clase base y la clase base no lo declaró `virtual`, probablemente la clase base no fue diseñada para ser extendida de esa forma. Considera composición en su lugar.
