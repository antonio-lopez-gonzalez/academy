// ============================================================================
// Lección 04 — LINQ (Language Integrated Query)
// ============================================================================
// LINQ es un conjunto de extensiones sobre IEnumerable<T> e IQueryable<T> que
// permite escribir consultas declarativas sobre colecciones de datos.
// Existen dos sintaxis: método (Where, Select, etc.) y consulta (from...select).

// ─── Datos de ejemplo ─────────────────────────────────────────────────────

// Colección de personas para las demostraciones.
List<Person> people =
[
    new("Ana",      28, "Madrid",     "C#"),
    new("Luis",     35, "Barcelona",  "Java"),
    new("María",    22, "Madrid",     "C#"),
    new("Carlos",   30, "Sevilla",    "Python"),
    new("Elena",    26, "Barcelona",  "C#"),
    new("Pedro",    40, "Madrid",     "Java"),
    new("Sofía",    19, "Sevilla",    "JavaScript"),
    new("Javier",   33, "Barcelona",  "Python"),
    new("Lucía",    24, "Madrid",     "JavaScript"),
    new("Miguel",   29, "Sevilla",    "C#"),
];

// Colección de productos para demostraciones de agregación y conjuntos.
List<Product> products =
[
    new() { Id = 1, Name = "Teclado",       Price = 79.99m, Category = "Periféricos", Stock = 25 },
    new() { Id = 2, Name = "Ratón",         Price = 49.99m, Category = "Periféricos", Stock = 40 },
    new() { Id = 3, Name = "Monitor",       Price = 299.99m, Category = "Pantallas",  Stock = 10 },
    new() { Id = 4, Name = "Portátil",      Price = 899.99m, Category = "Equipos",    Stock = 5  },
    new() { Id = 5, Name = "Auriculares",   Price = 89.99m, Category = "Audio",       Stock = 30 },
    new() { Id = 6, Name = "Webcam",        Price = 59.99m, Category = "Periféricos", Stock = 15 },
    new() { Id = 7, Name = "Hub USB",       Price = 24.99m, Category = "Periféricos", Stock = 0  },
    new() { Id = 8, Name = "Tablet",        Price = 399.99m, Category = "Equipos",    Stock = 8  },
];

// Pedidos para demostrar Join.
List<Order> orders =
[
    new() { OrderId = 1, ProductId = 1, Quantity = 2, Date = new(2026, 1, 10) },
    new() { OrderId = 2, ProductId = 3, Quantity = 1, Date = new(2026, 1, 12) },
    new() { OrderId = 3, ProductId = 1, Quantity = 3, Date = new(2026, 2, 5)  },
    new() { OrderId = 4, ProductId = 5, Quantity = 2, Date = new(2026, 2, 8)  },
    new() { OrderId = 5, ProductId = 7, Quantity = 4, Date = new(2026, 3, 1)  },
    new() { OrderId = 6, ProductId = 3, Quantity = 1, Date = new(2026, 3, 15) },
];

Console.OutputEncoding = System.Text.Encoding.UTF8;

// ============================================================================
// 1. WHERE — Filtrar elementos de una secuencia
// ============================================================================
Console.WriteLine("═══ 1. WHERE — Filtrar elementos ═══");

// Where recibe un delegado Func<T, bool>. Devuelve solo los elementos que
// cumplen la condición.
var adults = people.Where(p => p.Age >= 30);
Console.WriteLine("  Personas con 30 años o más:");
foreach (var p in adults)
    Console.WriteLine($"    {p.Name} ({p.Age})");

var fromMadridUsingCSharp = people.Where(p => p.City == "Madrid" && p.Language == "C#");
Console.WriteLine($"  Madrid + C#: {fromMadridUsingCSharp.Count()} personas");

// Where con índice: el segundo parámetro del delegado es la posición (base 0).
var everySecondPerson = people.Where((p, index) => index % 2 == 0);
Console.WriteLine($"  Personas en posición par (0,2,4...): {string.Join(", ", everySecondPerson.Select(p => p.Name))}");

Console.WriteLine();

