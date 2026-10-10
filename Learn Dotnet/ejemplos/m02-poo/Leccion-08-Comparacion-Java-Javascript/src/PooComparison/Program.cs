// =============================================================================
// Lección 08 — Comparación de POO entre C#, Java y JavaScript/TypeScript
// Módulo 2: POO en C#
// =============================================================================
// Esta lección muestra conceptos clave de POO en C# y los compara (vía
// comentarios) con sus equivalentes en Java y JavaScript/TypeScript.
// Solo se ejecuta código C#; las comparaciones están en comentarios.
// =============================================================================

using System.Globalization;

// =============================================================================
// SECCIÓN 0 — SEPARADOR VISUAL
// =============================================================================
Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
Console.WriteLine("║  COMPARACIÓN DE POO: C# vs Java vs JavaScript/TS        ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
Console.WriteLine();

// =============================================================================
// SECCIÓN 1 — CLASES
// =============================================================================
// En C# las propiedades (auto-implemented properties) generan un backing
// field privado automáticamente. No necesitas escribir getters/setters a mano.
//
// Java equivalente:
//   public class Customer {
//       private string name;
//       private string email;
//       private decimal creditLimit;
//
//       public Customer(string name, string email, decimal creditLimit) {
//           this.name = name;
//           this.email = email;
//           this.creditLimit = creditLimit;
//       }
//
//       public string getName() { return name; }
//       public void setName(string name) { this.name = name; }
//       public string getEmail() { return email; }
//       public void setEmail(string email) { this.email = email; }
//       public decimal getCreditLimit() { return creditLimit; }
//       public void setCreditLimit(decimal creditLimit) { this.creditLimit = creditLimit; }
//
//       public string getSummary() {
//           return name + " <" + email + "> — Límite: " + creditLimit;
//       }
//   }
//   ¡Más de 20 líneas extra solo por los getters/setters!
//
// TypeScript equivalente:
//   class Customer {
//       constructor(
//           public name: string,
//           public email: string,
//           public creditLimit: number
//       ) {}
//
//       getSummary(): string {
//           return `${this.name} <${this.email}> — Límite: ${this.creditLimit}`;
//       }
//   }
//   TypeScript tiene sintaxis de parámetros de constructor con modificadores
//   de acceso, similar a los primary constructors de C# 12.
// =============================================================================
Console.WriteLine("─── 1. CLASES ───");

Customer customer = new Customer("María García", "maria@email.com", 5000m);
Console.WriteLine(customer.GetSummary());

// Demostramos que las propiedades tienen get/set implícito
customer.CreditLimit = 7500m; // set implícito
Console.WriteLine($"Nuevo límite de crédito: {customer.CreditLimit:C}"); // get implícito
Console.WriteLine();

// =============================================================================
// SECCIÓN 2 — HERENCIA
// =============================================================================
// En los tres lenguajes la sintaxis es muy similar (C# usa `:`, Java y TS
// usan `extends`). La diferencia clave está en la cadena de prototipos de
// JavaScript (aunque la sintaxis `class` de ES6 la oculta).
//
// Java (idéntico concepto, distinta sintaxis):
//   public class Person { ... }
//   public class Employee extends Person { ... }
//   public class Manager extends Employee { ... }
//   // En Java todos los métodos son virtuales por defecto.
//   // En C# debes marcar explícitamente con `virtual`.
//
// TypeScript:
//   class Person { ... }
//   class Employee extends Person { ... }
//   class Manager extends Employee { ... }
//
// JavaScript (ES5, sin `class`):
//   function Person(name, age) {
//       this.name = name;
//       this.age = age;
//   }
//   Person.prototype.Introduce = function() {
//       return "Soy " + this.name;
//   };
//
//   function Employee(name, age, salary) {
//       Person.call(this, name, age); // llamar al "constructor" padre
//       this.salary = salary;
//   }
//   Employee.prototype = Object.create(Person.prototype); // cadena de prototipos
//   Employee.prototype.constructor = Employee;
//
//   function Manager(name, age, salary, department) {
//       Employee.call(this, name, age, salary);
//       this.department = department;
//   }
//   Manager.prototype = Object.create(Employee.prototype);
//   Manager.prototype.constructor = Manager;
//
//   ¡Esto es lo que la sintaxis `class` de ES6 oculta! La herencia en JS
//   está basada en prototipos, no en clases reales.
// =============================================================================
Console.WriteLine("─── 2. HERENCIA ───");

Person person = new Person("Carlos López", 45);
Console.WriteLine(person.Introduce());

Employee employee = new Employee("Ana Martínez", 32, 45000m, "IT");
Console.WriteLine(employee.Introduce());

Manager manager = new Manager("Luis Fernández", 50, 80000m, "Ventas", 12);
Console.WriteLine(manager.Introduce());
Console.WriteLine();

// =============================================================================
// SECCIÓN 3 — INTERFACES
// =============================================================================
// Las interfaces de C# y Java son prácticamente idénticas en concepto.
// TypeScript también las tiene, pero desaparecen al compilar a JavaScript
// (son solo para el compilador, no existen en runtime).
//
// Java:
//   public interface IIdentifiable {
//       UUID getId();           // método getter, no propiedad
//       void setId(UUID id);    // método setter
//   }
//   public interface IAuditable {
//       LocalDateTime getCreatedAt();
//       LocalDateTime getUpdatedAt();
//   }
//   public class Product implements IIdentifiable, IAuditable {
//       private UUID id;
//       private LocalDateTime createdAt;
//       private LocalDateTime updatedAt;
//       private string name;
//       // ... getters y setters para TODO
//   }
//
// TypeScript:
//   interface IIdentifiable {
//       id: string;
//       createdAt: Date;
//       updatedAt?: Date; // opcional con ?
//   }
//   // TypeScript usa "structural typing" (duck typing): si un objeto tiene
//   // las mismas propiedades que la interfaz, la cumple automáticamente.
//   // En C#/Java necesitas declarar explícitamente `: IIdentifiable`.
//
// JavaScript puro: no tiene interfaces. Se usa "duck typing":
//   "Si camina como pato y suena como pato, es un pato."
//   Simplemente compruebas que el objeto tenga las propiedades esperadas.
// =============================================================================
Console.WriteLine("─── 3. INTERFACES ───");

Product product = new Product("Laptop Gaming", 1299.99m);
Console.WriteLine($"Producto: {product.Name} — Precio: {product.Price:C}");
Console.WriteLine($"  Id: {product.Id}");
Console.WriteLine($"  Creado: {product.CreatedAt:yyyy-MM-dd HH:mm:ss}");
Console.WriteLine($"  Actualizado: {product.UpdatedAt:yyyy-MM-dd HH:mm:ss}");
Console.WriteLine();

// =============================================================================
// SECCIÓN 4 — POLIMORFISMO (Virtual Dispatch)
// =============================================================================
// Diferencia CLAVE entre C# y Java:
//   - En Java, TODOS los métodos son virtuales por defecto.
//   - En C#, los métodos NO son virtuales por defecto. Necesitas `virtual`
//     en la clase base y `override` en la derivada.
//   - Esta decisión de diseño de C# favorece el rendimiento (evita la
//     indirección de la vtable cuando no es necesaria) y la seguridad
//     (una clase derivada no puede cambiar accidentalmente el comportamiento
//     de la base).
//
// Java equivalente (todo virtual implícito):
//   abstract class Shape {
//       public abstract double calculateArea(); // abstract = obligatorio override
//   }
//   class Circle extends Shape {
//       private double radius;
//       @Override  // anotación opcional en Java (recomendada)
//       public double calculateArea() {
//           return Math.PI * radius * radius;
//       }
//   }
//
// JavaScript: el dynamic dispatch funciona a través de la cadena de prototipos.
// No hay verificación de tipos en tiempo de compilación.
//   const shapes = [new Circle(5), new Rectangle(4, 6)];
//   shapes.forEach(s => console.log(s.calculateArea()));
//   // JS busca el método en el objeto, luego en su prototipo, luego en el
//   // prototipo del prototipo... hasta encontrarlo o llegar a null.
// =============================================================================
Console.WriteLine("─── 4. POLIMORFISMO ───");

List<Shape> shapes = new List<Shape>
{
    new Circle(5.0),
    new Rectangle(4.0, 6.0),
    new Triangle(3.0, 4.0, 5.0),
    new Circle(2.5)
};

foreach (Shape shape in shapes)
{
    string typeName = shape.GetType().Name;
    double area = shape.CalculateArea();
    Console.WriteLine($"  {typeName}: Área = {area:F2}");
}
Console.WriteLine();

// =============================================================================
// SECCIÓN 5 — GENÉRICOS
// =============================================================================
// DIFERENCIA FUNDAMENTAL: C# tiene genéricos reificados, Java tiene type erasure.
//
// Type erasure (Java): el compilador borra la información de tipo genérico.
//   List<Integer> numbers = new ArrayList<>();
//   List<String> names = new ArrayList<>();
//   // En runtime: AMBAS son ArrayList<Object>. No puedes distinguirlas.
//   // numbers.getClass() == names.getClass() → TRUE (triste pero cierto)
//   // No puedes hacer `new T()` dentro de una clase genérica.
//   // No puedes sobrecargar métodos que difieran solo en parámetros genéricos.
//
// Reification (C#): el tipo genérico se conserva en runtime.
//   List<int> numbers = new List<int>();
//   List<string> names = new List<string>();
//   // En runtime: Son tipos DISTINTOS. Puedes inspeccionarlos con typeof.
//   // numbers.GetType() != names.GetType() → TRUE
//   // typeof(List<int>) != typeof(List<string>) → TRUE
//
// Ventajas de reification en C#:
//   - Puedes usar `typeof(T)` dentro de un método genérico.
//   - `default(T)` devuelve null para reference types, 0 para value types.
//   - Puedes crear arrays de tipos genéricos sin warnings.
//   - Mejor rendimiento: no hay boxing/unboxing para value types.
// =============================================================================
Console.WriteLine("─── 5. GENÉRICOS ───");

Repository<string> stringRepo = new Repository<string>();
stringRepo.Add("Clean Architecture");
stringRepo.Add("Domain-Driven Design");
stringRepo.Add("CQRS Pattern");

Console.WriteLine($"Tipo del repositorio: {stringRepo.GetStoredType()}");
Console.WriteLine($"Número de elementos: {stringRepo.Count()}");
List<string> all = stringRepo.GetAll();
Console.WriteLine($"Elementos: {string.Join(", ", all)}");

// Demostración de reification: puedes ver el tipo en runtime
Repository<int> intRepo = new Repository<int>();
intRepo.Add(10);
intRepo.Add(20);
Console.WriteLine($"Tipo del repositorio int: {intRepo.GetStoredType()}");
Console.WriteLine();

// =============================================================================
// SECCIÓN 6 — DELEGADOS Y EVENTOS (Exclusivo de C#)
// =============================================================================
// Los delegados son tipos que representan referencias a métodos con una
// firma específica. Son la base de los eventos, LINQ y async/await.
//
// NO existe equivalente directo en Java ni JavaScript:
//
// Java: usa "interfaces funcionales" (SAM — Single Abstract Method).
//   @FunctionalInterface
//   interface PriceChangedHandler {
//       void onPriceChanged(Product product, decimal oldPrice, decimal newPrice);
//   }
//   // Luego se implementa con clases anónimas o lambdas (Java 8+):
//   PriceChangedHandler handler = (p, oldP, newP) -> {
//       System.out.println("Precio cambiado");
//   };
//
// JavaScript: usa callbacks (funciones pasadas como argumento).
//   function onPriceChanged(product, oldPrice, newPrice) {
//       console.log("Precio cambiado");
//   }
//   // En Node.js se usa EventEmitter para múltiples suscriptores.
//   // En el navegador, addEventListener para eventos del DOM.
//
// C#: los delegados son tipos de primera clase. Pueden ser multicast
// (múltiples métodos en un solo delegado) gracias a `+=` y `-=`.
// Los eventos encapsulan delegados y restringen quién puede invocarlos.
// =============================================================================
Console.WriteLine("─── 6. DELEGADOS Y EVENTOS ───");

PriceTracker tracker = new PriceTracker("Monitor 4K", 500m);

// Suscribir manejadores al evento (multicast delegate)
tracker.PriceChanged += (sender, args) =>
{
    Console.WriteLine(
        $"  [LOG] {args.ProductName}: " +
        $"{args.OldPrice:C} → {args.NewPrice:C} " +
        $"(diferencia: {args.Difference:C})"
    );
};

tracker.PriceChanged += (sender, args) =>
{
    if (args.Difference > 50m)
    {
        Console.WriteLine($"  [ALERTA] ¡Cambio significativo de precio en {args.ProductName}!");
    }
};

// Disparar el evento cambiando el precio
tracker.UpdatePrice(550m);  // +50 → alerta
tracker.UpdatePrice(520m);  // -30 → sin alerta
tracker.UpdatePrice(650m);  // +130 → alerta
Console.WriteLine();

// =============================================================================
// SECCIÓN 7 — STRUCTS (Exclusivo de C#)
// =============================================================================
// Los structs son VALUE TYPES que se almacenan en el stack (normalmente).
// Las clases son REFERENCE TYPES que se almacenan en el heap.
//
// Java: NO tiene structs. Todo son clases (reference types) excepto los 8
//   tipos primitivos (int, long, double, boolean, char, byte, short, float).
//   Los primitivos viven en el stack, pero no puedes definir tus propios
//   value types. Esto es una limitación importante.
//
// JavaScript: NO tiene distinción. Todos los objetos van al heap. Los
//   primitivos (number, string, boolean, null, undefined, symbol, bigint)
//   son inmutables y se pasan por valor, pero no son "structs".
//   TypeScript no añade value types personalizados.
//
// Importancia de los structs en C#:
//   - Asignación copia el valor completo (no la referencia).
//   - Pasar un struct a un método crea una copia (a menos que uses `ref`).
//   - Ideal para tipos pequeños e inmutables (Point, Color, DateTime).
//   - No pueden heredar (son sealed implícitamente).
//   - Tienen constructor por defecto que inicializa todo a cero (C# 10+).
//   - Menor presión sobre el Garbage Collector.
// =============================================================================
Console.WriteLine("─── 7. STRUCTS (Exclusivo C#) ───");

Point p1 = new Point(3, 4);
Point p2 = p1; // ¡COPIA! No es una referencia
p2 = new Point(10, p1.Y); // Creamos una copia modificada con el constructor

Console.WriteLine($"  Point p1: ({p1.X}, {p1.Y}) — distancia desde origen: {p1.DistanceFromOrigin():F2}");
Console.WriteLine($"  Point p2: ({p2.X}, {p2.Y}) — distancia desde origen: {p2.DistanceFromOrigin():F2}");
Console.WriteLine($"  p1.Equals(p2): {p1.Equals(p2)}"); // false, son valores distintos

// Comparativa: class vs struct
PointClass pc1 = new PointClass(3, 4);
PointClass pc2 = pc1; // ¡REFERENCIA! Ambas variables apuntan al mismo objeto
pc2.X = 10;
Console.WriteLine($"  PointClass: pc1.X = {pc1.X}, pc2.X = {pc2.X}"); // Ambos 10
Console.WriteLine($"  Con struct esto no pasa: la copia es independiente.");
Console.WriteLine();

// =============================================================================
// SECCIÓN 8 — PROPIEDADES (Sintaxis exclusiva de C#)
// =============================================================================
// Las propiedades de C# son azúcar sintáctico sobre get/set que el compilador
// traduce a métodos get_X() y set_X(value). Ofrecen tres variantes:
//
// 1. Auto-implemented: { get; set; }
//    El compilador genera el backing field privado automáticamente.
//
// 2. Expression-bodied: get => field; set => field = value;
//    Para lógica simple en una línea.
//
// 3. Full property con backing field explícito:
//    Para cuando necesitas validación, notificación de cambios, etc.
//
// 4. Init-only (C# 9+): { get; init; }
//    Solo se puede asignar en el constructor o inicializador de objeto.
//    Útil para tipos inmutables.
//
// Java: NO tiene propiedades. Los IDEs generan getters/setters automáticamente
//   (Alt+Insert en IntelliJ, Source Action en Eclipse). Lombok genera el
//   código en tiempo de compilación con anotaciones @Data, @Getter, @Setter.
//
//   @Data  // equivalente a @Getter @Setter @RequiredArgsConstructor
//          // @ToString @EqualsAndHashCode
//   public class Person {
//       private String name;
//       private int age;
//   }
//   // Lombok genera getName(), setName(), getAge(), setAge()...
//   // Pero sigue siendo un workaround, no una característica del lenguaje.
//
// JavaScript/TypeScript:
//   - JS clásico: acceso directo a campos (this.name = ...). Sin encapsulación.
//   - JS ES5+: get name() { ... }, set name(v) { ... } — getters/setters nativos.
//   - TypeScript: igual que JS, pero con verificación de tipos.
//   class Person {
//       private _name: string;
//       get name(): string { return this._name; }
//       set name(v: string) { this._name = v; }
//   }
// =============================================================================
Console.WriteLine("─── 8. PROPIEDADES ───");

PropertyDemo demo = new PropertyDemo
{
    AutoProperty = "Auto-implementada (backing field automático)",
    InitOnlyProperty = "Solo puedo asignarme aquí, soy inmutable tras construir"
};
Console.WriteLine($"  Auto: {demo.AutoProperty}");
Console.WriteLine($"  Init-only: {demo.InitOnlyProperty}");

demo.ManualProperty = "Validado por setter manual";
Console.WriteLine($"  Manual (con validación): {demo.ManualProperty}");
Console.WriteLine($"  Computed (solo get): {demo.ComputedProperty}");

// demo.InitOnlyProperty = "otro valor"; // Error de compilación: init-only
Console.WriteLine();

// =============================================================================
// SECCIÓN 9 — TABLA COMPARATIVA
// =============================================================================
Console.WriteLine("─── 9. TABLA COMPARATIVA ───");
Console.WriteLine(
"""
Característica              C#                     Java                   JavaScript / TS
─────────────────────────── ────────────────────── ────────────────────── ──────────────────────────
Clases                      class                  class                  class (ES6+) / class (TS)
Herencia                    : BaseClass            extends                extends
Interfaces                  interface              interface              interface (TS, no en JS)
Propiedades                 { get; set; }          getX() / setX()        campos o get/set (ES5+)
Virtual por defecto         NO (explicit)          SÍ                     SÍ (cadena de prototipos)
Genéricos                   Reificados             Type Erasure           Erased (TS), N/A en JS
Structs (value types)       SÍ                     NO                     NO
Delegados / Eventos         SÍ                     Listener interfaces    Callbacks / EventEmitter
Extension Methods           SÍ                     NO                     prototype (JS)
Operator Overloading        SÍ                     NO                     NO
ref / out / in params       SÍ                     NO                     NO
Nullable value types        SÍ (int?)              NO (Integer wrapper)   SÍ (? en TS)
Destructor / Finalizer      ~Finalizer()           finalize()             NO (solo GC)
Métodos parciales           SÍ                     NO                     NO
Indexadores                 SÍ (this[])            NO                     NO
LINQ                        SÍ                     Stream API (similar)   map/filter/reduce
async / await               Task<T>                CompletableFuture<T>   Promise<T>
Record types                SÍ (C# 9+)             Record (Java 14+)      NO nativo
Pattern Matching            SÍ (avanzado)          SÍ (básico, Java 16+)  NO (TS: union types)
Primary Constructors        SÍ (C# 12)             NO                     NO (TS: param properties)
"""
);
Console.WriteLine();

// =============================================================================
// SECCIÓN 10 — RESUMEN Y CONCLUSIÓN
// =============================================================================
Console.WriteLine("─── 10. RESUMEN ───");
Console.WriteLine(
"""
C# ofrece el sistema de tipos más rico de los tres lenguajes:
  • Structs para value types personalizados (evita presión del GC).
  • Genéricos reificados (más rendimiento, introspección en runtime).
  • Propiedades como ciudadanos de primera clase (no getters/setters manuales).
  • Delegados y eventos integrados en el lenguaje (no listeners).
  • LINQ para consultas declarativas sobre cualquier colección.

Java destaca por:
  • Ecosistema masivo y madurez (Spring, Hibernate, etc.).
  • Multiplataforma real (JVM en cualquier SO).
  • Herramientas de build muy potentes (Maven, Gradle).

JavaScript/TypeScript destaca por:
  • Flexibilidad extrema (duck typing, prototipos).
  • Mismo lenguaje en frontend y backend (Node.js).
  • TypeScript añade seguridad de tipos sin perder la flexibilidad de JS.
"""
);
Console.WriteLine();

// =============================================================================
// TIPOS DEFINIDOS A CONTINUACIÓN
// =============================================================================
// En C# con top-level statements, los tipos se pueden definir en cualquier
// lugar del archivo. Por convención, los ponemos al final para que el flujo
// de ejecución (el código de arriba) sea lo primero que se lee.
// =============================================================================
return; // El return evita que el compilador genere código después de los tipos

// =============================================================================
// CLASES PARA SECCIÓN 1 — CLASES
// =============================================================================

/// <summary>
/// Representa un cliente del sistema.
/// En C# usamos propiedades auto-implementadas: el compilador genera el
/// backing field privado, el getter y el setter automáticamente.
/// En Java necesitarías ~25 líneas extra para los getters/setters.
/// En TypeScript puedes usar parámetros de constructor con modificadores
/// de acceso (similar a primary constructors de C# 12).
/// </summary>
public class Customer
{
    // Propiedad auto-implementada: backing field + get + set generados
    public string Name { get; set; }

    // Propiedad auto-implementada
    public string Email { get; set; }

    // Propiedad auto-implementada con valor por defecto
    public decimal CreditLimit { get; set; }

    // Constructor tradicional (no usamos primary constructor aquí para
    // que el ejemplo sea más claro en la comparación con Java)
    public Customer(string name, string email, decimal creditLimit)
    {
        Name = name;
        Email = email;
        CreditLimit = creditLimit;
    }

    // Método que usa las propiedades (no getters como en Java)
    public string GetSummary()
    {
        // string interpolation ($"...") es más legible que concatenación
        return $"{Name} <{Email}> — Límite de crédito: {CreditLimit:C}";
    }
}

// =============================================================================
// CLASES PARA SECCIÓN 2 — HERENCIA
// =============================================================================

/// <summary>
/// Clase base para todas las personas del sistema.
/// C# usa `:` para herencia (Java y TS usan `extends`).
/// </summary>
public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }

    public Person(string name, int age)
    {
        Name = name;
        Age = age;
    }

    // `virtual` permite que las clases derivadas sobrescriban este método.
    // En Java esto NO es necesario: todos los métodos son virtuales.
    // En C# es una decisión de diseño consciente: mejor rendimiento y
    // evita que una clase derivada rompa el contrato accidentalmente.
    public virtual string Introduce()
    {
        return $"Soy {Name}, tengo {Age} años.";
    }
}

