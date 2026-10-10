# Lección 04 — sealed, static, partial

## 1. Introducción

**¿Qué vamos a aprender?**

Los modificadores `sealed`, `static` y `partial` controlan tres aspectos fundamentales del diseño de tipos en C#:

- **`sealed`**: impide la herencia. Aplicado a una clase, nadie puede extenderla. Aplicado a un método (`sealed override`), las subclases no pueden sobrescribirlo más allá de ese punto.
- **`static`**: elimina la necesidad de instanciar. Clases, métodos y campos que pertenecen al tipo en sí, no a instancias individuales.
- **`partial`**: divide la definición de una clase, struct, interfaz o método en varios archivos. El compilador las une en tiempo de compilación.

**¿Por qué existen?**

Cada uno resuelve un problema de diseño real:

| Modificador | Problema que resuelve |
|---|---|
| `sealed` | Impedir que una clase sea base de otra por seguridad, integridad del dominio u optimización del JIT |
| `static` | Agrupar funcionalidad sin estado sin obligar a instanciar objetos innecesarios |
| `partial` | Separar código generado automáticamente (tools, Source Generators) del código escrito a mano |

**¿Cuándo se utilizan?**

- `sealed`: en entidades de dominio que no deben tener subtipos, en records que quieres mantener cerrados, en métodos de una cadena de herencia cuyo comportamiento no debe alterarse más.
- `static`: en utilidades matemáticas, helpers de conversión, constantes de aplicación, métodos de extensión, factories.
- `partial`: cuando un generador de código (EF Core, gRPC, WinForms, Source Generators) escribe parte de la clase y tú escribes el resto.

**Errores típicos:**

- Sellar clases que forman parte de una jerarquía diseñada para extensión.
- Usar clases `static` como "cajón de sastre" (God Object estático con estado mutable).
- Campos estáticos mutables sin sincronización en entornos multihilo.
- Crear clases `partial` cuando un solo archivo bastaría (complejidad innecesaria).
- Declarar `partial methods` sin implementación y esperar que hagan algo en runtime.

---

## 2. Teoría

### sealed — Clases y métodos sellados

El modificador `sealed` indica al compilador y al runtime: "esta clase no puede ser clase base de ninguna otra". Es una restricción deliberada del diseñador de la API.

```
                 ┌─────────────────────┐
                 │     BaseEntity      │  ← Abierta a herencia
                 │  (no sealed)        │
                 └─────────┬───────────┘
                           │ hereda
                 ┌─────────▼───────────┐
                 │       User          │  ← SEALED: nadie puede heredar de User
                 │  (sealed class)     │
                 └─────────────────────┘
                           │
                    ❌ PROHIBIDO
                           │
                 ┌─────────▼───────────┐
                 │       Admin         │  ← ERROR CS0509: cannot derive from sealed type
                 └─────────────────────┘
```

**sealed en métodos (sealed override):**

Cuando un método virtual se ha sobrescrito varias veces en una cadena de herencia, `sealed override` detiene la cadena en ese punto:

```
Animal ──▶ virtual Sound() ──▶ Dog ──▶ sealed override Sound() ──▶ Bulldog ──❌ no puede override
```

**Beneficio de rendimiento con sealed:**

El JIT (Just-In-Time compiler) puede aplicar **devirtualization** cuando sabe que una clase es `sealed`. En lugar de una llamada virtual (búsqueda en vtable → indirección → método real), emite una llamada directa (`call` en lugar de `callvirt` en IL). Esto evita la indirección de la vtable y permite inlining adicional.

```
Clase no sealed:
  obj.Metodo() ──▶ vtable lookup ──▶ dirección real ──▶ ejecutar

Clase sealed:
  obj.Metodo() ──▶ call directo ──▶ ejecutar  (sin búsqueda)
```

**Records y sealed:**

- Los `record class` **NO** son sealed por defecto. Puedes heredar de ellos.
- Los `record struct` **SÍ** son sealed implícitamente (los structs no soportan herencia).
- Para un record inmutable sellado: `public sealed record Person(string Name, int Age);`

---

### static — Clases, constructores y miembros estáticos

**Clase estática (`static class`):**

Una clase `static` no puede instanciarse con `new`. Todos sus miembros deben ser estáticos. El compilador la marca como `abstract sealed` en IL (no se puede heredar ni instanciar).

