// ──────────────────────────────────────────────────────────────────────────
// MÓDULO 3 — SOLID en C#
// Lección 03 — Principio de Sustitución de Liskov (LSP)
// ──────────────────────────────────────────────────────────────────────────
// LSP (Barbara Liskov, 1987):
// "Los subtipos deben poder sustituir a sus tipos base sin alterar el
//  correcto funcionamiento del programa."
//
// Si B es subtipo de A, los objetos de tipo A pueden reemplazarse por
// objetos de tipo B sin que el programa deje de funcionar.
//
// Reglas para cumplir LSP:
//   - Las precondiciones no pueden ser MÁS FUERTES en el subtipo.
//   - Las postcondiciones no pueden ser MÁS DÉBILES en el subtipo.
//   - Los invariantes de la clase base deben conservarse en el subtipo.
//   - El subtipo no debe lanzar excepciones nuevas que la base no declara.
//   - El subtipo no debe modificar el significado del método base.
// ──────────────────────────────────────────────────────────────────────────

using System;
using System.Collections.Generic;

// ──────────────────────────────────────────────────────────────────────────
// SECCIÓN 1: VIOLACIÓN CLÁSICA — Rectángulo y Cuadrado
// ──────────────────────────────────────────────────────────────────────────
// El problema: un cuadrado ES un rectángulo en geometría, pero NO debería
// ser un subtipo de Rectangle en código. La razón es que las reglas de
// mutabilidad de Rectangle (cambiar Width sin alterar Height) no se
// cumplen en Square. Violamos LSP.

namespace Violacion1_SquareRectangle
{
    // Clase base: un rectángulo con ancho y alto independientes.
    public class Rectangle
    {
        // Propiedades virtuales para que las subtipos puedan sobrescribirlas.
        public virtual double Width { get; set; }
        public virtual double Height { get; set; }

        // Propiedad calculada: el área de un rectángulo = ancho * alto.
        public double Area => Width * Height;

        public override string ToString()
            => $"Rectangle: Width={Width}, Height={Height}, Area={Area}";
    }

    // Subtipo: un cuadrado "es" un rectángulo, pero con restricción extra.
    // La restricción: Width y Height siempre deben ser iguales.
    public class Square : Rectangle
    {
        private double _side;

        public override double Width
        {
            get => _side;
            set
            {
                _side = value;
                // Forzamos que Height cambie también para mantener la forma cuadrada.
                // ¡Aquí está la trampa! Cambiar Width afecta a Height silenciosamente.
            }
        }

        public override double Height
        {
            get => _side;
            set
            {
                _side = value;
                // Igual: cambiar Height afecta a Width.
            }
        }

        public override string ToString()
            => $"Square: Side={_side}, Area={Area}";
    }

    // Código cliente que espera trabajar con Rectangle.
    // Falla con Square porque viola LSP.
    public static class RectangleTest
    {
        // Este método asume que Width y Height son independientes.
        // Funciona correctamente con Rectangle, pero ROMPE con Square.
        public static void TestRectangle(Rectangle rectangle)
        {
            rectangle.Width = 5;
            rectangle.Height = 10;

            // Esperamos: Area == 5 * 10 == 50
            double expected = 50;
            double actual = rectangle.Area;

            Console.WriteLine($"  {rectangle}");
            Console.WriteLine($"  Área esperada: {expected}, Área real: {actual}");

            if (Math.Abs(actual - expected) < 0.01)
            {
                Console.WriteLine("  ✅ LSP cumplido: el área es la esperada.");
            }
            else
            {
                Console.WriteLine("  ❌ VIOLACIÓN LSP: el área no es la esperada.");
                Console.WriteLine($"     Diferencia: {actual - expected}");
            }
            Console.WriteLine();
        }