/// <summary>
/// Empleado hereda de Persona.
/// En Java: public class Employee extends Person { ... }
/// En TypeScript: class Employee extends Person { ... }
/// La sintaxis es prácticamente idéntica en los tres.
/// </summary>
public class Employee : Person
{
    public decimal Salary { get; set; }
    public string Department { get; set; }

    public Employee(string name, int age, decimal salary, string department)
        : base(name, age) // llama al constructor de Person
    {
        Salary = salary;
        Department = department;
    }

    // `override` es obligatorio en C# para sobrescribir un método virtual.
    // En Java usas @Override (anotación opcional pero recomendada).
    // Si olvidas `override` en C#, el compilador te avisa con un warning
    // de que estás ocultando el método base (uso de `new`).
    public override string Introduce()
    {
        return $"{base.Introduce()} Trabajo en {Department}, salario: {Salary:C}.";
    }
}

/// <summary>
/// Manager hereda de Employee (demostrando cadena de herencia multinivel).
/// En Java/TS: class Manager extends Employee { ... }
/// </summary>
public class Manager : Employee
{
    public int TeamSize { get; set; }

    public Manager(string name, int age, decimal salary, string department, int teamSize)
        : base(name, age, salary, department)
    {
        TeamSize = teamSize;
    }

    public override string Introduce()
    {
        return $"{base.Introduce()} Gestiono un equipo de {TeamSize} personas.";
    }
}