// ============================================================================
// 2. SELECT — Proyectar cada elemento en una nueva forma
// ============================================================================
Console.WriteLine("═══ 2. SELECT — Proyectar elementos ═══");

// Select transforma cada elemento de la secuencia. No modifica el original;
// produce una nueva secuencia con los resultados del delegado.
var names = people.Select(p => p.Name);
Console.WriteLine($"  Nombres: {string.Join(", ", names)}");

// Proyección a tipo anónimo: crea objetos con solo las propiedades necesarias.
var nameAndCity = people.Select(p => new { p.Name, p.City });
Console.WriteLine("  Nombre y ciudad:");
foreach (var x in nameAndCity)
    Console.WriteLine($"    {x.Name} vive en {x.City}");

// Select con índice: permite incluir la posición en la proyección.
var ranked = people.Select((p, i) => new { Position = i + 1, p.Name, p.Age });
Console.WriteLine("  Ranking por posición:");
foreach (var r in ranked)
    Console.WriteLine($"    {r.Position}. {r.Name} ({r.Age})");

// Proyección a tupla (C# 7+): alternativa a tipos anónimos con nombres explícitos.
var nameAgeTuples = people.Select(p => (FullName: p.Name, Years: p.Age));
Console.WriteLine($"  Primera tupla: {nameAgeTuples.First().FullName}, {nameAgeTuples.First().Years} años");

Console.WriteLine();

// ============================================================================
// 3. SELECTMANY — Aplanar colecciones anidadas
// ============================================================================
Console.WriteLine("═══ 3. SELECTMANY — Aplanar colecciones ═══");

// SelectMany se usa cuando cada elemento contiene a su vez una colección
// y quieres obtener una única secuencia plana con todos los subelementos.

// Ejemplo: una lista de frases; queremos obtener todas las palabras.
List<string> phrases = ["Hola mundo LINQ", "C# es potente", "SelectMany aplana"];
var allWords = phrases.SelectMany(p => p.Split(' '));
Console.WriteLine($"  Palabras: {string.Join(", ", allWords)}");

// Ejemplo con colecciones anidadas: ciudades y sus personas.
var cities = new[]
{
    new { Name = "Madrid",    People = people.Where(p => p.City == "Madrid").ToList() },
    new { Name = "Barcelona", People = people.Where(p => p.City == "Barcelona").ToList() },
    new { Name = "Sevilla",   People = people.Where(p => p.City == "Sevilla").ToList() },
};
// SelectMany aplana todas las personas de todas las ciudades en una sola lista.
var flattened = cities.SelectMany(c => c.People);
Console.WriteLine($"  Total personas aplanadas: {flattened.Count()}");

Console.WriteLine();

// ============================================================================
// 4. ORDERBY / ORDERBYDESCENDING / THENBY — Ordenación
// ============================================================================
Console.WriteLine("═══ 4. ORDERBY / ORDERBYDESCENDING / THENBY ═══");

// OrderBy ordena ascendentemente según la clave proporcionada.
// OrderByDescending ordena descendentemente.
// ThenBy / ThenByDescending realizan ordenación secundaria para desempate.

var byAge = people.OrderBy(p => p.Age);
Console.WriteLine("  Por edad ascendente:");
foreach (var p in byAge)
    Console.WriteLine($"    {p.Name}: {p.Age}");

var byAgeDesc = people.OrderByDescending(p => p.Age);
Console.WriteLine($"  Mayor: {byAgeDesc.First().Name} ({byAgeDesc.First().Age})");

// Ordenación compuesta: primero por ciudad, luego por edad descendente.
var byCityThenAge = people.OrderBy(p => p.City).ThenByDescending(p => p.Age);
Console.WriteLine("  Por ciudad (asc) y edad (desc):");
foreach (var p in byCityThenAge)
    Console.WriteLine($"    {p.City} — {p.Name} ({p.Age})");

Console.WriteLine();

// ============================================================================
// 5. GROUPBY — Agrupar elementos por una clave
// ============================================================================
Console.WriteLine("═══ 5. GROUPBY — Agrupar por clave ═══");

