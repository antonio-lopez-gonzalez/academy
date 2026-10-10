# Lección 02 — abstract, virtual, override y new

---

## 1. Introducción

Cuando creas una jerarquía de clases, necesitas decidir **qué partes del comportamiento se pueden modificar** desde las subclases y **qué partes son obligatorias**. C# te da cuatro herramientas para controlar esto con precisión quirúrgica:

| Keyword | Significado |
|---|---|
| `abstract` | "Esto no tiene implementación todavía. **Tienes que implementarlo** en la subclase." |
| `virtual` | "Aquí tienes una implementación por defecto. **Puedes** sobrescribirla si quieres." |
| `override` | "Estoy **reemplazando** la implementación virtual/abstract de mi clase base. Participo en el despacho dinámico." |
| `new` | "Estoy **ocultando** el miembro de la clase base. Soy un método completamente distinto. No participo en el despacho dinámico." |

Estas cuatro keywords existen para un mismo propósito: **controlar cómo y cuándo se puede modificar el comportamiento en una jerarquía de herencia**. Sin ellas, la herencia sería un caos: no sabrías si un método se puede sobrescribir, si es obligatorio hacerlo, o si estás creando un método nuevo por accidente.

**¿Por qué existen?** Porque la herencia sin reglas es peligrosa. Imagina que tienes una clase `Shape` con un método `Draw()`. Si cualquier subclase pudiera redefinirlo sin avisar, el código que usa `Shape` no podría confiar en cómo se comporta `Draw()`. C# te obliga a ser explícito sobre tus intenciones con cada método.

**¿Cuándo se utiliza cada uno?**

- `abstract` → cuando la clase base **no sabe** cómo implementar el método (ej: `CalculateArea()` en `Shape` no tiene sentido sin saber si es un círculo o un rectángulo).
- `virtual` → cuando la clase base **sabe** cómo implementarlo, pero quiere permitir que las subclases lo **refinen** (ej: `ToString()` en `object`).
- `override` → cuando la subclase proporciona su **propia versión** de un método virtual o abstracto.
- `new` → prácticamente **nunca**. Solo en casos extremos donde no puedes modificar la clase base pero necesitas un miembro con el mismo nombre (ej: librería de terceros).

---

## 2. Teoría

### abstract

Una clase marcada como `abstract` **no se puede instanciar** directamente. Solo sirve como **plantilla** para que otras clases hereden de ella.

```csharp
public abstract class Shape
{
    // Método abstracto: NO tiene cuerpo. Las subclases DEBEN implementarlo.
    public abstract double CalculateArea();

    // Método concreto: SÍ tiene implementación. Las subclases la heredan tal cual
    // o pueden sobrescribirla si está marcado como virtual.
    public string GetDescription()
    {
        return $"Soy una figura con área {CalculateArea()}";
    }
}
```

**Clase abstracta vs Interfaz — la diferencia fundamental:**

| | Clase abstracta | Interfaz |
|---|---|---|
| ¿Puede tener estado (campos)? | Sí | No (solo propiedades, sin campos) |
| ¿Puede tener implementación? | Sí (métodos concretos) | Desde C# 8, métodos con implementación por defecto, pero es raro |
| ¿Puede tener constructor? | Sí | No |
| ¿Herencia múltiple? | No (solo una clase base) | Sí (múltiples interfaces) |
| ¿Cuándo usarla? | Cuando hay **comportamiento común** + estado compartido | Cuando solo necesitas un **contrato** sin estado |

Regla práctica: si necesitas compartir **estado** (campos) y **comportamiento** entre clases relacionadas, usa clase abstracta. Si solo necesitas un **contrato** (métodos que deben existir), usa interfaz.

### virtual

Un método `virtual` tiene una implementación por defecto en la clase base, pero **permite** que las subclases la reemplacen con `override`.

```csharp
public class Animal
{
    // Virtual: "Aquí tienes una implementación, pero puedes cambiarla."
    public virtual string MakeSound()
    {
        return "...";
    }
}

public class Dog : Animal
{
    // override: "Cambio la implementación por defecto."
    public override string MakeSound()
    {
        return "¡Guau!";
    }
}
```

**El despacho virtual (virtual dispatch):** cuando llamas a `MakeSound()` en una variable de tipo `Animal`, el runtime **mira el tipo real del objeto** (no el tipo de la variable) y ejecuta el método correspondiente. Esto se resuelve en tiempo de ejecución mediante la **tabla de métodos virtuales (vtable)**.

