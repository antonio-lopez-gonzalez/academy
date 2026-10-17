using FluentAssertions;

namespace Demo.Tests;

// ============================================================================
// Tests de la lección 04 — LINQ
// ============================================================================
// Se prueban los operadores LINQ más importantes usando xUnit + FluentAssertions.
// Las colecciones de datos se crean localmente en cada test para mantener
// independencia entre pruebas.

public class Leccion04Tests
{
    // ─── Datos de prueba reutilizables ───────────────────────────────────

    private record Person(string Name, int Age, string City);
    private record Product(int Id, string Name, decimal Price, string Category);

    private static List<Person> GetTestPeople() =>
    [
        new("Ana",    28, "Madrid"),
        new("Luis",   35, "Barcelona"),
        new("María",  22, "Madrid"),
        new("Carlos", 30, "Sevilla"),
        new("Elena",  26, "Barcelona"),
        new("Pedro",  40, "Madrid"),
    ];

    private static List<Product> GetTestProducts() =>
    [
        new(1, "Teclado", 79.99m, "Periféricos"),
        new(2, "Ratón",   49.99m, "Periféricos"),
        new(3, "Monitor", 299.99m, "Pantallas"),
        new(4, "Portátil", 899.99m, "Equipos"),
        new(5, "Hub USB",  24.99m, "Periféricos"),
    ];

    // ====================================================================
    // 1. Where — Filtra correctamente
    // ====================================================================
    [Fact]
    public void Where_DeberiaFiltrarElementosQueCumplenCondicion()
    {
        var people = GetTestPeople();

        var result = people.Where(p => p.Age >= 30).ToList();

        result.Should().HaveCount(3);
        result.Should().OnlyContain(p => p.Age >= 30);
        result.Select(p => p.Name).Should().Contain(["Luis", "Carlos", "Pedro"]);
    }

    [Fact]
    public void Where_ConCondicionQueNoCumpleNadie_DeberiaRetornarVacio()
    {
        var people = GetTestPeople();

        var result = people.Where(p => p.Age > 100);

        result.Should().BeEmpty();
    }

    // ====================================================================
    // 2. Select — Proyecta correctamente
    // ====================================================================
    [Fact]
    public void Select_DeberiaProyectarATipoAnonimo()
    {
        var people = GetTestPeople();

        var result = people.Select(p => new { p.Name, p.City }).ToList();

        result.Should().HaveCount(6);
        result.Should().AllSatisfy(x =>
        {
            x.Name.Should().NotBeNullOrEmpty();
            x.City.Should().NotBeNullOrEmpty();
        });
    }

    [Fact]
    public void Select_DeberiaProyectarATuplaConNombres()
    {
        var people = GetTestPeople();

        var result = people.Select(p => (FullName: p.Name, Years: p.Age)).ToList();

        result.Should().HaveCount(6);
        result.First().FullName.Should().Be("Ana");
        result.First().Years.Should().Be(28);
    }

    // ====================================================================
    // 3. OrderBy y OrderByDescending
    // ====================================================================
    [Fact]
    public void OrderBy_DeberiaOrdenarAscendentemente()
    {
        var people = GetTestPeople();

        var result = people.OrderBy(p => p.Age).ToList();

        result.First().Age.Should().Be(22);  // María es la más joven
        result.Last().Age.Should().Be(40);   // Pedro es el mayor
    }

    [Fact]
    public void OrderByDescending_DeberiaOrdenarDescendentemente()
    {
        var people = GetTestPeople();

        var result = people.OrderByDescending(p => p.Age).ToList();

        result.First().Name.Should().Be("Pedro"); // 40 años
        result.Last().Name.Should().Be("María");   // 22 años
    }

    [Fact]
    public void ThenBy_DeberiaDesempatarTrasOrderBy()
    {
        // Añadimos otra persona de Madrid para provocar empate.
        var people = GetTestPeople();
        var extended = people.Append(new Person("Zara", 28, "Madrid")).ToList();

        var result = extended
            .OrderBy(p => p.City)
            .ThenBy(p => p.Name)
            .ToList();

        // Barcelona va antes que Madrid alfabéticamente.
        // Dentro de Barcelona: Elena (E) antes que Luis (L).
        result[0].Name.Should().Be("Elena");
        result[1].Name.Should().Be("Luis");
        // Dentro de Madrid: Ana (A) < María (M) < Pedro (P) < Zara (Z)
        result[2].Name.Should().Be("Ana");
        result[3].Name.Should().Be("María");
        result[4].Name.Should().Be("Pedro");
        result[5].Name.Should().Be("Zara");
    }

    // ====================================================================
    // 4. GroupBy — Agrupa por clave
    // ====================================================================
    [Fact]
    public void GroupBy_DeberiaAgruparElementosPorClave()
    {
        var people = GetTestPeople();

        var groups = people.GroupBy(p => p.City).ToList();

        groups.Should().HaveCount(3);
        groups.Select(g => g.Key).Should().Contain(["Madrid", "Barcelona", "Sevilla"]);

        var madridGroup = groups.Single(g => g.Key == "Madrid");
        madridGroup.Should().HaveCount(3);
        madridGroup.Select(p => p.Name).Should().Contain(["Ana", "María", "Pedro"]);
    }