// =============================================================================
// INTERFACES PARA SECCIÓN 3 — INTERFACES
// =============================================================================

/// <summary>
/// Interfaz para entidades que tienen un identificador único.
/// En Java: public interface IIdentifiable { UUID getId(); void setId(UUID id); }
/// En TypeScript: interface IIdentifiable { id: string; }
/// Diferencia clave: C# permite propiedades en interfaces.
/// Java usa métodos get/set. TypeScript solo verifica la forma (structural typing).
/// </summary>
public interface IIdentifiable
{
    Guid Id { get; }
}

/// <summary>
/// Interfaz para entidades auditables (marcas de tiempo).
/// Demuestra que una interfaz puede tener múltiples propiedades.
/// </summary>
public interface IAuditable
{
    DateTime CreatedAt { get; }
    DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Product implementa DOS interfaces. En C# la herencia múltiple de
/// implementación no existe, pero la herencia múltiple de INTERFACES sí.
/// Esto es igual en Java y TypeScript.
/// Java: class Product implements IIdentifiable, IAuditable { ... }
/// TypeScript: class Product implements IIdentifiable, IAuditable { ... }
/// </summary>
public class Product : IIdentifiable, IAuditable
{
    // Implementación de IIdentifiable
    public Guid Id { get; }

    // Implementación de IAuditable
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; set; }