```
❌ new MathUtils()     → Error: Cannot create an instance of the static class
✅ MathUtils.Add(3,5)  → Correcto: llamada directa al método estático
```

**Constructores estáticos (`static Constructor()`):**

- Se ejecutan **UNA sola vez**, automáticamente, antes de que cualquier miembro de la clase sea usado.
- El runtime garantiza su ejecución con un lock interno (thread-safe, pero puede causar deadlocks si no se diseña con cuidado).
- No aceptan parámetros ni modificadores de acceso.
- Si lanzan una excepción, el tipo queda inutilizable para el resto de la vida del proceso.

```csharp
public class DatabaseConfig
{
    public static string ConnectionString { get; private set; } = string.Empty;

    static DatabaseConfig()
    {
        // Se ejecuta UNA vez, al primer acceso a DatabaseConfig
        ConnectionString = "Host=localhost;Database=cms";
    }
}
```

**Ciclo de vida de miembros estáticos vs instancia:**

```
Inicio de la aplicación
    │
    ├──▶ Primer acceso a DatabaseConfig.ConnectionString
    │        │
    │        └──▶ static DatabaseConfig() se ejecuta (UNA vez)
    │
    ├──▶ new DatabaseConfig("Dev")
    │        └──▶ Constructor de instancia (1ª vez)
    │
    ├──▶ new DatabaseConfig("Prod")
    │        └──▶ Constructor de instancia (2ª vez)
    │
    ▼
```

**Campos estáticos en clases no estáticas:**

Un campo `static` es compartido por **todas las instancias** de la clase. Es útil para contadores, caches, pools de objetos, singletons.

```csharp
public class Counter
{
    private static int s_instanceCount; // Compartido entre TODAS las instancias

    public Counter()
    {
        s_instanceCount++; // Cada new incrementa el mismo campo
    }
}
```

**Limitaciones de static:**

