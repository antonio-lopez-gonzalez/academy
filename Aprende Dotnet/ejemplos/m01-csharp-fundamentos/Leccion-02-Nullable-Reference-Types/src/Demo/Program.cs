using System.Diagnostics.CodeAnalysis;

namespace Demo;

public class Program
{
    public static void Main(string[] args)
    {
        // ═══════════════════════════════════════════════════════════════
        // Lección 02: Nullable Reference Types (Tipos de referencia anulables)
        //
        // Los NRT se habilitan con <Nullable>enable</Nullable> en el .csproj.
        // A partir de .NET 6, está activado por defecto en nuevos proyectos.
        //
        // ¿Por qué existe? El 80% de las excepciones en producción son
        // NullReferenceException. Los NRT convierten ese error de runtime
        // en una advertencia de compilación, atrapándolo antes de desplegar.
        // ═══════════════════════════════════════════════════════════════

        Console.WriteLine("══════ Lección 02: Nullable Reference Types ══════");
        Console.WriteLine();

        DemoStringVsNullableString();
        DemoNullForgivingOperator();
        DemoNullConditionalOperator();
        DemoNullCoalescingOperator();
        DemoNullCoalescingAssignment();
        DemoThrowIfNull();
        DemoNotNullAttributes();
        DemoMemberNotNull();
        DemoMaybeNull();
        DemoWarningWaves();

        Console.WriteLine();
        Console.WriteLine("══════ Fin de la demo ══════");
    }

    // ─────────────────────────────────────────────────────────────
    // 1. string vs string? — La base de NRT
    // ─────────────────────────────────────────────────────────────
    static void DemoStringVsNullableString()
    {
        Console.WriteLine("─── 1. string vs string? ───");

        // string (sin ?): el compilador asume que NUNCA debe ser null.
        // Si intentamos asignar null, obtenemos la advertencia CS8600.
        string nonNullable = "Hola .NET";

        // Esto genera ADVERTENCIA del compilador (CS8600):
        //   "Converting null literal or possible null value to non-nullable type."
        // string roto = null; // ⚠️ CS8600

        // string? (con ?): declaramos explícitamente que PUEDE ser null.
        // El compilador nos obliga a comprobar null antes de usarlo.
        string? nullable = null;           // ✅ Sin advertencia: acepta null
        nullable = "Ahora tengo valor";    // ✅ También acepta string normal

        Console.WriteLine($"  nonNullable: '{nonNullable}' (no puede ser null)");
        Console.WriteLine($"  nullable:    '{nullable}' (puede ser null)");

        // Diferencia clave con Java:
        // Java NO tiene NRT nativos. Depende de anotaciones @Nullable/@NotNull
        // de librerías externas (javax.annotation, JetBrains, Lombok, etc.).
        //
        // Diferencia con TypeScript:
        // strictNullChecks en tsconfig.json da un comportamiento similar
        // con union types (string | null), pero solo en tiempo de compilación.
        // C# integra NRT nativamente en compilador + BCL.
    }

    // ─────────────────────────────────────────────────────────────
    // 2. Null-forgiving operator (!) — "Confía en mí"
    // ─────────────────────────────────────────────────────────────
    static void DemoNullForgivingOperator()
    {
        Console.WriteLine("─── 2. Null-forgiving operator (!) ───");

        // El operador ! (null-forgiving) le dice al compilador:
        // "Sé que esto parece null, pero te aseguro que no lo es".
        // Suprime la advertencia, pero NO cambia el valor en runtime.
        //
        // ⚠️ ES PELIGROSO: si te equivocas, obtienes NullReferenceException.

        string? maybeNull = ObtenerNombre();

        // Sin !: el compilador advierte sobre posible dereferencia de null
        // Console.WriteLine(maybeNull.Length); // ⚠️ CS8602

        // Con !: silenciamos la advertencia (bajo nuestra responsabilidad)
        Console.WriteLine($"  Longitud con !: {maybeNull!.Length}");

        // CUÁNDO USARLO (pocas veces):
        // 1. Cuando usas reflection y sabes que una propiedad existe.
        // 2. En tests, para pasar null a propósito y verificar comportamiento.
        // 3. Con APIs externas que sabes que nunca retornan null pero no están anotadas.

        // CUÁNDO NO USARLO:
        // 1. Para "arreglar" advertencias sin entender por qué ocurren.
        // 2. En código de producción sin una comprobación previa.
        // 3. Cuando puedes usar ?. o una comprobación explícita en su lugar.

        // Alternativa segura: comprobar antes de usar
        if (maybeNull is not null)
        {
            Console.WriteLine($"  Longitud segura (sin !): {maybeNull.Length}");
        }
    }

