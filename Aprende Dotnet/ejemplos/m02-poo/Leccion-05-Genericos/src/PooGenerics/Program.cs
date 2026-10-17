// ============================================================
// Módulo 2 — Lección 05: Genéricos en C#
// ============================================================
// Temas:
//   1. Clases genéricas (Repository<T>)
//   2. Métodos genéricos (Swap<T>, FirstOrDefault<T>)
//   3. Interfaces genéricas (IValidator<T>)
//   4. Constraints (where T : EntityBase, new(), IComparable<T>, múltiples)
//   5. Covarianza (out T)
//   6. Contravarianza (in T)
// ============================================================
// Los genéricos permiten escribir código reutilizable y con
// seguridad de tipos en tiempo de compilación, evitando el uso
// de object y los castings manuales.
// ============================================================

// ============================================================
// DEMOSTRACIÓN PRINCIPAL
// ============================================================

Console.WriteLine("╔══════════════════════════════════════════╗");
Console.WriteLine("║     GENÉRICOS EN C# 12 CON .NET 8       ║");
Console.WriteLine("╚══════════════════════════════════════════╝");
Console.WriteLine();

// ---------- 1. CLASES GENÉRICAS: Repository<T> ----------

Console.WriteLine("--- 1. Repository<T> (clase genérica) ---");
Console.WriteLine();

// Repository<User>: repositorio en memoria para usuarios
Repository<User> userRepo = new Repository<User>();
userRepo.Add(new User { Id = Guid.NewGuid(), Username = "alice", Email = "alice@email.com" });
userRepo.Add(new User { Id = Guid.NewGuid(), Username = "bob",   Email = "bob@email.com" });

Console.WriteLine($"  Usuarios en el repositorio: {userRepo.Count}");

// GetById usa un predicado (Func<T, bool>) para buscar
User? foundUser = userRepo.GetById(u => u.Username == "alice");
Console.WriteLine($"  Búsqueda por predicado (Username == alice): {(foundUser is not null ? foundUser.Username : "no encontrado")}");

User? notFound = userRepo.GetById(u => u.Username == "nonexistent");
Console.WriteLine($"  Búsqueda fallida (Username == nonexistent): {(notFound is not null ? "encontrado" : "null (no encontrado)")}");

// GetAll devuelve IReadOnlyList<T> (no modificable desde fuera)
IReadOnlyList<User> allUsers = userRepo.GetAll();
Console.WriteLine("  Todos los usuarios:");
foreach (User user in allUsers)
    Console.WriteLine($"    - {user.Username} ({user.Email})");

// Mismo Repository<T> funciona con cualquier tipo: Product
Repository<Product> productRepo = new Repository<Product>();
productRepo.Add(new Product { Id = Guid.NewGuid(), Name = "Laptop",  Price = 999.99m });
productRepo.Add(new Product { Id = Guid.NewGuid(), Name = "Mouse",   Price = 29.99m });
productRepo.Add(new Product { Id = Guid.NewGuid(), Name = "Teclado", Price = 79.99m });

Console.WriteLine();
Console.WriteLine($"  Productos en el repositorio: {productRepo.Count}");
IReadOnlyList<Product> allProducts = productRepo.GetAll();
Console.WriteLine("  Todos los productos:");
foreach (Product prod in allProducts)
    Console.WriteLine($"    - {prod.Name}: ${prod.Price:F2}");

// Remove devuelve true si el elemento existía
Product mouse = allProducts[1]; // "Mouse"
bool removed = productRepo.Remove(mouse);
Console.WriteLine($"  Se eliminó '{mouse.Name}': {removed} (quedan {productRepo.Count})");

Console.WriteLine();

// ---------- 2. MÉTODOS GENÉRICOS ----------

Console.WriteLine("--- 2. Métodos genéricos (Swap<T>, FirstOrDefault<T>) ---");
Console.WriteLine();

// Swap<T>: intercambia dos valores del mismo tipo usando ref
int a = 5, b = 10;
Console.WriteLine($"  Antes  de Swap<int>:  a = {a}, b = {b}");
Utils.Swap<int>(ref a, ref b);
Console.WriteLine($"  Después de Swap<int>:  a = {a}, b = {b}");

string first = "hola", second = "mundo";
Console.WriteLine($"  Antes  de Swap<string>:  first = \"{first}\", second = \"{second}\"");
Utils.Swap<string>(ref first, ref second);
Console.WriteLine($"  Después de Swap<string>:  first = \"{first}\", second = \"{second}\"");