- No pueden implementar interfaces (antes de C# 11).
- No pueden tener métodos de extensión que sean estáticos (los métodos de extensión se definen en clases estáticas, pero son llamados como si fueran de instancia).
- Dificultan el testing unitario (no se pueden mockear).

---

### partial — Clases y métodos parciales

**Clase parcial (`partial class`):**

Una clase marcada como `partial` puede tener su definición repartida en varios archivos `.cs`. El compilador las une en una sola clase en tiempo de compilación.

```
┌─────────────────────────────┐     ┌─────────────────────────────┐
│  Person.DatosBasicos.cs     │     │  Person.Contacto.cs         │
│                             │     │                             │
│  partial class Person       │     │  partial class Person       │
│  {                          │     │  {                          │
│      string FirstName;      │     │      string Email;          │
│      string LastName;       │     │      string Phone;          │
│      DateTime BirthDate;    │     │  }                          │
│  }                          │     │                             │
└──────────────┬──────────────┘     └──────────────┬──────────────┘
               │                                   │
               └───────────────┬───────────────────┘
                               │ Compilador une
                               ▼
               ┌───────────────────────────────┐
               │       Person (IL final)       │
               │                               │
               │  string FirstName;            │
               │  string LastName;             │
               │  DateTime BirthDate;          │
               │  string Email;                │
               │  string Phone;                │
               └───────────────────────────────┘
```

**Casos de uso reales de partial:**

| Herramienta | Archivo generado | Archivo manual |
|---|---|---|
| Entity Framework Core | `DbContextModel.cs` (modelo) | Tú añades configuraciones parciales |
| WinForms / WPF | `Form1.Designer.cs` (UI auto-generada) | `Form1.cs` (tu lógica) |
| gRPC | `Greeter.cs` (cliente/servidor auto-generado) | Tú extiendes con lógica parcial |
| Source Generators | Código generado en compilación | Tú implementas hooks parciales |
| Blazor | `Component.razor.cs` (code-behind) | `Component.razor` (markup) |

**Métodos parciales (`partial void`):**

Un método parcial se compone de dos partes:

1. **Declaración** (firma, sin cuerpo): el generador de código la escribe.
2. **Implementación** (cuerpo opcional): tú decides si implementarla.

```csharp
// ── Archivo generado (ej: por un Source Generator) ──
public partial class OrderProcessor
{
    partial void OnBeforeProcessing(Order order); // Solo declaración

    public void Process(Order order)
    {
        OnBeforeProcessing(order); // Si no hay implementación, ESTA LÍNEA SE ELIMINA
        // ... lógica real ...
    }
}

// ── Tu archivo (implementación opcional) ──
public partial class OrderProcessor
{
    partial void OnBeforeProcessing(Order order)
    {
        // Tu lógica personalizada aquí
        Console.WriteLine($"Validando pedido {order.Id}");
    }
}
```

**Regla clave**: si un `partial method` no tiene implementación, el compilador **elimina completamente la llamada** del IL generado. Cero coste en runtime. Esto permite a los generadores de código ofrecer "hooks" opcionales sin penalización de rendimiento.

**Partial methods con retorno (C# 9+):**

Desde C# 9, los partial methods pueden devolver valores y tener parámetros `out`:

```csharp
partial bool TryValidate(Order order, out string? error); // Declaración
partial bool TryValidate(Order order, out string? error) { ... } // Implementación
```

---

## 3. Ejemplo mínimo

```csharp
// ── SEALED: clase que nadie puede heredar ──
sealed class FinalClass { }
// class Child : FinalClass { } // ❌ Error CS0509

// ── SEALED OVERRIDE: método sellado en la cadena de herencia ──
class Animal { public virtual string Sound() => "?"; }
class Dog : Animal { public sealed override string Sound() => "Guau"; }
// class Bulldog : Dog { public override string Sound() => "..."; } // ❌ Error CS0239

// ── STATIC: clase sin instancias, solo miembros estáticos ──
static class Utils
{
    public static int Add(int a, int b) => a + b;
}
Console.WriteLine(Utils.Add(3, 4)); // 7

// ── STATIC CONSTRUCTOR: se ejecuta una vez, automáticamente ──
class Config
{
    public static string Value { get; private set; } = "";
    static Config() { Value = "cargado"; } // Se ejecuta al primer acceso a Config
}

// ── PARTIAL: clase dividida en varias partes ──
partial class Person { public string Name { get; set; } = ""; }
partial class Person { public int Age { get; set; } }
var p = new Person { Name = "Ana", Age = 28 }; // Ambas propiedades en una sola clase

// ── PARTIAL METHOD: hook opcional sin coste ──
partial class Processor
{
    partial void OnStart(); // Declaración sin cuerpo
    public void Run()
    {
        OnStart(); // Si no se implementa, esta línea desaparece del IL
        Console.WriteLine("Procesando...");
    }
}
// partial void Processor.OnStart() { Console.WriteLine("Iniciando..."); } // Opcional
```

Ejecutar con:
```bash
dotnet run --project ejemplos/m02-poo/Leccion-04-Sealed-Static-Partial/src/PooModifiers
```

---

## 4. Ejemplo real

El `Program.cs` de esta lección cubre todos los escenarios reales en un solo archivo ejecutable, organizado en tres regiones:

### 1. sealed — Protección del dominio

- **`BaseEntity`**: clase base con `Id` para todas las entidades del sistema.
- **`sealed class User : BaseEntity`**: entidad de usuario sellada. Ninguna otra clase puede heredar de ella. Protege el modelo de dominio contra subtipos inesperados.
- **`Animal → Dog → Bulldog`**: cadena de herencia donde `Dog.Sound()` usa `sealed override`. Bulldog no puede cambiar el sonido.

### 2. static — Utilidades, configuración y estado compartido

- **`static class MathUtils`**: métodos puros (Add, Subtract, Clamp, IsEven). Sin estado, thread-safe por definición.
- **`static class AppConfig`**: constantes globales (`AppName`, `Version`, `DefaultPageSize`) mediante campos `readonly`.
- **`class DatabaseConfig`**: combina constructor estático (inicializa `ConnectionString`, `MaxRetries` una sola vez) con constructor de instancia (cada `new` recibe el nombre del entorno).
- **`class Counter`**: clase no estática con campo estático `s_instanceCount` compartido entre todas las instancias. Cada `new Counter()` incrementa el mismo contador.

### 3. partial — Clases repartidas y hooks condicionales

- **`partial class Person` en 3 partes**: Parte 1 (datos básicos: FirstName, LastName, BirthDate), Parte 2 (contacto: Email, Phone, Address), Parte 3 (métodos: GetFullName, GetAge). El compilador las une.
- **`Order`**: record sellado inmutable (`sealed record`) con Id y Total.
- **`partial class OrderProcessor` con partial methods**: declara `OnBeforeProcessing` y `OnAfterProcessing` como hooks. La implementación está en otra parte de la clase. Si no existiera la implementación, las llamadas se eliminarían del compilado.

```bash
dotnet run --project ejemplos/m02-poo/Leccion-04-Sealed-Static-Partial/src/PooModifiers
```

Salida esperada:
```
============================================================
  MÓDULO 2 — Lección 04: sealed, static, partial
============================================================

─── 1. SEALED ───
  Usuario creado: ana.garcia (ana@cms.local)
  ID: ...
  Perro dice: Guau

─── 2. STATIC ───
  MathUtils.Add(10, 5)     = 15
  MathUtils.Subtract(10, 5) = 5
  MathUtils.Clamp(42,0,30)  = 30
  MathUtils.IsEven(7)       = False

  App: MiCMS v1.0.0
  PageSize por defecto: 20

  [Constructor estático] DatabaseConfig inicializado.
  ConnectionString: Host=localhost;Database=cms;Username=admin
  MaxRetries: 3
  IsInitialized: True

  [Constructor instancia] Entorno: Development
  [Constructor instancia] Entorno: Production

  Instancias antes de crear: 0
  c1.Id = 1, c2.Id = 2, c3.Id = 3
  Total instancias: 3

─── 3. PARTIAL ───
  Nombre completo: Carlos López
  Edad: ... años
  Email: carlos@email.com
  Teléfono: +34 600 000 000

  Iniciando procesamiento del pedido ...
  [Hook ANTES] Validando que el total 150,75 € sea > 0.
  Procesando pedido por 150,75 €...
  [Hook DESPUÉS] Enviando notificación del pedido ...
  Pedido ... procesado correctamente.

  Iniciando procesamiento del pedido ...
  [Hook ANTES] Validando que el total 0,00 € sea > 0.
  ⚠️ ADVERTENCIA: El total del pedido es 0 o negativo.
  Procesando pedido por 0,00 €...
  [Hook DESPUÉS] Enviando notificación del pedido ...
  Pedido ... procesado correctamente.

============================================================
  Fin de la lección 04
============================================================
```

---

## 5. Buenas prácticas

### sealed

✅ **Sellar clases por defecto** si no están diseñadas para herencia. Es la filosofía moderna de .NET: "design for inheritance or prohibit it". Las clases no selladas obligan a mantener compatibilidad hacia adelante en todas las versiones.

✅ **Usar `sealed override` en métodos cuyo comportamiento no debe cambiar** en subtipos. Por ejemplo, un método `Validate()` que implementa reglas de negocio críticas.

✅ **Declarar records como sealed si son parte del dominio**: `public sealed record User(Guid Id, string Name);`. Evitas jerarquías de records no deseadas.

✅ **Aprovechar la devirtualización del JIT** en clases de alto rendimiento.

❌ **No sellar clases base abstractas** diseñadas como plantillas (ej: `abstract class Animal`). Son el punto de extensión.

❌ **No sellar DTOs que puedan necesitar variantes** (aunque en general los DTOs son planos y finales).

### static

✅ **Usar clases static para funciones puras sin estado**: `MathUtils`, `StringHelper`, `DateTimeExtensions`.

✅ **Usar campos `static readonly` para constantes de configuración** que no cambian en runtime.

✅ **Mantener los constructores estáticos ligeros**. Se ejecutan bajo un lock implícito del runtime. Si haces I/O, llamadas de red o `Thread.Sleep`, puedes causar deadlocks en la inicialización del tipo.

✅ **Considerar `Lazy<T>` para inicialización estática costosa** en lugar de un constructor estático pesado.

```csharp
// ✅ Inicialización perezosa y segura
private static readonly Lazy<ExpensiveResource> s_resource =
    new(() => new ExpensiveResource());
```

❌ **Evitar estado mutable estático** (`static int`, `static List<T>`, `static Dictionary<K,V>`). Dificulta el testing (no puedes aislar tests entre sí) y no es thread-safe sin sincronización explícita.

❌ **No usar clases static como "service locator" global**. Es el anti-patrón `StaticServiceLocator` que oculta dependencias y hace el código imposible de testear.

### partial

✅ **Usar partial SOLO cuando sea necesario**:
- Código auto-generado + código manual (Source Generators, EF Core, gRPC).
- Dividir una clase muy grande (> 500 líneas) en archivos lógicos (datos, lógica, validación).

✅ **Nombrar los archivos con sufijo descriptivo**: `Person.Datos.cs`, `Person.Validacion.cs`, `Person.Mapping.cs`.

❌ **No crear partial para clases de 50 líneas**. Un solo archivo es más mantenible.

❌ **No repartir la misma responsabilidad en varios archivos partial**. Cada archivo debe tener cohesión interna.

❌ **No abusar de partial methods como sistema de eventos**. Para eso existen los delegados, eventos y MediatR.

---

## 6. Errores habituales

### Error 1: Sellar una clase que forma parte de una jerarquía de extensión

```csharp
// ❌ INCORRECTO — Los plugins deben poder extender PluginBase
public sealed class PluginBase { }
// Ningún plugin del sistema podrá heredar de ella.
```

```csharp
// ✅ CORRECTO — Deja la clase base abierta
public abstract class PluginBase
{
    public abstract void Execute();
}
```

### Error 2: Static mutable state en multihilo sin lock

```csharp
// ❌ INCORRECTO — Condición de carrera garantizada
public static class Cache
{
    public static List<string> Items = [];

    public static void Add(string item)
    {
        Items.Add(item); // ❌ Dos hilos llamando a Add() al mismo tiempo
    }
}
```

```csharp
// ✅ CORRECTO — Colección thread-safe
public static class Cache
{
    private static readonly ConcurrentBag<string> s_items = [];

    public static void Add(string item) => s_items.Add(item);
}
```

### Error 3: Clase estática como "cajón de sastre"

```csharp
// ❌ INCORRECTO — God Object estático. Acopla todo el sistema.
public static class Global
{
    public static string ConnectionString;
    public static User CurrentUser;
    public static Logger Logger;
    public static EmailSender EmailSender;
    // ... 50 campos más
}
```

```csharp
// ✅ CORRECTO — Dependencias explícitas mediante DI
public class UserService
{
    private readonly IEmailSender _emailSender;

    public UserService(IEmailSender emailSender) => _emailSender = emailSender;
}
```

### Error 4: Partial sin necesidad real

```csharp
// ❌ INCORRECTO — Una clase de 30 líneas en 3 archivos
// Archivo1.cs
partial class Calculator { public int Add(int a, int b) => a + b; }
// Archivo2.cs
partial class Calculator { public int Sub(int a, int b) => a - b; }
// Archivo3.cs
partial class Calculator { public int Mul(int a, int b) => a * b; }
```

```csharp
// ✅ CORRECTO — Un solo archivo para una clase pequeña
class Calculator
{
    public int Add(int a, int b) => a + b;
    public int Sub(int a, int b) => a - b;
    public int Mul(int a, int b) => a * b;
}
```

### Error 5: Partial method sin implementación esperando que haga algo

```csharp
// ❌ INCORRECTO — OnStart nunca se implementó, la llamada se elimina
partial class Worker
{
    partial void OnStart();
    public void Run() { OnStart(); /* Esta línea se ELIMINA del IL */ }
}
// OnStart nunca recibió implementación → el compilador la borra.
```

```csharp
// ✅ CORRECTO — Si necesitas un hook obligatorio, usa abstract
abstract class Worker
{
    protected abstract void OnStart(); // Obligatorio para las subclases
    public void Run() { OnStart(); }
}
```

---

## 7. Ejercicio guiado

**Objetivo:** Construir un sistema de configuración y perfiles de usuario aplicando `sealed`, `static` y `partial`.

### Paso 1: Clase estática de configuración

Crea `AppSettings` con propiedades estáticas cargadas desde un diccionario y un constructor estático que establezca valores por defecto:

```csharp
public static class AppSettings
{
    private static readonly Dictionary<string, string> s_values;

    public static string Theme => GetValue("Theme", "Claro");
    public static string Language => GetValue("Language", "es");
    public static int ItemsPerPage => int.Parse(GetValue("ItemsPerPage", "25"));

    // Constructor estático: carga valores por defecto
    static AppSettings()
    {
        s_values = new Dictionary<string, string>
        {
            ["Theme"] = "Claro",
            ["Language"] = "es",
            ["ItemsPerPage"] = "25"
        };
        Console.WriteLine("AppSettings inicializado con valores por defecto.");
    }

    public static string GetValue(string key, string defaultValue)
        => s_values.TryGetValue(key, out string? value) ? value : defaultValue;

    public static void Override(string key, string value)
        => s_values[key] = value;
}
```

### Paso 2: Clase parcial UserProfile

Divide `UserProfile` en tres archivos lógicos:

```csharp
// ── Archivo: UserProfile.Datos.cs ──
public partial class UserProfile
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public DateTime BirthDate { get; set; }

    public string GetFullName() => $"{FirstName} {LastName}";
}

// ── Archivo: UserProfile.Preferencias.cs ──
public partial class UserProfile
{
    public string Theme { get; set; } = "Claro";
    public bool NotificationsEnabled { get; set; } = true;
    public string Email { get; set; } = "";

    public void ApplyTheme(string theme) => Theme = theme;
}

// ── Archivo: UserProfile.Validacion.cs ──
public partial class UserProfile
{
    partial void OnValidating(); // Hook parcial

    public bool Validate()
    {
        OnValidating(); // Si se implementa, se ejecuta

        if (string.IsNullOrWhiteSpace(FirstName)) return false;
        if (string.IsNullOrWhiteSpace(LastName)) return false;
        if (BirthDate > DateTime.Today) return false;

        return true;
    }
}
```

### Paso 3: Perfil sellado de administrador

```csharp
// AdminProfile hereda de UserProfile pero no permite más herencia
public sealed class AdminProfile : UserProfile
{
    public string AdminCode { get; set; } = "";
    public DateTime GrantedAt { get; set; } = DateTime.UtcNow;
}

// ❌ Esto no compilaría:
// public class SuperAdminProfile : AdminProfile { } // Error CS0509
```

### Paso 4: Programa principal

```csharp
// Usar AppSettings sin instanciar
Console.WriteLine($"Tema: {AppSettings.Theme}");
Console.WriteLine($"Idioma: {AppSettings.Language}");

AppSettings.Override("Theme", "Oscuro");
Console.WriteLine($"Tema actualizado: {AppSettings.Theme}");

// Crear un usuario con propiedades de todas las partes del partial
var user = new UserProfile
{
    FirstName = "María",
    LastName = "González",
    Email = "maria@email.com",
    Theme = "Oscuro"
};
Console.WriteLine($"Usuario: {user.GetFullName()}, Tema: {user.Theme}");
Console.WriteLine($"¿Válido?: {user.Validate()}");

// AdminProfile sellado: no tiene subtipos
var admin = new AdminProfile
{
    FirstName = "Carlos",
    LastName = "Admin",
    AdminCode = "ADM-001"
};
Console.WriteLine($"Admin: {admin.GetFullName()}, Código: {admin.AdminCode}");
```

### Ejecutar:

```bash
dotnet run --project ejemplos/m02-poo/Leccion-04-Sealed-Static-Partial/src/PooModifiers
```

---

## 8. Reto

Diseña tres componentes aplicando los modificadores de esta lección:

**1. Calculadora genérica sellada:**

```csharp
// Requisito: solo operaciones Add, Subtract, Multiply, Divide.
// No debe permitir herencia.
// Debe funcionar con cualquier tipo numérico (int, double, decimal).
// Pista: interfaz INumber<T> (.NET 7+), constraint : INumber<T>
public static class Calculator<T> where T : INumber<T> { ... }
```

**2. Caché sellada thread-safe:**

```csharp
// Requisito: almacenar pares clave-valor con límite de elementos.
// No debe permitir herencia (seguridad: nadie puede alterar el almacenamiento).
// Debe ser thread-safe para Add, Get, Remove.
// Pista: ConcurrentDictionary + sealed class.
public sealed class Cache<TKey, TValue> where TKey : notnull { ... }
```

**3. Pipeline de datos con partial:**

```csharp
// Requisito: separar DataPipeline en tres archivos partial:
//   - DataPipeline.Configuracion.cs → fuentes de datos, filtros activos
//   - DataPipeline.Procesamiento.cs → lógica de transformación
//   - DataPipeline.Logging.cs     → partial methods para hooks de log
// Cada archivo es responsabilidad de un equipo distinto (simulación).
```

Intenta implementar al menos dos de los tres componentes. El código debe compilar y ejecutarse.

---

## 9. Resumen

| Modificador | Aplica a | Efecto | Caso de uso típico |
|---|---|---|---|
| `sealed` | Clases | Impide que otras clases hereden de ella | Entidades de dominio, records finales, clases de seguridad |
| `sealed override` | Métodos | Bloquea la cadena de sobrescritura en ese punto | Métodos con comportamiento crítico que no debe alterarse |
| `static` | Clases | No se puede instanciar, solo miembros estáticos | Utilidades, helpers, constantes, métodos de extensión |
| `static` (constructor) | Clase | Se ejecuta UNA vez antes del primer uso | Cargar configuración, inicializar recursos compartidos |
| `static` (miembros) | Campos/propiedades/métodos | Compartido entre todas las instancias | Contadores, caches, singletons, pools |
| `partial` | Clases/structs/interfaces | Definición repartida en varios archivos, unidas en compilación | Código generado + manual, clases grandes |
| `partial` (métodos) | Métodos void | Hook opcional: sin implementación = llamada eliminada del IL | Source Generators, hooks condicionales |

**Reglas de oro:**

- **Sellar por defecto, abrir solo si el diseño lo requiere.**
- **Static = sin estado mutable.** Si necesitas estado, usa instancias con DI.
- **Partial = necesidad real.** No abuses para clases pequeñas.
- **El constructor estático es un `lock` implícito.** Mantenlo ligero y sin I/O.

---

## 10. Preguntas frecuentes

**P: ¿Sealed mejora el rendimiento?**
R: Sí. El JIT puede aplicar devirtualization: si sabe que un tipo es `sealed`, convierte una llamada virtual (`callvirt` en IL, búsqueda en vtable) en una llamada directa (`call`) e incluso hacer inline del método. En puntos calientes de rendimiento, sellar clases puede dar mejoras medibles. Usa BenchmarkDotNet para verificarlo en tu caso concreto.

**P: ¿Puedo tener una clase `static` dentro de otra clase?**
R: Sí, desde C# 8. Las clases anidadas pueden ser `static`. Ejemplo:

```csharp
public class Outer
{
    public static class NestedHelper
    {
        public static int Double(int x) => x * 2;
    }
}
// Uso: Outer.NestedHelper.Double(5)
```

**P: ¿Puedo heredar de una clase `sealed` usando reflection?**
R: No. `sealed` es una restricción a nivel de IL (metadata de assembly). El runtime verifica esta restricción al cargar el tipo y al ejecutar `new`. Ni reflection, ni `Emit`, ni `dynamic` pueden saltarse la restricción `sealed`. Internamente, el CLR trata las clases sealed como `final` en términos de metadata de tipo.

**P: ¿Los `partial methods` pueden tener valor de retorno?**
R: Sí, desde C# 9. Pueden devolver `bool`, `string`, etc. y aceptar parámetros `out`. Pero si el partial method no tiene implementación, el código que lo llama debe compilar también sin ella (el compilador no puede adivinar el valor de retorno). Por tanto, solo se permiten retornos si hay implementación.

**P: ¿Cuándo un `partial method` sin implementación se elimina realmente?**
R: En tiempo de compilación. El compilador de C# elimina la instrucción `call` del IL generado. Es cero coste en runtime: no hay salto, no hay retorno, no hay stack frame. Es como si la llamada nunca hubiera existido en el código fuente.

**P: ¿Puede una clase `static` implementar una interfaz?**
R: Sí, desde C# 11. Las clases estáticas pueden implementar interfaces. Esto es útil para genéricos matemáticos (`static class Calculator<T> where T : INumber<T>`), pero el miembro de interfaz que implementes debe ser también estático.

**P: ¿Por qué mi campo `static` se reinicia en los tests?**
R: Porque los campos `static` viven durante la vida del `AppDomain`/`AssemblyLoadContext`. Los tests se ejecutan dentro del mismo proceso y los campos estáticos sobreviven entre tests. Esto causa acoplamiento entre tests (un test modifica el estado, el siguiente lo ve). **Solución:** no uses campos estáticos mutables para lógica de negocio. Usa instancias con DI o limpia el estado en `IDisposable` del test.

**P: ¿Qué diferencia hay entre `static readonly` y `const`?**
R:

| | `const` | `static readonly` |
|---|---|---|
| Cuándo se evalúa | Tiempo de compilación | Tiempo de ejecución |
| Dónde se almacena | Inline en el código IL del llamante | En el tipo que lo declara |
| Tipos permitidos | Solo primitivos, string, enum | Cualquier tipo |
| Cambiar sin recompilar | ❌ Requiere recompilar todo | ✅ Basta con recompilar el assembly |

Prefiere `static readonly` para valores que podrían cambiar entre versiones de una librería.

---

**Siguiente:** Lección 05 — Genéricos, delegados y eventos.
