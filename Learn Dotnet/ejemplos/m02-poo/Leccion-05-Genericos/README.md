# Lección 5 — Genéricos en C#

---

## 1. Introducción

Los **genéricos** permiten escribir código reutilizable y **type-safe** sin sacrificar rendimiento. Aparecieron en **C# 2.0** como respuesta al problema de `ArrayList` y otras colecciones basadas en `object`:

- **Sin genéricos**: cada inserción requiere **boxing** (envolver un `int` en un `object` en el heap), cada lectura requiere **unboxing** y un **cast** manual, y nada impide meter un `string` dentro de una lista pensada para `int`.
- **Con genéricos**: el compilador garantiza que solo puedes guardar el tipo declarado. No hay boxing de tipos de valor, no hay casts, y el código resultante es igual de rápido que si hubieras escrito una clase específica para `int`.

El **propósito** de los genéricos es triple:
1. **Seguridad de tipos en tiempo de compilación** — detectas errores antes de ejecutar.
2. **Reutilización de código** — una sola clase `List<T>` sirve para `List<int>`, `List<string>`, `List<Post>`.
3. **Rendimiento** — evita el boxing/unboxing de tipos de valor y elimina los casts en tiempo de ejecución.

**Cuándo se utilizan**: cada vez que escribas una colección, un repositorio, un validador, un comparador, un mapper o cualquier componente cuya lógica no dependa del tipo concreto de los datos que maneja.

**Errores típicos**:
- Asumir que `List<Perro>` es un `List<Animal>` (no lo es — las clases son invariantes).
- Comparar `T == default` con `==` en lugar de `EqualityComparer<T>.Default`.
- Crear genéricos excesivamente complejos con 5 parámetros de tipo y 6 constraints "por si acaso".

---

## 2. Teoría

### 2.1 Clases genéricas

```csharp
// Una clase genérica toma uno o más parámetros de tipo entre <>
public class List<T>
{
    public void Add(T item) { /* ... */ }
    public T this[int index] { get { /* ... */ } }
}
```

En **C# los genéricos son reificados**: el runtime conoce el tipo exacto en tiempo de ejecución. `typeof(List<int>)` y `typeof(List<string>)` son tipos **diferentes**.

Esto contrasta con **Java**, que usa **type erasure** (borrado de tipos): en Java, `List<Integer>` y `List<String>` son el mismo tipo en runtime (`List`). El compilador de Java elimina la información del tipo genérico, lo que impide cosas como `typeof(T)` o `new T()` sin pasar una `Class<T>` manualmente. C# no tiene esta limitación.

```
┌──────────────────────────────────────────────────────┐
│               Genéricos en C# (reificación)          │
│                                                      │
│  Código fuente:        List<int>                     │
│       │                                              │
│       ▼                                              │
│  IL (MSIL):            class List`1<int>             │
│       │                                              │
│       ▼                                              │
│  Runtime:              Tipo concreto: List<int>      │
│                        typeof(List<int>) funciona    │
│                        sizeof(T) funciona            │
│                        default(T) funciona           │
│                        new T() funciona (con new())  │
└──────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────┐
│               Genéricos en Java (type erasure)       │
│                                                      │
│  Código fuente:        List<Integer>                 │
│       │                                              │
│       ▼                                              │
│  Bytecode:             List (sin información de tipo)│
│       │                                              │
│       ▼                                              │
│  Runtime:              Solo existe List<Object>      │
│                        typeof(T) NO funciona         │
│                        new T() NO funciona           │
└──────────────────────────────────────────────────────┘
```

**Optimización de runtime**: el CLR comparte el código JIT entre `List<string>`, `List<FileInfo>` y todos los tipos referencia (todos son punteros del mismo tamaño). Para tipos valor (`List<int>`, `List<decimal>`) genera una versión separada porque cada uno tiene distinto tamaño y necesita instrucciones máquina distintas.

### 2.2 Métodos genéricos

Un método puede declarar sus propios parámetros de tipo, independientes de la clase que lo contiene:

```csharp
public static void Swap<T>(ref T a, ref T b)
{
    T temp = a;
    a = b;
    b = temp;
}

// Uso con inferencia de tipos — el compilador deduce T=int
int x = 5, y = 10;
Swap(ref x, ref y); // No necesitas Swap<int>(ref x, ref y)