// El compilador infiere el tipo automáticamente, podemos omitir <T>
string x = "izquierda", y = "derecha";
Console.WriteLine($"  Antes  de Swap (inferido):  x = \"{x}\", y = \"{y}\"");
Utils.Swap(ref x, ref y); // El compilador infiere T = string
Console.WriteLine($"  Después de Swap (inferido):  x = \"{x}\", y = \"{y}\"");

// FirstOrDefault<T>: nuestra propia implementación genérica
Console.WriteLine();
List<int> numbers = new List<int> { 1, 2, 3, 4, 5, 6 };
int? firstEven = Utils.FirstOrDefault<int>(numbers, n => n % 2 == 0);
Console.WriteLine($"  FirstOrDefault<int> (primer par):       {firstEven}");
int defaultInt = Utils.FirstOrDefault<int>(numbers, n => n > 10); // No encuentra → default(int) = 0
Console.WriteLine($"  FirstOrDefault<int> (mayor que 10):     {defaultInt} (default = 0 para int)");

List<string> names = new List<string> { "Ana", "Carlos", "Beatriz", "David" };
string? nameStartingWithB = Utils.FirstOrDefault<string>(names, n => n.StartsWith("B"));
Console.WriteLine($"  FirstOrDefault<string> (empieza con B): {nameStartingWithB}");

Console.WriteLine();

// ---------- 3. INTERFACES GENÉRICAS: IValidator<T> ----------

Console.WriteLine("--- 3. Interfaces genéricas: IValidator<T> ---");
Console.WriteLine();

// EmailValidator implementa IValidator<string>
IValidator<string> emailValidator = new EmailValidator();
Console.WriteLine($"  EmailValidator.Validate(\"test@mail.com\"): {emailValidator.Validate("test@mail.com")}");
Console.WriteLine($"  EmailValidator.Validate(\"no-es-email\"):   {emailValidator.Validate("no-es-email")}");

// ProductValidator implementa IValidator<Product>
IValidator<Product> productValidator = new ProductValidator();
Product validProduct = new Product { Id = Guid.NewGuid(), Name = "Monitor", Price = 299.99m };
Product invalidProduct = new Product { Id = Guid.NewGuid(), Name = "", Price = -10m };
Console.WriteLine($"  ProductValidator.Validate(válido):     {productValidator.Validate(validProduct)}");
Console.WriteLine($"  ProductValidator.Validate(inválido):   {productValidator.Validate(invalidProduct)}");

Console.WriteLine();

// ---------- 4. CONSTRAINTS ----------

Console.WriteLine("--- 4. Constraints (restricciones con where) ---");
Console.WriteLine();

// 4a. where T : EntityBase — T debe heredar de EntityBase
Console.WriteLine("  4a. where T : EntityBase");
GenericRepository<User> constrainedRepo = new GenericRepository<User>();
// Si el Id está vacío, la constraint permite acceder a .Id y asignarlo
User charlieUser = new User { Username = "charlie", Email = "charlie@email.com" };
constrainedRepo.Add(charlieUser); // Id = Guid.Empty → se asigna dentro de Add automáticamente
User? found = constrainedRepo.GetById(charlieUser.Id); // Buscamos por el Id recién asignado
Console.WriteLine($"    Usuario en constrained repo: {found?.Username} (Id autogenerado: {found?.Id})");
Console.WriteLine($"    Cantidad: {constrainedRepo.Count}");
// Esto NO compilaría: GenericRepository<string> porque string no hereda de EntityBase
Console.WriteLine();

// 4b. where T : new() — T debe tener constructor público sin parámetros
Console.WriteLine("  4b. where T : new()");
Factory<Product> productFactory = new Factory<Product>(); // Product tiene constructor sin parámetros
Product defaultProduct = productFactory.Create();
Console.WriteLine($"    Product creado: Name = \"{defaultProduct.Name}\", Price = {defaultProduct.Price}");
// Esto NO compilaría si Product solo tuviese un constructor con parámetros
Console.WriteLine();

// 4c. where T : IComparable<T> — T debe ser comparable consigo mismo
Console.WriteLine("  4c. where T : IComparable<T>");
int maxInt = Comparer<int>.FindMax(42, 99);
Console.WriteLine($"    Comparer<int>.FindMax(42, 99) = {maxInt}");