        public static void EjecutarDemo()
        {
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine("1. VIOLACIÓN Rectángulo / Cuadrado");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine();

            Rectangle rectangle = new Rectangle();
            Console.WriteLine("→ Test con Rectangle (clase base):");
            TestRectangle(rectangle);
            // Output: Area = 50 ✅ Funciona

            Square square = new Square();
            Console.WriteLine("→ Test con Square (subtipo):");
            TestRectangle(square);
            // Output: Width=5, Height cambió a 10 por el setter de Height.
            // Luego Height=10 cambia Width también a 10. Area = 100 ❌ ¡Falla!

            // Explicación:
            Console.WriteLine("--- Explicación ---");
            Console.WriteLine("Cuando asignamos Width=5, el setter de Square guarda _side=5.");
            Console.WriteLine("Cuando asignamos Height=10, el setter de Square guarda _side=10.");
            Console.WriteLine("Pero el setter de Height también sobrescribe Width (vía _side).");
            Console.WriteLine("Resultado: _side termina en 10, Width=10, Height=10, Area=100.");
            Console.WriteLine("El cliente esperaba Area=50, pero obtiene 100.");
            Console.WriteLine();
            Console.WriteLine("Moraleja: Square NO puede sustituir a Rectangle sin romper el");
            Console.WriteLine("comportamiento esperado. La herencia viola LSP.");
            Console.WriteLine();
        }
    }
}

// ──────────────────────────────────────────────────────────────────────────
// SECCIÓN 2: VIOLACIÓN — 'new' y cómo rompe LSP
// ──────────────────────────────────────────────────────────────────────────
// Cuando un subtipo usa 'new' en lugar de 'override', oculta el método
// base pero NO participa en polimorfismo. Esto puede hacer que el
// comportamiento cambie drásticamente según el tipo de la referencia.

namespace Violacion2_NewKeyword
{
    // Clase base: un ave genérica que VUELA.
    public class Bird
    {
        // Método virtual: puede ser sobrescrito por subtipos.
        public virtual string Fly()
        {
            return "Volando por el cielo...";
        }

        public virtual string Eat()
        {
            return "Comiendo semillas...";
        }
    }

    // Subtipo: un pingüino NO vuela.
    // Usamos 'new' para OCULTAR el método Fly de Bird.
    public class Penguin : Bird
    {
        // ⚠️  'new' oculta el método, NO lo sobrescribe polimórficamente.
        //    Esto significa que el comportamiento depende del tipo de la variable
        //    (en tiempo de compilación), no del tipo real del objeto (en tiempo de ejecución).
        public new string Fly()
        {
            throw new InvalidOperationException(
                "¡Los pingüinos no vuelan! Esto es una violación LSP.");
        }

        // Sobrescribimos correctamente Eat sin problemas.
        public override string Eat()
        {
            return "Comiendo peces...";
        }
    }

    public static class BirdTest
    {
        public static void HacerVolar(Bird bird)
        {
            Console.WriteLine($"  → Llamando a Fly() desde referencia Bird...");
            string result = bird.Fly();
            Console.WriteLine($"  Resultado: {result}");
        }

        public static void EjecutarDemo()
        {
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine("2. VIOLACIÓN — 'new' y polimorfismo roto");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine();

            Bird sparrow = new Bird();
            Console.WriteLine("→ Gorrión (Bird) desde referencia Bird:");
            HacerVolar(sparrow);
            Console.WriteLine();

            // Caso 1: referencia de tipo base → llama al método BASE (NO al 'new')
            //         LSP parece "cumplirse", pero es un espejismo.
            Bird penguinAsBird = new Penguin();
            Console.WriteLine("→ Pingüino (Penguin) desde referencia Bird:");
            Console.WriteLine("  (¿Qué ocurre? El compilador resuelve Bird.Fly(), no Penguin.Fly())");
            HacerVolar(penguinAsBird);
            // Output: "Volando por el cielo..." ← ¡FALSO! Un pingüino NO vuela.
            Console.WriteLine("  ⚠️  ¡El pingüino 'vuela'! Esto es incorrecto conceptualmente.");
            Console.WriteLine("     Como usamos 'new', se ejecutó Bird.Fly(), no Penguin.Fly().");
            Console.WriteLine();

            // Caso 2: referencia de tipo derivado → llama al método 'new'
            //         ¡EXPLOTA! Violación clara de LSP.
            Penguin penguin = new Penguin();
            Console.WriteLine("→ Pingüino (Penguin) desde referencia Penguin:");
            try
            {
                string result = penguin.Fly();
                Console.WriteLine($"  Resultado: {result}");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"  💥 EXCEPCIÓN: {ex.Message}");
                Console.WriteLine("  ❌ LSP ROTO: el subtipo lanza una excepción que la base no declara.");
                Console.WriteLine("     Si el código cliente espera un Bird, no puede manejar esto.");
            }
            Console.WriteLine();

