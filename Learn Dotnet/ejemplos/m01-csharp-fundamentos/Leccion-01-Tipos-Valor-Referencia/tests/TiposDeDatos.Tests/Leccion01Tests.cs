using FluentAssertions;
using System.Collections;

namespace TiposDeDatos.Tests;

public class Leccion01_TiposValorYReferencia
{
    // ══════════════════════════════════════════════════════════════════════
    // 1. Value types: se copian por valor
    // ══════════════════════════════════════════════════════════════════════

    [Fact]
    public void Int_SeCopiaPorValor_ModificarCopiaNoAfectaOriginal()
    {
        int original = 5;
        int copia = original;
        copia = 10;

        original.Should().Be(5);
        copia.Should().Be(10);
    }

    [Fact]
    public void Struct_SeCopiaPorValor_CamposSonIndependientes()
    {
        Punto2D p1 = new() { X = 1, Y = 2 };
        Punto2D p2 = p1;
        p2.X = 99;

        p1.X.Should().Be(1);
        p2.X.Should().Be(99);
    }

    [Fact]
    public void Decimal_EsTipoValor_PrecisoParaDinero()
    {
        // decimal es 128-bit, el tipo recomendado para dinero/finanzas
        decimal total = 0m;
        for (int i = 0; i < 1000; i++)
        {
            total += 0.01m;
        }

        // Con double, 1000 * 0.01 puede ser 9.999999999... por error de punto flotante
        total.Should().Be(10.00m);
    }

    // ══════════════════════════════════════════════════════════════════════
    // 2. Reference types: se comparten por referencia
    // ══════════════════════════════════════════════════════════════════════

    [Fact]
    public void Class_SeCompartePorReferencia_ModificarAfectaAlOriginal()
    {
        var c1 = new ClienteTest("Ana", "ana@test.com");
        var c2 = c1;
        c2.Nombre = "María";

        c1.Nombre.Should().Be("María");
        ReferenceEquals(c1, c2).Should().BeTrue();
    }

    [Fact]
    public void Array_EsTipoReferencia_ModificarAfectaTodasLasReferencias()
    {
        int[] a = [1, 2, 3];
        int[] b = a;
        b[0] = 99;

        a[0].Should().Be(99);
    }

    [Fact]
    public void String_EsInmutable_ReasignarCreaNuevoObjeto()
    {
        string a = "Hola";
        string b = a;
        b = "Mundo";

        a.Should().Be("Hola");
        b.Should().Be("Mundo");
    }

    // ══════════════════════════════════════════════════════════════════════
    // 3. Record: value equality
    // ══════════════════════════════════════════════════════════════════════

    [Fact]
    public void RecordClass_IgualdadEstructural_DosInstanciasConMismosValoresSonIguales()
    {
        var p1 = new PersonaRecord("Ana", "López", 1990);
        var p2 = new PersonaRecord("Ana", "López", 1990);

        p1.Should().Be(p2);
        p1.Should().NotBeSameAs(p2); // Diferente referencia, mismo valor
    }

    [Fact]
    public void RecordClass_WithExpression_CreaCopiaConModificaciones()
    {
        var original = new PersonaRecord("Ana", "López", 1990);
        var modificada = original with { Apellido = "García" };

        modificada.Nombre.Should().Be("Ana");
        modificada.Apellido.Should().Be("García");
        original.Apellido.Should().Be("López"); // No se modificó
    }

    [Fact]
    public void RecordStruct_EsTipoValor_ConIgualdadEstructural()
    {
        var p1 = new Coordenada3D(1, 2, 3);
        var p2 = new Coordenada3D(1, 2, 3);

        p1.Should().Be(p2);
        p1.GetType().IsValueType.Should().BeTrue();
    }

    [Fact]
    public void RecordStruct_With_CreaCopiaIndependiente()
    {
        var original = new Coordenada3D(0, 0, 0);
        var modificado = original with { Z = 10 };

        original.Z.Should().Be(0);
        modificado.Z.Should().Be(10);
    }

    // ══════════════════════════════════════════════════════════════════════
    // 4. Boxing / Unboxing
    // ══════════════════════════════════════════════════════════════════════