// También puedes ser explícito si la inferencia no es posible
Swap<string>(ref s1, ref s2);
```

La **inferencia de tipos** funciona analizando los tipos de los argumentos. Solo falla si T no aparece en ningún parámetro de entrada (por ejemplo, si T solo aparece como tipo de retorno).

### 2.3 Interfaces genéricas

Las interfaces más importantes del BCL son genéricas:

| Interfaz | Propósito |
|---|---|
| `IEnumerable<T>` | Iterar sobre una colección de T |
| `IList<T>` | Acceso por índice a una colección de T |
| `IDictionary<TKey, TValue>` | Acceso por clave a valores |
| `IComparable<T>` | Comparar this con otro T |
| `IComparer<T>` | Comparar dos T externamente |
| `IEquatable<T>` | Comparar igualdad con otro T |

### 2.4 Constraints (`where`)

Las constraints restringen qué tipos pueden usarse como argumento de tipo. Se colocan con la palabra `where`:

```csharp
// T debe ser un tipo referencia (clase)
class Repository<T> where T : class { }

// T debe ser un tipo valor (struct)
class ValueWrapper<T> where T : struct { }

// T debe tener constructor sin parámetros
class Factory<T> where T : new()
{
    public T Create() => new T(); // Válido gracias a new()
}

// T debe heredar de EntityBase
class Repository<T> where T : EntityBase { }

// T debe implementar IComparable<T>
class SortedContainer<T> where T : IComparable<T> { }