    // Propiedades propias de Product
    public string Name { get; set; }
    public decimal Price { get; set; }

    public Product(string name, decimal price)
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        Name = name;
        Price = price;
    }
}

// =============================================================================
// CLASES PARA SECCIÓN 4 — POLIMORFISMO
// =============================================================================

/// <summary>
/// Clase base abstracta para figuras geométricas.
/// `abstract` impide instanciar Shape directamente.
/// En Java: public abstract class Shape { public abstract double calculateArea(); }
/// En TypeScript: abstract class Shape { abstract calculateArea(): number; }
/// </summary>
public abstract class Shape
{
    public string Name { get; protected set; } = "Shape";

    // Método abstracto: las clases derivadas DEBEN implementarlo.
    // En Java también se usa `abstract`. En TS también.
    public abstract double CalculateArea();

    // Método virtual (con implementación por defecto) que las derivadas
    // PUEDEN sobrescribir. No es obligatorio.
    public virtual string Describe()
    {
        return $"Soy un {Name} con área {CalculateArea():F2}";
    }
}

public class Circle : Shape
{
    public double Radius { get; set; }

    public Circle(double radius)
    {
        Name = "Círculo";
        Radius = radius;
    }

    public override double CalculateArea()
    {
        return Math.PI * Radius * Radius;
    }
}

