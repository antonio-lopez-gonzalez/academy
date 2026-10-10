// ============================================================
// Lección 02 — Abstract, Virtual, Override y New
// Módulo 2: POO en C#
//
// Este programa demuestra:
//   1. Clases y métodos abstract  (Shape → Circle/Rectangle/Triangle)
//   2. Métodos virtual y override (Animal → Dog/Cat/Bird)
//   3. Ocultación con new vs polimorfismo real con override
//   4. sealed override para bloquear herencia adicional
// ============================================================

using System.Text;

Console.OutputEncoding = Encoding.UTF8;

// ──────────────────────────────────────────────────────────
// 1. ABSTRACT — Polimorfismo con Shape
// ──────────────────────────────────────────────────────────
Console.WriteLine("═══════════════════════════════════════════");
Console.WriteLine("  1. ABSTRACT — Clases y métodos abstractos");
Console.WriteLine("═══════════════════════════════════════════");
Console.WriteLine();

List<Shape> shapes = new()
{
    new Circle("Círculo", 5.0),
    new Rectangle("Rectángulo", 4.0, 6.0),
    new Triangle("Triángulo", 3.0, 8.0)
};

foreach (Shape shape in shapes)
{
    // GetDescription() es virtual → se puede sobrescribir o usar la base
    Console.WriteLine(shape.GetDescription());
    // Sides es abstract → cada subclase DEBE implementarlo
    Console.WriteLine($"  Lados              : {shape.Sides}");
    // CalculateArea() es abstract → cada subclase DEBE implementarlo
    Console.WriteLine($"  Área               : {shape.CalculateArea():F2}");
    // Mostramos el tipo real en ejecución
    Console.WriteLine($"  Tipo en ejecución  : {shape.GetType().Name}");
    Console.WriteLine();
}

// ──────────────────────────────────────────────────────────
// 2. VIRTUAL y OVERRIDE — Despacho virtual (polimorfismo real)
// ──────────────────────────────────────────────────────────
Console.WriteLine("═══════════════════════════════════════════");
Console.WriteLine("  2. VIRTUAL y OVERRIDE — Animales");
Console.WriteLine("═══════════════════════════════════════════");
Console.WriteLine();

List<Animal> animals = new()
{
    new Dog("Rex"),
    new Cat("Mishi"),
    new Bird("Piolín"),
    new Animal("Criatura desconocida")  // Usa las implementaciones base
};

// Aunque la variable es de tipo Animal, en ejecución se llama al método
// de la subclase gracias al mecanismo de despacho virtual (vtable).
foreach (Animal animal in animals)
{
    Console.WriteLine($"[{animal.GetType().Name}] {animal.Name}:");
    Console.WriteLine($"  Sonido  : {animal.MakeSound()}");
    Console.WriteLine($"  Moverse : {animal.Move()}");
    Console.WriteLine();
}

// ──────────────────────────────────────────────────────────
// 3. DEMO EXTRA — virtual vs no-virtual
//    Llamar al método directamente sin polimorfismo (no virtual)
// ──────────────────────────────────────────────────────────
Console.WriteLine("--- Demo: método virtual vs no virtual ---");
Animal genericDog = new Dog("Toby");
Dog concreteDog = new Dog("Toby");
// Con referencia Animal → despacho virtual → usa Dog.MakeSound()
Console.WriteLine($"  Animal reference  → {genericDog.MakeSound()}");
// Con referencia Dog → llama a Dog.MakeSound() directamente
Console.WriteLine($"  Dog reference     → {concreteDog.MakeSound()}");
Console.WriteLine();

// ──────────────────────────────────────────────────────────
// 4. NEW — Ocultación de métodos (method hiding)
// ──────────────────────────────────────────────────────────
Console.WriteLine("═══════════════════════════════════════════");
Console.WriteLine("  3. NEW vs OVERRIDE — La gran diferencia");
Console.WriteLine("═══════════════════════════════════════════");
Console.WriteLine();

// La clave está en el TIPO DE LA REFERENCIA, no en el tipo del objeto.
// - override: participa en el despacho virtual → siempre se llama al método del objeto real
// - new: NO participa en el despacho virtual → se llama al método según el tipo de la referencia

Console.WriteLine("--- Caso A: Referencia de la clase base ---");
BaseClass baseRefNew = new DerivedWithNew();
BaseClass baseRefOverride = new DerivedWithOverride();

Console.Write("  DerivedWithNew    con referencia Base → ");
baseRefNew.Show();      // ¡Llama a Base.Show()! El new NO es polimórfico.