    // ─────────────────────────────────────────────────────────────
    // 3. Null-conditional operator (?.)
    // ─────────────────────────────────────────────────────────────
    static void DemoNullConditionalOperator()
    {
        Console.WriteLine("─── 3. Null-conditional operator (?.) ───");

        // ?. accede al miembro SOLO si el operando NO es null.
        // Si es null, toda la expresión devuelve null sin lanzar excepción.
        // Es el equivalente seguro de "punto" (.).

        string? nombre = null;

        // nombre?.Length es equivalente a:
        //   nombre is null ? (int?)null : nombre.Length
        int? longitud = nombre?.Length;
        Console.WriteLine($"  nombre?.Length con nombre=null: {longitud}");

        // Encadenamiento: una sola comprobación null en cualquier paso
        // detiene toda la cadena y devuelve null.
        Persona? persona = null;
        string? ciudad = persona?.Direccion?.Ciudad;
        Console.WriteLine($"  persona?.Direccion?.Ciudad (persona null): {ciudad ?? "null"}");

        // Con objeto no null, la cadena funciona normalmente:
        persona = new Persona
        {
            Nombre = "Ana",
            Direccion = new Direccion { Calle = "Gran Vía", Ciudad = "Madrid" }
        };
        Console.WriteLine($"  persona?.Nombre: {persona?.Nombre}");
        Console.WriteLine($"  persona?.Direccion?.Ciudad: {persona?.Direccion?.Ciudad}");

        // ?. también funciona con indexadores []:
        List<string>? lista = null;
        string? primerElemento = lista?[0];
        Console.WriteLine($"  lista?[0] con lista=null: {primerElemento ?? "null"}");

        // Y con invocación de delegados:
        Action? callback = null;
        callback?.Invoke(); // No se ejecuta, no lanza excepción
        Console.WriteLine("  callback?.Invoke() con callback=null: sin excepción");
    }

    // ─────────────────────────────────────────────────────────────
    // 4. Null-coalescing operator (??)
    // ─────────────────────────────────────────────────────────────
    static void DemoNullCoalescingOperator()
    {
        Console.WriteLine("─── 4. Null-coalescing operator (??) ───");

        // ?? devuelve el operando izquierdo si NO es null;
        // si es null, devuelve el operando derecho (valor por defecto).
        //
        // Equivalente a: x is not null ? x : y

        string? nombre = null;
        string resultado = nombre ?? "Anónimo";
        Console.WriteLine($"  nombre ?? \"Anónimo\" (nombre=null): '{resultado}'");

        nombre = "María";
        resultado = nombre ?? "Anónimo";
        Console.WriteLine($"  nombre ?? \"Anónimo\" (nombre='María'): '{resultado}'");

        // Coalescencia en cascada: se evalúa de izquierda a derecha
        string? a = null;
        string? b = null;
        string c = "Valor final";
        string final = a ?? b ?? c;
        Console.WriteLine($"  a ?? b ?? c: '{final}'");

        // Combinación ?. + ?? (el patrón más común en código real):
        Persona? p = null;
        string nombreMayus = p?.Nombre?.ToUpperInvariant() ?? "SIN NOMBRE";
        Console.WriteLine($"  p?.Nombre?.ToUpper() ?? \"SIN NOMBRE\": '{nombreMayus}'");

        // ?? también funciona con Nullable<T> (tipos de valor anulables):
        int? edad = null;
        int edadReal = edad ?? 18;
        Console.WriteLine($"  edad ?? 18 (edad=null): {edadReal}");
    }

