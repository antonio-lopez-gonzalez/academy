// ======================================================================
// Módulo 2 — POO en C# en profundidad
// Lección 01 — Clases, Herencia, Interfaces y Polimorfismo
// ======================================================================
// Este programa demuestra los 4 pilares de la Programación Orientada a
// Objetos en C#: encapsulación, herencia, polimorfismo e interfaces.
// Todo el código se ejecuta con `dotnet run` desde la carpeta del .csproj.
// ======================================================================

// ══════════════════════════════════════════════════════════════════════
// PROGRAMA PRINCIPAL (Top-Level Statements)
// ══════════════════════════════════════════════════════════════════════
// En .NET 6+, el punto de entrada puede ser código suelto en vez del
// clásico `static void Main(string[] args)`. El compilador genera la
// clase `Program` y el método `Main` automáticamente.
// Las declaraciones de tipos (clases, interfaces) van DESPUÉS de este bloque.
// ══════════════════════════════════════════════════════════════════════

// ─── 1. CREACIÓN DE OBJETOS ──────────────────────────────────────────
// Instanciamos objetos de cada tipo. Observa que usamos tipos explícitos
// (no `var`) para que quede claro qué tipo es cada variable.

Console.WriteLine("═══════════════════════════════════════════════");
Console.WriteLine("  1. CREACIÓN DE OBJETOS");
Console.WriteLine("═══════════════════════════════════════════════");

Employee ana = new Employee("Ana García", 30_000m);
Console.WriteLine(ana);
Console.WriteLine($"  Bonus de Ana: {ana.CalculateBonus():C}");

Manager carlos = new Manager("Carlos López", 60_000m, "IT");
Console.WriteLine(carlos);
Console.WriteLine($"  Bonus de Carlos: {carlos.CalculateBonus():C}");

Developer laura = new Developer(
    "Laura Martínez", 45_000m,
    new List<string> { "C#", "SQL Server", "Docker" });
Console.WriteLine(laura);
Console.WriteLine($"  Bonus de Laura: {laura.CalculateBonus():C}");

// ─── 2. POLIMORFISMO CON DISPATCH VIRTUAL ────────────────────────────
// Aquí está la magia del polimorfismo: tratamos objetos de distintos
// tipos a través de una referencia a la clase base (Employee), pero cada
// uno ejecuta SU PROPIA versión de CalculateBonus().
// Esto se llama "virtual dispatch" o "dynamic dispatch".
// El CLR en tiempo de ejecución mira el tipo REAL del objeto (no el de
// la variable) y llama al método correcto. Esto usa la "virtual method
// table" (vtable) que todo objeto con métodos virtuales tiene.

Console.WriteLine("\n═══════════════════════════════════════════════");
Console.WriteLine("  2. POLIMORFISMO — List<Employee>");
Console.WriteLine("═══════════════════════════════════════════════");

List<Employee> employees = new List<Employee>
{
    new Employee("Pedro Ruiz", 28_000m),           // Employee base
    new Manager("Sofía Díaz", 70_000m, "Marketing"), // Manager (hereda de Employee)
    new Developer("Diego Torres", 50_000m,            // Developer (hereda de Employee)
        new List<string> { "Python", "React", ".NET" }),
    new Developer("Elena Castro", 48_000m,
        new List<string> { "Java", "Spring Boot" }),  // Sin C# ni .NET → sin bonus extra
    new Manager("Roberto Gil", 80_000m, "Ventas")
};

Console.WriteLine("ID      Nombre               Salario     Bonus       Tipo real");
Console.WriteLine(new string('-', 75));

foreach (Employee emp in employees)
{
    // Aunque la variable `emp` es de tipo Employee, el método llamado
    // depende del tipo REAL del objeto (Manager, Developer, etc.)
    decimal bonus = emp.CalculateBonus();

    string tipoReal = emp.GetType().Name; // Reflection: nombre real de la clase

    Console.WriteLine(
        $"{emp.Id.ToString()[..8]}... " +
        $"{emp.Name,-20} " +
        $"{emp.Salary,10:C} " +
        $"{bonus,10:C} " +
        $"({tipoReal})");
}