public class Rectangle : Shape
{
    public double Width { get; set; }
    public double Height { get; set; }

    public Rectangle(double width, double height)
    {
        Name = "Rectángulo";
        Width = width;
        Height = height;
    }

    public override double CalculateArea()
    {
        return Width * Height;
    }
}

public class Triangle : Shape
{
    public double SideA { get; set; }
    public double SideB { get; set; }
    public double SideC { get; set; }

    public Triangle(double sideA, double sideB, double sideC)
    {
        Name = "Triángulo";
        SideA = sideA;
        SideB = sideB;
        SideC = sideC;

        // Verificación básica de desigualdad triangular
        if (sideA + sideB <= sideC || sideA + sideC <= sideB || sideB + sideC <= sideA)
        {
            throw new ArgumentException("Los lados no forman un triángulo válido.");
        }
    }

    public override double CalculateArea()
    {
        // Fórmula de Herón para el área de un triángulo dados sus lados
        double semiperimeter = (SideA + SideB + SideC) / 2.0;
        return Math.Sqrt(
            semiperimeter *
            (semiperimeter - SideA) *
            (semiperimeter - SideB) *
            (semiperimeter - SideC)
        );
    }
}

// =============================================================================
// CLASE PARA SECCIÓN 5 — GENÉRICOS
// =============================================================================