            // Explicación detallada:
            Console.WriteLine("--- Explicación ---");
            Console.WriteLine("'new' oculta el método de la clase base SIN participar en polimorfismo.");
            Console.WriteLine("Esto crea una situación peligrosa:");
            Console.WriteLine("  - Si usas una variable de tipo Bird, ejecutas Bird.Fly() (vuela).");
            Console.WriteLine("  - Si usas una variable de tipo Penguin, ejecutas Penguin.Fly() (explota).");
            Console.WriteLine("El código cliente que programa contra Bird NUNCA sabrá que un");
            Console.WriteLine("Penguin no vuela, porque Bird.Fly() siempre funciona.");
            Console.WriteLine();
        }
    }
}

// ──────────────────────────────────────────────────────────────────────────
// SECCIÓN 3: VIOLACIÓN — Precondiciones más fuertes en el subtipo
// ──────────────────────────────────────────────────────────────────────────
// LSP exige que el subtipo NO refuerce las precondiciones. Si la clase base
// acepta un rango de valores, el subtipo debe aceptar AL MENOS ese mismo
// rango (o uno más amplio). Si lo restringe, rompe LSP.

namespace Violacion3_Precondiciones
{
    // Clase base: una cuenta bancaria genérica.
    // Precondición de Withdraw: amount > 0 (no se permiten retiros negativos o cero).
    public class Account
    {
        public string Owner { get; }
        public decimal Balance { get; protected set; }

        public Account(string owner, decimal initialBalance)
        {
            Owner = owner;
            Balance = initialBalance;
        }

        // Método virtual para retirar dinero.
        // Precondición (documentada en código):
        //   amount > 0
        // Postcondición:
        //   Balance se reduce en amount (si no hay excepción).
        public virtual void Withdraw(decimal amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException(
                    $"El monto a retirar debe ser positivo. Recibido: {amount}");
            }

            Balance -= amount;
        }