// ─── 3. UPCASTING Y DOWNCASTING ──────────────────────────────────────
// Upcasting: convertir una referencia de tipo derivado a tipo base.
// Es SIEMPRE seguro e implícito (no necesita sintaxis especial).
//
// Downcasting: convertir de tipo base a tipo derivado.
// NO siempre es seguro → hay que verificar con `is` o `as`.

Console.WriteLine("\n═══════════════════════════════════════════════");
Console.WriteLine("  3. UPCASTING Y DOWNCASTING");
Console.WriteLine("═══════════════════════════════════════════════");

// Upcasting implícito — Manager "sube" a Employee automáticamente
Employee empleadoGenerico = carlos; // carlos es Manager, pero cabe en Employee
Console.WriteLine($"Upcasting: Manager → Employee → {empleadoGenerico.Name}");

// ── Downcasting con el operador `is` (pattern matching) ──────────────
// `is` comprueba el tipo Y declara una variable del tipo concreto en un
// solo paso. Es la forma moderna y recomendada de hacer downcasting.

Console.WriteLine("\n--- Downcasting con `is` (pattern matching) ---");
foreach (Employee emp in employees)
{
    // El operador `is` intenta hacer match del tipo. Si coincide,
    // declara la variable `m` o `d` con el tipo ya convertido.
    if (emp is Manager m)
    {
        Console.WriteLine($"  Manager detectado: {m.Name} → Depto: {m.Department}");
    }
    else if (emp is Developer d)
    {
        string techs = string.Join(", ", d.TechStack);
        Console.WriteLine($"  Developer detectado: {d.Name} → Stack: [{techs}]");
    }
    else
    {
        Console.WriteLine($"  Employee base: {emp.Name}");
    }
}

// ── Downcasting con el operador `as` ─────────────────────────────────
// `as` intenta la conversión. Si falla, devuelve `null` (con tipos por
// referencia). NO lanza excepción. Más parecido a Java o TypeScript.
// Después del `as` hay que comprobar null.

Console.WriteLine("\n--- Downcasting con `as` ---");
foreach (Employee emp in employees)
{
    Manager? posibleManager = emp as Manager;
    if (posibleManager is not null)
    {
        Console.WriteLine($"  {emp.Name} ES Manager (as no fue null). Depto: {posibleManager.Department}");
    }
    else
    {
        Console.WriteLine($"  {emp.Name} NO es Manager (as devolvió null)");
    }
}

// ── Casting directo (peligroso) ──────────────────────────────────────
// `(Manager)emp` lanza InvalidCastException si el tipo no coincide.
// Solo usarlo cuando estemos 100% seguros del tipo.
Console.WriteLine("\n--- Casting directo (solo seguro si ya verificamos) ---");
try
{
    // employees[1] es Manager, esto funciona:
    Manager seguro = (Manager)employees[1];
    Console.WriteLine($"  Casting exitoso: {seguro.Name} ({seguro.Department})");

    // employees[0] es Employee base, esto lanza excepción:
    Manager fallo = (Manager)employees[0];
}
catch (InvalidCastException ex)
{
    Console.WriteLine($"  ERROR de casting: {ex.Message}");
}

// ─── 4. SWITCH EXPRESSION SOBRE TIPOS ────────────────────────────────
// C# permite usar pattern matching en switch expressions para decidir
// qué hacer según el tipo real del objeto en tiempo de ejecución.

Console.WriteLine("\n═══════════════════════════════════════════════");
Console.WriteLine("  4. SWITCH EXPRESSION SOBRE TIPOS");
Console.WriteLine("═══════════════════════════════════════════════");

foreach (Employee emp in employees)
{
    // Este switch expression aplica un patrón de tipo para cada case.
    // Cada rama recibe la variable ya convertida al tipo concreto.
    string descripcion = emp switch
    {
        Manager m  => $"MANAGER   | {m.Name} lidera el departamento {m.Department}",
        Developer d => $"DEVELOPER | {d.Name} trabaja con {string.Join(", ", d.TechStack)}",
        Employee e  => $"EMPLOYEE  | {e.Name} — rol base sin especialización",
        _           => "TIPO DESCONOCIDO" // Teóricamente no se alcanza, pero cubre todos los casos
    };

    Console.WriteLine($"  {descripcion}");
}