// Múltiples constraints combinadas
class GenericService<T> where T : EntityBase, IComparable<T>, new() { }
```

**Jerarquía de constraints**:

```
┌─────────────────────────────────────────────────┐
│              where T : ...                       │
│                                                 │
│  class          ← tipo referencia               │
│  ┌───────────────────────────┐                 │
│  │ class?       ← tipo ref anulable (C# 8+)    │
│  │ BaseClass    ← hereda de BaseClass           │
│  │ IInterface   ← implementa IInterface         │
│  └───────────────────────────┘                 │
│                                                 │
│  struct         ← tipo valor (incluye Nullable) │
│  ┌───────────────────────────┐                 │
│  │ new()         ← constructor sin parámetros   │
│  │ unmanaged     ← tipo no gestionado (blittable)│
│  │ notnull       ← no puede ser null            │
│  └───────────────────────────┘                 │
│                                                 │
│  default        ← sin restricción               │
└─────────────────────────────────────────────────┘
```

### 2.5 Covarianza (`out T`)

La **covarianza** permite usar un tipo **más derivado** donde se espera uno **más general** en contextos de **solo salida** (T solo aparece como tipo de retorno).

```csharp
// IEnumerable<T> está marcado como covariante: public interface IEnumerable<out T>
IEnumerable<string> strings = new List<string> { "a", "b", "c" };
IEnumerable<object> objects = strings; // ✅ Válido: string → object

// Pero List<T> NO es covariante (porque T aparece en métodos como Add(T))
IList<string> stringList = new List<string> { "a", "b" };
// IList<object> objectList = stringList; // ❌ Error de compilación
```

**Regla**: `out T` solo puede usarse en posiciones de **salida** (tipos de retorno, propiedades get). Si marcas una interfaz con `out T` e intentas poner T en un parámetro de entrada, el compilador dará error.

### 2.6 Contravarianza (`in T`)

La **contravarianza** permite usar un tipo **más general** donde se espera uno **más derivado** en contextos de **solo entrada** (T solo aparece como parámetro).

```csharp
// IComparer<T> está marcado como contravariante: public interface IComparer<in T>
IComparer<object> objectComparer = new ObjectSizeComparer();
IComparer<string> stringComparer = objectComparer; // ✅ Válido: object → string

// Uso: si un comparador puede comparar cualquier object, también puede comparar strings
```

**Regla**: `in T` solo puede usarse en posiciones de **entrada** (parámetros de métodos). Si marcas una interfaz con `in T` e intentas poner T como tipo de retorno, el compilador dará error.

### 2.7 Resumen de varianza

| Varianza | Palabra clave | Dirección | T solo aparece en... | Ejemplo |
|---|---|---|---|
| **Invarianza** | *(sin marcador)* | Sin conversión | Entrada y salida | `List<T>`, `Dictionary<TKey, TValue>` |
| **Covarianza** | `out T` | `Derivado → Base` | Salida (returns, get) | `IEnumerable<T>`, `IReadOnlyList<T>` |
| **Contravarianza** | `in T` | `Base → Derivado` | Entrada (parámetros) | `IComparer<T>`, `Action<T>` |

**Analogía intuitiva**:
- ***Covarianza*** (`out`): "lo que sale puede ser más específico" — una caja que produce manzanas puede tratarse como una caja que produce frutas.
- ***Contravarianza*** (`in`): "lo que entra puede ser más general" — un triturador que acepta frutas puede aceptar manzanas también.
- ***Invarianza***: cuando la caja permite tanto meter como sacar — no puedes sustituir `Caja<Manzana>` por `Caja<Fruta>` porque alguien podría meter una Pera.

### 2.8 Reificación en profundidad

La reificación (del inglés *reification*, "hacer real") significa que la información del tipo sobrevive en tiempo de ejecución. Consecuencias prácticas:

```csharp
// typeof funciona con T
public string GetTypeName<T>() => typeof(T).Name;

// default(T) funciona para cualquier T
public T CreateDefault<T>() => default(T); // null para clases, 0 para int, etc.

// new T() funciona si hay constraint new()
public T CreateInstance<T>() where T : new() => new T();

// Los arrays genéricos funcionan
T[] array = new T[10]; // Válido siempre

// Puedes hacer overload por tipo genérico (sobrecarga)
void Process<T>(List<T> items) { }
void Process<T>(Dictionary<string, T> items) { } // Sobrecarga válida
```

---

## 3. Ejemplo mínimo

```csharp
// ─── Clase genérica más simple posible ───
// Box<T> envuelve un valor de cualquier tipo T
public class Box<T>
{
    public T Value { get; set; }

    public Box(T value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return $"Box contiene: {Value}";
    }
}

// ─── Método genérico independiente ───
// Devuelve el mayor de dos valores que implementan IComparable<T>
public static T Max<T>(T a, T b) where T : IComparable<T>
{
    return a.CompareTo(b) > 0 ? a : b;
}

// ─── Uso ───
Box<int> intBox = new Box<int>(42);
Console.WriteLine(intBox);              // "Box contiene: 42"

Box<string> strBox = new Box<string>("hola");
Console.WriteLine(strBox);              // "Box contiene: hola"

int mayorInt = Max(5, 10);             // 10 (inferencia: T=int)
string mayorStr = Max("abc", "xyz");   // "xyz" (inferencia: T=string)

// ─── typeof muestra que son tipos distintos ───
Console.WriteLine(typeof(Box<int>) == typeof(Box<string>)); // False
```

**Explicación línea por línea**:

1. `public class Box<T>` — Declara una clase genérica con un parámetro de tipo `T`. `T` es un marcador de posición que se sustituirá por un tipo real cuando alguien use `Box<int>` o `Box<string>`.
2. `public T Value { get; set; }` — La propiedad usa `T`. Si `Box<int>`, `Value` es `int`. Si `Box<string>`, `Value` es `string`.
3. `public Box(T value)` — El constructor recibe y almacena un valor del tipo `T`.
4. `public static T Max<T>(T a, T b) where T : IComparable<T>` — Método genérico con constraint. La constraint `IComparable<T>` permite llamar a `CompareTo`.
5. `Max(5, 10)` — La inferencia de tipos deduce `T=int` a partir de los argumentos. No necesitas `Max<int>(5, 10)`.
6. `typeof(Box<int>) == typeof(Box<string>)` — Demuestra la reificación: son tipos distintos en runtime.

---

## 4. Ejemplo real

Consulta el código en `src/PooGenerics/Program.cs`. Este programa demuestra:

### 4.1 `Repository<T>` — Repositorio genérico con constraints

```csharp
// Solo permite entidades que hereden de EntityBase y tengan constructor sin parámetros
class Repository<T> where T : EntityBase, new()
{
    private readonly List<T> _items = new();

    public void Add(T item) => _items.Add(item);
    public T? GetById(int id) => _items.FirstOrDefault(i => i.Id == id);
    public T Update(T item)
    {
        var index = _items.FindIndex(i => i.Id == item.Id);
        if (index == -1) throw new KeyNotFoundException($"Entidad con Id={item.Id} no encontrada");
        _items[index] = item;
        return item;
    }
    public bool Delete(int id) => _items.RemoveAll(i => i.Id == id) > 0;
    public IReadOnlyList<T> GetAll() => _items.AsReadOnly();
}
```

La constraint `where T : EntityBase, new()` permite acceder a `item.Id` (definido en `EntityBase`) y crear instancias con `new T()` (gracias a `new()`).

### 4.2 Métodos genéricos de utilidad

```csharp
// Swap genérico — intercambia valores de cualquier tipo por referencia
public static void Swap<T>(ref T a, ref T b)
{
    T temp = a;
    a = b;
    b = temp;
}

// FirstOrDefault genérico — implementación didáctica de lo que hace LINQ
public static T? FirstOrDefault<T>(IEnumerable<T> source, Func<T, bool> predicate)
{
    foreach (var item in source)
    {
        if (predicate(item))
            return item;
    }
    return default; // null para tipos referencia, 0 para int, false para bool...
}
```

### 4.3 `IValidator<T>` — Interfaz genérica con múltiples implementaciones

```csharp
interface IValidator<in T> // Contravarianza: un validador de EntityBase puede validar User
{
    ValidationResult Validate(T entity);
}

class GenericValidator<T> : IValidator<T> where T : EntityBase
{
    public ValidationResult Validate(T entity)
    {
        // Validación común para todas las entidades (Id > 0, etc.)
    }
}
```

La contravarianza `in T` permite que `IValidator<EntityBase>` se use como `IValidator<User>` porque User hereda de EntityBase.

### 4.4 Varianza en repositorios de solo lectura/escritura

```csharp
// Covariante: solo expone T en salida (getters)
interface IReadOnlyRepository<out T>
{
    T? GetById(int id);
    IEnumerable<T> GetAll();
}

// Contravariante: solo expone T en entrada (parámetros)
interface IWriteOnlyRepository<in T>
{
    void Add(T item);
    void Update(T item);
}

// El repositorio completo combina ambas (es invariante porque T aparece en ambas direcciones)
interface IRepository<T> : IReadOnlyRepository<T>, IWriteOnlyRepository<T>
{
    bool Delete(int id);
}
```

Esta separación permite:
```csharp
IReadOnlyRepository<User> users = repo;
IReadOnlyRepository<EntityBase> entities = users; // ✅ Covarianza: User → EntityBase

IWriteOnlyRepository<EntityBase> writer = repo;
IWriteOnlyRepository<User> userWriter = writer;     // ✅ Contravarianza: EntityBase → User
```

### 4.5 Ejecución del ejemplo

```bash
dotnet run --project ejemplos/m02-poo/Leccion-05-Genericos/src/PooGenerics
```

---

## 5. Buenas prácticas

### ✅ Hacer

1. **Nombrar parámetros de tipo con T seguido de nombre descriptivo**:
   ```csharp
   // Bien: descriptivo
   interface IRepository<TEntity> where TEntity : EntityBase { }
   Dictionary<TKey, TValue> dictionary;

   // Mal: T genérico sin contexto cuando hay múltiples
   interface IRepository<A, B, C> { } // ¿Qué es A, B, C?
   ```

2. **Usar constraints para documentar expectativas y habilitar operaciones**:
   ```csharp
   // Sin constraint, no puedes hacer nada útil con T
   class Sorter<T> where T : IComparable<T> // Ahora puedes comparar
   { }

   class Factory<T> where T : new() // Ahora puedes instanciar
   {
       public T Create() => new T();
   }
   ```

3. **Preferir genéricos sobre `object` para type safety y rendimiento**:
   ```csharp
   // Mal: boxing, casts, sin type safety
   ArrayList list = new ArrayList();
   list.Add(42);           // Boxing
   int x = (int)list[0];   // Unboxing + cast manual
   list.Add("error");      // El compilador no avisa

   // Bien: con genéricos
   List<int> list = new List<int>();
   list.Add(42);           // Sin boxing (int se guarda en el array interno)
   int x = list[0];        // Sin cast
   // list.Add("error");   // ❌ Error de compilación — type safety
   ```

4. **Marcar interfaces como covariantes (`out`) o contravariantes (`in`) cuando sea seguro**:
   ```csharp
   interface IEnumerable<out T> { }     // Solo devuelve T → covariante
   interface IComparer<in T> { }        // Solo recibe T → contravariante
   interface IEqualityComparer<in T> { } // Solo recibe T → contravariante
   ```

5. **Usar `EqualityComparer<T>.Default` en lugar de `==` dentro de código genérico**:
   ```csharp
   void Find<T>(List<T> items, T target)
   {
       // Mal: == puede no funcionar o llamar a Object.ReferenceEquals
       // if (item == target) { ... }

       // Bien
       var comparer = EqualityComparer<T>.Default;
       if (comparer.Equals(item, target)) { }
   }
   ```

### ❌ Evitar

1. **Evitar genéricos anidados profundos**: más de 2-3 niveles se vuelve ilegible.
   ```csharp
   // Pesadilla de leer
   Dictionary<string, List<Task<Result<IEnumerable<User>>>>> chaos;

   // Mejor: crea alias o tipos intermedios con nombres descriptivos
   var userTasks = new List<Task<Result<IEnumerable<User>>>>();
   Dictionary<string, List<Task<Result<IEnumerable<User>>>>> organized;
   ```

2. **No crear genéricos "por si acaso"**: si solo usarás un tipo concreto, no necesitas un genérico. Refactoriza a genérico cuando aparezca el segundo caso.

3. **No aplicar `out`/`in` sin pensar**: si la interfaz algún día añadirá métodos que rompen la varianza, tendrás problemas de compatibilidad hacia atrás.

4. **No asumir que `T?` funciona igual para todos los T**: con `where T : class`, `T?` es nullable reference type; con `where T : struct`, `T?` es `Nullable<T>`; sin constraint, `T?` no está permitido para `return default`.

---

## 6. Errores habituales

### Error 1 — Usar `==` para comparar valores de tipo T

```csharp
// ❌ INCORRECTO
public bool Contains<T>(T[] items, T target)
{
    foreach (var item in items)
        if (item == target)  // Esto no compila o se resuelve a Object.ReferenceEquals
            return true;
    return false;
}

// ✅ CORRECTO
public bool Contains<T>(T[] items, T target)
{
    var comparer = EqualityComparer<T>.Default;
    foreach (var item in items)
        if (comparer.Equals(item, target))
            return true;
    return false;
}
```

**Por qué falla**: los operadores no son parte de las interfaces y no están disponibles en código genérico. `EqualityComparer<T>.Default` delega en `IEquatable<T>.Equals`, `object.Equals` o las comparaciones por defecto del runtime.

### Error 2 — Olvidar que `where T : struct` excluye Nullable<T>

```csharp
// ❌ ESTO NO COMPILA
public void Process<T>(T? value) where T : struct
{
    // Error: T? ya es Nullable<T>, pero where T : struct
    // incluye Nullable<T> como tipo válido para T, no como constraint
}

// ✅ CORRECTO: T? ya es Nullable<T> cuando T es struct
public void Process<T>(T? value) where T : struct { } // Esto SÍ compila en C# 8+

// ✅ CORRECTO: si quieres T anulable sin restricción, usa default(T?)
public T? GetDefault<T>()
{
    return default; // null para clases, default(T?) para structs
}
```

### Error 3 — Crear genéricos demasiado complejos

```csharp
// ❌ DEMASIADO COMPLEJO
class MegaHandler<TEntity, TDto, TFilter, TKey, TContext, TValidator>
    where TEntity : EntityBase, IAuditable, ISoftDeletable, new()
    where TDto : class, IDto<TEntity>
    where TFilter : IFilter<TEntity>
    where TKey : IEquatable<TKey>
    where TContext : DbContext, new()
    where TValidator : IValidator<TEntity>, IValidator<TDto>
{
    // ¿Qué hace esto siquiera?
}

// ✅ MEJOR: divide en componentes más pequeños.
//    Cada clase genérica debería tener 1-2 parámetros de tipo.
```

### Error 4 — No entender que `List<Dog>` NO es `List<Animal>`

```csharp
class Animal { }
class Dog : Animal { }

// ❌ ERROR DE COMPILACIÓN
List<Dog> dogs = new List<Dog> { new Dog() };
List<Animal> animals = dogs; // Error CS0266
// animals.Add(new Cat()); ← Si esto compilara, ¡romperías la type safety!

// ✅ CORRECTO: usa interfaz covariante
IEnumerable<Dog> dogs = new List<Dog> { new Dog() };
IEnumerable<Animal> animals = dogs; // Válido: IEnumerable<out T>
// animals.Add(new Cat()); ← No compila porque IEnumerable no tiene Add
```

### Error 5 — Over-engineering con genéricos

```csharp
// ❌ OVER-ENGINEERING — solo un tipo concreto usa esto
class StringProcessor<T> where T : string { } // Error: string es sealed

class SingleUseValidator<T> // Solo se usa con User
    where T : User
    , IValidatable
{
    // Simplemente no es genérico si solo tiene un uso real
}

// ✅ SIMPLE: escribe la versión concreta y generaliza cuando surja el segundo caso
class UserValidator
{
    public ValidationResult Validate(User user) { }
}
```

---

## 7. Ejercicio guiado

Vamos a construir paso a paso una `Pair<T1, T2>` y un `Repository<T>` con operaciones CRUD.

### Paso 1 — `Pair<T1, T2>`: clase genérica con dos parámetros de tipo

```csharp
// Representa un par de valores de tipos potencialmente distintos
public class Pair<T1, T2>
{
    public T1 First { get; }
    public T2 Second { get; }

    public Pair(T1 first, T2 second)
    {
        First = first;
        Second = second;
    }

    // Método de instancia que crea un nuevo par con los valores intercambiados
    public Pair<T2, T1> Swap()
    {
        return new Pair<T2, T1>(Second, First);
    }

    // Compara este par con otro usando EqualityComparer<T>.Default
    public bool Equals(Pair<T1, T2> other)
    {
        if (other is null) return false;

        var comparer1 = EqualityComparer<T1>.Default;
        var comparer2 = EqualityComparer<T2>.Default;

        return comparer1.Equals(First, other.First)
            && comparer2.Equals(Second, other.Second);
    }

    // Sobrescribe Equals de object
    public override bool Equals(object? obj)
    {
        return obj is Pair<T1, T2> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(First, Second);
    }

    public override string ToString()
    {
        return $"({First}, {Second})";
    }
}
```

**Puntos importantes**:
- `Swap()` devuelve `Pair<T2, T1>` — los parámetros de tipo se invierten en el tipo de retorno.
- Usamos `EqualityComparer<T>.Default.Equals()` en lugar de `==` para soportar cualquier tipo.
- `HashCode.Combine()` genera un hash combinado de forma eficiente (disponible desde .NET Standard 2.1).

### Paso 2 — Métodos genéricos de utilidad

```csharp
// Método estático genérico: crea un Pair con inferencia de tipos
public static class Pair
{
    // Factory method: evita tener que escribir new Pair<int, string>(42, "hola")
    public static Pair<T1, T2> Create<T1, T2>(T1 first, T2 second)
    {
        return new Pair<T1, T2>(first, second);
    }
}

// Uso:
var pair = Pair.Create(42, "hola");       // Pair<int, string>
var swapped = pair.Swap();                // Pair<string, int>
Console.WriteLine(pair);                  // (42, hola)
Console.WriteLine(swapped);               // (hola, 42)
Console.WriteLine(pair.Equals(swapped));  // False
```

### Paso 3 — `Repository<T>`: repositorio CRUD en memoria

```csharp
// Entidad base — todas las entidades del repositorio deben heredar de esta
public abstract class EntityBase
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// Repositorio genérico con constraints
public class Repository<T> : IReadOnlyRepository<T>, IWriteOnlyRepository<T>
    where T : EntityBase, new()
{
    private readonly List<T> _items = new();

    // ─── Operaciones de escritura ───
    public void Add(T item)
    {
        // Si no se asignó Id, le damos el siguiente
        if (item.Id == 0)
        {
            item.Id = _items.Count > 0 ? _items.Max(i => i.Id) + 1 : 1;
        }
        item.CreatedAt = DateTime.UtcNow;
        _items.Add(item);
    }

    public void Update(T item)
    {
        var index = _items.FindIndex(i => i.Id == item.Id);
        if (index == -1)
            throw new KeyNotFoundException(
                $"No se encontró {typeof(T).Name} con Id={item.Id}");
        _items[index] = item;
    }

    public bool Delete(int id)
    {
        return _items.RemoveAll(i => i.Id == id) > 0;
    }

    // ─── Operaciones de lectura ───
    public T? GetById(int id)
    {
        return _items.FirstOrDefault(i => i.Id == id);
    }

    public IReadOnlyList<T> GetAll()
    {
        return _items.AsReadOnly(); // Devuelve envoltura inmutable
    }

    // ─── Consultas filtradas ───
    public IEnumerable<T> Find(Func<T, bool> predicate)
    {
        return _items.Where(predicate);
    }

    public int Count => _items.Count;
}

// ─── Interfaces con varianza ───
public interface IReadOnlyRepository<out T>
{
    T? GetById(int id);
    IReadOnlyList<T> GetAll();
}

public interface IWriteOnlyRepository<in T>
{
    void Add(T item);
    void Update(T item);
}
```

### Paso 4 — Probar el repositorio

```csharp
// Entidad de ejemplo
public class User : EntityBase
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public override string ToString() => $"User({Id}): {Username} ({Email})";
}