// GroupBy devuelve una secuencia de IGrouping<TKey, TElement>. Cada grupo
// tiene una propiedad Key y es a su vez una colección de elementos.

var byCity = people.GroupBy(p => p.City);
foreach (var group in byCity)
{
    Console.WriteLine($"  {group.Key} ({group.Count()} personas):");
    foreach (var p in group)
        Console.WriteLine($"    - {p.Name} ({p.Age}, {p.Language})");
}

// Agrupación con proyección: obtener el promedio de edad por lenguaje.
var avgAgeByLanguage = people
    .GroupBy(p => p.Language)
    .Select(g => new { Language = g.Key, AverageAge = g.Average(p => p.Age) });
Console.WriteLine("  Promedio de edad por lenguaje:");
foreach (var x in avgAgeByLanguage)
    Console.WriteLine($"    {x.Language}: {x.AverageAge:F1} años");

Console.WriteLine();

// ============================================================================
// 6. JOIN Y GROUPJOIN — Combinar dos colecciones
// ============================================================================
Console.WriteLine("═══ 6. JOIN Y GROUPJOIN ═══");

// Join: inner join entre dos secuencias basado en una clave común.
// Equivale a INNER JOIN en SQL.
// Parámetros: colección externa, colección interna, selector clave externa,
// selector clave interna, selector de resultado.
var orderDetails = orders.Join(
    products,
    order => order.ProductId,
    product => product.Id,
    (order, product) => new
    {
        order.OrderId,
        ProductName = product.Name,
        order.Quantity,
        Total = order.Quantity * product.Price
    });

Console.WriteLine("  Detalles de pedidos (Inner Join):");
foreach (var od in orderDetails)
    Console.WriteLine($"    Pedido #{od.OrderId}: {od.ProductName} x{od.Quantity} = {od.Total:C}");

// GroupJoin: left outer join que agrupa los elementos coincidentes de la
// colección interna para cada elemento de la externa.
var productsWithOrders = products.GroupJoin(
    orders,
    product => product.Id,
    order => order.ProductId,
    (product, matchingOrders) => new
    {
        product.Name,
        Orders = matchingOrders.ToList()
    });

Console.WriteLine("\n  Productos con sus pedidos (Group Join):");
foreach (var po in productsWithOrders)
{
    var totalQuantity = po.Orders.Sum(o => o.Quantity);
    Console.WriteLine($"    {po.Name}: {po.Orders.Count} pedidos, {totalQuantity} unidades");
}

Console.WriteLine();

// ============================================================================
// 7. AGREGACIÓN — Any, All, Count, Sum, Min, Max, Average, Aggregate
// ============================================================================
Console.WriteLine("═══ 7. AGREGACIÓN ═══");

// Any: ¿existe al menos un elemento que cumpla la condición?
bool hasMinors = people.Any(p => p.Age < 18);
Console.WriteLine($"  ¿Hay menores de 18? {hasMinors}");

// All: ¿todos los elementos cumplen la condición?
bool allAdults = people.All(p => p.Age >= 18);
Console.WriteLine($"  ¿Todos son mayores de edad? {allAdults}");

// Count: número de elementos (opcionalmente, que cumplan una condición).
int csCount = people.Count(p => p.Language == "C#");
Console.WriteLine($"  Programadores C#: {csCount}");

// Sum, Min, Max, Average: operaciones matemáticas sobre una proyección numérica.
decimal totalStockValue = products.Sum(p => p.Price * p.Stock);
Console.WriteLine($"  Valor total del inventario: {totalStockValue:C}");

decimal minPrice = products.Min(p => p.Price);
Console.WriteLine($"  Producto más barato: {minPrice:C}");

decimal maxPrice = products.Max(p => p.Price);
Console.WriteLine($"  Producto más caro: {maxPrice:C}");

double avgPrice = products.Average(p => (double)p.Price);
Console.WriteLine($"  Precio medio: {avgPrice:F2} €");

// Aggregate: acumula valores aplicando una función a cada elemento.
// Útil para operaciones personalizadas (concatenar, producto, etc.).
// Sobrecarga: (semilla, acumulador) o solo acumulador.
string allNames = people
    .Select(p => p.Name)
    .Aggregate((current, next) => $"{current}, {next}");