// ─── 5. POLIMORFISMO DE INTERFACES ───────────────────────────────────
// Las interfaces son otro mecanismo de polimorfismo: podemos tratar
// objetos de clases distintas (incluso sin relación de herencia) de
// forma uniforme a través de una interfaz común.
// Aquí toda nuestra jerarquía implementa IPayable e IReportable.

Console.WriteLine("\n═══════════════════════════════════════════════");
Console.WriteLine("  5. POLIMORFISMO DE INTERFACES");
Console.WriteLine("═══════════════════════════════════════════════");

// ── Tratando objetos como IPayable ───────────────────────────────────
Console.WriteLine("\n--- Vista IPayable (pago mensual) ---");
List<IPayable> pagables = new List<IPayable>
{
    ana,
    carlos,
    laura,
    new Employee("Freelancer Externo", 36_000m) // No está en la lista de employees
};

foreach (IPayable pagable in pagables)
{
    // No sabemos si es Employee, Manager o Developer; solo nos importa
    // que implemente IPayable. GetMonthlyPay() se resuelve en cada clase.
    Console.WriteLine($"  Pago mensual: {pagable.GetMonthlyPay(),10:C}");
}

// ── Tratando objetos como IReportable ────────────────────────────────
Console.WriteLine("\n--- Vista IReportable (informes) ---");
foreach (Employee emp in employees)
{
    // Aunque iteramos como Employee, podemos ver cada objeto como
    // IReportable gracias a que Employee implementa la interfaz.
    IReportable reportable = emp; // Upcasting a interfaz (implícito)
    Console.WriteLine($"  {reportable.GenerateReport()}");
}

// ══════════════════════════════════════════════════════════════════════
// RESUMEN DE CONCEPTOS CLAVE
// ══════════════════════════════════════════════════════════════════════
Console.WriteLine("\n═══════════════════════════════════════════════");
Console.WriteLine("  RESUMEN DE CONCEPTOS CLAVE");
Console.WriteLine("═══════════════════════════════════════════════");
Console.WriteLine("""
    Clase:       Plantilla que agrupa datos (campos/propiedades) y
                 comportamiento (métodos). Employee es nuestra clase base.

    Herencia:    Una clase derivada (Manager, Developer) reutiliza y
                 extiende a la clase base (Employee) usando `: BaseClass`.
                 Usamos `base()` para invocar el constructor de la base.

    Polimorfismo: Una referencia de tipo base puede apuntar a objetos de
                  tipos derivados. Al llamar a un método `virtual`, se
                  ejecuta la versión del tipo REAL (virtual dispatch).

    Interfaz:    Un contrato que declara qué métodos/propiedades debe
                 tener una clase, sin implementarlos. Una clase puede
                 implementar MÚLTIPLES interfaces (a diferencia de la
                 herencia, que es simple en C#). IPayable e IReportable
                 son interfaces que Employee implementa.

    virtual:     Permite que un método sea SOBRESCRITO en clases derivadas.
                 Sin `virtual`, el método no puede sobreescribirse.

    override:    En la clase derivada, REEMPLAZA la implementación del
                 método virtual de la clase base.

    is:          Comprueba si un objeto es de un tipo Y lo convierte
                 (pattern matching). Forma moderna de downcasting.

    as:          Intenta convertir. Devuelve null si falla (tipos ref.).
                 No lanza excepción. Menos usado hoy; se prefiere `is`.
    """);

Console.WriteLine("\n✅ Fin de la demostración. ¡Todos los conceptos clave de POO en C#!");

// ══════════════════════════════════════════════════════════════════════
// DECLARACIONES DE TIPOS
// ══════════════════════════════════════════════════════════════════════
// A partir de aquí se declaran las interfaces y clases usadas arriba.
// Van DESPUÉS de los top-level statements.
// ══════════════════════════════════════════════════════════════════════