        public override string ToString()
            => $"{Owner}: Balance = {Balance:C}";
    }

    // Subtipo: una cuenta de ahorros con saldo mínimo.
    // ⚠️  ¡VIOLACIÓN! Refuerza la precondición:
    //   Ahora exige amount > 0 Y amount <= Balance.
    //   Un cliente que usa Account espera que amount > 0 sea suficiente.
    public class SavingsAccount : Account
    {
        public SavingsAccount(string owner, decimal initialBalance)
            : base(owner, initialBalance) { }

        // Sobrescribimos Withdraw añadiendo una restricción extra.
        // El cliente que programa contra Account NO sabe que necesita
        // comprobar el saldo antes de retirar.
        public override void Withdraw(decimal amount)
        {
            // Llamamos al método base para validar amount > 0.
            base.Withdraw(amount);

            // ⚠️  NUEVA precondición: el saldo debe ser suficiente.
            //    Esto ROMPE LSP porque un código que funciona con Account
            //    puede fallar al recibir un SavingsAccount.
            if (amount > Balance + amount) // Siempre falso aquí, es ilustrativo
            {
                throw new InvalidOperationException("Saldo insuficiente.");
            }
        }
    }

    public static class AccountTest
    {
        // Este método trabaja contra la abstracción Account.
        // Conoce la precondición mínima: amount > 0.
        public static void ProcesarRetiro(Account account, decimal amount)
        {
            Console.WriteLine($"  Cuenta: {account}");
            Console.WriteLine($"  Intentando retirar {amount:C}...");

            try
            {
                account.Withdraw(amount);
                Console.WriteLine($"  ✅ Retiro exitoso. Nuevo saldo: {account.Balance:C}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"  ⚠️  Error de argumento (esperado): {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ❌ Error inesperado: {ex.GetType().Name}: {ex.Message}");
                Console.WriteLine($"     Esto rompe LSP: el subtipo lanza excepciones que la base no.");
            }
            Console.WriteLine();
        }

        public static void EjecutarDemo()
        {
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine("3. VIOLACIÓN — Precondiciones más fuertes");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine();

            // Con Account funciona para cualquier amount > 0.
            Account account = new Account("Alice", 500m);
            Console.WriteLine("→ Retiro en Account (balance=500, retiro=600):");
            Console.WriteLine("  Precondición: solo comprueba amount > 0. No comprueba saldo.");
            ProcesarRetiro(account, 600m);
            // Funciona: balance queda en -100. Account no tiene restricción de saldo.

            // Con SavingsAccount... ¿funciona igual?
            SavingsAccount savings = new SavingsAccount("Bob", 500m);
            Console.WriteLine("→ Retiro en SavingsAccount (balance=500, retiro=600):");
            Console.WriteLine("  Precondición: amount > 0 Y amount <= Balance.");
            ProcesarRetiro(savings, 600m);
            // ¿Qué ocurre? El código cliente no sabe que debe comprobar el saldo.

            Console.WriteLine("--- Explicación ---");
            Console.WriteLine("La clase base Account solo exige amount > 0.");
            Console.WriteLine("El subtipo SavingsAccount añade amount <= Balance.");
            Console.WriteLine("Un cliente que programa contra Account retirará 600 de una");
            Console.WriteLine("cuenta con 500 y no espera que falle por saldo insuficiente.");
            Console.WriteLine("Si el subtipo impone restricciones que la base no tiene,");
            Console.WriteLine("el código cliente se romperá. Eso es violar LSP.");
            Console.WriteLine();
        }
    }
}

// ──────────────────────────────────────────────────────────────────────────
// SECCIÓN 4: CUMPLIENDO LSP — Rediseño correcto
// ──────────────────────────────────────────────────────────────────────────
// Solución 1 (Rectangle/Square):  Extraer una abstracción común (IShape)
//                                 sin forzar herencia de mutabilidad.
// Solución 2 (Bird/Penguin):      Separar comportamientos en interfaces.
// Solución 3 (Account):           Documentar invariantes y no reforzar
//                                 precondiciones, o usar composición.

namespace CumpliendoLSP
{
    // ──────────────────────────────────────────────────────────────────────
    // Solución 1: Interface IShape para Rectangle y Square
    // ──────────────────────────────────────────────────────────────────────
    // En lugar de forzar herencia, ambos implementan una interfaz común.
    // La interfaz solo expone Area (sin setters mutables que puedan romper).

    public interface IShape
    {
        double Area { get; }
    }

    // Rectangle implementa IShape. Ahora no es heredable para Square.
    public class Rectangle : IShape
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public double Area => Width * Height;

        public Rectangle(double width, double height)
        {
            Width = width;
            Height = height;
        }

        public override string ToString()
            => $"Rectangle: Width={Width}, Height={Height}, Area={Area}";
    }

    // Square implementa IShape. No hereda de Rectangle, así que no hay
    // restricciones de mutabilidad que rompan el contrato.
    public class Square : IShape
    {
        public double Side { get; set; }
        public double Area => Side * Side;

        public Square(double side)
        {
            Side = side;
        }

        public override string ToString()
            => $"Square: Side={Side}, Area={Area}";
    }

    // ──────────────────────────────────────────────────────────────────────
    // Solución 2: Interfaces para capacidades de vuelo
    // ──────────────────────────────────────────────────────────────────────
    // Separamos el comportamiento de volar en una interfaz.
    // Solo las aves que realmente vuelan la implementan.

    public interface IFlyable
    {
        string Fly();
    }

    // Clase base Bird: solo capacidades COMUNES a todas las aves.
    public class Bird
    {
        public string Name { get; }

        public Bird(string name)
        {
            Name = name;
        }

        public virtual string Eat()
        {
            return $"{Name} está comiendo.";
        }
    }

    // Sparrow VUELA → implementa IFlyable.
    public class Sparrow : Bird, IFlyable
    {
        public Sparrow(string name) : base(name) { }

        public string Fly()
        {
            return $"{Name} vuela entre los árboles.";
        }
    }

    // Penguin NO VUELA → NO implementa IFlyable.
    public class Penguin : Bird
    {
        public Penguin(string name) : base(name) { }

        public string Swim()
        {
            return $"{Name} nada en el agua helada.";
        }
    }

    // ──────────────────────────────────────────────────────────────────────
    // Solución 3: Documentar el contrato e invariantes
    // ──────────────────────────────────────────────────────────────────────
    // Usamos una clase base que define el contrato claramente.
    // Los subtipos no refuerzan precondiciones, solo añaden comportamiento.

    public interface IAccount
    {
        string Owner { get; }
        decimal Balance { get; }
        void Deposit(decimal amount);
        bool TryWithdraw(decimal amount, out decimal newBalance);
    }

    // Cuenta genérica sin restricciones de saldo (salvo las básicas).
    public class StandardAccount : IAccount
    {
        public string Owner { get; }
        public decimal Balance { get; private set; }

        public StandardAccount(string owner, decimal initialBalance)
        {
            Owner = owner;
            Balance = initialBalance;
        }

        public void Deposit(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("El depósito debe ser positivo.");

            Balance += amount;
        }

        // Devuelve bool en lugar de lanzar excepción.
        // Esto permite que el código cliente maneje el resultado sin sorpresas.
        public virtual bool TryWithdraw(decimal amount, out decimal newBalance)
        {
            newBalance = Balance;

            if (amount <= 0)
                return false;

            Balance -= amount;
            newBalance = Balance;
            return true;
        }

        public override string ToString()
            => $"{Owner}: Balance = {Balance:C}";
    }

    // Cuenta de ahorros que requiere saldo mínimo.
    // No cambia la firma ni lanza excepciones nuevas.
    // Extiende el comportamiento sin romper el contrato.
    public class SavingsAccount : IAccount
    {
        private readonly StandardAccount _inner; // Composición en lugar de herencia

        public string Owner => _inner.Owner;
        public decimal Balance => _inner.Balance;

        public SavingsAccount(string owner, decimal initialBalance)
        {
            _inner = new StandardAccount(owner, initialBalance);
        }

        public void Deposit(decimal amount)
        {
            _inner.Deposit(amount);
        }

        // Añade la restricción pero sin romper el contrato de IAccount.
        // El código cliente siempre recibe bool y newBalance.
        public bool TryWithdraw(decimal amount, out decimal newBalance)
        {
            if (amount > Balance)
            {
                newBalance = Balance;
                return false; // Saldo insuficiente: no es excepción, es resultado.
            }

            return _inner.TryWithdraw(amount, out newBalance);
        }

        public override string ToString()
            => $"{Owner}: Balance = {Balance:C} (Savings)";
    }

    // ──────────────────────────────────────────────────────────────────────
    // Demo de las soluciones correctas
    // ──────────────────────────────────────────────────────────────────────
    public static class DemoSoluciones
    {
        // Ahora este método funciona con CUALQUIER IShape sin sorpresas.
        public static void ImprimirArea(IShape shape)
        {
            Console.WriteLine($"  Forma: {shape}");
        }

        // Este método solo acepta aves que vuelan. El compilador nos protege.
        public static void HacerVolar(IFlyable flyable)
        {
            Console.WriteLine($"  {flyable.Fly()}");
        }

        // Método genérico que trabaja con cualquier cuenta.
        public static void ProcesarRetiro(IAccount account, decimal amount)
        {
            Console.WriteLine($"  Cuenta: {account}");
            Console.WriteLine($"  Intentando retirar {amount:C}...");

            if (account.TryWithdraw(amount, out decimal newBalance))
            {
                Console.WriteLine($"  ✅ Retiro exitoso. Nuevo saldo: {newBalance:C}");
            }
            else
            {
                Console.WriteLine($"  ❌ No se pudo retirar. Saldo actual: {account.Balance:C}");
            }
            Console.WriteLine();
        }

        public static void EjecutarDemo()
        {
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine("4. CUMPLIENDO LSP — Rediseño correcto");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine();

            // ── Solución 1: IShape ──
            Console.WriteLine("── Solución 1: Interface IShape en lugar de herencia ──");
            Console.WriteLine();
            Rectangle rect = new Rectangle(5, 10);
            Square sq = new Square(5);
            ImprimirArea(rect);  // Área = 50 ✅
            ImprimirArea(sq);    // Área = 25 ✅ Ambos funcionan sin sorpresas.
            Console.WriteLine("Ambas formas implementan IShape. Ninguna puede romper a la otra.");
            Console.WriteLine();

            // ── Solución 2: Interfaces para volar ──
            Console.WriteLine("── Solución 2: IFlyable separa el vuelo ──");
            Console.WriteLine();
            Sparrow sparrow = new Sparrow("Piolín");
            Penguin penguin = new Penguin("Skipper");

            HacerVolar(sparrow);
            // HacerVolar(penguin); // ❌ Error de compilación. ¡Perfecto!
            Console.WriteLine("  Penguin no implementa IFlyable → no se puede llamar a HacerVolar().");
            Console.WriteLine($"  Pero sí puede nadar: {penguin.Swim()}");
            Console.WriteLine($"  Y ambos comen: {sparrow.Eat()}, {penguin.Eat()}");
            Console.WriteLine();

            // ── Solución 3: Contrato explícito con TryWithdraw ──
            Console.WriteLine("── Solución 3: IAccount con TryWithdraw ──");
            Console.WriteLine();
            IAccount standard = new StandardAccount("Alice", 500m);
            IAccount savings = new SavingsAccount("Bob", 500m);

            // Ambos funcionan exactamente igual desde el punto de vista del cliente.
            ProcesarRetiro(standard, 100m);  // ✅
            ProcesarRetiro(savings, 100m);   // ✅
            ProcesarRetiro(standard, 900m);  // ✅ Standard lo permite (balance negativo)
            ProcesarRetiro(savings, 900m);   // ❌ Savings rechaza (saldo insuficiente)
            Console.WriteLine("El código cliente maneja ambos casos sin romperse.");
            Console.WriteLine("SavingsAccount rechaza el retiro devolviendo false, no lanzando excepción.");
            Console.WriteLine();
        }
    }
}