Console.Write("  DerivedWithOverride con referencia Base → ");
baseRefOverride.Show(); // Llama a DerivedWithOverride.Show(). El override SÍ es polimórfico.
Console.WriteLine();

Console.WriteLine("--- Caso B: Referencia de la clase derivada ---");
DerivedWithNew derivedNew = new DerivedWithNew();
DerivedWithOverride derivedOverride = new DerivedWithOverride();

Console.Write("  DerivedWithNew    con referencia Derived → ");
derivedNew.Show();      // Llama a DerivedWithNew.Show() (tipo de referencia = DerivedWithNew)

Console.Write("  DerivedWithOverride con referencia Derived → ");
derivedOverride.Show(); // Llama a DerivedWithOverride.Show()
Console.WriteLine();

// ──────────────────────────────────────────────────────────
// 5. SEALED OVERRIDE — Bloquear herencia adicional
// ──────────────────────────────────────────────────────────
Console.WriteLine("--- Demo: sealed override ---");
FinalDog finalDog = new FinalDog("Max");
Console.WriteLine($"  FinalDog.MakeSound() → {finalDog.MakeSound()}");
Console.WriteLine("  Ninguna subclase de FinalDog puede volver a sobrescribir MakeSound().");
Console.WriteLine();

// ──────────────────────────────────────────────────────────
// 6. RESUMEN VISUAL — Tabla comparativa
// ──────────────────────────────────────────────────────────
Console.WriteLine("═══════════════════════════════════════════");
Console.WriteLine("  RESUMEN");
Console.WriteLine("═══════════════════════════════════════════");
Console.WriteLine();
Console.WriteLine("  Palabra    │ ¿Puede tener cuerpo? │ ¿Obliga a heredar? │ ¿Polimórfico?");
Console.WriteLine("  ───────────┼──────────────────────┼────────────────────┼──────────────");
Console.WriteLine("  abstract   │ NO                   │ SÍ (obligatorio)   │ N/A (sin cuerpo)");
Console.WriteLine("  virtual    │ SÍ                   │ NO (opcional)      │ SÍ");
Console.WriteLine("  override   │ SÍ                   │ ─                  │ SÍ");
Console.WriteLine("  new        │ SÍ                   │ NO                 │ NO");
Console.WriteLine("  sealed ovr │ SÍ                   │ NO (bloquea más)   │ SÍ");
Console.WriteLine();

Console.WriteLine("  Regla de oro:");
Console.WriteLine("  - override => 'Soy la versión definitiva, sin importar desde dónde me llamen'");
Console.WriteLine("  - new      => 'Solo existo si me llaman con mi tipo exacto'");

// ══════════════════════════════════════════════════════════
// CLASES
// ══════════════════════════════════════════════════════════

// ──────────────────────────────────────────────────────────
// SECCIÓN 1: Clases abstractas — Shape
// ──────────────────────────────────────────────────────────

/// <summary>
/// Clase abstracta base para figuras geométricas.
/// No se puede instanciar directamente: new Shape() es ilegal.
/// Sirve como "contrato" que todas las subclases deben cumplir.
/// </summary>
abstract class Shape
{
    // Propiedad de solo lectura (se asigna en el constructor)
    public string Name { get; }

    // Constructor protegido: solo las subclases pueden llamarlo.
    // Esto garantiza que toda Shape tenga un nombre.
    protected Shape(string name)
    {
        Name = name;
    }

    // Método abstracto: NO tiene cuerpo.
    // Cada subclase está OBLIGADA a implementarlo.
    public abstract double CalculateArea();

    // Propiedad abstracta: igual que un método abstracto,
    // pero expuesto como propiedad. Cada subclase DEBE implementarla.
    public abstract int Sides { get; }

    // Método virtual: TIENE implementación por defecto,
    // pero las subclases PUEDEN sobrescribirlo si quieren.
    public virtual string GetDescription()
    {
        return $"Soy un {Name}";
    }
}

/// <summary>
/// Círculo: 1 lado curvo. Área = PI * radio²
/// </summary>
class Circle : Shape
{
    public double Radius { get; }

    // Llamamos a base(name) para inicializar Name en la clase padre
    public Circle(string name, double radius) : base(name)
    {
        Radius = radius;
    }

    // Implementación obligatoria del método abstracto
    public override double CalculateArea()
    {
        return Math.PI * Radius * Radius;
    }

    // Implementación obligatoria de la propiedad abstracta
    public override int Sides => 1; // 1 lado curvo (no es un polígono)
}

/// <summary>
/// Rectángulo: 4 lados. Área = base × altura
/// </summary>
class Rectangle : Shape
{
    public double Width { get; }
    public double Height { get; }

