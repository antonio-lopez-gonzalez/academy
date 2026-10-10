// ────────────────────────────────────────────────────────────
// Módulo 2 — POO en C# en profundidad
// Lección 04 — sealed, static, partial
//
// Temas:
//   1. sealed  — Clases y métodos sellados (herencia bloqueada)
//   2. static  — Clases, constructores y miembros estáticos
//   3. partial — Clases y métodos parciales (código repartido)
// ────────────────────────────────────────────────────────────

// ================================================================
// DEMOSTRACIÓN PRINCIPAL
//
// NOTA: En C# con top-level statements, el código ejecutable
// (instrucciones sueltas) debe aparecer ANTES de cualquier
// declaración de clase, struct o record en el mismo archivo.
// El compilador las mueve implícitamente a un método <Main>$().
// ================================================================

// Separador visual para la salida en consola.
Console.WriteLine(new string('=', 60));
Console.WriteLine("  MÓDULO 2 — Lección 04: sealed, static, partial");
Console.WriteLine(new string('=', 60));

// ───── SEALED ─────
Console.WriteLine("\n─── 1. SEALED ───");

User user = new User
{
    Username = "ana.garcia",
    Email = "ana@cms.local"
};
Console.WriteLine($"  Usuario creado: {user.GetDisplayName()}");
Console.WriteLine($"  ID: {user.Id}");
// Nota: no podemos heredar de User. Está sellado a propósito.

Dog dog = new Dog();
Console.WriteLine($"  Perro dice: {dog.Sound()}");
// Nota: el método Sound() de Dog está sellado con `sealed override`.
// Ninguna subclase de Dog puede cambiarlo.

// ───── STATIC ─────
Console.WriteLine("\n─── 2. STATIC ───");

// Clase estática: usamos sus métodos sin instanciar.
int sum = MathUtils.Add(10, 5);
int diff = MathUtils.Subtract(10, 5);
int clamped = MathUtils.Clamp(42, 0, 30);
bool isEven = MathUtils.IsEven(7);

Console.WriteLine($"  MathUtils.Add(10, 5)     = {sum}");
Console.WriteLine($"  MathUtils.Subtract(10, 5) = {diff}");
Console.WriteLine($"  MathUtils.Clamp(42,0,30)  = {clamped}");
Console.WriteLine($"  MathUtils.IsEven(7)       = {isEven}");

// Clase estática de configuración: acceso directo.
Console.WriteLine($"\n  App: {AppConfig.AppName} v{AppConfig.Version}");
Console.WriteLine($"  PageSize por defecto: {AppConfig.DefaultPageSize}");

// Constructor estático: se ejecuta al primer acceso.
Console.WriteLine($"\n  ConnectionString: {DatabaseConfig.ConnectionString}");
Console.WriteLine($"  MaxRetries: {DatabaseConfig.MaxRetries}");
Console.WriteLine($"  IsInitialized: {DatabaseConfig.IsInitialized}");

// Constructor de instancia: se ejecuta por cada `new`.
DatabaseConfig devConfig = new DatabaseConfig("Development");
DatabaseConfig prodConfig = new DatabaseConfig("Production");

// Miembros estáticos en clase no estática: contador de instancias.
Console.WriteLine($"\n  Instancias antes de crear: {Counter.InstanceCount}");

Counter c1 = new Counter();
Counter c2 = new Counter();
Counter c3 = new Counter();

Console.WriteLine($"  c1.Id = {c1.Id}, c2.Id = {c2.Id}, c3.Id = {c3.Id}");
Console.WriteLine($"  Total instancias: {Counter.GetTotalInstances()}");

// ───── PARTIAL ─────
Console.WriteLine("\n─── 3. PARTIAL ───");

// Person tiene propiedades de DOS partes diferentes (básicas + contacto)
// y métodos de una tercera parte. El compilador las unió en una sola clase.
Person person = new Person
{
    FirstName = "Carlos",
    LastName = "López",
    BirthDate = new DateTime(1990, 5, 15),
    Email = "carlos@email.com",
    Phone = "+34 600 000 000",
    Address = "Calle Mayor 1, Madrid"
};

Console.WriteLine($"  Nombre completo: {person.GetFullName()}");
Console.WriteLine($"  Edad: {person.GetAge()} años");
Console.WriteLine($"  Email: {person.Email}");
Console.WriteLine($"  Teléfono: {person.Phone}");

// OrderProcessor con partial methods: los hooks se ejecutan.
Console.WriteLine();
OrderProcessor processor = new OrderProcessor();
Order order1 = new Order(Guid.NewGuid(), 150.75m);
Order order2 = new Order(Guid.NewGuid(), 0m); // Pedido con total 0 — el hook lo detecta.

processor.Process(order1);
Console.WriteLine();
processor.Process(order2);

Console.WriteLine($"\n{new string('=', 60)}");
Console.WriteLine("  Fin de la lección 04");
Console.WriteLine(new string('=', 60));