// ─── Uso ───
var userRepo = new Repository<User>();

// Agregar usuarios
userRepo.Add(new User { Username = "alice", Email = "alice@example.com" });
userRepo.Add(new User { Username = "bob", Email = "bob@example.com" });

// Buscar por Id
var alice = userRepo.GetById(1);
Console.WriteLine(alice); // User(1): alice (alice@example.com)

// Listar todos
foreach (var user in userRepo.GetAll())
    Console.WriteLine(user);

// Covarianza: IReadOnlyRepository<User> → IReadOnlyRepository<EntityBase>
IReadOnlyRepository<User> readUsers = userRepo;
IReadOnlyRepository<EntityBase> readEntities = readUsers; // ✅

// Contravarianza: IWriteOnlyRepository<EntityBase> → IWriteOnlyRepository<User>
IWriteOnlyRepository<EntityBase> writeEntities = userRepo;
IWriteOnlyRepository<User> writeUsers = writeEntities; // ✅
```

---

## 8. Reto

Implementa un **pipeline de transformación** usando covarianza y contravarianza.

### Requisitos

1. Define una interfaz `ITransformer<in TInput, out TOutput>` con un método `TOutput Transform(TInput input)`.

2. Crea una clase `Pipeline<TInput, TOutput>` que:
   - Tenga un método `Then<TNext>(ITransformer<TOutput, TNext> next)` que devuelva un nuevo `Pipeline<TInput, TNext>`.
   - Tenga un método `TOutput Execute(TInput input)` que ejecute todas las transformaciones encadenadas.

3. Implementa al menos 3 transformadores concretos:
   - `StringToIntTransformer : ITransformer<string, int>` — convierte "42" → 42.
   - `IntToDoubleTransformer : ITransformer<int, double>` — multiplica por 2.5.
   - `DoubleToBoolTransformer : ITransformer<double, bool>` — devuelve true si > 100.

4. Encadénalos en un pipeline: `string → int → double → bool`.

5. Aprovecha la **covarianza** para tratar un `Pipeline<string,int>` como `ITransformer<string,object>` y la **contravarianza** para que un transformador que acepta `object` también acepte `string`.

**Pistas**:
- `out TOutput` permite que `ITransformer<string, int>` sea convertible a `ITransformer<string, object>`.
- `in TInput` permite que `ITransformer<object, bool>` sea convertible a `ITransformer<string, bool>`.
- Para `Then<TNext>`, necesitas guardar la lista de transformadores (puedes usar `List<object>` o una lista tipada) y ejecutarlos en orden.

**Ejemplo de uso esperado**:

```csharp
var pipeline = new Pipeline<string, int>(new StringToIntTransformer())
    .Then(new IntToDoubleTransformer())
    .Then(new DoubleToBoolTransformer());