// ─── INTERFAZ: IPayable ──────────────────────────────────────────────
// Una interfaz es un CONTRATO. No contiene implementación (solo firmas).
// Las clases que la implementan están OBLIGADAS a proporcionar el
// método GetMonthlyPay(). Esto permite polimorfismo sin herencia común.
// Por convención, los nombres de interfaz empiezan con "I" en C#.
interface IPayable
{
    /// <summary>Calcula el pago mensual del ente pagable.</summary>
    decimal GetMonthlyPay();
}

// ─── INTERFAZ: IReportable ───────────────────────────────────────────
// Segunda interfaz para mostrar que una clase puede implementar VARIAS.
// A diferencia de la herencia (que es simple en C#), una clase puede
// implementar tantas interfaces como necesite.
interface IReportable
{
    /// <summary>Genera un informe en texto del objeto.</summary>
    string GenerateReport();
}

// ─── CLASE BASE: Employee ────────────────────────────────────────────
// Employee sella el "contrato" de IPayable y IReportable implementándolos.
// Es la clase base de nuestra jerarquía.
// `Employee : IPayable, IReportable` significa:
//   "Employee hereda de System.Object (implícito) e implementa 2 interfaces"
class Employee : IPayable, IReportable
{
    // ── CAMPOS PRIVADOS (encapsulación) ───────────────────────────────
    // `readonly`: solo se asigna en el constructor. Una vez creado el
    // objeto, el Id no puede cambiar. Esto es inmutabilidad a nivel de campo.
    private readonly Guid _id;

    // `_name` es un campo privado respaldado por una propiedad pública.
    // La propiedad Name controla el acceso (get/set con validación).
    private string _name;

    // ── PROPIEDADES ──────────────────────────────────────────────────
    // Id es de solo lectura (solo tiene get). Se expone como Guid,
    // inmutable para el exterior. El `=>` es un "expression-bodied member".
    public Guid Id => _id;

    // Propiedad con getter y setter tradicional. El setter valida que
    // el valor no sea null en tiempo de ejecución (gracias a Nullable enable).
    public string Name
    {
        get => _name;
        set => _name = value ?? throw new ArgumentNullException(nameof(value));
    }

    // Auto-property: el compilador genera el campo privado (_salary)
    // automáticamente. La sintaxis es concisa pero permite get y set.
    public decimal Salary { get; set; }

    // ── CONSTRUCTOR ──────────────────────────────────────────────────
    // Constructor tradicional con parámetros. Inicializa los campos
    // obligatorios al crear el objeto. No se puede crear un Employee
    // sin name y salary.
    // En C# 12 existe el "primary constructor" (constructor primario)
    // para clases, pero aquí usamos el tradicional por claridad didáctica:
    // vemos explícitamente campos, propiedades y su inicialización.
    public Employee(string name, decimal salary)
    {
        // Guid.NewGuid() genera un identificador único global.
        // UUID v4 en la práctica, con muy baja probabilidad de colisión.
        _id = Guid.NewGuid();

        // ?? (null-coalescing): si name es null, lanza excepción.
        _name = name ?? throw new ArgumentNullException(nameof(name));

        Salary = salary;
    }

    // ── MÉTODO VIRTUAL ───────────────────────────────────────────────
    // `virtual` es la PALABRA CLAVE que habilita el polimorfismo.
    // Significa: "las clases derivadas PUEDEN sobrescribir este método".
    // Sin `virtual`, una clase hija que declare `CalculateBonus()` estaría
    // OCULTANDO el método base (con `new`), no sobrescribiéndolo.
    public virtual decimal CalculateBonus()
    {
        // Por defecto, un empleado recibe el 10% de su salario anual.
        // `m` al final del literal indica que es decimal (128-bit).
        return Salary * 0.10m;
    }

    // ── SOBRESCRITURA DE ToString() ──────────────────────────────────
    // Toda clase en C# hereda de System.Object, que define ToString().
    // Sobrescribirlo permite mostrar información legible del objeto,
    // útil para debugging y logging.
    // `:C` es el especificador de formato de moneda (Currency).
    public override string ToString()
    {
        // Muestra solo los primeros 8 caracteres del Guid por brevedad.
        return $"[{_id.ToString()[..8]}...] {Name} — Salario: {Salary:C}";
    }

