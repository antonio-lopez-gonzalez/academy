// =====================================================================
// Módulo 3 — SOLID en C#
// Lección 02 — Principio de Abierto/Cerrado (OCP)
//
// OCP: "Abierto a extensión, cerrado a modificación"
//
// Una clase debe permitir extender su comportamiento sin necesidad de
// modificar su código fuente. Esto se logra mediante abstracciones
// (interfaces, herencia) y polimorfismo.
// =====================================================================

using System.Text.Json;

namespace SolidOCP;

// =====================================================================
// PARTE 1: MODELOS DE DOMINIO (formas geométricas)
// =====================================================================

// --- Versión Mala: Tipos concretos sin abstracción ---
// Cada forma es una clase aislada. No comparten contrato común.
// AreaCalculator tiene que inspeccionar el tipo con is/as/if/else.

public class BadCircle
{
    public decimal Radius { get; set; }
}

public class BadRectangle
{
    public decimal Width { get; set; }
    public decimal Height { get; set; }
}

public class BadTriangle
{
    public decimal Base { get; set; }
    public decimal Height { get; set; }
}

// Nueva forma: Hexágono regular
public class BadHexagon
{
    public decimal Side { get; set; }
}

// =====================================================================
// VIOLACIÓN OCP: AreaCalculator conoce TODOS los tipos concretos.
// Cada vez que se añade una forma hay que MODIFICAR esta clase.
// =====================================================================

public class BadAreaCalculator
{
    // Este método VIOLA OCP porque:
    // 1. Usa if/else anidados para discriminar tipos
    // 2. Cada nueva forma obliga a tocar este código
    // 3. El compilador no puede avisarnos si olvidamos una forma
    // 4. Crece sin límite y se vuelve frágil
    public decimal CalculateArea(object shape)
    {
        if (shape is BadCircle circle)
        {
            // Área del círculo: π * r²
            return (decimal)Math.PI * circle.Radius * circle.Radius;
        }
        else if (shape is BadRectangle rectangle)
        {
            // Área del rectángulo: base * altura
            return rectangle.Width * rectangle.Height;
        }
        else if (shape is BadTriangle triangle)
        {
            // Área del triángulo: (base * altura) / 2
            return triangle.Base * triangle.Height / 2m;
        }
        else if (shape is BadHexagon hexagon)
        {
            // Área del hexágono regular: (3 * √3 / 2) * lado²
            decimal sqrt3 = 1.7320508075688772m;
            return 3m * sqrt3 / 2m * hexagon.Side * hexagon.Side;
        }
        else
        {
            throw new ArgumentException(
                $"Forma no soportada: {shape.GetType().Name}. " +
                "Hay que modificar BadAreaCalculator para añadirla.");
        }
    }

    // Calcula el área total de una colección de formas
    public decimal TotalArea(IEnumerable<object> shapes)
    {
        decimal total = 0m;
        foreach (object shape in shapes)
        {
            total += CalculateArea(shape);
        }
        return total;
    }
}

// =====================================================================
// PARTE 2: VERSIÓN BUENA — APLICANDO OCP
// =====================================================================

// Definimos una abstracción (interfaz) que todas las formas implementan.
// AreaCalculator depende de la abstracción, no de los detalles concretos.
// Nuevas formas = nuevas clases. Cero modificaciones en AreaCalculator.

public interface IShape
{
    // Cada forma sabe cómo calcular su propia área.
    // El contrato es simple: un método sin parámetros que devuelve decimal.
    decimal CalculateArea();
}

public class Circle : IShape
{
    public decimal Radius { get; set; }

    public decimal CalculateArea()
    {
        return (decimal)Math.PI * Radius * Radius;
    }
}

public class Rectangle : IShape
{
    public decimal Width { get; set; }
    public decimal Height { get; set; }

    public decimal CalculateArea()
    {
        return Width * Height;
    }
}

public class Triangle : IShape
{
    public decimal Base { get; set; }
    public decimal Height { get; set; }

    public decimal CalculateArea()
    {
        return Base * Height / 2m;
    }
}

// =====================================================================
// DEMOSTRACIÓN: Añadir una nueva forma SIN tocar AreaCalculator
// Basta con crear una nueva clase que implemente IShape.
// AreaCalculator no se modifica, solo se "extiende".
// =====================================================================

public class Hexagon : IShape
{
    public decimal Side { get; set; }

    public decimal CalculateArea()
    {
        // Área del hexágono regular: (3√3 / 2) * lado²
        decimal sqrt3 = 1.7320508075688772m;
        return 3m * sqrt3 / 2m * Side * Side;
    }
}

// =====================================================================
// AreaCalculator CORRECTO: cumple OCP.
// Depende de IShape (abstracción), no de clases concretas.
// =====================================================================

public class GoodAreaCalculator
{
    // Recibe cualquier colección de IShape.
    // No necesita saber qué tipo concreto es cada elemento.
    // Para añadir una forma nueva solo hay que crear una clase.
    // Esta clase NO SE MODIFICA. Está CERRADA a modificación.
    public decimal TotalArea(IEnumerable<IShape> shapes)
    {
        decimal total = 0m;
        foreach (IShape shape in shapes)
        {
            // Polimorfismo: cada objeto ejecuta su propia versión de CalculateArea
            total += shape.CalculateArea();
        }
        return total;
    }
}