```
Animal a = new Dog();
a.MakeSound(); // "¡Guau!" — se ejecuta Dog.MakeSound, no Animal.MakeSound
```

### override

`override` **reemplaza** la implementación virtual o abstracta de la clase base. La keyword es obligatoria: C# no permite sobrescribir un método sin declararlo explícitamente (a diferencia de Java, donde todos los métodos son virtuales por defecto).

```csharp
public class Cat : Animal
{
    public override string MakeSound()
    {
        return "¡Miau!";
    }
}
```

Puedes llamar a la implementación base desde el override:

```csharp
public class LoudDog : Dog
{
    public override string MakeSound()
    {
        var baseSound = base.MakeSound(); // "¡Guau!"
        return baseSound.ToUpper();       // "¡GUAU!"
    }
}
```

### sealed override

Si no quieres que las subclases de tu subclase puedan volver a sobrescribir el método, usa `sealed override`:

```csharp
public class FinalDog : Dog
{
    public sealed override string MakeSound()
    {
        return "¡GUAU DEFINITIVO!";
    }
}

public class Puppy : FinalDog
{
    // ERROR de compilación: no se puede sobrescribir un método sealed
    // public override string MakeSound() { ... }
}
```

### new — la trampa

`new` **oculta** un miembro de la clase base. Crea un método **completamente distinto** que no participa en el despacho virtual. Es como si el método de la clase base y el de la subclase fueran dos métodos diferentes que casualmente tienen el mismo nombre.

```csharp
public class BaseClass
{
    public void SayHello()
    {
        Console.WriteLine("Hola desde BaseClass");
    }
}

public class DerivedWithNew : BaseClass
{
    public new void SayHello()
    {
        Console.WriteLine("Hola desde DerivedWithNew");
    }
}
```

La diferencia fatal con `override`:

```csharp
BaseClass b = new DerivedWithNew();
b.SayHello(); // "Hola desde BaseClass"  ← ¡NO es el de DerivedWithNew!

DerivedWithNew d = new DerivedWithNew();
d.SayHello(); // "Hola desde DerivedWithNew"
```

Con `new`, el método que se ejecuta depende del **tipo de la variable**, no del tipo real del objeto. Esto rompe el polimorfismo.

### Tabla comparativa definitiva

| Keyword | ¿Instanciable? | ¿Tiene implementación? | ¿Subclase obligada a implementar? | Virtual dispatch | ¿Se puede heredar más? |
|---|---|---|---|---|---|
| `abstract` class | No | Puede tener | Métodos abstractos: sí | Sí | Sí |
| `virtual` | N/A (es un modificador de método) | Sí (obligatorio) | No | Sí | Sí |
| `override` | N/A | Sí (obligatorio) | No | Sí | Sí (salvo `sealed override`) |
| `new` | N/A | Sí (obligatorio) | No | **No** | Sí (pero oculta, no sobrescribe) |

### Diagrama ASCII de la jerarquía de Shape

```
┌─────────────────────────────────────────────┐
│          abstract class Shape               │
│─────────────────────────────────────────────│
│  + abstract double CalculateArea()          │  ← SIN cuerpo, obligatorio
│  + virtual string GetDescription()          │  ← CON cuerpo por defecto
│  + virtual void Draw()                      │  ← implementación vacía
└──────────────┬──────────────────────────────┘
               │
       ┌───────┼───────┬──────────────┐
       │               │              │
       ▼               ▼              ▼
┌──────────────┐ ┌──────────────┐ ┌──────────────────┐
│   Circle     │ │  Rectangle   │ │    Triangle      │
│ (override)   │ │  (override)  │ │   (override)     │
└──────────────┘ └──────────────┘ └──────────────────┘
  CalculateArea()  CalculateArea()   CalculateArea()
  = π × r²         = base × altura    = (base × altura)/2
  Draw() override  Draw() override   Draw() override

Cada subclase:
  - DEBE implementar CalculateArea() (es abstract)
  - PUEDE sobrescribir GetDescription() (es virtual)
  - PUEDE sobrescribir Draw() (es virtual)
```

---

## 3. Ejemplo mínimo

### Animal con virtual y override

```csharp
// Clase base con un método virtual: tiene implementación por defecto.
public class Animal
{
    public virtual string MakeSound()
    {
        return "..."; // Sonido genérico por defecto
    }
}

// Dog sobrescribe MakeSound con override.
public class Dog : Animal
{
    public override string MakeSound()
    {
        return "¡Guau!";
    }
}

// Cat también sobrescribe MakeSound.
public class Cat : Animal
{
    public override string MakeSound()
    {
        return "¡Miau!";
    }
}

// Uso:
Animal generic = new Animal();
Console.WriteLine(generic.MakeSound()); // "..."

Animal dog = new Dog();
Console.WriteLine(dog.MakeSound());     // "¡Guau!" ← virtual dispatch

Animal cat = new Cat();
Console.WriteLine(cat.MakeSound());     // "¡Miau!" ← virtual dispatch
```