// ──────────────────────────────────────────────────────────────────────────
// SECCIÓN 5: Precondiciones, Postcondiciones e Invariantes (Teoría aplicada)
// ──────────────────────────────────────────────────────────────────────────
// LSP se basa en el concepto de "Design by Contract" (Diseño por Contrato)
// de Bertrand Meyer. El contrato de un método tiene tres partes:
//
//   PRECONDICIÓN:  Lo que debe ser cierto ANTES de ejecutar el método.
//                  El cliente es responsable de cumplirla.
//                  Regla: el subtipo NO puede reforzarla.
//
//   POSTCONDICIÓN: Lo que debe ser cierto DESPUÉS de ejecutar el método.
//                  El método es responsable de cumplirla.
//                  Regla: el subtipo NO puede debilitarla.
//
//   INVARIANTE:    Condición que siempre es cierta para el objeto.
//                  Debe mantenerse antes y después de cada método público.
//                  Regla: el subtipo debe conservar todos los invariantes.

namespace DisenoPorContrato
{
    // Ejemplo de invariante documentado con código.
    // Una cola (Queue) que garantiza que Count nunca es negativo.
    public class BoundedQueue<T>
    {
        private readonly T[] _items;
        private int _head;
        private int _tail;
        private int _count;

        // INVARIANTE: 0 <= _count <= _items.Length
        // INVARIANTE: _count == (_tail - _head + _items.Length) % _items.Length
        public int Capacity => _items.Length;
        public int Count => _count;