Console.WriteLine($"  Todos los nombres concatenados: {allNames}");

// Aggregate con semilla: calcular suma de edades partiendo de 0.
int totalAge = people.Aggregate(0, (acc, p) => acc + p.Age);
Console.WriteLine($"  Suma total de edades (Aggregate): {totalAge}");

Console.WriteLine();

// ============================================================================
// 8. FIRST, FIRSTORDEFAULT, SINGLE, SINGLEORDEFAULT, LAST, LASTORDEFAULT
// ============================================================================
Console.WriteLine("═══ 8. ELEMENTOS ÚNICOS ═══");

// First: devuelve el primer elemento. Lanza InvalidOperationException si
// la secuencia está vacía o ningún elemento cumple la condición.
var firstPerson = people.First(); // Primer elemento de la lista
Console.WriteLine($"  First (sin condición): {firstPerson.Name}");

var firstMadrid = people.First(p => p.City == "Madrid");
Console.WriteLine($"  First madrileño: {firstMadrid.Name}");

// FirstOrDefault: devuelve el valor por defecto (null para referencias, 0
// para valores) en lugar de lanzar excepción si no hay coincidencias.
var firstUnder18 = people.FirstOrDefault(p => p.Age < 18);
Console.WriteLine($"  FirstOrDefault menor de 18: {(firstUnder18 is null ? "ninguno" : firstUnder18.Name)}");

// Single: devuelve el único elemento que cumple la condición.
// Lanza si hay 0 coincidencias o más de 1.
try
{
    var singleSevilla = people.Single(p => p.City == "Sevilla");
    Console.WriteLine($"  Single Sevilla: (error — hay más de uno)");
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"  Single Sevilla lanza: {ex.GetType().Name} (hay 3 personas en Sevilla)");
}

var singleByName = people.Single(p => p.Name == "Miguel");
Console.WriteLine($"  Single Miguel: {singleByName.Age} años");

// SingleOrDefault: como Single pero devuelve default si hay 0 coincidencias.
// Sigue lanzando si hay más de 1.
var singleDefault = people.SingleOrDefault(p => p.Name == "Zacarías");
Console.WriteLine($"  SingleOrDefault 'Zacarías': {(singleDefault is null ? "null" : singleDefault.Name)}");

// Last / LastOrDefault: devuelven el último elemento (con o sin condición).
var lastPerson = people.Last();
Console.WriteLine($"  Last persona: {lastPerson.Name}");

Console.WriteLine();

// ============================================================================
// 9. SKIP Y TAKE — Paginación
// ============================================================================
Console.WriteLine("═══ 9. SKIP Y TAKE — Paginación ═══");

// Skip: salta los primeros N elementos.
// Take: toma los siguientes N elementos.
// Combinados permiten paginar: página N = Skip((page-1) * pageSize).Take(pageSize).

int pageSize = 3;
int totalPages = (int)Math.Ceiling(people.Count / (double)pageSize);

for (int page = 1; page <= totalPages; page++)
{
    // Fórmula de paginación: saltar los elementos de páginas anteriores,
    // tomar solo los de la página actual.
    var pageItems = people
        .OrderBy(p => p.Name)
        .Skip((page - 1) * pageSize)
        .Take(pageSize);

    Console.WriteLine($"  Página {page}/{totalPages}:");
    foreach (var p in pageItems)
        Console.WriteLine($"    {p.Name} ({p.Age}, {p.City})");
}

// SkipLast / TakeLast (.NET 6+): saltan o toman desde el final.
var firstThree = people.Take(3).Select(p => p.Name);
Console.WriteLine($"  Take(3): {string.Join(", ", firstThree)}");

var skipTwo = people.Skip(2).Take(3).Select(p => p.Name);
Console.WriteLine($"  Skip(2).Take(3): {string.Join(", ", skipTwo)}");

Console.WriteLine();