### La diferencia fatal entre new y override

```csharp
// Clase base con método NO virtual (no se puede sobrescribir).
public class BaseClass
{
    public void SayHello()
    {
        Console.WriteLine("BaseClass dice: Hola");
    }
}

// Clase que oculta SayHello con new.
public class DerivedWithNew : BaseClass
{
    public new void SayHello()
    {
        Console.WriteLine("DerivedWithNew dice: Hola");
    }
}

// Uso — observa la diferencia según el tipo de la variable:
BaseClass viaBase = new DerivedWithNew();
viaBase.SayHello();      // "BaseClass dice: Hola" ← ¡NO usa new!

DerivedWithNew viaDerived = new DerivedWithNew();
viaDerived.SayHello();   // "DerivedWithNew dice: Hola" ← solo funciona así
```

**Lección:** `new` solo se ve cuando la variable está tipada como la clase derivada. En cuanto usas una referencia de la clase base, el método original es el que se ejecuta. `new` no es polimorfismo, es ocultación.

---

## 4. Ejemplo real

El código en `src/PooOverride/Program.cs` demuestra tres conceptos clave en acción:

### Parte 1 — Shape (clase abstracta con virtual dispatch)

```csharp
public abstract class Shape
{
    public string Name { get; init; }

    // Método abstracto: SIN cuerpo. Las subclases DEBEN implementarlo.
    public abstract double CalculateArea();

    // Método virtual: CON cuerpo por defecto. Las subclases PUEDEN sobrescribirlo.
    public virtual string GetDescription()
    {
        return $"Soy un(a) {Name} con área {CalculateArea():F2}";
    }
}
```

- `Shape` es `abstract` → no se puede hacer `new Shape()`.
- `CalculateArea()` es `abstract` → cada figura **DEBE** decir cómo se calcula su área.
- `GetDescription()` es `virtual` → tiene implementación por defecto, pero cada figura puede personalizarla.

**Subclases:**

```csharp
public class Circle : Shape
{
    public double Radius { get; init; }
    public override double CalculateArea() => Math.PI * Radius * Radius;
}

public class Rectangle : Shape
{
    public double Width { get; init; }
    public double Height { get; init; }
    public override double CalculateArea() => Width * Height;
}

public class Triangle : Shape
{
    public double Base { get; init; }
    public double Height { get; init; }
    public override double CalculateArea() => (Base * Height) / 2.0;
}
```

Cada subclase **debe** implementar `CalculateArea()` porque es abstracto. Si no lo hiciera, el compilador daría error.

La parte de `virtual dispatch` se demuestra al recorrer una lista de `Shape`:

```csharp
List<Shape> shapes = new()
{
    new Circle { Name = "Círculo", Radius = 5 },
    new Rectangle { Name = "Rectángulo", Width = 4, Height = 6 },
    new Triangle { Name = "Triángulo", Base = 3, Height = 4 }
};

foreach (var shape in shapes)
{
    // Virtual dispatch: el runtime llama al método correcto según el tipo real.
    Console.WriteLine(shape.GetDescription());
}
```

Aunque la variable es de tipo `Shape`, se ejecuta el `CalculateArea()` de `Circle`, `Rectangle` o `Triangle` según corresponda. Esto es polimorfismo real.

### Parte 2 — Animal (virtual con sobrescritura y llamada a base)

```csharp
public class Animal
{
    // Virtual: implementación por defecto que las subclases PUEDEN cambiar.
    public virtual string MakeSound() => "...";
}

public class Dog : Animal
{
    public override string MakeSound() => "¡Guau!";
}

public class Cat : Animal
{
    public override string MakeSound() => "¡Miau!";
}

// Bird extiende el comportamiento llamando a base.
public class Bird : Animal
{
    public override string MakeSound()
    {
        var baseResult = base.MakeSound();  // "..." (el de Animal)
        return $"{baseResult} ¡Pío pío!";   // "... ¡Pío pío!"
    }
}
```

`Bird` demuestra que puedes **extender** (no solo reemplazar) el comportamiento base llamando a `base.MakeSound()`. Esto es útil cuando quieres añadir comportamiento sin perder lo que ya hacía la clase base.