        public BoundedQueue(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentException("La capacidad debe ser positiva.");

            _items = new T[capacity];
        }

        // PRECONDICIÓN: item != null (si T es reference type) o valor válido.
        // PRECONDICIÓN: Count < Capacity (hay espacio).
        // POSTCONDICIÓN: Count == old(Count) + 1.
        // POSTCONDICIÓN: el elemento fue añadido.
        public virtual bool TryEnqueue(T item)
        {
            if (_count >= _items.Length)
                return false; // No lanzamos excepción, devolvemos false.

            _items[_tail] = item;
            _tail = (_tail + 1) % _items.Length;
            _count++;
            return true;
        }

        // PRECONDICIÓN: Count > 0 (hay elementos).
        // POSTCONDICIÓN: Count == old(Count) - 1.
        // POSTCONDICIÓN: el elemento devuelto es el que llevaba más tiempo en la cola.
        public virtual bool TryDequeue(out T? item)
        {
            if (_count == 0)
            {
                item = default;
                return false;
            }

            item = _items[_head];
            _items[_head] = default!; // Ayuda al GC
            _head = (_head + 1) % _items.Length;
            _count--;
            return true;
        }

        // Método helper para verificar el invariante.
        // En producción usarías Debug.Assert o un método Conditional.
        public bool ValidarInvariante()
        {
            // El subtipo DEBE preservar este invariante.
            return _count >= 0 && _count <= _items.Length;
        }
    }