    public Rectangle(string name, double width, double height) : base(name)
    {
        Width = width;
        Height = height;
    }

    public override double CalculateArea()
    {
        return Width * Height;
    }

    public override int Sides => 4;
}

/// <summary>
/// Triángulo: 3 lados. Área = (base × altura) / 2
/// </summary>
class Triangle : Shape
{
    public double BaseLength { get; }
    public double HeightLength { get; }

    public Triangle(string name, double baseLength, double heightLength) : base(name)
    {
        BaseLength = baseLength;
        HeightLength = heightLength;
    }

    public override double CalculateArea()
    {
        return BaseLength * HeightLength / 2.0;
    }

    public override int Sides => 3;

    // Sobrescribimos GetDescription() para dar más detalle.
    // base.GetDescription() reutiliza la lógica de la clase padre.
    public override string GetDescription()
    {
        return $"{base.GetDescription()} — Base: {BaseLength}, Altura: {HeightLength}";
    }
}

// ──────────────────────────────────────────────────────────
// SECCIÓN 2: Métodos virtuales — Animal
// ──────────────────────────────────────────────────────────

/// <summary>
/// Clase base CONCRETA (no abstracta): se puede instanciar.
/// Sus métodos virtuales proporcionan un comportamiento por defecto
/// que las subclases pueden cambiar con override.
/// </summary>
class Animal
{
    public string Name { get; }

    public Animal(string name)
    {
        Name = name;
    }

    // virtual = "tengo implementación, pero mis hijos pueden cambiarla"
    public virtual string MakeSound()
    {
        return "Sonido genérico";
    }

    public virtual string Move()
    {
        return "Se mueve";
    }
}

class Dog : Animal
{
    public Dog(string name) : base(name) { }

    // override reemplaza el método virtual de la clase base.
    // El mecanismo de despacho virtual asegura que SIEMPRE se llame
    // a esta versión, incluso si la referencia es de tipo Animal.
    public override string MakeSound()
    {
        return "¡Guau!";
    }

    public override string Move()
    {
        return "Corre en cuatro patas";
    }
}

class Cat : Animal
{
    public Cat(string name) : base(name) { }

    public override string MakeSound()
    {
        return "¡Miau!";
    }

    // No sobrescribimos Move() → hereda el comportamiento base: "Se mueve"
}

class Bird : Animal
{
    public Bird(string name) : base(name) { }

    public override string MakeSound()
    {
        return "¡Pío pío!";
    }

    public override string Move()
    {
        return "Vuela por el cielo";
    }
}

/// <summary>
/// Demostración de sealed override:
/// Hereda de Dog, pero IMPIDE que las subclases de FinalDog
/// vuelvan a sobrescribir MakeSound().
/// </summary>
class FinalDog : Dog
{
    public FinalDog(string name) : base(name) { }

    // sealed override = "esta es la versión final, nadie más la cambia"
    public sealed override string MakeSound()
    {
        return "¡GUAU! (versión sellada)";
    }
}

// ──────────────────────────────────────────────────────────
// SECCIÓN 3: new vs override — Method Hiding
// ──────────────────────────────────────────────────────────

/// <summary>
/// Clase base con un método virtual.
/// </summary>
class BaseClass
{
    // virtual habilita el despacho polimórfico
    public virtual void Show()
    {
        Console.WriteLine("Base.Show()");
    }
}

/// <summary>
/// new OCULTA el método de la clase base.
/// NO participa en el despacho virtual.
/// 
/// Comportamiento:
///   BaseClass ref = new DerivedWithNew();
///   ref.Show();  →  "Base.Show()"  (¡sorpresa!)
/// 
///   DerivedWithNew ref2 = new DerivedWithNew();
///   ref2.Show(); →  "Derived.Show()"
/// </summary>
class DerivedWithNew : BaseClass
{
    // new rompe la cadena de polimorfismo.
    // El compilador avisa con CS0108 si no pones 'new' explícitamente.
    public new void Show()
    {
        Console.WriteLine("Derived.Show()  ← ocultado con new");
    }
}

/// <summary>
/// override EXTIENDE el método virtual de la clase base.
/// SÍ participa en el despacho virtual → polimorfismo real.
/// 
/// Comportamiento:
///   BaseClass ref = new DerivedWithOverride();
///   ref.Show();  →  "Derived.Show()"  (¡polimorfismo!)
/// </summary>
class DerivedWithOverride : BaseClass
{
    public override void Show()
    {
        Console.WriteLine("Derived.Show()  ← sobrescrito con override (polimórfico)");
    }
}