// ============================================================================
// 10. DISTINCT, UNION, INTERSECT, EXCEPT — Operaciones de conjunto
// ============================================================================
Console.WriteLine("═══ 10. DISTINCT, UNION, INTERSECT, EXCEPT ═══");

// Distinct: elimina duplicados. Usa el comparador de igualdad por defecto
// (Equals/GetHashCode). Para objetos, record usa igualdad estructural.
var citiesList = people.Select(p => p.City);
var uniqueCities = citiesList.Distinct();
Console.WriteLine($"  Ciudades distintas: {string.Join(", ", uniqueCities)}");

// Union: combina dos secuencias eliminando duplicados.
var groupA = new[] { "C#", "Java", "Python" };
var groupB = new[] { "Python", "JavaScript", "C#" };
var union = groupA.Union(groupB);
Console.WriteLine($"  Union: {string.Join(", ", union)}");

// Intersect: elementos presentes en ambas secuencias.
var intersect = groupA.Intersect(groupB);
Console.WriteLine($"  Intersect: {string.Join(", ", intersect)}");

// Except: elementos de la primera secuencia que no están en la segunda.
var except = groupA.Except(groupB);
Console.WriteLine($"  Except (A - B): {string.Join(", ", except)}");

// DistinctBy, UnionBy, IntersectBy, ExceptBy (.NET 6+): permiten especificar
// una clave para comparar.
var distinctByLanguage = people.DistinctBy(p => p.Language);
Console.WriteLine($"  DistinctBy Language: {string.Join(", ", distinctByLanguage.Select(p => $"{p.Name} ({p.Language})"))}");

Console.WriteLine();

// ============================================================================
// 11. SINTAXIS DE CONSULTA (Query syntax) vs SINTAXIS DE MÉTODO
// ============================================================================
Console.WriteLine("═══ 11. SINTAXIS DE CONSULTA vs MÉTODO ═══");

// Sintaxis de consulta: similar a SQL, se escribe con palabras clave como
// from, where, select, group, join, orderby. El compilador la traduce a
// llamadas de método en tiempo de compilación.

// ── Query syntax ──
var querySyntaxResult = 
    from p in people
    where p.Age > 25
    orderby p.Name
    select new { p.Name, p.Age, p.City };

Console.WriteLine("  Query syntax (from...where...orderby...select):");
foreach (var r in querySyntaxResult)
    Console.WriteLine($"    {r.Name}, {r.Age}, {r.City}");

// ── Method syntax (equivalente exacto) ──
var methodSyntaxResult = people
    .Where(p => p.Age > 25)
    .OrderBy(p => p.Name)
    .Select(p => new { p.Name, p.Age, p.City });

Console.WriteLine("  Method syntax: mismo resultado.");

// Group con query syntax:
var queryGroup = 
    from p in people
    group p by p.Language into g
    select new { Language = g.Key, Count = g.Count() };

Console.WriteLine("  Group por lenguaje (query syntax):");
foreach (var g in queryGroup)
    Console.WriteLine($"    {g.Language}: {g.Count}");

// Join con query syntax:
var queryJoin =
    from o in orders
    join p in products on o.ProductId equals p.Id
    select new { o.OrderId, ProductName = p.Name, Total = o.Quantity * p.Price };

Console.WriteLine("  Join con query syntax:");
foreach (var j in queryJoin)
    Console.WriteLine($"    Pedido #{j.OrderId}: {j.ProductName} — {j.Total:C}");

// Regla práctica:
// - Usa query syntax cuando tengas joins o agrupaciones complejas (más legible).
// - Usa method syntax para Where/Select/OrderBy simples (más conciso).
// - Puedes mezclarlas: query syntax con .ToList() al final.

Console.WriteLine();

// ============================================================================
// 12. EJECUCIÓN DIFERIDA vs EJECUCIÓN INMEDIATA
// ============================================================================
Console.WriteLine("═══ 12. EJECUCIÓN DIFERIDA vs INMEDIATA ═══");

// ── Ejecución diferida (deferred execution) ──
// La consulta NO se ejecuta al definirla. Se ejecuta al iterarla (foreach,
// ToList, Count, etc.). Esto permite modificar la fuente y que la consulta
// refleje los cambios.