    [Fact]
    public void Boxing_IntAObject_VaAlHeap()
    {
        int valor = 42;
        object caja = valor; // BOXING

        caja.Should().Be(42);
        caja.GetType().Should().Be(typeof(int));
        caja.Should().NotBeSameAs(valor); // Son copias distintas
    }

    [Fact]
    public void Unboxing_RequiereCastExplicito()
    {
        object caja = 42;
        int desempaquetado = (int)caja; // UNBOXING

        desempaquetado.Should().Be(42);
    }

    [Fact]
    public void ArrayList_ProduceBoxing_ListGenericaNo()
    {
        // Boxing con ArrayList
        ArrayList list = [];
        list.Add(1); // Boxing
        list.Add(2); // Boxing

        list[0].Should().Be(1);
        list[0]!.GetType().Should().Be(typeof(int));

        // List<T> no produce boxing
        List<int> generic = [1, 2];
        generic[0].Should().Be(1);
    }

    // ══════════════════════════════════════════════════════════════════════
    // 5. Span<T> y ReadOnlySpan<T>
    // ══════════════════════════════════════════════════════════════════════

    [Fact]
    public void Span_EsVistaMutable_ModificarSpanModificaArray()
    {
        int[] array = [10, 20, 30];
        Span<int> vista = array.AsSpan();

        vista[0] = 99;

        array[0].Should().Be(99);
    }

    [Fact]
    public void ReadOnlySpan_EsVistaInmutable_NoPermiteModificar()
    {
        string texto = "Aprendiendo .NET";
        ReadOnlySpan<char> vista = texto.AsSpan(0, 11);

        vista.ToString().Should().Be("Aprendiendo");
        texto.Length.Should().Be(16); // "Aprendiendo .NET" tiene 16 caracteres
    }

    [Fact]
    public void Span_Slice_CreaSubVistaSinCopiar()
    {
        int[] array = [1, 2, 3, 4, 5];
        Span<int> slice = array.AsSpan(1, 3); // [2, 3, 4]

        slice.Length.Should().Be(3);
        slice[0].Should().Be(2);
        slice[2].Should().Be(4);
    }

    // ══════════════════════════════════════════════════════════════════════
    // 6. ref, in, out parameters
    // ══════════════════════════════════════════════════════════════════════

    [Fact]
    public void Ref_MetodoModificaVariableExterna()
    {
        int x = 5;
        Duplicar(ref x);

        x.Should().Be(10);
        static void Duplicar(ref int n) => n *= 2;
    }

    [Fact]
    public void Out_MetodoDebeAsignarValor()
    {
        Inicializar(out int resultado);

        resultado.Should().Be(100);
        static void Inicializar(out int n) => n = 100;
    }

    [Fact]
    public void In_ParametroSoloLectura_NoPuedeModificarse()
    {
        int x = 50;

        int resultado = LeerSoloLectura(x);

        resultado.Should().Be(50);
        x.Should().Be(50); // No se modificó
        static int LeerSoloLectura(in int n) => n;
    }

    // ══════════════════════════════════════════════════════════════════════
    // 7. Default values
    // ══════════════════════════════════════════════════════════════════════

    [Fact]
    public void Default_Int_EsCero()
    {
        int x = default;
        x.Should().Be(0);
    }

    [Fact]
    public void Default_Bool_EsFalse()
    {
        bool flag = default;
        flag.Should().BeFalse();
    }

    [Fact]
    public void Default_Class_EsNull()
    {
        ClienteTest? c = default;
        c.Should().BeNull();
    }

    [Fact]
    public void Default_Struct_EsTodoCero()
    {
        Punto2D p = default;
        p.X.Should().Be(0);
        p.Y.Should().Be(0);
    }

    // ══════════════════════════════════════════════════════════════════════
    // Tipos auxiliares para tests
    // ══════════════════════════════════════════════════════════════════════

    private struct Punto2D { public int X; public int Y; }

    private class ClienteTest
    {
        public string Nombre { get; set; }
        public string Email { get; }
        public ClienteTest(string nombre, string email)
        {
            Nombre = nombre;
            Email = email;
        }
    }

    private record PersonaRecord(string Nombre, string Apellido, int AñoNacimiento);

    private readonly record struct Coordenada3D(int X, int Y, int Z);
}