### Parte 3 — La diferencia entre new y override (demostración práctica)

```csharp
public class BaseClass
{
    public void NonVirtualMethod() => Console.WriteLine("BaseClass.NonVirtualMethod");
    public virtual void VirtualMethod() => Console.WriteLine("BaseClass.VirtualMethod");
}

public class OverrideDerived : BaseClass
{
    public override void VirtualMethod() => Console.WriteLine("OverrideDerived.VirtualMethod");
}

public class NewDerived : BaseClass
{
    public new void NonVirtualMethod() => Console.WriteLine("NewDerived.NonVirtualMethod");
    public new void VirtualMethod() => Console.WriteLine("NewDerived.VirtualMethod");
}
```

La demostración clave:

```csharp
BaseClass overrideObj = new OverrideDerived();
overrideObj.VirtualMethod();  // "OverrideDerived.VirtualMethod" ← override funciona

BaseClass newObj = new NewDerived();
newObj.NonVirtualMethod(); // "BaseClass.NonVirtualMethod" ← new NO funciona con ref base
newObj.VirtualMethod();    // "BaseClass.VirtualMethod"    ← new NO funciona con ref base

NewDerived directNew = new NewDerived();
directNew.NonVirtualMethod(); // "NewDerived.NonVirtualMethod" ← new solo funciona así
directNew.VirtualMethod();    // "NewDerived.VirtualMethod"    ← new solo funciona así
```

**Conclusión:** `override` respeta el polimorfismo. `new` lo rompe. Usa `new` solo cuando no te quede más remedio (librerías de terceros que no puedes modificar).

---

## 5. Buenas prácticas

### ✅ QUÉ HACER

**1. Marca como `abstract` cuando la clase base no tenga sentido por sí sola.**

Si instanciar la clase base no tiene sentido en tu dominio, hazla abstracta. Nadie debería crear un `Animal` genérico o una `Shape` genérica; esos conceptos solo existen para ser extendidos.

```csharp
// Bien: no tiene sentido un Animal sin especie concreta
public abstract class Animal { }

// Mal: permite crear Animales "genéricos" sin sentido
public class Animal { }
```

**2. Marca métodos como `abstract` cuando la implementación dependa totalmente de la subclase.**

```csharp
// Bien: cada figura tiene su propia fórmula de área
public abstract double CalculateArea();

// Mal: implementación falsa que lanza excepción
public virtual double CalculateArea() => throw new NotImplementedException();
```

**3. Usa `virtual` con moderación y documenta el contrato.**

Cada método `virtual` es un punto de extensión. Demasiados puntos de extensión hacen el código impredecible. Documenta qué se espera del override: precondiciones, postcondiciones, invariantes.

```csharp
/// <summary>
/// Valida si un pago puede ser procesado.
/// Precondición: Payment.Amount > 0
/// Postcondición: el resultado no debe depender de efectos secundarios externos.
/// Las subclases pueden extender la validación llamando a base.ValidatePayment().
/// </summary>
public virtual bool ValidatePayment(Payment payment)
{
    return payment.Amount > 0;
}
```

**4. Usa `sealed override` para cerrar la puerta a más sobrescrituras.**

Cuando tu implementación es definitiva y no quieres que clases derivadas la cambien:

```csharp
public class SecureProcessor : PaymentProcessor
{
    public sealed override bool ValidatePayment(Payment payment)
    {
        // Validación de seguridad que NADIE debe poder saltarse
        return base.ValidatePayment(payment) && !IsBlacklisted(payment);
    }
}
```

**5. Prefiere composición sobre herencia cuando tengas dudas.**

Si solo necesitas reutilizar comportamiento, no crees una jerarquía compleja. Usa composición (inyecta el comportamiento como dependencia). La herencia es una de las formas más fuertes de acoplamiento.

### ❌ QUÉ EVITAR

**1. Evita `new` para ocultar miembros. El 99% de las veces es un error de diseño.**

`new` rompe el polimorfismo y crea confusión. Si necesitas cambiar el comportamiento, refactoriza la clase base o usa composición. La única excepción legítima: necesitas un miembro con el mismo nombre que uno de una librería de terceros que no puedes modificar.

**2. No marques todo como `virtual` "por si acaso".**

Cada `virtual` es una decisión de diseño. Un método virtual dice: "he pensado en que querrás cambiarme y he diseñado la clase para que sea seguro hacerlo". Si lo marcas virtual sin pensar, estás invitando a errores sutiles (subclases que rompen invariantes de la clase base).

**3. No crees clases abstractas sin métodos abstractos sin una buena razón.**