// =====================================================================
// PARTE 3: STRATEGY PATTERN — OCP aplicado a comportamiento
// =====================================================================

// El patrón Strategy encapsula algoritmos intercambiables detrás
// de una interfaz común. Es OCP en estado puro: se pueden añadir
// nuevas estrategias sin tocar el código que las consume.

// Modelo de pedido
public record Order(Guid Id, decimal Total, string Country);

// Abstracción: interfaz para cualquier calculadora de impuestos
public interface ITaxCalculator
{
    decimal CalculateTax(Order order);

    // Nombre descriptivo para el demo
    string CountryName { get; }
}

// Estrategia concreta 1: IVA español (21%)
public class SpainTaxCalculator : ITaxCalculator
{
    public string CountryName => "España";

    public decimal CalculateTax(Order order)
    {
        // En España, el IVA general es del 21%
        const decimal ivaRate = 0.21m;
        return order.Total * ivaRate;
    }
}

// Estrategia concreta 2: Sales Tax de EEUU (varía por estado, simplificamos al 8%)
public class UsTaxCalculator : ITaxCalculator
{
    public string CountryName => "EEUU";

    public decimal CalculateTax(Order order)
    {
        // En EEUU el sales tax varía por estado. Usamos 8% como media.
        const decimal salesTaxRate = 0.08m;
        return order.Total * salesTaxRate;
    }
}

// Estrategia concreta 3: Exportación (0% IVA — exento)
public class ExportTaxCalculator : ITaxCalculator
{
    public string CountryName => "Exportación";

    public decimal CalculateTax(Order order)
    {
        // Las exportaciones están exentas de IVA
        return 0m;
    }
}

// Estrategia concreta 4: Reino Unido (VAT 20%)
public class UkTaxCalculator : ITaxCalculator
{
    public string CountryName => "Reino Unido";

    public decimal CalculateTax(Order order)
    {
        const decimal vatRate = 0.20m;
        return order.Total * vatRate;
    }
}

// =====================================================================
// TaxService: consume ITaxCalculator sin conocer la implementación.
// Cumple OCP: añadir un nuevo país = añadir una nueva clase.
// TaxService NO SE MODIFICA.
// =====================================================================

public class TaxService
{
    private readonly ITaxCalculator _taxCalculator;

    // Inyección por constructor: recibe cualquier ITaxCalculator
    // No sabe si es España, EEUU, UK...
    public TaxService(ITaxCalculator taxCalculator)
    {
        _taxCalculator = taxCalculator;
    }

    public void PrintTax(Order order)
    {
        decimal tax = _taxCalculator.CalculateTax(order);
        Console.WriteLine(
            $"  Pedido {order.Id} | País: {_taxCalculator.CountryName,-14} | " +
            $"Subtotal: {order.Total,8:C} | Impuesto: {tax,8:C} | " +
            $"Total: {order.Total + tax,8:C}");
    }
}

// =====================================================================
// PARTE 4: PROGRAMA PRINCIPAL — DEMOSTRACIÓN
// =====================================================================