string maxString = Comparer<string>.FindMax("manzana", "banana");
Console.WriteLine($"    Comparer<string>.FindMax(\"manzana\", \"banana\") = {maxString}");

Console.WriteLine($"    Comparer<decimal>.FindMax(3.14m, 2.71m) = {Comparer<decimal>.FindMax(3.14m, 2.71m)}");
Console.WriteLine();

// 4d. Múltiples constraints: where T : class, IValidator<T>, new()
Console.WriteLine("  4d. where T : class, IValidator<T>, new() (múltiples constraints)");
// SelfValidatingEntity cumple las 3 condiciones
MultiConstraintProcessor<SelfValidatingEntity> multi = new MultiConstraintProcessor<SelfValidatingEntity>();
SelfValidatingEntity validEntity = new SelfValidatingEntity { Name = "Entidad Válida" };
SelfValidatingEntity invalidEntity = new SelfValidatingEntity { Name = "" };
Console.WriteLine($"    Procesar entidad válida:   {multi.Process(validEntity)}");
Console.WriteLine($"    Procesar entidad inválida: {multi.Process(invalidEntity)}");

Console.WriteLine();

// ---------- 5. COVARIANZA (out T) ----------

Console.WriteLine("--- 5. Covarianza (out T) ---");
Console.WriteLine("  La covarianza permite que IReadOnlyRepository<Derivada>");
Console.WriteLine("  se asigne a IReadOnlyRepository<Base>.");
Console.WriteLine();

ReadOnlyUserRepository concreteUserRepo = new ReadOnlyUserRepository(
    new List<User>
    {
        new User { Id = Guid.NewGuid(), Username = "diana", Email = "diana@email.com" },
        new User { Id = Guid.NewGuid(), Username = "eric",  Email = "eric@email.com" }
    }
);

// Covarianza: IReadOnlyRepository<User> → IReadOnlyRepository<EntityBase>
// User hereda de EntityBase, y out T solo aparece en posiciones de salida
IReadOnlyRepository<EntityBase> covariantRepo = concreteUserRepo;
Console.WriteLine("  Asignación covariante: IReadOnlyRepository<User> → IReadOnlyRepository<EntityBase>");
Console.WriteLine("  Elementos obtenidos (como EntityBase):");
foreach (EntityBase entity in covariantRepo.GetAll())
    Console.WriteLine($"    Entity Id: {entity.Id}");

Console.WriteLine();

// ---------- 6. CONTRAVARIANZA (in T) ----------

Console.WriteLine("--- 6. Contravarianza (in T) ---");
Console.WriteLine("  La contravarianza permite que IWriteOnlyRepository<Base>");
Console.WriteLine("  se asigne a IWriteOnlyRepository<Derivada>.");
Console.WriteLine();

WriteOnlyObjectRepository concreteObjWriter = new WriteOnlyObjectRepository();

// Contravarianza: IWriteOnlyRepository<object> → IWriteOnlyRepository<string>
// El método Add recibe T (posición de entrada), por lo que invertimos la jerarquía
IWriteOnlyRepository<string> contravariantStrWriter = concreteObjWriter;
Console.WriteLine("  Asignación contravariante: IWriteOnlyRepository<object> → IWriteOnlyRepository<string>");
contravariantStrWriter.Add("Mensaje 1 vía contravarianza");
contravariantStrWriter.Add("Mensaje 2 vía contravarianza");

// También funciona con User (User es object)
IWriteOnlyRepository<User> contravariantUserWriter = concreteObjWriter;
contravariantUserWriter.Add(new User { Username = "fiona", Email = "fiona@email.com" });
Console.WriteLine("  También: IWriteOnlyRepository<object> → IWriteOnlyRepository<User>");

Console.WriteLine();
Console.WriteLine("╔══════════════════════════════════════════╗");
Console.WriteLine("║        FIN DE LA DEMOSTRACIÓN           ║");
Console.WriteLine("╚══════════════════════════════════════════╝");


// ============================================================
// CLASES DE DOMINIO
// ============================================================

// Clase base para todas las entidades del sistema.
// Usada como constraint en GenericRepository<T>.
public class EntityBase
{
    public Guid Id { get; set; }
}