Si una clase abstracta no tiene métodos abstractos, pregúntate: ¿realmente necesita ser abstracta? A veces sí (ej: `Stream` en .NET, que tiene implementación concreta pero no tiene sentido instanciarla directamente). Pero si es solo para "organizar código", probablemente deberías usar composición.

**4. No llames a métodos virtuales/abstractos desde el constructor.**

Este es uno de los errores más peligrosos y sutiles. Ver sección 6.

**5. No uses `new` para "arreglar" que un método no sea virtual en la clase base.**

Si no puedes modificar la clase base, `new` no te da polimorfismo real. Considera un adapter o wrapper en su lugar.

---

## 6. Errores habituales

### Error 1: Usar `new` pensando que funciona como `override` (el error más común)

```csharp
// ❌ MAL: el desarrollador cree que está sobrescribiendo
public class MyList<T> : List<T>
{
    public new void Add(T item)
    {
        Console.WriteLine("Añadiendo elemento...");
        base.Add(item);
    }
}

// El problema:
List<int> list = new MyList<int>();
list.Add(5); // NO escribe nada. Llama a List<T>.Add, no a MyList<T>.Add
```

**Versión correcta:** no se puede. `List<T>.Add()` no es virtual. Si necesitas este comportamiento, usa composición (envuelve `List<T>` en tu propia clase que implemente `IList<T>`).

### Error 2: No llamar a `base.Method()` en el override cuando es necesario

```csharp
// ❌ MAL: la clase base inicializa algo importante que la subclase se salta
public class BaseValidator
{
    public virtual void Initialize()
    {
        LoadValidationRules(); // Carga reglas desde BD — ¡importante!
    }
}

public class CustomValidator : BaseValidator
{
    public override void Initialize()
    {
        // Olvida llamar a base.Initialize() — las reglas no se cargan
        LoadCustomRules();
    }
}
```

**Versión correcta:**

```csharp
public override void Initialize()
{
    base.Initialize(); // Primero lo de la base
    LoadCustomRules(); // Luego lo nuestro
}
```

### Error 3: Olvidar `override` y que el compilador te salve (o usar `new` sin darte cuenta)

```csharp
// ❌ MAL: el compilador avisa: "member hides inherited member"
public class Dog : Animal
{
    public string MakeSound() // Falta 'override' → ocultación implícita
    {
        return "¡Guau!";
    }
}
```

```csharp
// ❌ PEOR: el desarrollador añade 'new' para callar al compilador
public class Dog : Animal
{
    public new string MakeSound() // Esto calla el warning pero es incorrecto
    {
        return "¡Guau!";
    }
}
```

**Versión correcta:**

```csharp
// ✅ BIEN: override explícito
public class Dog : Animal
{
    public override string MakeSound()
    {
        return "¡Guau!";
    }
}
```

### Error 4: Llamar a métodos virtuales/abstractos desde el constructor (el error más peligroso)

```csharp
// ❌ MUY PELIGROSO
public abstract class Base
{
    protected Base()
    {
        Initialize(); // ← Llama a un método virtual desde el constructor
    }

    protected abstract void Initialize();
}

public class Derived : Base
{
    private readonly string _connectionString;

    public Derived(string connectionString)
    {
        _connectionString = connectionString; // ← Esto AL AÚN NO se ha ejecutado
    }

    protected override void Initialize()
    {
        // _connectionString es NULL aquí porque el constructor de Derived
        // todavía no se ha ejecutado. ¡El constructor de Base se ejecuta primero!
        ConnectToDatabase(_connectionString); // NullReferenceException o peor
    }

    private void ConnectToDatabase(string cs) { /* ... */ }
}
```

**Versión correcta:** usa inicialización en dos fases o factory method.

```csharp
// ✅ Opción 1: Inicialización explícita en dos fases
public abstract class Base
{
    public void Initialize() // Ya no se llama desde el constructor
    {
        InitializeCore();
    }

    protected abstract void InitializeCore();
}

// Uso:
var derived = new Derived("connection-string");
derived.Initialize(); // Ahora sí, el constructor de Derived ya terminó
```

### Error 5: Crear clases abstractas sin necesidad real

```csharp
// ❌ MAL: abstracta sin métodos abstractos y sin razón para no instanciarse
public abstract class StringHelper
{
    public string Capitalize(string input) => /* ... */
    public string Truncate(string input, int length) => /* ... */
}

// ✅ BIEN: clase estática (no se instancia, no se hereda)
public static class StringHelper
{
    public static string Capitalize(string input) => /* ... */
    public static string Truncate(string input, int length) => /* ... */
}
```