// ================================================================
// A partir de aquí: DECLARACIONES DE TIPOS
// (clases, records, structs — NO puede haber más top-level code)
// ================================================================

#region ==================== 1. SEALED — CLASES SELLADAS ====================

// --------------------------------------------------
// Clase base para entidades del dominio.
// --------------------------------------------------
public class BaseEntity
{
    // Toda entidad del sistema tiene un identificador único.
    public Guid Id { get; set; } = Guid.NewGuid();
}

// --------------------------------------------------
// User es SEALED: ninguna otra clase puede heredar de él.
// Esto protege el modelo de dominio: no queremos subtipos
// inesperados que rompan la lógica de negocio.
// --------------------------------------------------
public sealed class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    // Método específico de User. Nadie puede sobreescribirlo
    // porque la clase es sealed.
    public string GetDisplayName() => $"{Username} ({Email})";
}

// ❌ ESTO NO COMPILARÍA — Descomenta para ver el error CS0509:
//    'Admin': cannot derive from sealed type 'User'
//
// public class Admin : User
// {
//     public string Department { get; set; } = string.Empty;
// }

// --------------------------------------------------
// SEALED OVERRIDE: sella un método concreto de la cadena
// de herencia, impidiendo que las subclases lo sobreescriban.
// --------------------------------------------------

// Clase base con un método virtual.
public class Animal
{
    public virtual string Sound() => "?";
}

// Dog sobreescribe Sound y lo sella.
// A partir de aquí, ninguna subclase de Dog puede cambiar Sound().
public class Dog : Animal
{
    public sealed override string Sound() => "Guau";
}

// ❌ ESTO NO COMPILARÍA — Descomenta para ver el error CS0239:
//    'Bulldog.Sound()': cannot override inherited member
//    'Dog.Sound()' because it is sealed
//
// public class Bulldog : Dog
// {
//     public override string Sound() => "Guau ronco";
// }

// --------------------------------------------------
// ¿CUÁNDO USAR SEALED?
//
//   - Clases de dominio que no deben tener subtipos.
//   - Records (por defecto los record class NO son sealed,
//     pero puedes declararlos como `public sealed record`).
//   - Métodos sellados por seguridad: evitas que una subclase
//     modifique un comportamiento crítico.
//   - Optimización del JIT: el compilador puede aplicar
//     "devirtualization" — si sabe que una clase es sealed,
//     transforma la llamada virtual en llamada directa (call en
//     lugar de callvirt), ahorrando una indirección.
// --------------------------------------------------

#endregion

#region ==================== 2. STATIC — CLASES ESTÁTICAS ====================

// --------------------------------------------------
// Clase estática: no se puede instanciar con `new`.
// Todos sus miembros deben ser estáticos.
// No puede heredar de nada (salvo System.Object, implícito).
// Antes de C# 11 no podía implementar interfaces.
// Ideal para: utilidades, helpers, constantes de aplicación.
// --------------------------------------------------
public static class MathUtils
{
    // Métodos puros: no dependen de estado de instancia.
    public static int Add(int a, int b) => a + b;

    public static int Subtract(int a, int b) => a - b;

    // Clamp: fuerza un valor a estar entre min y max.
    public static int Clamp(int value, int min, int max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }

    // Verifica si un número es par.
    public static bool IsEven(int value) => value % 2 == 0;
}

// --------------------------------------------------
// Clase estática para configuración global de la app.
// Los campos estáticos se inicializan UNA vez y viven
// durante toda la vida de la aplicación.
// --------------------------------------------------
public static class AppConfig
{
    // Campos de solo lectura: se asignan una vez y no cambian.
    public static readonly string AppName = "MiCMS";
    public static readonly string Version = "1.0.0";
    public static readonly int DefaultPageSize = 20;

    // ❌ No puedes declarar un constructor de instancia en
    //    una clase estática. Solo se permite el constructor
    //    estático (static constructor).
}

// --------------------------------------------------
// CONSTRUCTOR ESTÁTICO:
//  - Se ejecuta UNA sola vez, ANTES de cualquier uso de la clase.
//  - El runtime lo invoca automáticamente; tú no lo llamas.
//  - No acepta parámetros ni modificadores de acceso.
//  - Si lanza una excepción, la clase queda inutilizable
//    para el resto de la ejecución del AppDomain.
//
// DIFERENCIA CON CONSTRUCTOR DE INSTANCIA:
//  - Instancia: se ejecuta por cada `new`.
//  - Estático: se ejecuta una vez para toda la clase.
// --------------------------------------------------
public class DatabaseConfig
{
    // Propiedades que se cargan desde el constructor estático.
    public static string ConnectionString { get; private set; } = string.Empty;

    public static int MaxRetries { get; private set; }

    public static bool IsInitialized { get; private set; }