var mutableNumbers = new List<int> { 1, 2, 3, 4, 5 };

// La consulta se define pero no se ejecuta todavía.
var evenQuery = mutableNumbers.Where(n => n % 2 == 0);

Console.WriteLine($"  Antes de modificar: {string.Join(", ", evenQuery)}"); // 2, 4

// Modificamos la fuente después de definir la consulta.
mutableNumbers.Add(6);
mutableNumbers.Add(8);

// La consulta refleja los nuevos elementos porque se ejecuta AHORA al iterar.
Console.WriteLine($"  Después de añadir 6 y 8: {string.Join(", ", evenQuery)}"); // 2, 4, 6, 8

// ── Ejecución inmediata ──
// ToList, ToArray, ToDictionary, ToHashSet fuerzan la ejecución inmediata
// y materializan los resultados en una colección concreta. Los cambios
// posteriores en la fuente no afectan al resultado.

var numbersCopy = new List<int> { 1, 2, 3, 4, 5 };
var materialized = numbersCopy.Where(n => n > 3).ToList(); // Se ejecuta AHORA

numbersCopy.Add(10); // Cambio posterior, no afecta a materialized
Console.WriteLine($"  Materializado con ToList: {string.Join(", ", materialized)}");
Console.WriteLine($"  Fuente después de añadir 10: {string.Join(", ", numbersCopy)}");
Console.WriteLine($"  Materializado no cambió: {string.Join(", ", materialized)}");

// ToDictionary: crea un diccionario con clave y valor a partir de la secuencia.
var personDict = people.ToDictionary(p => p.Name, p => p.Age);
Console.WriteLine($"  Edad de Ana (desde diccionario): {personDict["Ana"]}");

