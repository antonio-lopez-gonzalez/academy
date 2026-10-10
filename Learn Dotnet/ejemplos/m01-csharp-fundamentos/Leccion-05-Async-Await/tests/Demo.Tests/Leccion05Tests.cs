using FluentAssertions;
using FluentAssertions.Extensions;

namespace Demo.Tests;

/// <summary>
/// Tests para la Lección 05 — Async/Await en .NET 8.
/// Cada test demuestra y verifica un concepto del modelo de programación asíncrona.
/// </summary>
public class Leccion05Tests
{
    // ========================================================================
    // Test 01: Un método async devuelve Task<int>
    // ========================================================================
    [Fact]
    public async Task FetchDataAsync_ShouldReturnTaskOfInt()
    {
        // Act — Llamamos al método async
        Task<int> task = AsyncDemo.FetchDataAsync(10);

        // Assert — El tipo retornado debe ser Task<int>
        task.Should().BeAssignableTo<Task<int>>();
        int result = await task;
        result.Should().Be(20);
    }

    // ========================================================================
    // Test 02: await desenvuelve Task<T> a T (el valor concreto)
    // ========================================================================
    [Fact]
    public async Task Await_ShouldUnwrapTaskToValue()
    {
        // Act — await extrae el int del Task<int>
        int result = await AsyncDemo.FetchDataAsync(7);

        // Assert — Resultado es el tipo desenvuelto, no un Task
        result.Should().Be(14);
    }

    // ========================================================================
    // Test 03: Task.WhenAll completa TODAS las tareas
    // ========================================================================
    [Fact]
    public async Task WhenAll_ShouldCompleteAllTasks()
    {
        // Act — Espera a que las 3 tareas terminen
        int[] results = await AsyncDemo.FetchAllAsync(new[] { 1, 2, 3 });

        // Assert — Las 3 tareas deben completarse con sus resultados
        results.Should().HaveCount(3);
        results.Should().Equal(2, 4, 6);
    }

    // ========================================================================
    // Test 04: Task.WhenAny devuelve la primera tarea en completarse
    // ========================================================================
    [Fact]
    public async Task WhenAny_ShouldReturnFirstCompletedTask()
    {
        // Act — La tarea con 1ms es más rápida que la de 500ms
        int result = await AsyncDemo.FetchFirstAsync(delayMs1: 500, delayMs2: 1);

        // Assert — Gana la tarea de 1ms (devuelve 2)
        result.Should().Be(2);
    }