bool result = pipeline.Execute("42");
Console.WriteLine(result); // Depende del threshold
```

¡Inténtalo antes de mirar la solución!

---

## 9. Resumen

### Conceptos clave

| Concepto | Sintaxis | Ejemplo |
|---|---|---|
| **Clase genérica** | `class Foo<T>` | `class List<T>`, `class Repository<TEntity>` |
| **Método genérico** | `void M<T>(T x)` | `void Swap<T>(ref T a, ref T b)` |
| **Interfaz genérica** | `interface IFoo<T>` | `interface IRepository<T>` |
| **Constraint `class`** | `where T : class` | Solo tipos referencia |
| **Constraint `struct`** | `where T : struct` | Solo tipos valor |
| **Constraint `new()`** | `where T : new()` | Debe tener constructor sin parámetros |
| **Constraint de herencia** | `where T : Base` | Debe heredar de `Base` |
| **Constraint de interfaz** | `where T : IFoo` | Debe implementar `IFoo` |
| **Múltiples constraints** | `where T : Base, IComparable<T>, new()` | Todas deben cumplirse |
| **Covarianza** | `interface IFoo<out T>` | `IEnumerable<string>` → `IEnumerable<object>` |
| **Contravarianza** | `interface IFoo<in T>` | `IComparer<object>` → `IComparer<string>` |
| **Inferencia de tipos** | `M(x, y)` sin `<T>` | El compilador deduce T de los argumentos |
| **`default(T)`** | `T x = default;` | null para clases, 0 para int, false para bool |

### Varianza visual

```
                    Covarianza (out T)
    ┌──────────┐          │          ┌──────────┐
    │  string  │ ────────►│────────► │  object  │
    └──────────┘   más    │  más     └──────────┘
                 derivado │ general

                    Contravarianza (in T)
    ┌──────────┐          │          ┌──────────┐
    │  object  │ ────────►│────────► │  string  │
    └──────────┘   más    │  más     └──────────┘
                 general  │ derivado

                    Invarianza (sin marcador)
    ┌──────────┐                     ┌──────────┐
    │  string  │ ────────►❌◄────── │  object  │
    └──────────┘   NO hay           └──────────┘
                 conversión
