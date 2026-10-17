
# Lección 03 — Principio de Sustitución de Liskov (LSP)

---

## 1. Introducción

**¿Qué vamos a aprender?**

El **Principio de Sustitución de Liskov** (Liskov Substitution Principle, LSP) es la "L" de SOLID. Fue formulado por Barbara Liskov en 1987 y dice:

> "Si S es un subtipo de T, los objetos de tipo T pueden ser reemplazados por objetos de tipo S sin alterar ninguna de las propiedades deseables del programa."

Traducido a lenguaje llano: **si tu código usa una clase base, debe poder usar cualquier clase derivada sin enterarse y sin que el programa falle**.

**¿Por qué existe?**

Porque la herencia no es solo una herramienta para reutilizar código. La herencia establece un **contrato** entre la clase base y sus subtipos. Si el subtipo rompe ese contrato (lanzando excepciones inesperadas, cambiando el significado de métodos, o imponiendo restricciones que la base no tenía), cualquier código que confíe en la clase base se romperá cuando reciba el subtipo.

**¿Qué problema resuelve?**

- **Excepciones inesperadas**: un método que nunca fallaba con la clase base, de repente lanza `NotSupportedException` con el subtipo.
- **Comportamiento inconsistente**: el mismo método hace cosas radicalmente distintas según el subtipo, rompiendo las expectativas del código cliente.
- **`if (x is Subtipo)` por todas partes**: el código cliente necesita inspeccionar el tipo real para saber cómo comportarse.
- **Imposibilidad de reutilizar**: no puedes escribir código genérico contra la clase base porque cada subtipo se comporta diferente.

**Errores típicos**

- Creer que la herencia en código debe reflejar la jerarquía del mundo real. Un pingüino ES un ave en el mundo real, pero si `Ave` incluye `Volar()`, `Pingüino` NO debería heredar de `Ave`.
- Usar `new` en lugar de `override` y creer que se está aplicando polimorfismo.
- Reforzar precondiciones en subtipos sin que el código cliente lo sepa.

---

## 2. Teoría

### Contrato de un método: precondiciones, postcondiciones e invariantes

LSP se fundamenta en el concepto de **Diseño por Contrato** (Design by Contract) de Bertrand Meyer. Cada método tiene un contrato con tres partes:

```
┌─────────────────────────────────────────────────────────────┐
│                     CONTRATO DE UN MÉTODO                   │
│                                                             │
│  PRECONDICIÓN:  Lo que debe ser cierto ANTES de ejecutar.   │
│                 El CLIENTE es responsable de cumplirla.     │
│                 Regla LSP: el subtipo NO puede reforzarla.  │
│                                                             │
│  POSTCONDICIÓN: Lo que debe ser cierto DESPUÉS de ejecutar. │
│                 El MÉTODO es responsable de cumplirla.      │
│                 Regla LSP: el subtipo NO puede debilitarla. │
│                                                             │
│  INVARIANTE:    Condición que siempre es CIERTA para el     │
│                 objeto (antes y después de cada método).    │
│                 Regla LSP: el subtipo DEBE conservarlo.     │
└─────────────────────────────────────────────────────────────┘
```

### Violación clásica: Rectángulo y Cuadrado

El ejemplo más famoso de violación LSP:

```
        ┌────────────┐
        │ Rectangle  │
        │            │
        │ Width      │  ← Independientes: cambiar uno no afecta al otro
        │ Height     │
        │ Area()     │
        └─────┬──────┘
              │
        ┌─────┴──────┐
        │   Square   │  ← Restricción: Width == Height SIEMPRE
        │            │
        │ Width  ────│── Al cambiar Width, Height también cambia
        │ Height ────│── Al cambiar Height, Width también cambia
        └────────────┘
```

Un cuadrado ES un rectángulo en matemáticas, pero en programación **no debe heredar de Rectangle**. La razón:

```csharp
// Este código funciona con Rectangle pero FALLA con Square
void TestRectangle(Rectangle r)
{
    r.Width = 5;
    r.Height = 10;
    Assert.Equal(50, r.Area()); // Con Rectangle: ✅ 5×10=50
                                // Con Square:    ❌ 10×10=100 (Height sobreescribió Width)
}
```

El código cliente asume que `Width` y `Height` son independientes. `Square` rompe esa asunción. El cliente no debería necesitar saber si el rectángulo es en realidad un cuadrado.

### La trampa de `new` vs `override`

En C#, `new` en un método **oculta** el método de la clase base en lugar de sobrescribirlo:

```csharp
public class Bird
{
    public virtual string Fly() => "Volando...";
}

public class Penguin : Bird
{
    public new string Fly() =>
        throw new InvalidOperationException("¡Los pingüinos no vuelan!");
}

// Comportamiento según el tipo de la referencia:
Bird penguin1 = new Penguin();
penguin1.Fly();  // "Volando..." ← ¡MAL! Un pingüino NO vuela.
                 // Se ejecuta Bird.Fly(), no Penguin.Fly()

Penguin penguin2 = new Penguin();
penguin2.Fly();  // 💥 InvalidOperationException ← ¡PEOR! Explota.
```

`new` rompe el polimorfismo. El comportamiento depende del tipo de la **variable** (tiempo de compilación), no del tipo del **objeto** (tiempo de ejecución). El código que programa contra `Bird` nunca sabrá que `Penguin.Fly()` lanza excepción.

### Reglas prácticas para cumplir LSP

1. **No refuerces precondiciones**: si la base acepta `amount > 0`, el subtipo no puede exigir `amount > 100`.
2. **No debilites postcondiciones**: si la base garantiza que `Balance` se reduce, el subtipo debe garantizarlo también.
3. **Conserva los invariantes**: si la base garantiza `0 <= Count <= Capacity`, el subtipo también.
4. **No lances excepciones nuevas**: si la base no declara `NotSupportedException`, el subtipo no debería lanzarla.
5. **No cambies el significado de los métodos**: si `Add()` añade un elemento, el subtipo no puede hacer que `Add()` envíe un email.

---

## 3. Ejemplo mínimo

```csharp
// ─── VIOLACIÓN LSP: Pato de goma que no hace "cuac" ───
public class Duck
{
    public virtual string Quack() => "¡Cuac!";
    public virtual string Swim() => "Nadando...";
}

public class RubberDuck : Duck
{
    // Un pato de goma NO hace cuac — suena un silbato.
    // Pero la interfaz obliga a implementar Quack().
    public override string Quack() =>
        throw new NotSupportedException("Un pato de goma no hace cuac.");

    // LSP ROTO: el código que itera sobre List<Duck> y llama a Quack()
    // explotará cuando llegue al RubberDuck.
}

// ─── CORRECTO LSP: Separar comportamiento en interfaces ───
public interface IQuackable
{
    string Quack();
}

public interface ISwimmable
{
    string Swim();
}

// Un pato real hace cuac Y nada
public class MallardDuck : IQuackable, ISwimmable
{
    public string Quack() => "¡Cuac!";
    public string Swim() => "Nadando en el estanque...";
}

// Un pato de goma solo nada, NO hace cuac → no implementa IQuackable
public class RubberDuck : ISwimmable
{
    public string Swim() => "Flotando en la bañera...";
}

// Ahora el código que espera IQuackable nunca recibirá un RubberDuck.
// El compilador lo impide. LSP cumplido.
```

Para ejecutar el ejemplo completo con todas las demos:

```bash
dotnet run --project ejemplos/m03-solid/Leccion-03-LSP/src/SolidLSP
```

---

## 4. Ejemplo real

El código en `src/SolidLSP/Program.cs` contiene **5 secciones** que cubren todas las caras de LSP. Está organizado en namespaces independientes:

### Sección 1 (línea 31): Rectángulo y Cuadrado — violación clásica

- `Rectangle` con `Width` y `Height` independientes.
- `Square : Rectangle` con setters que fuerzan `Width == Height`.
- `RectangleTest.TestRectangle()` rompe con `Square` porque espera que Width y Height sean independientes.