    // ====================================================================
    // 5. Any — Retorna true/false según existencia
    // ====================================================================
    [Fact]
    public void Any_ConElementosQueCumplenCondicion_DeberiaRetornarTrue()
    {
        var people = GetTestPeople();

        var result = people.Any(p => p.City == "Sevilla");

        result.Should().BeTrue();
    }

    [Fact]
    public void Any_ConCondicionImposible_DeberiaRetornarFalse()
    {
        var people = GetTestPeople();

        var result = people.Any(p => p.Age > 100);

        result.Should().BeFalse();
    }

    // ====================================================================
    // 6. All — Verifica todos los elementos
    // ====================================================================
    [Fact]
    public void All_DeberiaRetornarTrueCuandoTodosCumplen()
    {
        var people = GetTestPeople();

        var result = people.All(p => p.Age >= 18);

        result.Should().BeTrue();
    }

    [Fact]
    public void All_DeberiaRetornarFalseCuandoAlgunoNoCumple()
    {
        var people = GetTestPeople();

        var result = people.All(p => p.City == "Madrid");

        result.Should().BeFalse();
    }

    // ====================================================================
    // 7. Aggregate — Computa valor acumulado
    // ====================================================================
    [Fact]
    public void Aggregate_DeberiaSumarTodasLasEdades()
    {
        var people = GetTestPeople();

        // Suma de edades empezando en 0: 28+35+22+30+26+40 = 181
        var totalAge = people.Aggregate(0, (acc, p) => acc + p.Age);

        totalAge.Should().Be(181);
    }

    [Fact]
    public void Aggregate_DeberiaConcatenarNombres()
    {
        var people = GetTestPeople();

        var concatenated = people
            .Select(p => p.Name)
            .Aggregate((current, next) => $"{current}, {next}");

        concatenated.Should().Be("Ana, Luis, María, Carlos, Elena, Pedro");
    }