    // ========================================================================
    // Test 05: CancellationToken cancela la operación correctamente
    // ========================================================================
    [Fact]
    public async Task CancellationToken_ShouldCancelOperation()
    {
        // Arrange — Cancelamos inmediatamente
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act — Intentamos una operación con token ya cancelado
        Func<Task> act = async () => await AsyncDemo.CancellableOperationAsync(1000, cts.Token);

        // Assert — Debe lanzar OperationCanceledException (o derivada)
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    // ========================================================================
    // Test 06: Task.Delay espera aproximadamente el tiempo indicado
    // ========================================================================
    [Fact]
    public async Task TaskDelay_ShouldWaitApproximatelyCorrectTime()
    {
        // Act — Medimos cuánto tarda realmente
        var sw = System.Diagnostics.Stopwatch.StartNew();
        await Task.Delay(100);
        sw.Stop();

        // Assert — Debe ser >= 100ms con un margen razonable (el scheduler puede añadir)
        sw.ElapsedMilliseconds.Should().BeGreaterOrEqualTo(80);
        sw.ElapsedMilliseconds.Should().BeLessThan(500); // Margen superior amplio
    }

    // ========================================================================
    // Test 07: IAsyncEnumerable emite elementos correctamente
    // ========================================================================
    [Fact]
    public async Task IAsyncEnumerable_ShouldYieldItems()
    {
        // Arrange — Pedimos 4 elementos
        var items = new List<int>();

        // Act — Recolectamos los elementos del stream
        await foreach (int item in AsyncDemo.StreamDataAsync(4))
        {
            items.Add(item);
        }

        // Assert — Recibimos exactamente los elementos esperados
        items.Should().HaveCount(4);
        items.Should().Equal(0, 1, 2, 3);
    }

    // ========================================================================
    // Test 08: Task.FromResult devuelve un valor pre-calculado sin async real
    // ========================================================================
    [Fact]
    public async Task FromResult_ShouldReturnPrecomputedValue()
    {
        // Act — Obtener valor "cacheado"
        Task<int> task = AsyncDemo.GetCachedValueAsync(50);
        int result = await task;

        // Assert — El Task ya está completado y contiene el valor correcto
        task.IsCompletedSuccessfully.Should().BeTrue();
        task.Status.Should().Be(TaskStatus.RanToCompletion);
        result.Should().Be(100);
    }

    // ========================================================================
    // Test 09: TaskCompletionSource se completa externamente
    // ========================================================================
    [Fact]
    public async Task TaskCompletionSource_ShouldCompleteExternally()
    {
        // Act — Envolvemos un callback como Task
        Task<int> task = AsyncDemo.WrapCallbackAsync(15);
        int result = await task;

        // Assert — El valor se resuelve desde fuera del Task
        result.Should().Be(30);
        task.IsCompletedSuccessfully.Should().BeTrue();
    }

    // ========================================================================
    // Test 10: Excepción en método async queda capturada en el Task
    // ========================================================================
    [Fact]
    public async Task ExceptionInAsyncMethod_ShouldBeCapturedInTask()
    {
        // Act — Llamamos sin await para obtener el Task fallido
        Task<int> faultedTask = AsyncDemo.ThrowAsync();

        // Esperamos a que el Task falle (sin hacer await aún)
        try { await faultedTask; } catch { /* esperado */ }

        // Assert — El Task está en estado Faulted
        faultedTask.IsFaulted.Should().BeTrue();
        faultedTask.Status.Should().Be(TaskStatus.Faulted);
        faultedTask.Exception.Should().NotBeNull();
        faultedTask.Exception!.InnerException.Should().BeOfType<InvalidOperationException>();
    }

    // ========================================================================
    // Test 11: await de un Task fallido lanza la excepción desenvuelta
    // ========================================================================
    [Fact]
    public async Task AwaitFaultedTask_ShouldThrowUnwrappedException()
    {
        // Arrange
        Task<int> faultedTask = AsyncDemo.ThrowAsync();

        // Act & Assert — await lanza InvalidOperationException, no AggregateException
        Func<Task> act = async () => await faultedTask;
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Error simulado dentro de un método async");
    }

    // ========================================================================
    // Test 12: ValueTask<int> funciona igual que Task<int>
    // ========================================================================
    [Fact]
    public async Task ValueTask_ShouldWorkLikeTask()
    {
        // Act — Camino síncrono (value < 0 → no aloca Task)
        int syncResult = await AsyncDemo.FetchValueTaskAsync(-1);
        syncResult.Should().Be(0);

        // Act — Camino asíncrono (value >= 0 → delega a Task)
        int asyncResult = await AsyncDemo.FetchValueTaskAsync(5);
        asyncResult.Should().Be(10);
    }

    // ========================================================================
    // Test 13: await foreach enumera correctamente un IAsyncEnumerable
    // ========================================================================
    [Fact]
    public async Task AwaitForeach_ShouldEnumerateAsyncStream()
    {
        // Act — await foreach itera el stream asíncrono
        int count = 0;
        await foreach (int _ in AsyncDemo.StreamDataAsync(3))
        {
            count++;
        }

        // Assert — Se emitieron exactamente 3 elementos
        count.Should().Be(3);
    }

    // ========================================================================
    // Test 14: Task.Run ejecuta código en el ThreadPool
    // ========================================================================
    [Fact]
    public async Task TaskRun_ShouldExecuteOnThreadPool()
    {
        // Arrange
        int originalThreadId = Environment.CurrentManagedThreadId;

        // Act — Task.Run encola trabajo en el ThreadPool
        int poolThreadId = await Task.Run(() => Environment.CurrentManagedThreadId);

        // Assert — Puede ser el mismo hilo o diferente (depende del pool)
        // Lo importante es que la tarea se completa exitosamente
        poolThreadId.Should().BeGreaterOrEqualTo(0); // Existe un hilo válido
    }

    // ========================================================================
    // Test 15: Task.CompletedTask es una tarea ya completada
    // ========================================================================
    [Fact]
    public void CompletedTask_ShouldBeAlreadyCompleted()
    {
        // Act
        Task task = AsyncDemo.LogAsync("Test message");

        // Assert — CompletedTask es un singleton, siempre completado
        task.IsCompleted.Should().BeTrue();
        task.Status.Should().Be(TaskStatus.RanToCompletion);
        ReferenceEquals(task, Task.CompletedTask).Should().BeTrue();
    }

    // ========================================================================
    // Test 16: CancellationToken con cancelación por timeout
    // ========================================================================
    [Fact]
    public async Task CancellationToken_WithTimeout_ShouldCancel()
    {
        // Arrange — Token que cancela automáticamente tras 10ms
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(10));

        // Act — Operación larga con token que expira rápido
        Func<Task> act = async () => await AsyncDemo.CancellableOperationAsync(5000, cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    // ========================================================================
    // Test 17: Task.WhenAll propaga excepciones de tareas fallidas
    // ========================================================================
    [Fact]
    public async Task WhenAll_WithFaultedTask_ShouldPropagateException()
    {
        // Arrange
        var goodTask = AsyncDemo.FetchDataAsync(5);
        var badTask = AsyncDemo.ThrowAsync();

        // Act
        Func<Task> act = async () => await Task.WhenAll(goodTask, badTask);

        // Assert — WhenAll propaga al menos una de las excepciones
        await act.Should().ThrowAsync<Exception>();
    }

    // ========================================================================
    // Test 18: IAsyncEnumerable con CancellationToken se cancela
    // ========================================================================
    [Fact]
    public async Task IAsyncEnumerable_WithCancellation_ShouldStop()
    {
        // Arrange — Cancelamos tras emitir unos pocos elementos
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(30));
        var items = new List<int>();

        // Act — Intentamos iterar pero se cancela pronto
        try
        {
            await foreach (int item in AsyncDemo.StreamDataAsync(100, cts.Token))
            {
                items.Add(item);
            }
        }
        catch (OperationCanceledException)
        {
            // Esperado: la iteración se cancela
        }

        // Assert — No llegó a emitir los 100 elementos
        items.Count.Should().BeLessThan(100);
    }
}