```

### Tabla comparativa de varianza

| Aspecto | Invarianza | Covarianza (`out`) | Contravarianza (`in`) |
|---|---|---|---|
| Conversión `Derivado → Base` | ❌ No | ✅ Sí | ❌ No |
| Conversión `Base → Derivado` | ❌ No | ❌ No | ✅ Sí |
| T en posición de retorno | ✅ | ✅ | ❌ |
| T en posición de parámetro | ✅ | ❌ | ✅ |
| Ejemplo | `List<T>` | `IEnumerable<T>` | `Action<T>` |

### ¿Cuándo usar cada herramienta?

| Situación | Solución |
|---|---|
| Clase que trabaja con muchos tipos distintos | Clase genérica `MyClass<T>` |
| Solo necesitas T en returns/getters | Interfaz covariante `out T` |
| Solo necesitas T en parámetros de entrada | Interfaz contravariante `in T` |
| Necesitas instanciar T dentro de la clase | Constraint `where T : new()` |
| Necesitas ordenar/comparar elementos de tipo T | Constraint `where T : IComparable<T>` |
| Necesitas asegurar que T es clase o struct | Constraint `where T : class` o `where T : struct` |

---

## 10. Preguntas frecuentes

### ¿Cuál es la diferencia entre genéricos de C# y templates de C++?

| Aspecto | C# Genéricos | C++ Templates |
|---|---|---|
| **Compilación** | El compilador genera una sola definición IL. El JIT genera código máquina específico para tipos valor, pero comparte código entre tipos referencia. | El compilador genera una copia completa del código para cada combinación de tipos (instanciación de template). |
| **Restricciones** | Constraints explícitas en la declaración (`where T : IComparable<T>`). | Duck typing implícito: el código compila si el tipo soporta las operaciones usadas (SFINAE). |
| **Sobrecarga de código** | Menor. Los tipos referencia comparten la misma implementación JIT. | Mayor (code bloat). Cada instanciación produce código objeto nuevo. |
| **Reificación** | Sí. `typeof(List<int>) != typeof(List<string>)` en runtime. | No aplica (cada instanciación es una clase distinta desde compilación). |
| **Especialización** | No existe. No puedes escribir `List<int>` con implementación distinta a `List<T>`. | Sí. Puedes escribir `template<> class Vector<bool> { ... }` con implementación especial. |
| **Expresiones constantes** | No. T no puede usarse en expresiones constantes. | Sí. Templates aceptan parámetros no-tipo (`template <int N>`). |

### ¿Puedo usar operadores aritméticos con T?

```csharp
// ❌ No compila antes de .NET 7
T Sum<T>(T a, T b) { return a + b; } // Error CS0019