---

## 7. Ejercicio guiado

### Sistema de cálculo de impuestos

Vamos a construir un sistema que calcula impuestos para diferentes jurisdicciones, demostrando `abstract`, `virtual` y `override`.

#### Paso 1: La clase base abstracta

Creamos `TaxCalculator` como clase abstracta. El método principal (`CalculateTax`) es abstracto porque cada jurisdicción tiene sus propias reglas. Añadimos un método virtual `IsTaxExempt` que por defecto devuelve `false`.

```csharp
public class Product
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty; // Para impuestos de EE.UU.
}

public abstract class TaxCalculator
{
    // Abstracto: cada país/región tiene su propia fórmula. Obligatorio implementar.
    public abstract decimal CalculateTax(Product product);

    // Virtual: por defecto nada está exento, pero las subclases pueden añadir exenciones.
    public virtual bool IsTaxExempt(Product product)
    {
        return false;
    }

    // Método concreto (no virtual): utilidad común. No diseñado para ser sobrescrito.
    public string GetTaxDescription(Product product)
    {
        if (IsTaxExempt(product))
            return $"{product.Name}: Exento de impuestos";

        var tax = CalculateTax(product);
        return $"{product.Name}: Impuesto = {tax:C}";
    }
}
```

**Pregunta: ¿por qué `IsTaxExempt` es virtual y no abstracto?**  
Porque no todas las jurisdicciones tienen exenciones. Si fuera abstracto, obligaríamos a cada calculadora a implementarlo aunque no lo necesite. Virtual ofrece un valor por defecto razonable que cada subclase puede refinar.

#### Paso 2: SpainTaxCalculator (IVA 21%)

```csharp
public class SpainTaxCalculator : TaxCalculator
{
    private const decimal IvaRate = 0.21m; // 21% IVA general

    public override decimal CalculateTax(Product product)
    {
        // En España, casi todo paga IVA. El método IsTaxExempt se hereda tal cual.
        return product.Price * IvaRate;
    }

    // Sobrescribimos IsTaxExempt para reflejar exenciones españolas.
    public override bool IsTaxExempt(Product product)
    {
        // Productos sanitarios y educativos están exentos de IVA en España.
        return product.Category is "Sanidad" or "Educación";
    }
}
```

#### Paso 3: USTaxCalculator (varía por estado)

```csharp
public class USTaxCalculator : TaxCalculator
{
    // En EE.UU., el impuesto depende del estado. No hay IVA nacional.
    public override decimal CalculateTax(Product product)
    {
        var stateRate = product.State switch
        {
            "CA" => 0.0725m, // California
            "NY" => 0.04m,   // New York (solo estatal, sin contar local)
            "TX" => 0.0625m, // Texas
            _    => 0.05m    // Tasa por defecto para otros estados
        };

        return product.Price * stateRate;
    }

    // En EE.UU., ciertos alimentos y ropa pueden estar exentos según el estado.
    public override bool IsTaxExempt(Product product)
    {
        return product.Category is "Groceries" or "Clothing"
               && product.State is "TX" or "FL"; // Solo en ciertos estados
    }
}
```

#### Paso 4: DigitalServicesTaxCalculator (tasa fija, ignora el precio)

```csharp
public class DigitalServicesTaxCalculator : TaxCalculator
{
    private const decimal DigitalServiceTaxRate = 0.03m; // 3% tasa fija

    public override decimal CalculateTax(Product product)
    {
        // Tasa especial para servicios digitales: 3% sobre el precio.
        return product.Price * DigitalServiceTaxRate;
    }

    // No sobrescribe IsTaxExempt: los servicios digitales nunca están exentos.
    // Hereda el comportamiento por defecto (siempre false).
}
```

#### Paso 5: Demostración del virtual dispatch

```csharp
var products = new List<Product>
{
    new() { Name = "Libro de C#", Price = 50m, Category = "Educación" },
    new() { Name = "Camiseta", Price = 25m, Category = "Clothing", State = "TX" },
    new() { Name = "Suscripción Netflix", Price = 15m, Category = "Digital" },
    new() { Name = "Portátil", Price = 1000m, Category = "Electrónica", State = "CA" }
};

var spainCalc = new SpainTaxCalculator();
var usCalc = new USTaxCalculator();
var digitalCalc = new DigitalServicesTaxCalculator();

// Virtual dispatch: el mismo método GetTaxDescription llama al CalculateTax
// e IsTaxExempt correctos según el tipo real de cada calculadora.
foreach (var product in products)
{
    Console.WriteLine(spainCalc.GetTaxDescription(product));
    Console.WriteLine(usCalc.GetTaxDescription(product));
    Console.WriteLine(digitalCalc.GetTaxDescription(product));
    Console.WriteLine("---");
}
```