    // Subtipo que añade logging pero RESPETA todas las
    // precondiciones, postcondiciones e invariantes del padre.
    public class LoggingBoundedQueue<T> : BoundedQueue<T>
    {
        public LoggingBoundedQueue(int capacity) : base(capacity) { }

        public override bool TryEnqueue(T item)
        {
            Console.WriteLine($"    [LOG] Encolando elemento. Count actual: {Count}");
            bool result = base.TryEnqueue(item);
            Console.WriteLine($"    [LOG] Resultado: {(result ? "encolado" : "cola llena")}. Nuevo Count: {Count}");
            return result;
            // ✅ Misma postcondición que la base: Count incrementa si hay espacio.
            // ✅ Mismos invariantes.
            // ✅ No refuerza precondiciones.
        }

        public override bool TryDequeue(out T? item)
        {
            Console.WriteLine($"    [LOG] Desencolando elemento. Count actual: {Count}");
            bool result = base.TryDequeue(out item);
            Console.WriteLine($"    [LOG] Resultado: {(result ? $"desencolado {item}" : "cola vacía")}. Nuevo Count: {Count}");
            return result;
            // ✅ Respeta el contrato completamente.
        }
    }

    public static class DemoContrato
    {
        public static void EjecutarDemo()
        {
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine("5. DISEÑO POR CONTRATO — Precondiciones, Postcondiciones, Invariantes");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine();

            Console.WriteLine("→ Usando BoundedQueue<int> (clase base):");
            BoundedQueue<int> baseQueue = new BoundedQueue<int>(3);
            baseQueue.TryEnqueue(10);
            baseQueue.TryEnqueue(20);
            baseQueue.TryEnqueue(30);
            baseQueue.TryDequeue(out int value);
            Console.WriteLine($"  Elemento desencolado: {value}");
            Console.WriteLine($"  Invariante válido: {baseQueue.ValidarInvariante()}");
            Console.WriteLine();

            Console.WriteLine("→ Usando LoggingBoundedQueue<int> (subtipo):");
            LoggingBoundedQueue<int> loggingQueue = new LoggingBoundedQueue<int>(3);
            loggingQueue.TryEnqueue(100);
            loggingQueue.TryEnqueue(200);
            loggingQueue.TryDequeue(out int value2);
            Console.WriteLine($"  Elemento desencolado: {value2}");
            Console.WriteLine($"  Invariante válido: {loggingQueue.ValidarInvariante()}");
            Console.WriteLine();

            Console.WriteLine("--- Explicación ---");
            Console.WriteLine("LoggingBoundedQueue añade logging pero:");
            Console.WriteLine("  1. No refuerza precondiciones (sigue aceptando cualquier T).");
            Console.WriteLine("  2. No debilita postcondiciones (Count sigue incrementando/decrementando).");
            Console.WriteLine("  3. Conserva los invariantes de BoundedQueue.");
            Console.WriteLine("Por tanto, LoggingBoundedQueue SÍ cumple LSP.");
            Console.WriteLine();
        }
    }
}