/// <summary>
/// Repositorio genérico que almacena elementos de tipo T.
/// Demuestra genéricos reificados de C#: typeof(T) funciona en runtime.
///
/// En Java (con type erasure) NO puedes hacer esto:
///   public class Repository<T> {
///       private List<T> items = new ArrayList<>();
///       // NO puedes hacer items.getClass() del tipo T
///       // NO puedes hacer T item = new T();
///       // Tienes que pasar Class<T> como parámetro al constructor.
///   }
///
/// En TypeScript los genéricos también se borran al compilar a JS,
/// pero puedes usar el decorador @Reflect.metadata o pasar el tipo.
/// </summary>
public class Repository<T>
{
    private readonly List<T> _items = new List<T>();

    public void Add(T item)
    {
        _items.Add(item);
    }

    public List<T> GetAll()
    {
        return new List<T>(_items);
    }

    public int Count()
    {
        return _items.Count;
    }

    // Esto NO es posible en Java (type erasure impide typeof(T)).
    // En C# es perfectamente válido gracias a la reificación.
    public string GetStoredType()
    {
        // typeof(T) devuelve el tipo REAL en runtime.
        // En Java no existe equivalente: T se borra a Object.
        Type type = typeof(T);

        // Si T es un value type (int, double, struct), lo muestra.
        // Si T es un reference type (string, Customer), también.
        return type.Name;
    }
}