**Resultado esperado:**
- El libro está exento en España (Educación), paga en EE.UU. (no exento en TX porque es Educación, no Groceries/Clothing).
- La camiseta está exenta en TX, paga IVA en España.
- La suscripción paga la tasa digital y el IVA español.
- El portátil paga en todas partes.

---

## 8. Reto

Crea un sistema de procesamiento de pagos que demuestre `abstract`, `virtual` con llamada a `base`, y por qué `new` es peligroso.

### Requisitos

1. **Crea `abstract class PaymentProcessor`** con:
   - `virtual bool ValidatePayment(Payment p)`: comprueba que `Amount > 0` y `CardNumber` no sea nulo/vacío. Devuelve `true` si pasa, `false` si no.
   - `abstract bool ProcessPayment(Payment p)`: realiza el pago. Devuelve `true` si se procesó correctamente.

2. **Implementa `CreditCardProcessor`** que extiende `ValidatePayment`:
   - Llama a `base.ValidatePayment(p)` para la validación común.
   - Añade validación extra: `CardNumber` debe tener 16 dígitos.
   - Implementa `ProcessPayment` simulando conexión a pasarela de pago.

3. **Implementa `PayPalProcessor`** que extiende `ValidatePayment`:
   - Llama a `base.ValidatePayment(p)` para la validación común.
   - Añade validación extra: debe existir `PayPalEmail` en los metadatos.
   - Implementa `ProcessPayment`.

4. **Implementa `BankTransferProcessor`** que **reemplaza completamente** `ValidatePayment`:
   - NO llama a `base.ValidatePayment(p)`.
   - Su validación: `Amount > 0` (no necesita tarjeta).
   - Implementa `ProcessPayment`.

5. **Demuestra por qué `new` no funciona como `override`**:
   - Crea `BrokenCryptoProcessor` que herede de `CreditCardProcessor`.
   - Que intente usar `new bool ProcessPayment(Payment p)` para "mejorar" el procesamiento.
   - Demuestra que cuando tratas el objeto como `PaymentProcessor`, se ejecuta el método original de `CreditCardProcessor`, no el `new`.

### Pistas

- No necesitas implementar pasarelas de pago reales, solo `Console.WriteLine` y devolver `true`/`false`.
- Usa `Dictionary<string, string> Metadata` en `Payment` para datos adicionales.
- La clase `Payment` debería tener `Amount`, `CardNumber` y `Metadata`.
- La demostración del `new` es la parte más importante: escribe el código de prueba que evidencie que `BrokenCryptoProcessor` no funciona polimórficamente.
- Intenta resolverlo antes de mirar la solución.

---

## 9. Resumen

| Keyword | ¿Instanciable? | ¿Tiene implementación? | ¿Subclase obligada a implementar? | Virtual dispatch |
|---|---|---|---|---|
| `abstract` (clase) | No | Puede tener (métodos concretos) | Solo métodos abstractos: sí | Sí, para miembros virtuales/abstractos |
| `abstract` (método) | N/A | No (cuerpo prohibido) | **Sí, obligatorio** | Sí |
| `virtual` | N/A | **Sí** (obligatorio que tenga cuerpo) | No (opcional) | **Sí** |
| `override` | N/A | **Sí** (obligatorio) | No | **Sí** (hereda el comportamiento virtual) |
| `new` | N/A | **Sí** (obligatorio) | No | **No** (es ocultación, no polimorfismo) |

### Cuándo usar cada uno (regla rápida)

- **`abstract`** → "Esto no lo puedo implementar yo, que lo haga la subclase."
- **`virtual`** → "Esto lo implemento yo, pero si quieres mejorarlo, adelante."
- **`override`** → "Cambio la implementación de mi clase base. Soy parte del polimorfismo."
- **`new`** → "No me dejan/puedo modificar la clase base pero necesito este nombre. Sé lo que hago (o no)."

### El flujo mental al diseñar una jerarquía

1. ¿Puede esta clase existir por sí sola? → No → **abstract class**
2. ¿Este método tiene un comportamiento base razonable? → No → **abstract method**
3. ¿Este método tiene un comportamiento base razonable? → Sí → **virtual method**
4. ¿Quiero permitir que las subclases lo cambien? → Sí → **virtual**
5. ¿Quiero prohibir que las sub-subclases lo cambien? → **sealed override**