### Sección 2 (línea 147): `new` y polimorfismo roto

- `Bird` con método `virtual Fly()`.
- `Penguin : Bird` con `new string Fly()` que lanza excepción.
- Demostración de que `Bird penguin = new Penguin()` llama a `Bird.Fly()` (incorrecto conceptualmente), mientras que `Penguin penguin = new Penguin()` lanza excepción (incorrecto en tiempo de ejecución).

### Sección 3 (línea 254): Precondiciones reforzadas

- `Account` con `Withdraw(amount)` que solo exige `amount > 0`.
- `SavingsAccount : Account` que añade la precondición `amount <= Balance`.
- Código cliente que programa contra `Account` retira más del saldo disponible y explota.

### Sección 4 (línea 384): Cumpliendo LSP — soluciones correctas

- **Solución 1**: `IShape` para `Rectangle` y `Square` (interfaz común sin setters problemáticos).
- **Solución 2**: `IFlyable` para aves que vuelan. `Penguin : Bird` no implementa `IFlyable`.
- **Solución 3**: `IAccount` con `TryWithdraw()` que devuelve `bool` en lugar de lanzar excepción. `SavingsAccount` rechaza retiros devolviendo `false`, sin romper el contrato.

### Sección 5 (línea 652): Diseño por Contrato aplicado

- `BoundedQueue<T>` con invariantes documentados: `0 <= Count <= Capacity`.
- `LoggingBoundedQueue<T> : BoundedQueue<T>` que añade logging sin romper el contrato.
- Método `ValidarInvariante()` que verifica que los invariantes se mantienen.

---

## 5. Buenas prácticas

### ✅ QUÉ HACER

1. **Diseñar por contrato**: documenta precondiciones, postcondiciones e invariantes aunque sea en comentarios.
2. **Usar composición sobre herencia**: si dudas si el subtipo puede sustituir a la base, usa composición. `SavingsAccount` contiene un `StandardAccount` en lugar de heredarlo.
3. **Separar comportamientos en interfaces**: `IFlyable`, `ISwimmable`, `IQuackable`. Cada clase implementa solo lo que realmente hace.
4. **Preferir métodos que devuelvan resultado** (`TryXxx`, `bool`, `Result<T>`) en lugar de lanzar excepciones para flujo de control.
5. **Respetar el significado semántico de los métodos**: `Add()` debe añadir, `Remove()` debe quitar. No les cambies el significado en subtipos.

### ❌ QUÉ EVITAR

1. **`throw new NotImplementedException()`** o **`throw new NotSupportedException()`** en subtipos: es la confesión de que la herencia no tiene sentido.
2. **`new` en métodos**: oculta el método base sin polimorfismo. Solo tiene sentido en casos muy concretos (ej: cambiar el tipo de retorno a uno más específico).
3. **`if (x is Subtipo)` en código cliente**: si necesitas comprobar el tipo real, la abstracción no está funcionando.
4. **Reforzar precondiciones en subtipos**: si la base acepta valores negativos, el subtipo no puede rechazarlos.
5. **Debilitar postcondiciones en subtipos**: si la base garantiza que algo se guarda, el subtipo también debe garantizarlo.

### ¿Por qué?

LSP garantiza que el polimorfismo funciona. Sin LSP, no puedes escribir código genérico contra una abstracción porque cada implementación se comporta de forma impredecible. LSP es lo que hace que el polimorfismo sea seguro y predecible.

---

## 6. Errores habituales

### Error 1: Forzar herencia del mundo real en el código

```csharp
// ❌ MAL: Pingüino hereda de Ave que vuela
public class Ave
{
    public virtual void Volar() { /* ... */ }
}

public class Pingüino : Ave
{
    public override void Volar() =>
        throw new NotSupportedException("Los pingüinos no vuelan.");
}
```

**Por qué está mal**: la herencia en OOP no es una taxonomía biológica. Es un contrato de comportamiento. Si el subtipo no puede cumplir el contrato, no debe heredar.