// ✅ Desde .NET 7 con interfaces genéricas de matemáticas
T Sum<T>(T a, T b) where T : INumber<T>
{
    return a + b; // Funciona para int, double, decimal, etc.
}

// INumber<T> está en System.Numerics (NuGet o .NET 7+)
```

La familia `INumber<T>`, `IAdditionOperators<T, T, T>`, etc. proporcionan operadores aritméticos como constraints genéricas.

### ¿Cuándo usar genéricos vs herencia?

| Usa **genéricos** cuando... | Usa **herencia** cuando... |
|---|---|
| La variación es de **tipo de datos**: `List<int>` vs `List<string>` — misma operación, distinto tipo. | La variación es de **comportamiento**: `Animal.Speak()` se comporta distinto en `Dog` y `Cat`. |
| Quieres type safety en tiempo de compilación para colecciones y algoritmos. | Quieres polimorfismo en tiempo de ejecución. |
| El tipo concreto importa: `Repository<User>` no es `Repository<Customer>`. | El tipo base abstrae varios hijos: `Stream` → `FileStream`, `MemoryStream`. |
| Ejemplo: repositorios, validadores, comparadores, mappers. | Ejemplo: figuras geométricas, conexiones de BD, controladores. |

**No son mutuamente excluyentes**: puedes tener `Repository<T> where T : EntityBase` que combina ambos — el genérico maneja la variación de tipo y la constraint de herencia garantiza que T tiene ciertas propiedades.

### ¿`List<object>` puede almacenar `List<string>`?

```csharp
List<string> strings = new List<string> { "a", "b" };
// List<object> objects = strings; // ❌ NO compila (Error CS0029)