---

## 10. Preguntas frecuentes

### ¿Se puede marcar un método como `abstract` y `virtual` a la vez?

**No, y no es necesario.** Un método `abstract` ya es implícitamente virtual. El compilador rechaza explícitamente la combinación. La razón: `abstract` significa "no tengo implementación, la subclase debe proporcionarla", y `virtual` significa "tengo implementación, la subclase puede cambiarla". Son mutuamente excluyentes en cuanto a la implementación base. Ambos participan en virtual dispatch.

### ¿Puedo tener una clase abstracta sin métodos abstractos?

**Sí.** La clase `Stream` en .NET es un ejemplo perfecto: tiene implementaciones concretas para muchos métodos, pero no tiene sentido instanciar un `Stream` genérico. Sin embargo, esto debería hacerte reflexionar: si no hay métodos abstractos, ¿es realmente necesaria la abstracción? A veces la respuesta es sí (porque la clase encapsula un concepto abstracto). Pero si es solo para "tener una clase base", probablemente estás abusando de la herencia y deberías usar composición.

### ¿`new` es lo mismo que no poner nada?

**Casi.** Si un método de la subclase tiene la misma firma que uno de la clase base y no pones `override` ni `new`, el compilador emite un **warning CS0114**: *"member hides inherited member"*. El comportamiento en ejecución es exactamente el mismo que con `new` (ocultación sin virtual dispatch). La diferencia es solo la intención expresada:

- Sin `new`: "Ups, no me di cuenta de que estaba ocultando algo." → warning.
- Con `new`: "Sé que estoy ocultando este miembro. Es intencionado." → sin warning.

Como regla: si alguna vez escribes `new` en un método, para y pregúntate si no hay una solución mejor.

### ¿Puedo llamar a un método abstracto desde el constructor de la clase abstracta?

**Sí, el compilador te lo permite. Pero es extremadamente peligroso.** El constructor de la clase base se ejecuta **antes** que el constructor de la subclase. Si el método abstracto es sobrescrito por la subclase, ese override se ejecutará **antes de que el constructor de la subclase haya terminado**. Esto significa que cualquier campo que la subclase inicialice en su constructor estará en su valor por defecto (`null` para referencias, `0` para números, etc.).

Ejemplo del desastre:

```csharp
public abstract class Base
{
    protected Base() { DoSomething(); }
    protected abstract void DoSomething();
}

public class Derived : Base
{
    private readonly string _data;
    public Derived(string data)
    {
        _data = data; // Se ejecuta DESPUÉS de Base()
    }
    protected override void DoSomething()
    {
        Console.WriteLine(_data.Length); // NullReferenceException: _data es null
    }
}
```

No lo hagas. Si necesitas inicialización virtual, usa un patrón de inicialización en dos fases o un factory method.

### ¿Por qué C# no hace todos los métodos virtuales por defecto como Java?

Java decidió que todos los métodos fueran virtuales por defecto y añadió `final` para impedir sobrescritura. C# tomó la decisión opuesta: los métodos son **no virtuales** por defecto y debes marcarlos explícitamente como `virtual`.

**La razón es el diseño por contrato.** Un método no virtual dice: "esta implementación es parte del contrato de la clase. Confía en que siempre se comportará así". Un método virtual dice: "he diseñado cuidadosamente esta clase para que modificar este método sea seguro; aquí tienes las precondiciones y postcondiciones que debes respetar". Hacer todo virtual por defecto (como Java) debilita este contrato.

En la práctica, esto significa que en C# tienes que **pensar** antes de permitir sobrescritura. Es una decisión consciente de diseño, no un accidente. Esto produce jerarquías más robustas y predecibles.

### ¿Puedo combinar `sealed` con `override` en una clase no sellada?

**Sí.** `sealed override` sella el método para que las subclases de esta clase no puedan sobrescribirlo, pero la clase en sí puede seguir siendo heredada para otros fines.

```csharp
public class Animal
{
    public virtual void Eat() { Console.WriteLine("Comiendo..."); }
}

public class Mammal : Animal
{
    // Sella Eat en este nivel: ningún mamífero más específico puede cambiar cómo come.
    public sealed override void Eat()
    {
        Console.WriteLine("Masticando comida...");
    }
}

public class Dog : Mammal
{
    // ERROR CS0239: no se puede sobrescribir un miembro sealed
    // public override void Eat() { ... }

    // Pero Dog puede añadir otros métodos nuevos.
    public void Bark() { Console.WriteLine("¡Guau!"); }
}
```