    // ─────────────────────────────────────────────────────────────
    // 5. Null-coalescing assignment (??=)
    // ─────────────────────────────────────────────────────────────
    static void DemoNullCoalescingAssignment()
    {
        Console.WriteLine("─── 5. Null-coalescing assignment (??=) ───");

        // ??= asigna el valor derecho SOLO si la variable izquierda es null.
        // Equivalente a: if (variable is null) variable = valor;
        // Muy útil para inicialización perezosa (lazy initialization).

        string? config = null;

        config ??= "Valor por defecto";
        Console.WriteLine($"  config ??= \"Valor por defecto\": '{config}'");

        // Si ya tiene valor, no se sobrescribe:
        config ??= "Nuevo valor";
        Console.WriteLine($"  Segundo ??= (ignorado): '{config}'");

        // Patrón común: inicializar colecciones bajo demanda
        List<int>? numeros = null;

        // Sin ??= necesitaríamos:
        // if (numeros is null) numeros = new List<int>();
        numeros ??= new List<int>();
        numeros.Add(42);
        numeros.Add(10);
        Console.WriteLine($"  Lista lazy con ??=: [{string.Join(", ", numeros)}]");

        // También con diccionarios:
        Dictionary<string, int>? cache = null;
        cache ??= new Dictionary<string, int>();
        cache["clave"] = 100;
        Console.WriteLine($"  Diccionario lazy con ??=: {cache.Count} elemento(s)");
    }

    // ─────────────────────────────────────────────────────────────
    // 6. ArgumentNullException.ThrowIfNull() (.NET 6+)
    // ─────────────────────────────────────────────────────────────
    static void DemoThrowIfNull()
    {
        Console.WriteLine("─── 6. ArgumentNullException.ThrowIfNull() ───");

        // ThrowIfNull es el reemplazo moderno del patrón clásico:
        //
        // ANTES (verboso, repetitivo):
        //   if (param is null) throw new ArgumentNullException(nameof(param));
        //
        // AHORA (conciso, una línea):
        //   ArgumentNullException.ThrowIfNull(param);
        //
        // Ventajas:
        // 1. Más legible: una línea vs cuatro
        // 2. nameof(param) se infiere automáticamente (gracias a [CallerArgumentExpression])
        // 3. El análisis de flujo del compilador entiende que después de ThrowIfNull,
        //    el parámetro NO es null (narrowing).

        try
        {
            ProcesarNombre(null!);
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine($"  Excepción: {ex.GetType().Name}");
            Console.WriteLine($"  Mensaje:   {ex.Message}");
        }

        // Cuando no es null, el código sigue normalmente:
        ProcesarNombre("Carlos");
    }

    // ─────────────────────────────────────────────────────────────
    // 7. [NotNullWhen] — Informar al compilador sobre patrones Try
    // ─────────────────────────────────────────────────────────────
    static void DemoNotNullAttributes()
    {
        Console.WriteLine("─── 7. [NotNullWhen] attribute ───");

        // [NotNullWhen(true)] le dice al compilador:
        // "Cuando este método retorna true, el parámetro out NO es null".
        //
        // Sin este atributo, el compilador asumiría que el out param
        // siempre puede ser null, incluso dentro del bloque if (true).

        string? input = null;

        // El compilador entiende que si ValidarYTransformar retorna true,
        // result NO es null. Podemos usar result.ToUpper() sin advertencia.
        if (ValidarYTransformar(input, out string? result))
        {
            // Gracias a [NotNullWhen(true)], result es non-nullable aquí
            Console.WriteLine($"  Resultado: '{result}'");
        }
        else
        {
            Console.WriteLine("  Validación falló (input era null o vacío)");
        }

        input = "hola mundo";
        if (ValidarYTransformar(input, out result))
        {
            Console.WriteLine($"  Resultado transformado: '{result}'");
        }

        // Este mismo patrón lo usa toda la BCL:
        // - int.TryParse(string, out int)
        // - Dictionary.TryGetValue(TKey, out TValue)
        // - Guid.TryParse(string, out Guid)
        // Todos llevan [NotNullWhen(true)] en el parámetro out.
    }