    // ── IMPLEMENTACIÓN DE IPayable ───────────────────────────────────
    // Al implementar una interfaz, debemos proporcionar el cuerpo del
    // método. Dividimos el salario anual entre 12 para obtener el pago
    // mensual bruto.
    // Este método NO es virtual por defecto, pero podríamos hacerlo
    // `virtual` si quisiéramos que las clases derivadas lo sobrescriban.
    public decimal GetMonthlyPay()
    {
        return Salary / 12m;
    }

    // ── IMPLEMENTACIÓN DE IReportable ────────────────────────────────
    // GenerateReport() produce un string con información resumida.
    // Observa que usa CalculateBonus(), que ES virtual: si una clase
    // derivada sobrescribe CalculateBonus(), este método usará la
    // versión sobrescrita automáticamente (virtual dispatch).
    public string GenerateReport()
    {
        return $"Empleado: {Name} | Salario anual: {Salary:C} | Bonus: {CalculateBonus():C}";
    }
}

// ─── CLASE DERIVADA: Manager ─────────────────────────────────────────
// `Manager : Employee` significa "Manager hereda de Employee".
// Manager obtiene TODOS los miembros (campos, props, métodos) de Employee
// y puede extenderlos o modificarlos (si son virtual).
// En C# la herencia es SIMPLE: una clase solo puede heredar de UNA clase base.
class Manager : Employee
{
    // Propiedad adicional exclusiva de Manager
    public string Department { get; set; }

    // ── CONSTRUCTOR CON `base()` ─────────────────────────────────────
    // `base(name, salary)` llama al constructor de Employee.
    // Esto es OBLIGATORIO porque Employee no tiene constructor sin
    // parámetros. Si no llamamos a `base(...)`, el compilador da error.
    // La clase base debe estar correctamente inicializada antes que la derivada.
    public Manager(string name, decimal salary, string department)
        : base(name, salary) // ← Invoca al constructor de Employee
    {
        Department = department ?? throw new ArgumentNullException(nameof(department));
    }

    // ── SOBRESCRITURA DE CalculateBonus ──────────────────────────────
    // `override` indica que ESTAMOS REEMPLAZANDO el método virtual de
    // la clase base. Si la clase base no tuviera `virtual`, esto no
    // compilaría (error CS0506).
    public override decimal CalculateBonus()
    {
        // Los managers tienen un bonus del 20% (mejor que el 10% base)
        decimal bonus = Salary * 0.20m;

        // Podríamos acceder al método base si quisiéramos con:
        //   decimal baseBonus = base.CalculateBonus();
        // Pero aquí damos una implementación completamente nueva.

        return bonus;
    }

    // Sobrescribimos ToString() para añadir el departamento.
    // `base.ToString()` invoca la versión de Employee; esto reutiliza
    // código y añade solo la información específica de Manager.
    public override string ToString()
    {
        return $"{base.ToString()} — Depto: {Department} (Manager)";
    }
}

// ─── CLASE DERIVADA: Developer ───────────────────────────────────────
class Developer : Employee
{
    // Lista de tecnologías que domina el developer.
    // List<T> está en System.Collections.Generic (incluido en ImplicitUsings).
    public List<string> TechStack { get; set; }

    public Developer(string name, decimal salary, List<string> techStack)
        : base(name, salary) // Obligatorio: llamar al constructor de Employee
    {
        TechStack = techStack ?? throw new ArgumentNullException(nameof(techStack));
    }

    public override decimal CalculateBonus()
    {
        // Base: 15% del salario (más que Employee base, menos que Manager)
        decimal bonus = Salary * 0.15m;

        // Bonus adicional de 500 si el developer conoce tecnologías .NET
        // clave. Esto muestra lógica de negocio dentro del método virtual.
        // LINQ (Any, Equals) está disponible gracias a ImplicitUsings.
        if (TechStack.Any(tech =>
                tech.Equals("C#", StringComparison.OrdinalIgnoreCase) ||
                tech.Equals(".NET", StringComparison.OrdinalIgnoreCase)))
        {
            bonus += 500m;
        }

        return bonus;
    }

    public override string ToString()
    {
        string techs = string.Join(", ", TechStack);
        return $"{base.ToString()} — Stack: [{techs}] (Developer)";
    }
}