public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine(new string('=', 70));
        Console.WriteLine("  LECCIÓN 02 — Principio de Abierto/Cerrado (OCP)");
        Console.WriteLine(new string('=', 70));

        DemoViolacionOcp();
        DemoOcpCorrecto();
        DemoStrategyPattern();
        DemoExtensionSinModificar();

        Console.WriteLine(new string('=', 70));
        Console.WriteLine("  FIN DE LA DEMOSTRACIÓN");
        Console.WriteLine(new string('=', 70));
    }

    // -----------------------------------------------------------------
    // Demo 1: Versión que VIOLA OCP
    // -----------------------------------------------------------------
    private static void DemoViolacionOcp()
    {
        Console.WriteLine("\n--- 1. VIOLACIÓN OCP: BadAreaCalculator ---\n");

        BadAreaCalculator badCalculator = new();

        object[] shapes = [
            new BadCircle { Radius = 5m },
            new BadRectangle { Width = 4m, Height = 6m },
            new BadTriangle { Base = 3m, Height = 8m },
        ];

        Console.WriteLine("  Calculando áreas con BadAreaCalculator (usa if/else):");
        foreach (object shape in shapes)
        {
            decimal area = badCalculator.CalculateArea(shape);
            Console.WriteLine($"  - {shape.GetType().Name,-15} => Área: {area,10:F2}");
        }

        decimal total = badCalculator.TotalArea(shapes);
        Console.WriteLine($"\n  Área total: {total:F2}");

        Console.WriteLine("\n  [PROBLEMA] Si queremos añadir Hexagon, debemos MODIFICAR");
        Console.WriteLine("  BadAreaCalculator.CalculateArea() añadiendo otro else if.");
        Console.WriteLine("  Esto rompe OCP: la clase no está cerrada a modificación.");
    }

    // -----------------------------------------------------------------
    // Demo 2: Versión CORRECTA con OCP
    // -----------------------------------------------------------------
    private static void DemoOcpCorrecto()
    {
        Console.WriteLine("\n--- 2. OCP CORRECTO: GoodAreaCalculator con IShape ---\n");

        GoodAreaCalculator goodCalculator = new();

        // Polimorfismo: tratamos todos como IShape
        IShape[] shapes = [
            new Circle { Radius = 5m },
            new Rectangle { Width = 4m, Height = 6m },
            new Triangle { Base = 3m, Height = 8m },
        ];

        Console.WriteLine("  Calculando áreas con GoodAreaCalculator (usa IShape):");
        foreach (IShape shape in shapes)
        {
            decimal area = shape.CalculateArea();
            Console.WriteLine($"  - {shape.GetType().Name,-12} => Área: {area,10:F2}");
        }

        decimal total = goodCalculator.TotalArea(shapes);
        Console.WriteLine($"\n  Área total: {total:F2}");

        Console.WriteLine("\n  [VENTAJA] Añadir Hexagon NO requiere tocar GoodAreaCalculator.");
        Console.WriteLine("  La clase está CERRADA a modificación, ABIERTA a extensión.");
    }

    // -----------------------------------------------------------------
    // Demo 3: Strategy Pattern (OCP aplicado a algoritmos)
    // -----------------------------------------------------------------
    private static void DemoStrategyPattern()
    {
        Console.WriteLine("\n--- 3. STRATEGY PATTERN: Calculadoras de impuestos ---\n");

        Order order1 = new(Guid.NewGuid(), 100m, "ES");
        Order order2 = new(Guid.NewGuid(), 250m, "US");
        Order order3 = new(Guid.NewGuid(), 500m, "EXPORT");

        Console.WriteLine("  Procesando pedidos con distintas estrategias de impuesto:\n");

        // Cada TaxService se construye con una estrategia diferente.
        // TaxService no sabe cuál es; solo consume ITaxCalculator.

        TaxService spainService = new(new SpainTaxCalculator());
        spainService.PrintTax(order1);

        TaxService usService = new(new UsTaxCalculator());
        usService.PrintTax(order2);

        TaxService exportService = new(new ExportTaxCalculator());
        exportService.PrintTax(order3);

        // Añadimos UK sin tocar TaxService: solo creamos UkTaxCalculator
        TaxService ukService = new(new UkTaxCalculator());
        Order order4 = new(Guid.NewGuid(), 300m, "UK");
        ukService.PrintTax(order4);

        Console.WriteLine("\n  [VENTAJA] Añadir UK requirió solo una clase nueva.");
        Console.WriteLine("  TaxService NO se modificó. Strategy = OCP en acción.");
    }

    // -----------------------------------------------------------------
    // Demo 4: Extensión sin modificación — comparativa directa
    // -----------------------------------------------------------------
    private static void DemoExtensionSinModificar()
    {
        Console.WriteLine("\n--- 4. COMPARATIVA: Añadir Hexagon ---\n");

        // ─── Versión mala ───
        Console.WriteLine("  [VERSIÓN MALA — BadAreaCalculator con else if]");
        Console.WriteLine("  Para añadir Hexagon hay que tocar 3 sitios:");
        Console.WriteLine("    1. Crear la clase BadHexagon (ok)");
        Console.WriteLine("    2. Añadir 'else if (shape is BadHexagon hexagon)' en CalculateArea()");
        Console.WriteLine("    3. Implementar la fórmula del área dentro del else if");
        Console.WriteLine("  => MODIFICAR código existente = riesgo de bugs.\n");

        // ─── Versión buena ───
        Console.WriteLine("  [VERSIÓN BUENA — GoodAreaCalculator con IShape]");
        Console.WriteLine("  Para añadir Hexagon solo hay que tocar 1 sitio:");
        Console.WriteLine("    1. Crear 'class Hexagon : IShape' e implementar CalculateArea()");
        Console.WriteLine("  => CERO modificaciones en código existente.\n");

        // Demostración con el Hexagon ya creado
        GoodAreaCalculator calculator = new();

        IShape[] allShapes = [
            new Circle { Radius = 5m },
            new Rectangle { Width = 4m, Height = 6m },
            new Triangle { Base = 3m, Height = 8m },
            new Hexagon { Side = 6m }, // ¡Añadido sin tocar GoodAreaCalculator!
        ];

        Console.WriteLine("  Todas las formas con Hexagon incluido (sin modificar calculadora):");
        foreach (IShape shape in allShapes)
        {
            decimal area = shape.CalculateArea();
            Console.WriteLine($"  - {shape.GetType().Name,-12} => Área: {area,10:F2}");
        }

        decimal totalAll = calculator.TotalArea(allShapes);
        Console.WriteLine($"\n  Área total (incluyendo hexágono): {totalAll:F2}");

        Console.WriteLine("\n  [CONCLUSIÓN] El código bueno (OCP) es: más seguro, más");
        Console.WriteLine("  mantenible, más extensible y más testeable.");
    }
}
