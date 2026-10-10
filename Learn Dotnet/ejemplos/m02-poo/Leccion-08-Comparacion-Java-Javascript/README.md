# Lección 08 — Comparación de POO: C# vs Java vs JavaScript/TypeScript

---

## 1. Introducción

Si vienes de Java, JavaScript/TypeScript o Python, gran parte de la Programación Orientada a Objetos te resultará familiar: clases, herencia, interfaces, polimorfismo... Los conceptos son universales. Pero **C# tiene diferencias fundamentales** que es importante entender para no escribir código "con acento Java" o "con acento JS".

El objetivo de esta lección no es enseñarte POO desde cero (eso lo cubrimos en las lecciones anteriores), sino **comparar los tres lenguajes lado a lado** en cada aspecto de POO para que entiendas:

- Qué es exactamente igual (y puedes transferir tu conocimiento sin fricción).
- Qué es parecido pero con diferencias sutiles (donde están las trampas).
- Qué es exclusivo de C# (y deberías aprovechar para escribir código más idiomático).

### ¿Por qué existe esta lección?

Porque el 90% de los desarrolladores que llegan a .NET vienen de Java o JavaScript. Y el error más común es **escribir C# como si fuera Java** (getters/setters manuales, asumir que todo es virtual, usar `new` para ocultar cuando querías `override`) o **escribir C# como si fuera JavaScript** (confiar en duck typing, no entender value types vs reference types, abusar de `dynamic`).

### Errores típicos al migrar

- **De Java a C#**: escribir `getName()`/`setName()` en lugar de propiedades, asumir que todos los métodos son virtuales, usar `new` cuando querías `override`, no aprovechar structs, pensar que los genéricos se borran en runtime.
- **De JavaScript/TypeScript a C#**: confiar en coerción de tipos, no entender que `==` en objetos compara referencias (no contenido), escribir código sin tipos, no usar LINQ y en su lugar hacer bucles `for` manuales, usar `dynamic` en lugar de tipos estáticos.
- **De Python a C#**: acostumbrarse a que las clases no son diccionarios glorificados, que no hay herencia múltiple de clases, y que los tipos importan en tiempo de compilación.

---

## 2. Teoría

### 2.1 Clases