// ──────────────────────────────────────────────────────────────────────────
// PROGRAMA PRINCIPAL (entry point explícito para coexistir con namespaces)
// ──────────────────────────────────────────────────────────────────────────

public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine();
        Console.WriteLine("╔══════════════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║  MÓDULO 3 — SOLID en C#                                                     ║");
        Console.WriteLine("║  Lección 03 — Principio de Sustitución de Liskov (LSP)                      ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════════════════════╝");
        Console.WriteLine();
        Console.WriteLine("LSP (Barbara Liskov, 1987):");
        Console.WriteLine("\"Los subtipos deben poder sustituir a sus tipos base sin alterar el");
        Console.WriteLine(" correcto funcionamiento del programa.\"");
        Console.WriteLine();
        Console.WriteLine("En otras palabras: si el código usa una clase base, debe poder usar");
        Console.WriteLine("cualquier subtipo sin saberlo y sin que el programa falle.");
        Console.WriteLine();
        Console.WriteLine("La herencia NO es solo reutilizar código. Es un CONTRATO.");
        Console.WriteLine("Si tu subtipo rompe el contrato del padre, no uses herencia.");
        Console.WriteLine();

        // ── Ejecutar todas las demos ──
        Violacion1_SquareRectangle.RectangleTest.EjecutarDemo();
        Violacion2_NewKeyword.BirdTest.EjecutarDemo();
        Violacion3_Precondiciones.AccountTest.EjecutarDemo();
        CumpliendoLSP.DemoSoluciones.EjecutarDemo();
        DisenoPorContrato.DemoContrato.EjecutarDemo();

        // ──────────────────────────────────────────────────────────────────────
        // RESUMEN DE LA LECCIÓN
        // ──────────────────────────────────────────────────────────────────────
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine("RESUMEN — Principio de Sustitución de Liskov (LSP)");
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine();
        Console.WriteLine("1. Un subtipo DEBE poder sustituir a su tipo base sin romper el programa.");
        Console.WriteLine();
        Console.WriteLine("2. Señales de que violas LSP:");
        Console.WriteLine("   • El subtipo lanza excepciones que la base no declara.");
        Console.WriteLine("   • El subtipo modifica el significado de métodos heredados.");
        Console.WriteLine("   • El subtipo refuerza precondiciones de la base.");
        Console.WriteLine("   • El subtipo debilita postcondiciones de la base.");
        Console.WriteLine("   • Necesitas 'if (x is Subtipo)' en el código cliente.");
        Console.WriteLine();
        Console.WriteLine("3. 'new' en métodos es una trampa:");
        Console.WriteLine("   • No participa en polimorfismo. Oculta sin sobrescribir.");
        Console.WriteLine("   • Solo debe usarse cuando no hay alternativa.");
        Console.WriteLine();
        Console.WriteLine("4. Soluciones cuando la herencia viola LSP:");
        Console.WriteLine("   • Extraer interfaces comunes (IShape en lugar de herencia).");
        Console.WriteLine("   • Separar comportamientos en interfaces (IFlyable).");
        Console.WriteLine("   • Usar composición en lugar de herencia (SavingsAccount).");
        Console.WriteLine("   • Diseñar por contrato explícito (TryXxx en lugar de excepciones).");
        Console.WriteLine();
        Console.WriteLine("5. Diseño por Contrato (Design by Contract):");
        Console.WriteLine("   • Precondición: lo que el MÉTODO exige al cliente.");
        Console.WriteLine("     → El subtipo NO puede reforzarla.");
        Console.WriteLine("   • Postcondición: lo que el MÉTODO garantiza tras ejecutarse.");
        Console.WriteLine("     → El subtipo NO puede debilitarla.");
        Console.WriteLine("   • Invariante: lo que SIEMPRE es cierto para el objeto.");
        Console.WriteLine("     → El subtipo DEBE conservarlo.");
        Console.WriteLine();
        Console.WriteLine("Regla de oro: si dudas si debes heredar, NO heredes. Usa composición.");
        Console.WriteLine();
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
    }
}