    // ====================================================================
    // 8. First — Lanza excepción en secuencia vacía
    // ====================================================================
    [Fact]
    public void First_ConSecuenciaVacia_DeberiaLanzarInvalidOperationException()
    {
        var empty = new List<Person>();

        var act = () => empty.First();

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void First_ConCondicionQueNoCumpleNadie_DeberiaLanzarInvalidOperationException()
    {
        var people = GetTestPeople();

        var act = () => people.First(p => p.Age > 100);

        act.Should().Throw<InvalidOperationException>();
    }

    // ====================================================================
    // 9. FirstOrDefault — Retorna default en lugar de lanzar
    // ====================================================================
    [Fact]
    public void FirstOrDefault_ConSecuenciaVacia_DeberiaRetornarNull()
    {
        var empty = new List<Person>();

        var result = empty.FirstOrDefault();

        result.Should().BeNull();
    }

    [Fact]
    public void FirstOrDefault_ConElementos_DeberiaRetornarPrimero()
    {
        var people = GetTestPeople();

        var result = people.FirstOrDefault(p => p.Age > 30);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Luis");
    }

    // ====================================================================
    // 10. Single — Lanza cuando hay más de una coincidencia
    // ====================================================================
    [Fact]
    public void Single_ConMultiplesCoincidencias_DeberiaLanzarInvalidOperationException()
    {
        var people = GetTestPeople();

        // Hay 3 personas en Madrid → Single debe lanzar.
        var act = () => people.Single(p => p.City == "Madrid");

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Single_ConElementoUnico_DeberiaRetornarlo()
    {
        var people = GetTestPeople();

        var result = people.Single(p => p.Name == "Carlos");

        result.Age.Should().Be(30);
        result.City.Should().Be("Sevilla");
    }

    // ====================================================================
    // 11. Skip y Take — Paginación
    // ====================================================================
    [Fact]
    public void SkipTake_DeberiaPaginarCorrectamente()
    {
        var people = GetTestPeople().OrderBy(p => p.Name).ToList();
        // Ordenados: Ana, Carlos, Elena, Luis, María, Pedro

        var page1 = people.Skip(0).Take(2).ToList();
        var page2 = people.Skip(2).Take(2).ToList();
        var page3 = people.Skip(4).Take(2).ToList();

        page1.Should().HaveCount(2);
        page1.Select(p => p.Name).Should().ContainInOrder("Ana", "Carlos");

        page2.Should().HaveCount(2);
        page2.Select(p => p.Name).Should().ContainInOrder("Elena", "Luis");

        page3.Should().HaveCount(2);
        page3.Select(p => p.Name).Should().ContainInOrder("María", "Pedro");
    }

    // ====================================================================
    // 12. Distinct — Elimina duplicados
    // ====================================================================
    [Fact]
    public void Distinct_DeberiaEliminarDuplicados()
    {
        var numbers = new[] { 1, 2, 2, 3, 3, 3, 4, 5, 5 };

        var result = numbers.Distinct().ToList();

        result.Should().BeEquivalentTo([1, 2, 3, 4, 5]);
        result.Should().HaveCount(5);
    }

    [Fact]
    public void DistinctBy_DeberiaEliminarDuplicadosPorClave()
    {
        var people = GetTestPeople();

        // DistinctBy ciudad: debería devolver una persona por ciudad.
        var result = people.DistinctBy(p => p.City).ToList();

        result.Should().HaveCount(3);
        result.Select(p => p.City).Distinct().Should().HaveCount(3);
    }

    // ====================================================================
    // 13. Ejecución diferida — Cambios en la fuente se reflejan
    // ====================================================================
    [Fact]
    public void EjecucionDiferida_ModificarFuenteDespuesDeDefinirQuery_DeberiaReflejarCambios()
    {
        var numbers = new List<int> { 1, 2, 3, 4, 5 };

        // La consulta se define pero no se ejecuta.
        var query = numbers.Where(n => n % 2 == 0);

        query.Should().BeEquivalentTo([2, 4]);

        // Modificamos la fuente.
        numbers.Add(6);
        numbers.Add(8);

        // La consulta refleja los cambios porque se vuelve a ejecutar al iterar.
        query.Should().BeEquivalentTo([2, 4, 6, 8]);
    }

    // ====================================================================
    // 14. Ejecución inmediata — ToList materializa los resultados
    // ====================================================================
    [Fact]
    public void ToList_DeberiaMaterializarResultadosYAislarDeCambiosPosteriores()
    {
        var numbers = new List<int> { 1, 2, 3, 4, 5 };

        // ToList fuerza la ejecución inmediata y copia los resultados.
        var materialized = numbers.Where(n => n > 3).ToList();

        materialized.Should().BeEquivalentTo([4, 5]);

        // Cambios posteriores no afectan a la copia.
        numbers.Add(10);

        materialized.Should().BeEquivalentTo([4, 5]);
        materialized.Should().HaveCount(2);
    }

    // ====================================================================
    // 15. Count / Sum / Min / Max / Average — Agregaciones
    // ====================================================================
    [Fact]
    public void Agregaciones_DeberianCalcularValoresCorrectos()
    {
        var products = GetTestProducts();

        products.Count().Should().Be(5);
        products.Count(p => p.Category == "Periféricos").Should().Be(3);
        products.Sum(p => p.Price).Should().Be(79.99m + 49.99m + 299.99m + 899.99m + 24.99m);
        products.Min(p => p.Price).Should().Be(24.99m);
        products.Max(p => p.Price).Should().Be(899.99m);
        products.Average(p => p.Price).Should().BeApproximately(
            (79.99m + 49.99m + 299.99m + 899.99m + 24.99m) / 5, 0.01m);
    }

    // ====================================================================
    // 16. Join — Combina dos colecciones (Inner Join)
    // ====================================================================
    [Fact]
    public void Join_DeberiaCombinarProductosConPedidos()
    {
        var orders = new[]
        {
            new { OrderId = 1, ProductId = 1, Quantity = 2 },
            new { OrderId = 2, ProductId = 3, Quantity = 1 },
            new { OrderId = 3, ProductId = 1, Quantity = 3 },
        };

        var products = new[]
        {
            new { Id = 1, Name = "Teclado", Price = 80m },
            new { Id = 2, Name = "Ratón",   Price = 50m },
            new { Id = 3, Name = "Monitor", Price = 300m },
        };

        var result = orders.Join(
            products,
            order => order.ProductId,
            product => product.Id,
            (order, product) => new
            {
                order.OrderId,
                product.Name,
                Total = order.Quantity * product.Price
            });

        result.Should().HaveCount(3);
        result.Should().ContainSingle(o => o.OrderId == 1 && o.Name == "Teclado" && o.Total == 160m);
        result.Should().ContainSingle(o => o.OrderId == 2 && o.Name == "Monitor" && o.Total == 300m);
        result.Should().ContainSingle(o => o.OrderId == 3 && o.Name == "Teclado" && o.Total == 240m);
    }

    // ====================================================================
    // 17. Union / Intersect / Except — Operaciones de conjunto
    // ====================================================================
    [Fact]
    public void OperacionesDeConjunto_DeberianFuncionarCorrectamente()
    {
        var a = new[] { 1, 2, 3, 4 };
        var b = new[] { 3, 4, 5, 6 };

        a.Union(b).Should().BeEquivalentTo([1, 2, 3, 4, 5, 6]);
        a.Intersect(b).Should().BeEquivalentTo([3, 4]);
        a.Except(b).Should().BeEquivalentTo([1, 2]);
    }

    // ====================================================================
    // 18. Last y LastOrDefault
    // ====================================================================
    [Fact]
    public void Last_DeberiaRetornarUltimoElemento()
    {
        var people = GetTestPeople();

        var last = people.Last();

        last.Name.Should().Be("Pedro");
    }

    [Fact]
    public void LastOrDefault_ConSecuenciaVacia_DeberiaRetornarNull()
    {
        var empty = new List<Person>();

        var result = empty.LastOrDefault();

        result.Should().BeNull();
    }
}