| Aspecto | C# | Java | JavaScript | TypeScript |
|---|---|---|---|---|
| Palabra clave | `class` | `class` | `class` (ES6) | `class` |
| Mecanismo interno | Tipos reales en CLR | Tipos reales en JVM | Azúcar sintáctico sobre **prototype** | Azúcar sintáctico sobre **prototype** |
| Propiedades | `{ get; set; }` nativas | Getters/setters manuales | Campos públicos o `get`/`set` (ES5) | Campos o `get`/`set` |
| Constructor | `Clase(param) { }` o primary constructor (C# 12) | `Clase(param) { }` | `constructor(param) { }` | `constructor(param) { }` |
| Backing fields | Generados por el compilador | Manuales (o Lombok) | No existen | No existen |

**En profundidad**: en C#, una propiedad `public string Name { get; set; }` se compila a un campo privado `_name` más dos métodos `get_Name()` y `set_Name(string)`. Pero tú solo escribes una línea. En Java necesitas ~7 líneas por propiedad (campo + getter + setter). En JavaScript accedes directamente a los campos (`this.name`). En TypeScript puedes usar `constructor(public name: string) {}` como atajo.

```
C# (1 línea):                    Java (~7 líneas):
public string Name { get; set; } private String name;
                                 public String getName() { return name; }
                                 public void setName(String name) { this.name = name; }

TypeScript (1 línea):            JavaScript (sin encapsulación):
constructor(public name: string) {}    this.name = name;
```

### 2.2 Herencia

| Aspecto | C# | Java | JavaScript/TypeScript |
|---|---|---|---|
| Sintaxis | `class A : B` | `class A extends B` | `class A extends B` |
| Herencia múltiple de clases | **NO** (single inheritance) | **NO** (single inheritance) | **NO** (single prototype chain) |
| Herencia múltiple de interfaces | **SÍ** | **SÍ** | **SÍ** (TS verifica forma, no herencia real) |
| Llamar a constructor base | `: base(args)` | `super(args)` (1ª línea) | `super(args)` (1ª línea) |
| Clase base implícita | `System.Object` | `java.lang.Object` | Ninguna (prototype chain acaba en `null`) |

La diferencia fundamental está en la **cadena de prototipos** de JavaScript. Aunque la sintaxis `class` de ES6 parece idéntica, por debajo:

```javascript
// Esto que escribes en ES6:
class Employee extends Person { }

// Es azúcar sintáctico de esto en ES5:
function Employee() { Person.call(this); }
Employee.prototype = Object.create(Person.prototype);
Employee.prototype.constructor = Employee;
```

En C# y Java, la herencia está implementada en la **máquina virtual** (CLR/JVM) con tablas de métodos virtuales (vtables). En JavaScript, la herencia es una **cadena de objetos** vinculados por la propiedad interna `[[Prototype]]`.

### 2.3 Virtual Dispatch (la diferencia MÁS importante)

Esta es probablemente la diferencia más importante entre C# y Java:

| Aspecto | C# | Java | JavaScript |
|---|---|---|---|
| Métodos virtuales por defecto | **NO** — necesitas `virtual` explícito | **SÍ** — todos son virtuales | **SÍ** — cadena de prototipos |
| Sobrescribir | `override` (obligatorio) | `@Override` (anotación opcional) | Simplemente redefinir el método |
| Ocultar en lugar de sobrescribir | `new` (explícito, con warning si falta) | Sin distinción (siempre sobrescribe) | Siempre sobrescribe |
| Rendimiento | Mejor (sin indirección innecesaria) | Peor (siempre hay indirección de vtable) | Peor (búsqueda en cadena de prototipos) |

**¿Por qué C# decidió que los métodos NO sean virtuales por defecto?**

Anders Hejlsberg (diseñador de C#) lo explicó así: en Java, cualquier clase derivada puede cambiar el comportamiento de cualquier método de la clase base, incluso si el diseñador de la clase base no lo previó. Esto viola el **principio de encapsulación** y puede romper invariantes de la clase base. Además, tiene un coste de rendimiento porque cada llamada a método pasa por la vtable.

En C#, el diseñador de la clase debe **declarar explícitamente** qué métodos son extensibles (`virtual`), documentando así el contrato de extensibilidad. Los métodos no virtuales se resuelven en tiempo de compilación (static dispatch), lo que permite inlining y otras optimizaciones del JIT.

```
┌──────────────────────────────────────────────────────────┐
│              VIRTUAL DISPATCH: C# vs Java                 │
│                                                          │
│  Java: TODOS los métodos son virtuales                   │
│                                                          │
│  clase Base       clase Derivada                         │
│  ┌──────────┐     ┌──────────┐                           │
│  │ método() │◄────│ método() │  ← siempre sobrescribe    │
│  └──────────┘     └──────────┘                           │
│                                                          │
│  C#: SOLO los métodos marcados virtual son virtuales     │
│                                                          │
│  clase Base       clase Derivada                         │
│  ┌──────────┐     ┌──────────────┐                       │
│  │ método() │     │ new método() │  ← oculta (warning)   │
│  │ virtual  │◄────│ override     │  ← sobrescribe        │
│  │ metodo() │     │ metodo()     │                       │
│  └──────────┘     └──────────────┘                       │
└──────────────────────────────────────────────────────────┘
```

### 2.4 Interfaces

| Aspecto | C# | Java | TypeScript |
|---|---|---|---|
| Declaración | `interface IName { }` | `interface IName { }` | `interface IName { }` |
| Implementación | `class A : IName` | `class A implements IName` | `class A implements IName` |
| Propiedades | **SÍ** (`string Name { get; set; }`) | **NO** (solo métodos) | **SÍ** (`name: string`) |
| Implementación por defecto | **SÍ** (C# 8+) | **SÍ** (Java 8+ `default`) | **NO** (TS no genera código) |
| Implementación explícita | **SÍ** (`IName.Method()`) | **NO** | **NO** |
| Verificación de tipos | **Nominal** (debe declarar `: IName`) | **Nominal** (debe declarar `implements IName`) | **Estructural** (duck typing) |
| Existencia en runtime | **SÍ** (el CLR las conserva) | **SÍ** (la JVM las conserva) | **NO** (se borran al compilar a JS) |

**Implementación explícita de interfaces (exclusivo de C#)**: permite implementar dos interfaces que tengan un método con el mismo nombre, pero con comportamientos distintos. El método solo es accesible a través de una referencia del tipo de la interfaz:

```csharp
interface IDrawable { void Draw(); }
interface IPrintable { void Draw(); }

class Document : IDrawable, IPrintable
{
    void IDrawable.Draw() => Console.WriteLine("Dibujando en pantalla");
    void IPrintable.Draw() => Console.WriteLine("Enviando a impresora");
}

// Uso:
Document doc = new();
// doc.Draw();              // Error: Draw() no es accesible directamente
((IDrawable)doc).Draw();    // "Dibujando en pantalla"
((IPrintable)doc).Draw();   // "Enviando a impresora"
```

**Structural typing (TypeScript)**: en TypeScript, si un objeto tiene las mismas propiedades que una interfaz, la cumple automáticamente sin necesidad de declararlo. En C# y Java debes declarar explícitamente `: IName` o `implements IName`.

### 2.5 Genéricos

Esta es la **segunda diferencia más importante** entre C# y Java:

| Aspecto | C# | Java | TypeScript |
|---|---|---|---|
| Implementación | **Reificación** (tipos conservados en runtime) | **Type Erasure** (tipos borrados en runtime) | **Type Erasure** (borrados al compilar a JS) |
| `typeof(T)` en runtime | **SÍ**, funciona | **NO** — `T` se convierte en `Object` | **NO** — desaparece en JS |
| `new T()` | **SÍ** (con constraint `new()`) | **NO** — necesitas `Class<T>` | **NO** |
| `List<int>` vs `List<String>` | Tipos **DISTINTOS** en runtime | **MISMO** tipo (`ArrayList`) | **MISMO** en JS (`Array`) |
| Boxing con tipos valor | **NO** (`List<int>` usa `int` real) | **SÍ** (`ArrayList<Integer>` usa `Integer` wrapper) | N/A |
| Sobrecarga con genéricos | **SÍ** (tipos distintos) | **NO** (mismo tipo tras erasure) | **NO** |

**Type Erasure (Java)**: el compilador de Java elimina la información de tipo genérico tras comprobar que es correcta. En runtime, `List<Integer>` y `List<String>` son exactamente lo mismo: `ArrayList<Object>`. Esto significa que:

```java
// Java — esto compila pero da TRUE (¡sorpresa!)
List<Integer> ints = new ArrayList<>();
List<String> strs = new ArrayList<>();
System.out.println(ints.getClass() == strs.getClass()); // true
```

```csharp
// C# — esto devuelve False porque SON tipos distintos
List<int> ints = new List<int>();
List<string> strs = new List<string>();
Console.WriteLine(ints.GetType() == strs.GetType()); // False
```

**Reificación (C#)**: el CLR conserva el tipo genérico completo. `List<int>` es un tipo real distinto de `List<string>`. El JIT incluso genera código máquina especializado para `List<int>` (aprovechando que `int` tiene un tamaño fijo) y comparte código entre todos los tipos referencia (`List<string>`, `List<Customer>`, etc.).

**Consecuencia práctica**: `List<int>` en C# no hace boxing de cada `int`. `ArrayList<Integer>` en Java **sí** hace boxing de cada `int` a `Integer`. Para un millón de enteros, la diferencia de memoria y rendimiento es enorme.

### 2.6 Structs (exclusivo de C#)

**Ni Java ni JavaScript tienen structs.** Es una característica exclusiva de C# (heredada de C/C++ pero modernizada).

| Aspecto | C# struct | C# class | Java (closest) | JavaScript |
|---|---|---|---|---|
| Tipo | **Value type** | Reference type | Primitivos (`int`, `double`, ...) | Primitivos (`number`, `string`, ...) |
| Almacenamiento | **Stack** (normalmente) | **Heap** | Stack (primitivos) | Depende del motor |
| Asignación | **Copia el valor** | Copia la referencia | Copia el valor | Copia el valor (primitivos) |
| Paso a método | **Por valor** (copia) | Por referencia | Por valor (primitivos) | Por valor (primitivos) |
| Herencia | **NO** (sealed implícito) | SÍ | NO (primitivos) | NO |
| Constructor sin parámetros | SÍ (C# 10+) | SÍ | N/A | N/A |
| Boxing | Cuando se convierte a `object` | No (ya está en heap) | Sí (primitivo → wrapper) | N/A |
| Garbage Collector | **Menos presión** | Cada instancia va al GC | Ninguna (stack) | Mark-and-sweep |

**Cuándo usar struct**: tipos pequeños (≤ 16 bytes), inmutables, que representan un valor simple (`Point`, `Color`, `Money`, `Complex`, `DateTime`). La ventaja principal es que evitas presión sobre el Garbage Collector.

**Demostración de la diferencia**:

```csharp
Point p1 = new Point(3, 4);   // struct: en el stack
Point p2 = p1;                // ¡COPIA! p2 es un valor independiente
p2 = new Point(10, p2.Y);    // Modificar p2 NO afecta a p1

PointClass pc1 = new PointClass(3, 4); // class: en el heap
PointClass pc2 = pc1;                  // ¡REFERENCIA! pc2 apunta al mismo objeto
pc2.X = 10;                            // pc1.X también es 10 ahora
```

### 2.7 Propiedades

Las propiedades son una de las características que más sorprenden a quien viene de Java:

| Variante | C# | Java equivalente |
|---|---|---|
| Auto-implemented | `string Name { get; set; }` | `private String name;`<br>`public String getName()`<br>`public void setName(String name)` |
| Init-only (C# 9+) | `string Id { get; init; }` | Campo `final` + constructor |
| Expression-bodied | `string Full => $"{First} {Last}";` | Método `getFullName()` |
| Backing field explícito | Campo privado + lógica en setter | Campo privado + lógica en setter |
| Calculada (solo get) | `int Age => DateTime.Now.Year - BirthYear;` | Método `getAge()` |

En Java, incluso con Lombok (`@Data`), sigues teniendo getters y setters como métodos. En C#, las propiedades son **ciudadanos de primera clase del lenguaje**. El compilador las trata de forma especial y el resto del ecosistema (serialización JSON, binding en ASP.NET, Entity Framework) las entiende nativamente.

### 2.8 Delegados y Eventos (exclusivo de C#)

**No existe equivalente directo en Java ni JavaScript.** Son una de las características más potentes y distintivas de C#.

| Concepto | C# | Java | JavaScript |
|---|---|---|---|
| Delegado | `delegate void Handler(args);` | `@FunctionalInterface` + SAM | Callback (`() => {}`) |
| Multicast | `+=` y `-=` | `List<Listener>` manual | `EventEmitter` (Node.js) |
| Eventos | `event Handler MyEvent;` | PropertyChangeListener | `.on()` / `.emit()` |
| Invocación segura | `MyEvent?.Invoke(args)` | Loop + try/catch | `emitter.emit()` |
| Encapsulación | Solo la clase dueña dispara el evento | Cualquiera con acceso al listener | Cualquiera puede `.emit()` |

**En C#**, los delegados son **tipos de primera clase** que representan referencias a métodos con una firma específica. Son la base de LINQ, eventos, async/await y el patrón observer. Los eventos encapsulan delegados y garantizan que solo la clase propietaria pueda dispararlos.

```csharp
// Definir el delegado (tipo que describe la firma)
public delegate void PriceChangedEventHandler(object sender, PriceChangedEventArgs args);

// Usar el delegado como evento
public class PriceTracker
{
    public event PriceChangedEventHandler? PriceChanged;
    
    public void UpdatePrice(decimal newPrice)
    {
        // ?.Invoke: solo invoca si hay suscriptores (no es null)
        PriceChanged?.Invoke(this, new PriceChangedEventArgs(oldPrice, newPrice));
    }
}

// Suscribirse (multicast: += añade, -= quita)
tracker.PriceChanged += (sender, args) => Console.WriteLine("¡Precio cambiado!");
```

**En Java**, necesitas definir una interfaz funcional, mantener una lista de listeners y notificarlos manualmente.

**En JavaScript (Node.js)**, usas `EventEmitter` que es similar en concepto pero sin tipos ni verificación en compilación.

### 2.9 Extension Methods (exclusivo de C#)

C# permite añadir métodos a tipos existentes sin modificar su código fuente:

```csharp
public static class StringExtensions
{
    public static int WordCount(this string str) => str.Split(' ').Length;
}

// Uso:
int count = "Hola mundo".WordCount(); // 2
```

Java no tiene esta característica (aunque hay propuestas). En JavaScript puedes modificar `prototype` directamente, pero es una mala práctica.

### 2.10 Operator Overloading (exclusivo de C#)

C# permite definir cómo se comportan los operadores (`+`, `-`, `==`, etc.) para tus propios tipos:

```csharp
public static Point operator +(Point a, Point b) => new(a.X + b.X, a.Y + b.Y);
```

Java no permite sobrecarga de operadores (salvo `+` para `String`). JavaScript no lo permite (aunque tiene `valueOf` y `Symbol.toPrimitive` como workaround limitado).

---

## 3. Ejemplo mínimo

Veamos exactamente el mismo concepto en los tres lenguajes: una clase `Person` con nombre, edad y un método para saludar.

### C# (idiomático)

```csharp
public class Person
{
    // Propiedades auto-implementadas: get/set generados por el compilador
    public string Name { get; set; }
    public int Age { get; set; }

    public Person(string name, int age)
    {
        Name = name;
        Age = age;
    }

    // Método no virtual (por defecto en C#)
    public string Greet()
    {
        return $"Hola, soy {Name} y tengo {Age} años.";
    }
}

// Uso:
var person = new Person("Ana", 30);
Console.WriteLine(person.Greet());
// Puedes usar las propiedades como si fueran campos públicos
person.Age = 31;
Console.WriteLine($"Ahora {person.Name} tiene {person.Age} años.");
```

### Java (idiomático)

```java
public class Person {
    // Campos privados (convención Java: no se exponen directamente)
    private String name;
    private int age;

    public Person(String name, int age) {
        this.name = name;
        this.age = age;
    }

    // Getters y setters manuales (~7 líneas por propiedad)
    public String getName() { return name; }
    public void setName(String name) { this.name = name; }
    public int getAge() { return age; }
    public void setAge(int age) { this.age = age; }

    // Método público (virtual por defecto en Java)
    public String greet() {
        return "Hola, soy " + name + " y tengo " + age + " años.";
    }
}

// Uso:
Person person = new Person("Ana", 30);
System.out.println(person.greet());
// Acceso mediante getters/setters, no propiedades directas
person.setAge(31);
System.out.println("Ahora " + person.getName() + " tiene " + person.getAge() + " años.");
```

### TypeScript (idiomático)

```typescript
class Person {
    // Parámetros de constructor con modificadores de acceso (atajo de TS)
    constructor(
        public name: string,
        public age: number
    ) {}

    greet(): string {
        return `Hola, soy ${this.name} y tengo ${this.age} años.`;
    }
}

// Uso:
const person = new Person("Ana", 30);
console.log(person.greet());
person.age = 31;
console.log(`Ahora ${person.name} tiene ${person.age} años.`);
```

### JavaScript (ES6+)

```javascript
class Person {
    constructor(name, age) {
        this.name = name;
        this.age = age;
    }

    greet() {
        return `Hola, soy ${this.name} y tengo ${this.age} años.`;
    }
}

// Uso
const person = new Person("Ana", 30);
console.log(person.greet());
person.age = 31;
console.log(`Ahora ${person.name} tiene ${person.age} años.`);
```

### Comparación línea a línea

| Concepto | C# | Java | TypeScript | JavaScript |
|---|---|---|---|---|
| Declarar clase | `class Person` | `class Person` | `class Person` | `class Person` |
| Campos/propiedades | `string Name { get; set; }` | `private String name;` + getters/setters | `public name: string` en constructor | `this.name = name` en constructor |
| Constructor | `Person(string name, int age)` | `Person(String name, int age)` | `constructor(public name: string, public age: number)` | `constructor(name, age)` |
| Interpolación | `$"Hola {Name}"` | `"Hola " + name` (o `String.format`) | `` `Hola ${this.name}` `` | `` `Hola ${this.name}` `` |
| Tipos | Estáticos y obligatorios | Estáticos y obligatorios | Estáticos y opcionales | Dinámicos (sin tipos) |
| Acceso a miembros | `person.Name` (propiedad) | `person.getName()` (método) | `person.name` (campo público) | `person.name` (campo público) |

---

## 4. Ejemplo real

El proyecto `src/PooComparison/Program.cs` contiene una demostración completa y ejecutable con 10 secciones que comparan cada aspecto de POO entre los tres lenguajes. El código C# se ejecuta, y las comparaciones con Java/JS/TS están en comentarios detallados.

Ejecútalo con:

```bash
dotnet run --project ejemplos/m02-poo/Leccion-08-Comparacion-Java-Javascript/src/PooComparison
```

### Secciones del ejemplo

**Sección 1 — Clases**: `Customer` con propiedades auto-implementadas. Compara con el equivalente Java (~25 líneas extra de getters/setters) y TypeScript (parámetros de constructor con modificadores de acceso).

**Sección 2 — Herencia**: Jerarquía `Person` → `Employee` → `Manager`. Muestra `virtual`/`override` explícito de C# frente al virtual implícito de Java. Incluye el equivalente en JavaScript ES5 (sin `class`) para mostrar la cadena de prototipos que la sintaxis `class` oculta.

**Sección 3 — Interfaces**: `Product` implementando `IIdentifiable` e `IAuditable`. Muestra propiedades en interfaces (exclusivo de C#) y herencia múltiple de tipo. Compara con `implements` de Java y structural typing de TypeScript.

**Sección 4 — Polimorfismo**: `Shape` abstracta con `Circle`, `Rectangle`, `Triangle`. Demuestra virtual dispatch explícito de C#: `abstract` (debe implementarse), `virtual` (puede sobrescribirse), y métodos no virtuales (no pueden sobrescribirse). Incluye el triángulo con fórmula de Herón.

**Sección 5 — Genéricos**: `Repository<T>` que demuestra **reificación** con `typeof(T)` en runtime. Compara con type erasure de Java (donde `T` se convierte en `Object` y `typeof(T)` no existe) y TypeScript (donde los tipos desaparecen al compilar).

**Sección 6 — Delegados y Eventos**: `PriceTracker` con `PriceChangedEventHandler` (delegado) y `PriceChangedEventArgs`. Muestra el patrón observer integrado en el lenguaje con `event` y `?.Invoke()`. Compara con interfaces funcionales + listeners manuales en Java y `EventEmitter` en Node.js.

**Sección 7 — Structs**: `Point` (struct inmutable, value type) vs `PointClass` (class, reference type). Demuestra la diferencia fundamental: asignar un struct **copia el valor**; asignar una clase **copia la referencia**. Incluye operator overloading (`==`, `!=`).

**Sección 8 — Propiedades**: `PropertyDemo` con cuatro variantes de propiedades: auto-implemented (`{ get; set; }`), init-only (`{ get; init; }`), backing field manual con validación, y computed property (solo get). Compara con getters/setters manuales de Java y Lombok.

**Sección 9 — Tabla comparativa**: tabla de 20 características comparando C#, Java y JavaScript/TypeScript lado a lado (clases, herencia, interfaces, propiedades, virtual dispatch, genéricos, structs, delegados, extension methods, operator overloading, ref/in/out, nullable value types, destructores, partial methods, indexadores, LINQ, async/await, record types, pattern matching, primary constructors).

**Sección 10 — Resumen**: conclusión con las fortalezas de cada ecosistema.

---

## 5. Buenas prácticas

### ✅ Qué hacer

1. **Aprovecha las propiedades en lugar de getters/setters manuales**
   ```csharp
   // ✅ C# idiomático
   public string Name { get; set; }
   
   // ❌ Acento Java
   private string _name;
   public string GetName() => _name;
   public void SetName(string value) => _name = value;
   ```
   Las propiedades se integran con serialización, binding, EF Core y el resto del ecosistema. Los getters/setters manuales no.

2. **Marca métodos como `virtual` SOLO si el diseño lo requiere**
   ```csharp
   // ✅ El diseñador decidió que CalculateBonus es extensible
   public virtual decimal CalculateBonus() => Salary * 0.1m;
   
   // ✅ GetMonthlyPay es fijo, no debe ser sobrescrito
   public decimal GetMonthlyPay() => Salary / 12;
   ```
   En Java no tienes esta opción: todo es virtual. En C# tienes el control.

3. **Usa structs para tipos pequeños e inmutables**
   ```csharp
   // ✅ Value semantics, sin presión en el GC
   public readonly struct Money
   {
       public decimal Amount { get; }
       public string Currency { get; }
   }
   ```
   Si tu tipo es pequeño, inmutable y representa un valor, probablemente debería ser struct.

4. **Usa genéricos con confianza (son reified)**
   ```csharp
   // ✅ Totalmente válido en C#, imposible en Java
   public Repository<T> where T : IEntity
   {
       public string GetEntityType() => typeof(T).Name;
   }
   ```
   No tengas miedo de usar `typeof(T)`, `default(T)` o `new T()` con constraints. En C# funcionan perfectamente.

5. **Usa `override` explícito, no `new` para ocultar**
   ```csharp
   // ✅ Quieres polimorfismo real: virtual + override
   public class Dog : Animal
   {
       public override string Speak() => "Guau";
   }
   
   // ❌ Esto oculta, no sobrescribe (y genera warning CS0108)
   public class Dog : Animal
   {
       public string Speak() => "Guau"; // Warning: oculta Animal.Speak()
   }
   ```

6. **Prefiere `string` interpolation sobre concatenación**
   ```csharp
   // ✅ C# idiomático
   var msg = $"Hola {name}, tienes {age} años.";
   
   // ❌ Acento Java
   var msg = "Hola " + name + ", tienes " + age + " años.";
   ```

### ❌ Qué evitar

1. **No asumir que lo que funciona en Java/JS funciona igual en C#**
   - Los métodos no son virtuales por defecto.
   - Los genéricos no se borran.
   - `==` en strings compara por valor (gracias a operator overloading), en objetos compara referencias.
   - No hay checked exceptions.

2. **No escribas getters/setters manuales "por costumbre"**
3. **No uses `dynamic` para evitar declarar tipos** (pierdes seguridad en compilación)
4. **No uses `new` para métodos cuando quieres `override`**
5. **No asumas que las variables de tipo clase se pasan por valor**

---

## 6. Errores habituales

### Error 1: Escribir getters/setters manuales en C# (como en Java)

```csharp
// ❌ Acento Java: 3 líneas por propiedad
private string _name;
public string GetName() { return _name; }
public void SetName(string value) { _name = value; }

// ✅ C# idiomático: 1 línea
public string Name { get; set; }
```

**Por qué está mal**: el ecosistema .NET espera propiedades, no métodos get/set. Serialización JSON, model binding en ASP.NET, Entity Framework, FluentValidation... todos funcionan con propiedades. Si usas `GetName()`/`SetName()`, nada de esto funcionará automáticamente.

### Error 2: Asumir que todos los métodos son virtuales (como en Java)

```csharp
public class Base
{
    public void DoWork() { Console.WriteLine("Base"); }
}

public class Derived : Base
{
    // ❌ Esto NO sobrescribe DoWork. Genera warning CS0108.
    //    Simplemente oculta el método base.
    public void DoWork() { Console.WriteLine("Derived"); }
}

// Comportamiento sorprendente:
Base obj = new Derived();
obj.DoWork(); // Imprime "Base", NO "Derived"
```

**Por qué está mal**: en Java esto habría funcionado como esperabas (polimorfismo). En C# necesitas `virtual` + `override`. Sin ellos, el método se resuelve en tiempo de compilación según el tipo de la variable, no el tipo real del objeto.

**Versión correcta**:

```csharp
public class Base
{
    public virtual void DoWork() { Console.WriteLine("Base"); }
}

public class Derived : Base
{
    public override void DoWork() { Console.WriteLine("Derived"); }
}

Base obj = new Derived();
obj.DoWork(); // Imprime "Derived" ✓
```

### Error 3: No entender struct vs class (copia vs referencia)

```csharp
// ❌ Esperar que un struct se comporte como una clase
var points = new List<Point>(); // Point es struct
var p = new Point(1, 2);
points.Add(p);
p.X = 10; // Esto NO modifica el elemento en la lista (porque Add copió el struct)
Console.WriteLine(points[0].X); // Sigue siendo 1
```

**Por qué está mal**: al añadir un struct a una lista, se **copia**. Modificar la variable original no afecta a la copia dentro de la lista. Con una clase, ambas variables apuntarían al mismo objeto.

### Error 4: Usar `new` para ocultar cuando se quería `override`

```csharp
public class Animal
{
    public virtual string Speak() => "???";
}

public class Cat : Animal
{
    // ❌ new oculta el método base. No hay polimorfismo.
    public new string Speak() => "Miau";
}

Animal a = new Cat();
Console.WriteLine(a.Speak()); // "???" — ¡no es lo que esperabas!
```

**Por qué está mal**: `new` le dice al compilador "sé que hay un método con el mismo nombre en la clase base, pero quiero ocultarlo intencionadamente". No hay polimorfismo. La llamada se resuelve según el tipo de la variable.

**Versión correcta**:

```csharp
public class Cat : Animal
{
    public override string Speak() => "Miau";
}

Animal a = new Cat();
Console.WriteLine(a.Speak()); // "Miau" ✓
```

### Error 5: No usar `var` cuando el tipo es obvio

```csharp
// ❌ Redundante (el tipo aparece a ambos lados)
Dictionary<string, List<int>> data = new Dictionary<string, List<int>>();

// ✅ El tipo es obvio por el new
var data = new Dictionary<string, List<int>>();
```

### Error 6: Comparar strings con `==` en otros lenguajes... espera, en C# SÍ funciona

```csharp
// ✅ En C#, == compara strings por VALOR (gracias a operator overloading)
string a = "hola";
string b = "hola";
Console.WriteLine(a == b); // True ✓

// En Java, == compara REFERENCIAS, necesitas .equals()
// String a = "hola";
// String b = new String("hola");
// a == b  →  false (compara referencias)
// a.equals(b)  →  true
```

---

## 7. Ejercicio guiado

### Traduce una clase Java de un sistema bancario a C# idiomático

#### Clase original en Java:

```java
public class Account {
    private String accountNumber;
    private String holderName;
    private double balance;
    private final String currency;

    public Account(String accountNumber, String holderName, double initialBalance, String currency) {
        if (initialBalance < 0) {
            throw new IllegalArgumentException("El saldo inicial no puede ser negativo");
        }
        this.accountNumber = accountNumber;
        this.holderName = holderName;
        this.balance = initialBalance;
        this.currency = currency;
    }

    public String getAccountNumber() { return accountNumber; }

    public String getHolderName() { return holderName; }
    public void setHolderName(String holderName) {
        if (holderName == null || holderName.trim().isEmpty()) {
            throw new IllegalArgumentException("El nombre no puede estar vacío");
        }
        this.holderName = holderName;
    }

    public double getBalance() { return balance; }

    public String getCurrency() { return currency; }

    public void deposit(double amount) {
        if (amount <= 0) {
            throw new IllegalArgumentException("El monto a depositar debe ser positivo");
        }
        this.balance += amount;
    }

    public boolean withdraw(double amount) {
        if (amount <= 0) {
            throw new IllegalArgumentException("El monto a retirar debe ser positivo");
        }
        if (amount > this.balance) {
            return false; // Fondos insuficientes
        }
        this.balance -= amount;
        return true;
    }

    public String getSummary() {
        return "Cuenta " + accountNumber + " | " + holderName + " | Saldo: " + balance + " " + currency;
    }
}
```

#### Traducción a C# idiomático:

```csharp
public class Account
{
    // Propiedades: getters y setters en UNA línea cada una
    // accountNumber es inmutable: solo get, sin set
    public string AccountNumber { get; }

    // holderName: get público, set con validación
    private string _holderName = string.Empty;
    public string HolderName
    {
        get => _holderName;
        set => _holderName = !string.IsNullOrWhiteSpace(value)
            ? value
            : throw new ArgumentException("El nombre del titular no puede estar vacío.");
    }

    // balance: get público, set PRIVADO (solo la clase modifica el saldo)
    public decimal Balance { get; private set; }

    // currency: init-only (solo en constructor, inmutable después)
    // Usamos decimal en lugar de double para dinero (¡importante!)
    public string Currency { get; init; }

    public Account(string accountNumber, string holderName, decimal initialBalance, string currency)
    {
        if (initialBalance < 0)
            throw new ArgumentException("El saldo inicial no puede ser negativo.");

        AccountNumber = accountNumber;
        HolderName = holderName; // Pasa por el setter con validación
        Balance = initialBalance;
        Currency = currency;
    }

    // Método con expression body (una línea)
    public void Deposit(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("El monto a depositar debe ser positivo.");

        Balance += amount;
    }

    // Retorna bool para indicar éxito/fallo (fondos insuficientes)
    public bool Withdraw(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("El monto a retirar debe ser positivo.");

        if (amount > Balance)
            return false;

        Balance -= amount;
        return true;
    }

    // String interpolation en lugar de concatenación
    public string GetSummary()
        => $"Cuenta {AccountNumber} | {HolderName} | Saldo: {Balance:N2} {Currency}";
}
```

#### Diferencias clave aplicadas:

| Aspecto | Java original | C# idiomático |
|---|---|---|
| `accountNumber` | `private String` + `getAccountNumber()` | `public string AccountNumber { get; }` (1 línea) |
| `holderName` | `private String` + getter + setter | Propiedad con backing field + validación en setter |
| `balance` | `private double` + `getBalance()` | `public decimal Balance { get; private set; }` |
| `currency` | `private final String` + `getCurrency()` | `public string Currency { get; init; }` (init-only) |
| Tipo para dinero | `double` (❌ pierde precisión) | `decimal` (✅ precisión financiera) |
| Método `deposit` | `void deposit(double amount)` | `void Deposit(decimal amount)` (PascalCase + decimal) |
| Método `withdraw` | `boolean withdraw(double amount)` | `bool Withdraw(decimal amount)` |
| Interpolación | `"texto " + variable + " más texto"` | `$"texto {variable} más texto"` |
| Formato numérico | Manual | `:N2` (formato estándar) |

**¿Por qué `decimal` y no `double`?** `double` es binario y no puede representar exactamente valores como 0.1€. `decimal` es decimal (base 10) y está diseñado específicamente para cálculos financieros. En Java usarías `BigDecimal`; en C# tienes `decimal` nativo con soporte del CLR.

---

## 8. Reto

### Traduce un componente TypeScript a C# idiomático

Toma el siguiente componente React/TypeScript que gestiona una lista de tareas y tradúcelo a su equivalente en C# como clase de dominio (sin UI, sin React):

```typescript
// TypeScript original
interface Todo {
  id: string;
  text: string;
  done: boolean;
}

class TodoManager {
  private todos: Todo[] = [];

  add(text: string): void {
    if (!text.trim()) {
      throw new Error("El texto no puede estar vacío");
    }
    this.todos.push({
      id: crypto.randomUUID(),
      text: text.trim(),
      done: false,
    });
  }

  toggle(id: string): void {
    const todo = this.todos.find((t) => t.id === id);
    if (!todo) {
      throw new Error(`No se encontró la tarea con id ${id}`);
    }
    todo.done = !todo.done;
  }

  remove(id: string): void {
    const index = this.todos.findIndex((t) => t.id === id);
    if (index === -1) {
      throw new Error(`No se encontró la tarea con id ${id}`);
    }
    this.todos.splice(index, 1);
  }

  getAll(): Todo[] {
    return [...this.todos];
  }

  getPending(): Todo[] {
    return this.todos.filter((t) => !t.done);
  }

  getCompleted(): Todo[] {
    return this.todos.filter((t) => t.done);
  }

  getStats(): { total: number; pending: number; completed: number } {
    return {
      total: this.todos.length,
      pending: this.todos.filter((t) => !t.done).length,
      completed: this.todos.filter((t) => t.done).length,
    };
  }
}
```

#### Requisitos para la implementación en C#:

1. Usa un `record` para `Todo` (inmutable, con `init`).
2. Usa `IReadOnlyList<Todo>` en los métodos que devuelven listas (no expongas la lista interna).
3. Usa LINQ en lugar de bucles manuales (`FirstOrDefault`, `Where`, `RemoveAll`).
4. Usa `Guid` en lugar de `crypto.randomUUID()`.
5. Usa `ArgumentException` en lugar de `Error` de JS.
6. Usa `string.IsNullOrWhiteSpace` en lugar de `!text.trim()`.
7. Usa propiedades calculadas para las estadísticas.

**No mires la solución hasta que lo hayas intentado.** Cuando termines, ejecuta el proyecto `PooComparison` para comparar tu enfoque con el código de ejemplo.

---

## 9. Resumen

### Tabla comparativa completa

| Característica | C# | Java | JavaScript / TypeScript |
|---|---|---|---|
| **Clases** | `class` (tipos reales en CLR) | `class` (tipos reales en JVM) | `class` ES6+ (azúcar sobre prototype) |
| **Herencia** | `: BaseClass` | `extends BaseClass` | `extends BaseClass` |
| **Interfaces** | `interface` con propiedades y default impl. | `interface` con default methods (Java 8+) | `interface` en TS (no existe en JS) |
| **Propiedades** | `{ get; set; }` nativas | getters/setters manuales (o Lombok) | campos públicos o `get`/`set` (ES5) |
| **Virtual por defecto** | **NO** (explícito con `virtual`) | **SÍ** (todos los métodos) | **SÍ** (cadena de prototipos) |
| **Genéricos** | **Reificados** (tipos en runtime) | **Type Erasure** (borrados) | **Erased** en TS, no existen en JS |
| **Structs (value types)** | **SÍ** | **NO** (solo primitivos) | **NO** |
| **Delegados / Eventos** | **SÍ** (tipos de 1ª clase) | Listener interfaces (SAM) | Callbacks / EventEmitter |
| **Extension Methods** | **SÍ** | **NO** | `prototype` (mala práctica) |
| **Operator Overloading** | **SÍ** | **NO** (solo `+` para String) | **NO** |
| **`ref` / `out` / `in` params** | **SÍ** | **NO** | **NO** |
| **Nullable value types** | `int?` (azúcar de `Nullable<T>`) | `Integer` (wrapper, no es lo mismo) | `?` en TS (union type) |
| **Destructor / Finalizer** | `~Finalizer()` | `finalize()` (deprecado) | **NO** (solo GC) |
| **Partial methods** | **SÍ** | **NO** | **NO** |
| **Indexadores** | `this[int i]` | **NO** | **NO** |
| **LINQ / Streams** | LINQ (100+ operadores) | Stream API (similar pero limitado) | `map`/`filter`/`reduce` |
| **`async` / `await`** | `Task<T>` (nativo del runtime) | `CompletableFuture<T>` | `Promise<T>` |
| **Record types** | `record` (C# 9+, inmutable por valor) | `record` (Java 14+, preview) | **NO** nativo |
| **Pattern Matching** | Avanzado (`is`, `switch`, property, list) | Básico (Java 16+) | **NO** (TS usa union types) |
| **Primary Constructors** | **SÍ** (C# 12) | **NO** | Parámetros con modificadores en TS |

### Fortalezas de cada ecosistema

| Ecosistema | Fortalezas principales |
|---|---|
| **C# / .NET** | Sistema de tipos más rico (structs, refs, spans), genéricos reificados, LINQ, propiedades nativas, async/await integrado en el runtime, delegados/eventos, mayor control de memoria, mejor rendimiento para value types. |
| **Java / JVM** | Ecosistema masivo y maduro (Spring, Hibernate, Maven), más opciones de lenguajes en la JVM (Kotlin, Scala, Groovy), multiplataforma sin fricción, herramientas de observabilidad muy maduras. |
| **JavaScript / TypeScript / Node.js** | Mismo lenguaje en frontend y backend (full-stack), ecosistema npm enorme, flexibilidad extrema, TypeScript añade seguridad sin perder la agilidad de JS, ideal para startups y prototipado rápido. |

### Conceptos clave de esta lección

1. **Virtual dispatch explícito**: la diferencia más importante con Java. En C# los métodos no son virtuales por defecto. Necesitas declarar `virtual` en la base y `override` en la derivada para tener polimorfismo.
2. **Genéricos reificados**: segunda diferencia más importante. En C# los tipos genéricos existen en runtime. `typeof(T)` funciona. `List<int>` no hace boxing. En Java todo se borra.
3. **Propiedades vs getters/setters**: las propiedades son ciudadanos de primera clase en C#. No escribas `GetName()`/`SetName()`.
4. **Structs**: value types personalizados. No existen en Java ni JS. Ideales para tipos pequeños e inmutables.
5. **Delegados y eventos**: integrados en el lenguaje. El patrón observer sin boilerplate.
6. **Sintaxis similar, semántica diferente**: aunque `class`, `extends`/`:` y `interface` se parezcan, el comportamiento subyacente (virtual dispatch, genéricos, paso de parámetros) es radicalmente distinto.

---

## 10. Preguntas frecuentes

### ¿Qué lenguaje tiene mejor rendimiento para POO?

**C# ≈ Java > JavaScript.** C# y Java compilan a código máquina (JIT) con optimizaciones avanzadas (inlining, escape analysis, devirtualization). JavaScript es interpretado/JIT pero con menos información de tipos, lo que limita las optimizaciones.

C# tiene una ventaja adicional: **los structs evitan heap allocation**. Puedes crear arrays de `Point` (struct) sin que cada elemento sea un objeto independiente en el heap. En Java, cada `Point` sería un objeto en el heap. Para aplicaciones con millones de objetos pequeños, la diferencia puede ser de 10x en rendimiento y memoria.

### ¿Debo dejar de escribir getters/setters al pasarme a C#?

**Sí, absolutamente.** Usa propiedades auto-implementadas (`{ get; set; }`). Es más idiomático, más corto y el ecosistema .NET está diseñado para funcionar con propiedades:

- **System.Text.Json** serializa propiedades, no métodos `GetX()`.
- **ASP.NET Core Model Binding** enlaza a propiedades.
- **Entity Framework Core** mapea propiedades a columnas.
- **FluentValidation** valida propiedades con `.RuleFor(x => x.Name)`.

Si escribes `GetName()`/`SetName()`, tendrás que configurar TODO manualmente.

### ¿Los genéricos de C# son realmente más rápidos que los de Java?

**Sí, especialmente con tipos de valor.** La razón es que C# no hace boxing:

```csharp
// C#: List<int> almacena ints reales (4 bytes cada uno, en el heap del array)
var numbers = new List<int> { 1, 2, 3 };

// Java: ArrayList<Integer> almacena punteros a objetos Integer (12+ bytes cada uno)
// List<Integer> numbers = new ArrayList<>();
// numbers.add(Integer.valueOf(1)); // boxing: int → Integer (objeto en heap)
```

Para un millón de enteros:
- C# `List<int>`: ~4 MB contiguos, sin objetos extra, sin presión de GC.
- Java `ArrayList<Integer>`: ~16 MB (4 MB de array de punteros + 12 MB de objetos Integer), más presión de GC por cada objeto Integer.

Además, `typeof(T)` y `default(T)` son operaciones O(1) en C#. En Java simplemente no existen.

### ¿Puedo usar patrones de Java como DAO/Service/Controller en C#?

**Sí, completamente.** El ecosistema .NET usa los mismos patrones con nombres similares:

| Patrón | Java (Spring) | C# (ASP.NET Core) |
|---|---|---|
| Acceso a datos | `@Repository` / DAO | `Repository<T>` / DbContext |
| Lógica de negocio | `@Service` | Servicio con interfaz (DI) |
| Endpoints REST | `@RestController` | `ControllerBase` / Minimal API |
| Inyección de dependencias | `@Autowired` | Constructor injection |
| Validación | Bean Validation | FluentValidation / DataAnnotations |
| Transacciones | `@Transactional` | `DbContext.SaveChanges()` |
| DTOs | Clases POJO | Records |
| Mapeo | MapStruct | AutoMapper / manual |

La diferencia está en la implementación: en lugar de anotaciones de Spring, usarás el contenedor de DI nativo de .NET y convenciones del framework.

### ¿TypeScript no es suficiente para backend? ¿Por qué elegir C#?

Depende del contexto:

- **TypeScript/Node.js** es excelente para: startups que iteran rápido, equipos full-stack JavaScript, APIs con poca lógica de negocio, serverless functions, prototipos, y cuando necesitas compartir tipos entre frontend y backend.

- **C#/.NET** es superior para: aplicaciones enterprise con mucha lógica de dominio, sistemas que requieren alto rendimiento (structs, spans, genéricos reificados), aplicaciones que necesitan LINQ para consultas complejas, sistemas financieros (decimal nativo), microservicios con gRPC, y cuando el sistema de tipos robusto aporta valor real (refactorizaciones seguras, análisis estático avanzado).

No es que uno sea mejor que el otro objetivamente. Son herramientas distintas para problemas distintos. La ventaja de aprender ambos es que puedes elegir la herramienta adecuada para cada proyecto.

### ¿Debo usar `var` o el tipo explícito?

La convención en C#: usa `var` cuando el tipo es **obvio** por el lado derecho; usa el tipo explícito cuando **no es obvio**.

```csharp
// ✅ var: el tipo es obvio por el constructor
var person = new Person("Ana", 30);
var numbers = new List<int> { 1, 2, 3 };
var result = await httpClient.GetAsync(url);

// ✅ Tipo explícito: el tipo NO es obvio por el nombre del método
List<Customer> active = repository.GetActiveCustomers();
decimal totalSales = CalculateQuarterlySales();
Person? found = people.Find(p => p.Name == "Ana");
```

Esto no es una regla rígida, es una convención de estilo. En Java, `var` existe desde Java 10 con exactamente el mismo criterio.

### ¿C# tiene algo como `java.lang.Object` del que heredan todas las clases?

Sí: `System.Object`. Toda clase en C# hereda implícitamente de `System.Object`. Los structs también heredan de `System.Object` (vía `System.ValueType`), pero de forma especial con boxing.

Los métodos que toda clase hereda: `ToString()`, `Equals(object)`, `GetHashCode()`, `GetType()`, y `MemberwiseClone()` (protegido). Son equivalentes a los de `java.lang.Object`.

### ¿Qué pasa con los métodos `equals()` y `hashCode()` de Java al migrar a C#?

Se traducen directamente:

```java
// Java
@Override
public boolean equals(Object o) { ... }

@Override
public int hashCode() { ... }
```

```csharp
// C#
public override bool Equals(object? obj) { ... }

public override int GetHashCode() { ... }
```

C# 9+ añadió los **records**, que generan `Equals` y `GetHashCode` automáticamente basándose en los valores de las propiedades (value equality), similar a los `record` de Java 14+ o las `data class` de Kotlin.

```csharp
// C#: Equals y GetHashCode generados automáticamente
public record Person(string Name, int Age);

var p1 = new Person("Ana", 30);
var p2 = new Person("Ana", 30);
Console.WriteLine(p1 == p2); // True (value equality)
```