// Entidad que representa un usuario del sistema.
// Hereda de EntityBase para poder usarse con GenericRepository<T>.
public class User : EntityBase
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public override string ToString() => $"{Username} ({Email})";
}

// Entidad que representa un producto del catálogo.
public class Product : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }

    public override string ToString() => $"{Name} — ${Price:F2}";
}


// ============================================================
// 1. CLASE GENÉRICA: Repository<T>
// ============================================================
// Repositorio genérico en memoria que funciona con cualquier tipo T.
// No tiene constraints: acepta tanto clases como structs.

public class Repository<T>
{
    // Lista privada interna. readonly evita reasignar la referencia.
    private readonly List<T> _items = new();

    // Añade un elemento al repositorio.
    public void Add(T item)
    {
        _items.Add(item);
    }

    // Busca el primer elemento que cumpla el predicado.
    // Devuelve default(T) (null para reference types) si no encuentra ninguno.
    public T? GetById(Func<T, bool> predicate)
    {
        foreach (T item in _items)
        {
            if (predicate(item))
                return item;
        }
        return default;
    }

    // Devuelve una vista de solo lectura de todos los elementos.
    // AsReadOnly() crea un wrapper que impide modificaciones externas.
    public IReadOnlyList<T> GetAll()
    {
        return _items.AsReadOnly();
    }

    // Elimina la primera ocurrencia del elemento.
    // Devuelve true si el elemento fue encontrado y eliminado.
    public bool Remove(T item)
    {
        return _items.Remove(item);
    }

    // Propiedad de solo lectura que expone la cantidad de elementos.
    public int Count => _items.Count;
}


// ============================================================
// 2. MÉTODOS GENÉRICOS: Utils
// ============================================================
// Los métodos genéricos se definen con <T> después del nombre.
// Pueden estar en clases genéricas o no genéricas.

public static class Utils
{
    // Intercambia dos valores usando paso por referencia (ref).
    // 'ref' es necesario para modificar las variables originales del llamante.
    public static void Swap<T>(ref T a, ref T b)
    {
        T temp = a;
        a = b;
        b = temp;
    }

    // Busca el primer elemento que cumple el predicado.
    // Implementación manual equivalente a LINQ FirstOrDefault.
    public static T? FirstOrDefault<T>(IEnumerable<T> source, Func<T, bool> predicate)
    {
        foreach (T item in source)
        {
            if (predicate(item))
                return item;
        }
        return default; // default(T): null para clases, 0 para int, false para bool...
    }
}


// ============================================================
// 3. INTERFACES GENÉRICAS: IValidator<T>
// ============================================================
// Una interfaz genérica define un contrato parametrizado por tipo.
// Cada implementación fija T al tipo concreto que necesita validar.

public interface IValidator<T>
{
    // Valida un elemento de tipo T. Devuelve true si es válido.
    bool Validate(T item);
}

// Implementación concreta: validador de cadenas de email.
// Fija T = string, por lo que Validate recibe un string.
public class EmailValidator : IValidator<string>
{
    public bool Validate(string item)
    {
        // Validación simple: no vacío, contiene '@' y '.'
        if (string.IsNullOrEmpty(item))
            return false;

        return item.Contains('@') && item.Contains('.');
    }
}

// Implementación concreta: validador de productos.
// Fija T = Product, por lo que Validate recibe un Product.
public class ProductValidator : IValidator<Product>
{
    public bool Validate(Product item)
    {
        // Un producto es válido si tiene nombre no vacío y precio positivo
        return !string.IsNullOrWhiteSpace(item.Name) && item.Price > 0;
    }
}


// ============================================================
// 4. CONSTRAINTS (restricciones con where)
// ============================================================
// Los constraints limitan los tipos que pueden usarse como T.
// Esto permite al compilador verificar el uso de miembros específicos de T
// y da acceso a operadores como new T().

// --- 4a. where T : EntityBase ---
// Solo tipos que hereden de EntityBase (clase base).
// Ventaja: podemos acceder a .Id dentro del repositorio.
public class GenericRepository<T> where T : EntityBase
{
    private readonly List<T> _items = new();

    public void Add(T item)
    {
        // Podemos acceder a .Id porque T hereda de EntityBase
        if (item.Id == Guid.Empty)
            item.Id = Guid.NewGuid(); // Asignamos Id automáticamente

        _items.Add(item);
    }