// =============================================================================
// CLASES PARA SECCIÓN 6 — DELEGADOS Y EVENTOS
// =============================================================================

/// <summary>
/// Delegado: tipo que representa una referencia a un método.
/// Define la firma que el método debe cumplir.
/// Equivalente en Java: @FunctionalInterface con SAM.
/// Equivalente en JS/TS: type PriceChangedHandler = (sender: object, args: PriceChangedEventArgs) => void;
/// </summary>
public delegate void PriceChangedEventHandler(object sender, PriceChangedEventArgs args);

/// <summary>
/// Argumentos personalizados para el evento de cambio de precio.
/// Contiene toda la información relevante sobre el cambio.
/// Hereda de EventArgs (convención de .NET).
/// </summary>
public class PriceChangedEventArgs : EventArgs
{
    public string ProductName { get; }
    public decimal OldPrice { get; }
    public decimal NewPrice { get; }
    public decimal Difference => NewPrice - OldPrice;
    public DateTime ChangedAt { get; }

    public PriceChangedEventArgs(string productName, decimal oldPrice, decimal newPrice)
    {
        ProductName = productName;
        OldPrice = oldPrice;
        NewPrice = newPrice;
        ChangedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Rastreador de precios que dispara un evento cuando el precio cambia.
/// Los eventos encapsulan delegados:
///   - Solo la clase propietaria puede invocar el evento.
///   - Los suscriptores externos solo pueden añadir/quitar manejadores (+= / -=).
///   - Patrón observer integrado en el lenguaje.
///
/// Java equivalente: usar PropertyChangeListener o crear una interfaz listener.
///   public interface PriceChangeListener {
///       void onPriceChanged(PriceChangeEvent event);
///   }
///   private List<PriceChangeListener> listeners = new ArrayList<>();
///   public void addPriceChangeListener(PriceChangeListener l) { ... }
///   // Mucho más boilerplate que los eventos de C#.
///
/// JavaScript equivalente (Node.js):
///   const EventEmitter = require('events');
///   class PriceTracker extends EventEmitter {
///       updatePrice(newPrice) {
///           this.emit('priceChanged', { oldPrice, newPrice });
///       }
///   }
/// </summary>
public class PriceTracker
{
    private decimal _price;

    public string ProductName { get; }
    public decimal Price => _price;

    // `event` es una palabra clave de C#. El compilador genera código
    // para gestionar la lista de suscriptores de forma segura para hilos.
    // El tipo del evento es el delegado PriceChangedEventHandler.
    public event PriceChangedEventHandler? PriceChanged;

    public PriceTracker(string productName, decimal initialPrice)
    {
        ProductName = productName;
        _price = initialPrice;
    }

    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice == _price)
        {
            return; // No hay cambio, no se dispara el evento
        }

        decimal oldPrice = _price;
        _price = newPrice;

        // Invocar el evento: notificar a todos los suscriptores.
        // ?.Invoke es seguro: si no hay suscriptores (null), no hace nada.
        // Cada suscriptor recibe la notificación en orden de suscripción.
        PriceChanged?.Invoke(this, new PriceChangedEventArgs(ProductName, oldPrice, newPrice));
    }
}