**Versión correcta**:

```csharp
// ✅ BIEN: Separar el vuelo en una interfaz
public interface IVolador { void Volar(); }
public class Ave { /* características comunes */ }
public class Gorrión : Ave, IVolador { public void Volar() { /* ... */ } }
public class Pingüino : Ave { /* no implementa IVolador */ }
```

### Error 2: Usar `new` por desconocimiento de `override`

```csharp
// ❌ MAL: new en lugar de override (normalmente sin querer)
public class Base
{
    public void DoWork() => Console.WriteLine("Base");
}

public class Derived : Base
{
    public new void DoWork() => Console.WriteLine("Derived");
    // El compilador da warning: "use new u override"
    // El desarrollador pone 'new' para quitarlo sin entender las consecuencias.
}

Base obj = new Derived();
obj.DoWork(); // "Base" ← ¡El desarrollador esperaba "Derived"!
```

**Por qué está mal**: `new` rompe el polimorfismo. El método que se ejecuta depende del tipo de la variable, no del tipo del objeto. Esto viola LSP porque el subtipo no se comporta como la base cuando se accede a través de una referencia base.

### Error 3: Lanzar excepciones más restrictivas

```csharp
// ❌ MAL: Subtipo lanza excepción que la base no declara
public class FileStore
{
    public virtual string Read(string path)
    {
        return File.ReadAllText(path); // Puede lanzar IOException
    }
}

public class ReadOnlyFileStore : FileStore
{
    public override string Read(string path)
    {
        if (path.Contains("secret"))
            throw new UnauthorizedAccessException("Acceso denegado.");
            // ❌ La base NUNCA lanza UnauthorizedAccessException.
            //    El código cliente no lo espera. LSP violado.
        return base.Read(path);
    }
}
```

---

## 7. Ejercicio guiado

### Enunciado

Tienes un sistema de gestión de vehículos. La jerarquía actual viola LSP porque `ElectricCar` no tiene combustible pero hereda de `Vehicle` que asume que todos los vehículos usan combustible.

```csharp
public class Vehicle
{
    public string Model { get; set; }

    // Asume que todos los vehículos tienen combustible
    public virtual void Refuel(double liters)
    {
        Console.WriteLine($"Repostando {liters} litros de combustible...");
        // Lógica de repostaje...
    }

    public virtual double GetFuelLevel()
    {
        return 100.0; // Tanque lleno
    }
}

public class Car : Vehicle
{
    public override void Refuel(double liters)
    {
        Console.WriteLine($"Coche {Model}: repostando {liters}L de gasolina.");
    }
}

public class Truck : Vehicle
{
    public override void Refuel(double liters)
    {
        Console.WriteLine($"Camión {Model}: repostando {liters}L de diésel.");
    }
}

// ⚠️ VIOLACIÓN LSP: un coche eléctrico NO tiene combustible
public class ElectricCar : Vehicle
{
    public override void Refuel(double liters)
    {
        throw new NotSupportedException(
            "Los coches eléctricos no usan combustible. Se recargan, no se repostan.");
    }

    public override double GetFuelLevel()
    {
        throw new NotSupportedException(
            "Los coches eléctricos no tienen nivel de combustible.");
    }
}
```

### Objetivo

Refactoriza la jerarquía para que cumpla LSP. Ningún subtipo debe lanzar `NotSupportedException`. Encuentra una abstracción común y separa los comportamientos que no aplican a todos.

### Solución paso a paso

#### Paso 1: Identificar qué es COMÚN a todos los vehículos

Todos los vehículos tienen `Model` y pueden `Drive()` (moverse). Eso es lo que va en la clase/interfaz base.

#### Paso 2: Separar los comportamientos divergentes

No todos los vehículos se repostan con combustible. Los eléctricos se recargan. Esto sugiere dos interfaces separadas:

```csharp
// Abstracción común a TODOS los vehículos
public interface IVehicle
{
    string Model { get; }
    void Drive();
}

// Vehículos que usan combustible líquido
public interface IFuelPowered
{
    void Refuel(double liters);
    double GetFuelLevel();
}

// Vehículos eléctricos
public interface IElectricPowered
{
    void Recharge(double kWh);
    double GetBatteryLevel();
}
```