// ToLookup: similar a Dictionary pero permite múltiples valores por clave.
var byLanguage = people.ToLookup(p => p.Language);
Console.WriteLine($"  Programadores C#: {string.Join(", ", byLanguage["C#"].Select(p => p.Name))}");

Console.WriteLine();

// ============================================================================
// 13. IEnumerable<T> vs IQueryable<T>
// ============================================================================
Console.WriteLine("═══ 13. IEnumerable<T> vs IQueryable<T> ═══");

// IEnumerable<T>: representa una secuencia en memoria. Las operaciones LINQ
// (Where, Select, etc.) se ejecutan en el lado del cliente (en memoria, en C#).
// Adecuado para colecciones en memoria: List<T>, arrays, etc.

IEnumerable<Person> enumerablePeople = people;
var enumerableResult = enumerablePeople
    .Where(p => p.Age > 30)
    .Select(p => p.Name);

Console.WriteLine($"  IEnumerable — El filtro se ejecuta en memoria (en este proceso).");
Console.WriteLine($"  Resultado: {string.Join(", ", enumerableResult)}");

// IQueryable<T>: representa una consulta que puede traducirse a otro lenguaje
// (SQL, MongoDB, etc.). Las operaciones LINQ se acumulan en un árbol de
// expresión y se traducen al lenguaje destino al ejecutar la consulta.
// Lo veremos en profundidad con Entity Framework Core (Módulo 7).

// Para convertir IEnumerable a IQueryable usamos AsQueryable():
IQueryable<Person> queryablePeople = people.AsQueryable();
var queryableResult = queryablePeople
    .Where(p => p.City == "Barcelona")
    .Select(p => p.Name);

Console.WriteLine($"  IQueryable — La consulta se construye como árbol de expresión.");
Console.WriteLine($"  Resultado (se evalúa contra la fuente subyacente): {string.Join(", ", queryableResult)}");

Console.WriteLine("""
   ╔═══════════════╦══════════════════════════╦════════════════════════════╗
   ║               ║ IEnumerable<T>           ║ IQueryable<T>              ║
   ╠═══════════════╬══════════════════════════╬════════════════════════════╣
   ║ Dónde ejecuta ║ En memoria (cliente)     ║ En el origen (BD, API...) ║
   ║ Delegados     ║ Func<T, bool> compilado  ║ Expression<Func<T,bool>>  ║
   ║ Ejemplo       ║ List<T>, Array           ║ DbSet<T> en EF Core       ║
   ║ Cuándo usar   ║ Datos en memoria         ║ Datos en base de datos    ║
   ╚═══════════════╩══════════════════════════╩════════════════════════════╝
  """);

Console.WriteLine();

// ============================================================================
// 14. LINQ CON TIPOS ANÓNIMOS Y TUPLAS
// ============================================================================
Console.WriteLine("═══ 14. LINQ CON TIPOS ANÓNIMOS Y TUPLAS ═══");

// Los tipos anónimos (new { Prop = valor }) son ideales para proyecciones
// intermedias en LINQ. El compilador genera una clase con propiedades de solo
// lectura y igualdad estructural.

var anonymousDemo = people
    .Where(p => p.Age >= 25)
    .Select(p => new
    {
        NombreCompleto = p.Name.ToUpper(),
        p.City,
        EsSenior = p.Age >= 35
    });

Console.WriteLine("  Proyección a tipo anónimo:");
foreach (var a in anonymousDemo)
    Console.WriteLine($"    {a.NombreCompleto} ({a.City}) — Senior: {a.EsSenior}");

// Las tuplas con nombre ofrecen una alternativa con tipo nominal.
// Ventaja: pueden usarse como tipo de retorno de métodos.
var tupleDemo = people
    .Where(p => p.Age >= 25)
    .Select(p => (Name: p.Name, Location: p.City, IsSenior: p.Age >= 35));

Console.WriteLine("  Proyección a tupla:");
foreach (var (name, location, isSenior) in tupleDemo)
    Console.WriteLine($"    {name} ({location}) — Senior: {isSenior}");

Console.WriteLine();

// ============================================================================
// 15. RESUMEN DE OPERADORES LINQ
// ============================================================================
Console.WriteLine("═══ 15. RESUMEN ═══");
Console.WriteLine("""
  ┌─────────────────────────┬──────────────────────────────────────────────┐
  │ Operador                │ Qué hace                                     │
  ├─────────────────────────┼──────────────────────────────────────────────┤
  │ Where                   │ Filtra elementos por una condición           │
  │ Select                  │ Proyecta cada elemento a una nueva forma     │
  │ SelectMany              │ Aplana colecciones anidadas                  │
  │ OrderBy / OrderByDesc   │ Ordena por una clave                         │
  │ ThenBy / ThenByDesc     │ Ordenación secundaria                        │
  │ GroupBy                 │ Agrupa elementos por clave                   │
  │ Join                    │ Inner join entre dos colecciones             │
  │ GroupJoin               │ Left outer join agrupado                     │
  │ Any / All               │ Condición booleana sobre la secuencia        │
  │ Count / Sum / Min / Max │ Agregaciones numéricas                       │
  │ Average / Aggregate     │ Promedio y acumulación personalizada         │
  │ First / FirstOrDefault  │ Primer elemento (o default si no hay)        │
  │ Single / SingleOrDefault│ Único elemento (lanza si hay más de uno)     │
  │ Last / LastOrDefault    │ Último elemento                              │
  │ Skip / Take             │ Saltar y tomar elementos (paginación)        │
  │ Distinct                │ Elimina duplicados                           │
  │ Union / Intersect       │ Unión e intersección de conjuntos            │
  │ Except                  │ Diferencia de conjuntos                      │
  │ ToList / ToArray        │ Materializa la consulta (ejecución inmediata)│
  │ ToDictionary / ToLookup │ Convierte a diccionario / lookup             │
  └─────────────────────────┴──────────────────────────────────────────────┘
  """);

// ============================================================================
// MODELOS DE DATOS (deben ir al final del archivo en top-level statements)
// ============================================================================

public record Person(string Name, int Age, string City, string Language);

public class Product
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public decimal Price { get; init; }
    public string Category { get; init; } = "";
    public int Stock { get; init; }
}

public class Order
{
    public int OrderId { get; init; }
    public int ProductId { get; init; }
    public int Quantity { get; init; }
    public DateTime Date { get; init; }
}