    public T? GetById(Guid id)
    {
        foreach (T item in _items)
        {
            if (item.Id == id) // Acceso a .Id gracias al constraint
                return item;
        }
        return default;
    }

    public IReadOnlyList<T> GetAll() => _items.AsReadOnly();

    public int Count => _items.Count;
}

// --- 4b. where T : new() ---
// T debe tener un constructor público sin parámetros.
// Permite usar new T() dentro de la clase.
public class Factory<T> where T : new()
{
    // Crea una nueva instancia de T usando el constructor sin parámetros.
    public T Create()
    {
        return new T(); // Solo posible gracias al constraint new()
    }
}

// --- 4c. where T : IComparable<T> ---
// T debe implementar IComparable<T>, es decir, ser comparable consigo mismo.
// Permite llamar a .CompareTo() dentro de la clase.
public class Comparer<T> where T : IComparable<T>
{
    // Método estático: devuelve el mayor de dos valores.
    // No necesita instancia de Comparer<T> para usarse.
    public static T FindMax(T a, T b)
    {
        // a.CompareTo(b) devuelve:
        //   > 0 si a es mayor que b
        //   < 0 si a es menor que b
        //   = 0 si son iguales
        if (a.CompareTo(b) >= 0)
            return a;
        else
            return b;
    }
}

// --- 4d. Múltiples constraints ---
// Se pueden combinar varios constraints separados por comas.
// T debe cumplir TODAS las condiciones simultáneamente:
//   class   → T debe ser un tipo por referencia (no struct)
//   IValidator<T> → T debe implementar IValidator<T> (validarse a sí mismo)
//   new()  → T debe tener constructor sin parámetros
public class MultiConstraintProcessor<T> where T : class, IValidator<T>, new()
{
    // Gracias a new() podemos crear instancias con new T()
    private readonly T _validator = new T();

    // Procesa un elemento validándolo con el validador interno
    public bool Process(T item)
    {
        return _validator.Validate(item);
    }
}

// Entidad de ejemplo que se valida a sí misma.
// Cumple los tres constraints: es class, IValidator<SelfValidatingEntity>, new()
public class SelfValidatingEntity : IValidator<SelfValidatingEntity>
{
    public string Name { get; set; } = string.Empty;

    // Implementa IValidator<SelfValidatingEntity>: se valida a sí misma
    public bool Validate(SelfValidatingEntity item)
    {
        return !string.IsNullOrWhiteSpace(item.Name);
    }

    public override string ToString() => Name;
}


// ============================================================
// 5. COVARIANZA (out T)
// ============================================================
// La covarianza permite usar un tipo más derivado del especificado.
// Se declara con 'out' antes de T en la interfaz.
// out T solo puede aparecer en POSICIONES DE SALIDA (valores de retorno).
// Regla nemotécnica: "out T significa que T sale de la interfaz".

public interface IReadOnlyRepository<out T>
{
    // T aparece como tipo de retorno (posición de salida) ✓
    IEnumerable<T> GetAll();

    // ❌ Esto NO compilaría porque T aparece como parámetro (entrada):
    // T GetById(Func<T, bool> predicate);
}

// Implementación concreta para User.
public class ReadOnlyUserRepository : IReadOnlyRepository<User>
{
    private readonly List<User> _users;

    public ReadOnlyUserRepository(List<User> users)
    {
        _users = users;
    }

    public IEnumerable<User> GetAll()
    {
        return _users;
    }
}


// ============================================================
// 6. CONTRAVARIANZA (in T)
// ============================================================
// La contravarianza permite usar un tipo más genérico (menos derivado)
// del especificado. Se declara con 'in' antes de T en la interfaz.
// in T solo puede aparecer en POSICIONES DE ENTRADA (parámetros).
// Regla nemotécnica: "in T significa que T entra en la interfaz".

public interface IWriteOnlyRepository<in T>
{
    // T aparece como parámetro (posición de entrada) ✓
    void Add(T item);

    // ❌ Esto NO compilaría porque T aparece como tipo de retorno (salida):
    // T GetLastAdded();
}

// Implementación concreta para object.
// Almacena cualquier tipo de objeto en una lista interna.
public class WriteOnlyObjectRepository : IWriteOnlyRepository<object>
{
    private readonly List<object> _items = new();

    public void Add(object item)
    {
        Console.WriteLine($"    [Writer] Añadido: {item}");
        _items.Add(item);
    }
}