#### Paso 3: Implementar cada tipo de vehículo

```csharp
// Un coche de gasolina implementa IVehicle + IFuelPowered
public class Car : IVehicle, IFuelPowered
{
    public string Model { get; }
    private double _fuelLevel = 100;

    public Car(string model) => Model = model;

    public void Drive()
    {
        Console.WriteLine($"[{Model}] Conduciendo...");
        _fuelLevel -= 5;
    }

    public void Refuel(double liters)
    {
        Console.WriteLine($"[{Model}] Repostando {liters}L de gasolina.");
        _fuelLevel = Math.Min(100, _fuelLevel + liters);
    }

    public double GetFuelLevel() => _fuelLevel;
}

// Un camión también es IFuelPowered
public class Truck : IVehicle, IFuelPowered
{
    public string Model { get; }
    private double _fuelLevel = 200;

    public Truck(string model) => Model = model;

    public void Drive()
    {
        Console.WriteLine($"[{Model}] Transportando carga...");
        _fuelLevel -= 10;
    }

    public void Refuel(double liters)
    {
        Console.WriteLine($"[{Model}] Repostando {liters}L de diésel.");
        _fuelLevel = Math.Min(200, _fuelLevel + liters);
    }

    public double GetFuelLevel() => _fuelLevel;
}

// Un coche eléctrico implementa IVehicle + IElectricPowered
public class ElectricCar : IVehicle, IElectricPowered
{
    public string Model { get; }
    private double _batteryLevel = 100;

    public ElectricCar(string model) => Model = model;

    public void Drive()
    {
        Console.WriteLine($"  [{Model}] Conduciendo en silencio...");
        _batteryLevel -= 8;
    }

    public void Recharge(double kWh)
    {
        Console.WriteLine($"  [{Model}] Recargando {kWh} kWh.");
        _batteryLevel = Math.Min(100, _batteryLevel + kWh);
    }

    public double GetBatteryLevel() => _batteryLevel;
}
```

#### Paso 4: Código cliente que usa las abstracciones correctas

```csharp
// Función genérica: funciona con CUALQUIER vehículo
void TestDrive(IVehicle vehicle)
{
    vehicle.Drive();
}

// Función específica: solo vehículos de combustible
void RefuelIfNeeded(IFuelPowered vehicle)
{
    if (vehicle.GetFuelLevel() < 20)
        vehicle.Refuel(50);
}

// Función específica: solo vehículos eléctricos
void RechargeIfNeeded(IElectricPowered vehicle)
{
    if (vehicle.GetBatteryLevel() < 30)
        vehicle.Recharge(40);
}

// ─── Uso ───
IVehicle[] fleet = {
    new Car("Toyota Corolla"),
    new Truck("Volvo FH"),
    new ElectricCar("Tesla Model 3"),
};

foreach (IVehicle v in fleet)
{
    TestDrive(v); // ✅ Funciona con todos, sin excepciones
}

// El compilador impide pasar ElectricCar a RefuelIfNeeded:
// RefuelIfNeeded(new ElectricCar("Tesla")); ← Error de compilación ✅
```

### ¿Qué hemos ganado?

- Cero `NotSupportedException`. Cada vehículo implementa solo lo que realmente puede hacer.
- El código que itera sobre `IVehicle` nunca falla.
- El compilador nos protege: no puedes pasar un `ElectricCar` donde se espera `IFuelPowered`.
- Añadir un `HydrogenCar` (que implemente `IFuelPowered` para hidrógeno) es trivial.

---

## 8. Reto

**NO lo resuelvas todavía. Inténtalo por tu cuenta antes de mirar la solución.**

Tienes esta jerarquía de figuras geométricas:

```csharp
public class Shape
{
    public virtual double Width { get; set; }
    public virtual double Height { get; set; }

    public virtual void Resize(double factor)
    {
        Width *= factor;
        Height *= factor;
    }

    public virtual double Area() => Width * Height;
}

public class Circle : Shape
{
    // Sobrescribe Width/Height para que siempre sean iguales (diámetro)
    public override double Width
    {
        get => base.Width;
        set { base.Width = value; base.Height = value; }
    }

    public override double Height
    {
        get => base.Height;
        set { base.Height = value; base.Width = value; }
    }

    public override double Area() => Math.PI * (Width / 2) * (Width / 2);
}

public class FixedSquare : Shape
{
    private bool _locked = false;

    public override void Resize(double factor)
    {
        if (_locked)
            throw new InvalidOperationException(
                "Esta figura tiene tamaño fijo. No se puede redimensionar.");

        base.Resize(factor);
    }
}
```

**Reto**: Identifica todas las violaciones de LSP en este código y refactoriza la jerarquía para que las cumpla. Pista: hay al menos 3 violaciones distintas de LSP.

---

## 9. Resumen

| Concepto | Descripción |
|---|---|
| **LSP** | Los subtipos deben poder sustituir a sus tipos base sin romper el programa. |
| **Precondición** | Lo que el método exige. El subtipo NO PUEDE reforzarla. |
| **Postcondición** | Lo que el método garantiza. El subtipo NO PUEDE debilitarla. |
| **Invariante** | Condición siempre cierta del objeto. El subtipo DEBE conservarla. |
| **`new` vs `override`** | `new` oculta sin polimorfismo. `override` participa en polimorfismo. |
| **Composición sobre herencia** | Si dudas si la herencia cumple LSP, usa composición. |

**Conceptos importantes**:
- LSP es el principio que valida si tu herencia es correcta. Si un subtipo no puede sustituir a la base, la herencia está mal aplicada.
- Diseño por Contrato (precondiciones, postcondiciones, invariantes) es la base teórica de LSP.
- La solución a una violación de LSP casi nunca es "arreglar el subtipo", sino **rediseñar la jerarquía**: extraer interfaces, usar composición o cambiar la clase base.
- LSP es lo que permite que el polimorfismo sea seguro. Sin LSP, no puedes confiar en que `List<Base>` funcione con elementos de distintos subtipos.

---

## 10. Preguntas frecuentes

**P: ¿Un Cuadrado NUNCA debe heredar de Rectángulo?**

R: En un modelo mutable (donde puedes cambiar Width y Height independientemente), NO. En un modelo inmutable (donde Width y Height se establecen en el constructor y nunca cambian), un `Square : Rectangle` **puede** ser correcto porque no hay setters que rompan el invariante.

**P: ¿Cuándo debo usar `new` en lugar de `override`?**

R: Casi nunca. `new` es útil en escenarios muy específicos como cambiar el tipo de retorno por uno más específico (covarianza de retorno) o cuando no tienes control sobre la clase base. Si puedes usar `override`, úsalo.

**P: ¿LSP solo aplica a herencia de clases?**

R: No. LSP aplica a cualquier relación de subtipo: interfaces, clases abstractas, herencia concreta. Si implementas una interfaz, tu implementación debe cumplir el contrato completo de la interfaz. No puedes implementar `IList<T>` y lanzar `NotSupportedException` en `Add()`.

**P: ¿Cómo detecto una violación de LSP en una code review?**

R: Busca: (1) `NotImplementedException` o `NotSupportedException` en métodos heredados, (2) `if (x is Subtipo)` en código cliente, (3) setters que tienen efectos colaterales ocultos, (4) métodos que lanzan excepciones que la base no documenta, (5) precondiciones añadidas en comentarios del tipo "cuidado: este método requiere que...".

**P: ¿Qué diferencia hay entre LSP y el Principio de Abierto/Cerrado (OCP)?**

R: OCP dice que debes poder extender sin modificar. LSP dice que la extensión (el subtipo) debe ser intercambiable con la base. Son complementarios: OCP te da el mecanismo para extender (interfaces + polimorfismo), LSP te da la garantía de que la extensión no romperá nada.