    // Constructor estático: se ejecuta automáticamente una sola vez.
    static DatabaseConfig()
    {
        // En un escenario real, aquí leerías appsettings.json,
        // variables de entorno o un servicio de secrets.
        ConnectionString = "Host=localhost;Database=cms;Username=admin";
        MaxRetries = 3;
        IsInitialized = true;

        // Nota: esto solo se imprime la primera vez que se accede
        // a cualquier miembro de DatabaseConfig.
        Console.WriteLine("  [Constructor estático] DatabaseConfig inicializado.");
    }

    // Constructor de instancia: se ejecuta por cada `new DatabaseConfig()`.
    public DatabaseConfig(string environment)
    {
        Console.WriteLine($"  [Constructor instancia] Entorno: {environment}");
    }
}

// --------------------------------------------------
// MIEMBROS ESTÁTICOS EN CLASES NO ESTÁTICAS:
// Útil para compartir estado entre todas las instancias
// (contadores, cachés, pools, etc.).
// --------------------------------------------------
public class Counter
{
    // Campo estático: compartido por TODAS las instancias.
    private static int s_instanceCount;

    // Propiedad estática de solo lectura desde fuera.
    public static int InstanceCount => s_instanceCount;

    // Identificador único de esta instancia.
    public int Id { get; }

    public Counter()
    {
        // Cada vez que se crea una instancia, incrementamos
        // el contador compartido.
        s_instanceCount++;
        Id = s_instanceCount;
    }

    // Método estático que opera sobre estado compartido.
    public static int GetTotalInstances() => s_instanceCount;

    // Reinicia el contador (solo para demostración).
    public static void Reset() => s_instanceCount = 0;
}

#endregion

#region ==================== 3. PARTIAL — CLASES PARCIALES ====================

// ================================================================
// PARTIAL CLASS: Person
//
// Una clase `partial` se puede definir en varios archivos
// (o en el mismo archivo, como aquí). El compilador las une
// en tiempo de compilación en una sola clase.
//
// Casos de uso reales:
//   - Código auto-generado + código manual (WinForms, EF Core,
//     Source Generators, gRPC).
//   - Separar una clase grande en secciones lógicas.
// ================================================================

// ───── PARTE 1: Propiedades básicas ─────
public partial class Person
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public DateTime BirthDate { get; set; }
}

// ───── PARTE 2: Propiedades de contacto ─────
public partial class Person
{
    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }
}

// ───── PARTE 3: Métodos ─────
public partial class Person
{
    // Une nombre y apellido.
    public string GetFullName() => $"{FirstName} {LastName}";

    // Calcula la edad a partir de la fecha de nacimiento.
    // En este ejemplo usamos DateTime.Today por simplicidad.
    public int GetAge()
    {
        DateTime today = DateTime.Today;
        int age = today.Year - BirthDate.Year;

        // Si aún no ha cumplido años este año, restamos 1.
        if (BirthDate.Date > today.AddYears(-age))
            age--;

        return age;
    }
}

// ================================================================
// PARTIAL METHODS: OrderProcessor
//
// Un método parcial tiene dos partes:
//   1. Declaración (firma): `partial void Nombre(params);`
//   2. Implementación (cuerpo): `partial void Nombre(params) { ... }`
//
// Si NO hay implementación, el compilador ELIMINA la llamada
// por completo del código final. Esto ahorra la instrucción call
// y permite "hooks" condicionales sin coste en runtime.
//
// Caso real típico: Source Generators declaran partial methods
// que tú puedes implementar opcionalmente.
// ================================================================

// Registro simple para representar un pedido.
public sealed record Order(Guid Id, decimal Total);

public partial class OrderProcessor
{
    // Declaración del método parcial (sin cuerpo).
    // Si no se implementa, esta llamada desaparece en compilación.
    partial void OnBeforeProcessing(Order order);

    // Otro hook parcial.
    partial void OnAfterProcessing(Order order);

    // Método principal que procesa el pedido.
    public void Process(Order order)
    {
        Console.WriteLine($"  Iniciando procesamiento del pedido {order.Id}...");

        // Si el método parcial NO está implementado, esta línea
        // se ELIMINA del IL generado. Cero coste.
        OnBeforeProcessing(order);

        // Simula trabajo real.
        Console.WriteLine($"  Procesando pedido por {order.Total:C}...");

        OnAfterProcessing(order);

        Console.WriteLine($"  Pedido {order.Id} procesado correctamente.");
    }
}

// ───── Implementación de los métodos parciales ─────
// Esta parte podría estar en otro archivo (ej: generado por un tool).
public partial class OrderProcessor
{
    // Ahora SÍ damos cuerpo al método parcial.
    // El compilador lo "engancha" automáticamente.
    partial void OnBeforeProcessing(Order order)
    {
        Console.WriteLine($"  [Hook ANTES] Validando que el total {order.Total:C} sea > 0.");

        if (order.Total <= 0)
            Console.WriteLine("  ⚠️ ADVERTENCIA: El total del pedido es 0 o negativo.");
    }

    partial void OnAfterProcessing(Order order)
    {
        Console.WriteLine($"  [Hook DESPUÉS] Enviando notificación del pedido {order.Id}.");
    }
}

#endregion