    // ─────────────────────────────────────────────────────────────
    // 8. [MemberNotNull] — Garantizar inicialización de campos
    // ─────────────────────────────────────────────────────────────
    static void DemoMemberNotNull()
    {
        Console.WriteLine("─── 8. [MemberNotNull] attribute ───");

        // [MemberNotNull(nameof(campo))] le dice al compilador que,
        // después de llamar a este método, los campos indicados NO son null.
        //
        // Es el equivalente para inicialización en dos fases:
        // 1. Constructor crea el objeto (campos pueden ser null)
        // 2. Método Initialize() asigna los campos
        // 3. [MemberNotNull] informa al compilador que el paso 2 ya ocurrió

        var repo = new Repository();

        // Sin [MemberNotNull] en Initialize(), esta línea daría advertencia
        // porque _data podría ser null. Con el atributo, el compilador confía.
        repo.Initialize();

        Console.WriteLine($"  Elementos: {repo.Count}");
        Console.WriteLine($"  Primero:   {repo.First()}");
    }

    // ─────────────────────────────────────────────────────────────
    // 9. [MaybeNull] — Cuando un out param T puede ser null
    // ─────────────────────────────────────────────────────────────
    static void DemoMaybeNull()
    {
        Console.WriteLine("─── 9. [MaybeNull] attribute ───");

        // [MaybeNull] le dice al compilador que un parámetro out/ref
        // de tipo T (non-nullable) PUEDE contener null al salir del método.
        //
        // Ejemplo real: Dictionary<TKey, TValue>.TryGetValue
        // Su firma real es:
        //   bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        //
        // Cuando retorna false, value puede ser default(TValue), que para
        // tipos de referencia es null.

        var diccionario = new Dictionary<string, string>
        {
            ["clave1"] = "valor1",
            ["clave2"] = "valor2"
        };

        if (diccionario.TryGetValue("clave1", out string? value))
        {
            Console.WriteLine($"  Encontrado: '{value}'");
        }

        if (!diccionario.TryGetValue("noExiste", out value))
        {
            // value es null (default de string?), y el compilador lo sabe
            Console.WriteLine($"  No encontrado, value es: '{value ?? "null"}'");
        }

        // Con TryGetValue de tipo non-nullable que no existe:
        // El compilador emite advertencia porque value puede ser null
        // a pesar de declararse como `string` (por [MaybeNullWhen(false)])
    }

    // ─────────────────────────────────────────────────────────────
    // 10. Warning Waves y comparación con otros lenguajes
    // ─────────────────────────────────────────────────────────────
    static void DemoWarningWaves()
    {
        Console.WriteLine("─── 10. Warning Waves ───");

        // Las "warning waves" son versiones del análisis de nulabilidad.
        // Cada wave añade reglas más estrictas y precisas.

        Console.WriteLine("  Evolución de las warning waves:");
        Console.WriteLine("  Wave 5  (.NET 5): análisis básico de flujo.");
        Console.WriteLine("  Wave 6  (.NET 6): guard clauses, mejor tracking de campos.");
        Console.WriteLine("  Wave 7  (.NET 7): análisis de atributos mejorado,");
        Console.WriteLine("                    mejor soporte para genéricos.");
        Console.WriteLine("  Wave 8  (.NET 8): análisis inter-procedural más preciso,");
        Console.WriteLine("                    detección de paths imposibles.");
        Console.WriteLine();
        Console.WriteLine("  Configuración en .csproj:");
        Console.WriteLine("    <AnalysisLevel>8.0-recommended</AnalysisLevel>");
        Console.WriteLine("    <WarningsAsErrors>nullable</WarningsAsErrors>");
        Console.WriteLine("      ↑ Convierte advertencias NRT en errores de compilación.");
        Console.WriteLine();

        // ─── Comparación detallada con Java ───
        Console.WriteLine("  Comparación con JAVA:");
        Console.WriteLine("  Java no tiene NRT en el lenguaje. Depende de anotaciones externas:");
        Console.WriteLine("    - @Nullable / @NotNull de javax.annotation (JSR 305, abandonada)");
        Console.WriteLine("    - @Nullable / @NotNull de JetBrains (@org.jetbrains.annotations)");
        Console.WriteLine("    - @NonNull de Lombok");
        Console.WriteLine("    - @Nullable de Checker Framework");
        Console.WriteLine("  Problema: cada librería usa sus propias anotaciones, no hay estándar.");
        Console.WriteLine("  C# ventaja: integración nativa en compilador, sin dependencias externas.");
        Console.WriteLine();

        // ─── Comparación detallada con TypeScript ───
        Console.WriteLine("  Comparación con TYPESCRIPT:");
        Console.WriteLine("  TypeScript usa union types: string | null | undefined.");
        Console.WriteLine("  strictNullChecks en tsconfig.json activa el comportamiento.");
        Console.WriteLine("  Ventaja TS: más flexible (null vs undefined), narrowing por control flow.");
        Console.WriteLine("  Ventaja C#: integrado con el runtime, atributos en la BCL,");
        Console.WriteLine("              el compilador puede optimizar basándose en la info de nulabilidad.");
        Console.WriteLine();

        // ─── Cuándo subir de wave ───
        Console.WriteLine("  Recomendación: usar siempre la última wave disponible.");
        Console.WriteLine("  Si un proyecto legacy tiene cientos de advertencias,");
        Console.WriteLine("  se puede subir gradualmente con:");
        Console.WriteLine("    <AnalysisLevel>preview</AnalysisLevel>");
        Console.WriteLine("  Y suprimir temporalmente las advertencias existentes con .editorconfig.");
    }