// =============================================================================
// TIPOS PARA SECCIÓN 7 — STRUCTS
// =============================================================================

/// <summary>
/// Struct: tipo por valor. Se almacena en el stack (normalmente).
/// Al asignar, se copia el valor completo. No hay referencia compartida.
///
/// NO existe en Java ni en JavaScript.
/// En Java, los tipos primitivos (int, double, ...) se comportan como
/// value types, pero no puedes crear tus propios value types.
///
/// En JavaScript/TypeScript no existe el concepto de stack vs heap para
/// objetos del usuario. string y number se pasan por valor.
///
/// Cuándo usar struct en C#:
///   - Tipo pequeño (menos de 16 bytes recomendado).
///   - Representa un valor simple (Point, Color, Complex, Money).
///   - Comportamiento de "valor" (inmutable).
///   - No necesita herencia.
///   - Quieres evitar presión en el Garbage Collector.
/// </summary>
public readonly struct Point
{
    // Campos de solo lectura (el struct es inmutable)
    public double X { get; }
    public double Y { get; }

    public Point(double x, double y)
    {
        X = x;
        Y = y;
    }

    public double DistanceFromOrigin()
    {
        return Math.Sqrt(X * X + Y * Y);
    }

    // Sobrescribir ToString() para representación legible
    public override string ToString()
    {
        return $"({X}, {Y})";
    }

    // Sobrescribir Equals para comparación por valor (no por referencia)
    public override bool Equals(object? obj)
    {
        return obj is Point other && X == other.X && Y == other.Y;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    // Sobrecarga de operadores (otra característica que no existe en Java/JS)
    public static bool operator ==(Point left, Point right) => left.Equals(right);
    public static bool operator !=(Point left, Point right) => !left.Equals(right);
}

/// <summary>
/// Clase equivalente a Point, para demostrar la diferencia entre value types
/// (struct) y reference types (class).
/// </summary>
public class PointClass
{
    public double X { get; set; }
    public double Y { get; set; }

    public PointClass(double x, double y)
    {
        X = x;
        Y = y;
    }
}

// =============================================================================
// CLASE PARA SECCIÓN 8 — PROPIEDADES
// =============================================================================

/// <summary>
/// Demostración de los distintos tipos de propiedades en C#.
/// Java no tiene equivalente directo. Necesitas:
///   private String autoProperty;        // backing field manual
///   public String getAutoProperty() { ... }
///   public void setAutoProperty(String v) { ... }
///
/// Con Lombok (alternativa popular en Java):
///   @Data
///   public class PropertyDemo {
///       private String autoProperty;     // @Data genera getters/setters
///       private String initOnlyProperty; // No hay equivalente a init-only
///       private String manualProperty;   // @Data genera getters/setters
///   }
/// </summary>
public class PropertyDemo
{
    // 1. Auto-implemented: el compilador genera el backing field
    public string AutoProperty { get; set; } = string.Empty;

    // 2. Init-only: solo se puede asignar en el constructor o inicializador
    public string InitOnlyProperty { get; init; } = string.Empty;

    // 3. Propiedad con backing field explícito y validación en el setter
    private string _manualProperty = string.Empty;

    public string ManualProperty
    {
        get => _manualProperty;
        set
        {
            // Validación: no permitir valores nulos
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("El valor no puede ser nulo o vacío.");
            }
            _manualProperty = value;
        }
    }

    // 4. Propiedad de solo lectura (calculada)
    public string ComputedProperty => $"Auto={AutoProperty}, Init={InitOnlyProperty}";
}