// Razón: List<T> es invariante. Si se permitiera la conversión, podrías hacer:
// objects.Add(42); // Meter un int en una lista de strings — type safety rota
```

**Alternativas correctas**:

```csharp
// 1. Usa una interfaz covariante (solo lectura)
IEnumerable<string> strings = new List<string> { "a", "b" };
IEnumerable<object> objects = strings; // ✅ Válido con covarianza de IEnumerable<out T>

// 2. Crea una copia con el tipo correcto (copia los elementos)
List<string> strings = new List<string> { "a", "b" };
List<object> objects = strings.Cast<object>().ToList(); // ✅ Copia, no referencia

// 3. Usa un tipo covariante propio
IReadOnlyList<string> strings = new List<string> { "a", "b" };
IReadOnlyList<object> objects = strings; // ❌ Tampoco funciona: IReadOnlyList NO es covariante
```

### ¿Por qué los arrays de C# son covariantes si es inseguro?

```csharp
string[] strings = new string[] { "a", "b" };
object[] objects = strings; // ✅ Compila (covarianza de arrays — legado de C# 1.0)
objects[0] = 42;            // ❌ ArrayTypeMismatchException en RUNTIME
```

La covarianza de arrays es un **defecto histórico** de C# 1.0 (heredado de Java). Se mantuvo por compatibilidad hacia atrás, pero es un agujero de type safety que se detecta en tiempo de ejecución en lugar de compilación. **Nunca** uses arrays como covariantes en código nuevo. Prefiere `IReadOnlyList<T>` o `IEnumerable<T>`.