    // ═══════════════════════════════════════════════════════════════
    // Métodos auxiliares (públicos para testing)
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Simula un método que puede retornar null.
    /// En código real podría ser una consulta a BD, API externa, etc.
    /// </summary>
    static string? ObtenerNombre() => "Ejemplo";

    /// <summary>
    /// Procesa un nombre, lanzando ArgumentNullException si es null.
    /// ThrowIfNull usa [CallerArgumentExpression] para inferir el nombre del parámetro.
    /// </summary>
    static void ProcesarNombre(string nombre)
    {
        // Lanza ArgumentNullException si nombre es null.
        // Equivalente a: if (nombre is null) throw new ArgumentNullException(nameof(nombre));
        ArgumentNullException.ThrowIfNull(nombre);
        Console.WriteLine($"  Procesando: '{nombre.ToUpperInvariant()}'");
    }

    /// <summary>
    /// Valida y transforma un string. Retorna true si el input es válido.
    /// [NotNullWhen(true)] informa al compilador que result no es null cuando retorna true.
    /// </summary>
    public static bool ValidarYTransformar(
        [NotNullWhen(true)] string? input,
        [NotNullWhen(true)] out string? result)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            result = null;
            return false;
        }

        result = input.ToUpperInvariant();
        return true;
    }
}

// ═══════════════════════════════════════════════════════════════════
// Tipos auxiliares para los ejemplos
// ═══════════════════════════════════════════════════════════════════

/// <summary>
/// Demuestra [MemberNotNull]: garantiza al compilador que ciertos
/// campos se inicializan después de llamar a un método.
/// Este es el patrón de inicialización en dos fases.
/// </summary>
public class Repository
{
    // _data se declara como nullable porque no se inicializa en el constructor.
    // Se inicializa en Initialize(), y [MemberNotNull] informa al compilador.
    private List<string>? _data;

    public int Count => _data!.Count;

    /// <summary>
    /// Inicializa el repositorio con datos de prueba.
    /// [MemberNotNull] le dice al compilador que, tras ejecutar este método,
    /// _data nunca será null.
    /// </summary>
    [MemberNotNull(nameof(_data))]
    public void Initialize()
    {
        _data = new List<string> { "Elemento1", "Elemento2", "Elemento3" };
    }

    public string First() => _data![0];
}

/// <summary>
/// Demuestra la diferencia entre string (obligatorio) y string? (opcional).
/// </summary>
public class Persona
{
    public string Nombre { get; set; } = "";       // Requerido: nunca null
    public string? Apodo { get; set; }              // Opcional: puede ser null
    public Direccion? Direccion { get; set; }       // Opcional: puede ser null
}

/// <summary>
/// Dirección asociada a una Persona. Propiedades con NRT aplicado.
/// </summary>
public class Direccion
{
    public string Calle { get; set; } = "";         // Requerido
    public string? Ciudad { get; set; }             // Opcional
    public string? CodigoPostal { get; set; }       // Opcional
}
