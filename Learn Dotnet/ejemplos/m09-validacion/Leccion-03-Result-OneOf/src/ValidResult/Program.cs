// MÓDULO 9 - LECCIÓN 03: Result<T>, OneOf, FluentResults

// ---- PROGRAMA ----
Console.WriteLine("══════════════════════════════════════════════════");
Console.WriteLine("  MÓDULO 9 - LECCIÓN 03: Result<T>, OneOf, FluentResults");
Console.WriteLine("══════════════════════════════════════════════════");

UserService service = new();

CreateUserRequest valid = new()
{
    Username = "carlos.lopez", Email = "carlos@nuevo.com",
    Password = "Password123!", ConfirmPassword = "Password123!"
};

CreateUserRequest invalid = new()
{
    Username = "ab", Email = "admin@sistema.com",
    Password = "123", ConfirmPassword = "456"
};

// A) Result<T> éxito
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("\n--- A) RESULT<T>: ÉXITO ---");
Console.ResetColor();
Result<User> ok = service.CreateUser_WithResult(valid);
Console.WriteLine($"  {ok.Match(u => $"✓ {u}", e => $"✗ {e}")}");

// B) Result<T> fracaso (primer error)
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("\n--- B) RESULT<T>: FRACASO (1er error) ---");
Console.ResetColor();
Result<User> fail = service.CreateUser_WithResult(invalid);
Console.WriteLine($"  {fail.Match(u => $"✓ {u}", e => $"✗ {e}")}");

// C) OneOf pattern matching
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("\n--- C) ONEOF: PATTERN MATCHING ---");
Console.ResetColor();
OneOf<User, ValidationError> oneOf = service.CreateUser_WithOneOf(invalid);
oneOf.Switch(
    user => Console.WriteLine($"  ✓ User: {user.Username}"),
    error => Console.WriteLine($"  ✗ {error}")
);
Console.WriteLine($"  Match: {oneOf.Match(u => $"Usuario '{u.Username}'", e => $"Error: {e}")}");
if (oneOf.TryGetT1(out ValidationError? ve))
    Console.WriteLine($"  Campo con error: {ve.Field}");

// D) Múltiples errores
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("\n--- D) MÚLTIPLES ERRORES ---");
Console.ResetColor();
Result<User> multi = service.CreateUser_WithMultipleErrors(invalid, out MultiError? allErrors);
if (multi.IsError && allErrors is not null)
{
    Console.WriteLine($"  {allErrors.Errors.Count} errores:");
    foreach (string e in allErrors.Errors) Console.WriteLine($"    • {e}");
}

// E) Excepciones
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("\n--- E) EXCEPCIONES (anti-patrón) ---");
Console.ResetColor();
try { service.CreateUser_WithExceptions(invalid); }
catch (Exception ex) { Console.WriteLine($"  ✗ {ex.GetType().Name}: {ex.Message}"); }

Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine("\n══════════════════════════════════════════════════");
Console.WriteLine("  ¿CUÁNDO USAR CADA PATRÓN?");
Console.WriteLine("══════════════════════════════════════════════════");
Console.ForegroundColor = ConsoleColor.White;
Console.WriteLine("""
  EXCEPCIONES → fallos excepcionales (red, BD caída, bugs)
  RESULT<T>   → APIs simples: éxito O un error
  ONEOF<T0,T1> → varios tipos de resultado, pattern matching
  FLUENTRESULTS → múltiples errores acumulados, metadatos

  RECOMENDACIÓN:
    Validación de requests → FluentResults (múltiples errores)
    Operaciones de dominio → Result<T> o OneOf<T,E>
    Infraestructura → Excepciones con middleware global
""");
Console.ResetColor();

// ================================================================
// TIPOS
// ================================================================

public class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsError => !IsSuccess;
    public T Value { get; }
    public string? Error { get; }

    private Result(bool isSuccess, T value, string? error)
    {
        IsSuccess = isSuccess; Value = value; Error = error;
    }

    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> Failure(string error) => new(false, default!, error);

    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<string, TResult> onError)
        => IsSuccess ? onSuccess(Value) : onError(Error!);

    public override string ToString() => IsSuccess ? $"Success({Value})" : $"Error({Error})";
}

public class OneOf<T0, T1>
{
    private readonly object _value;
    private readonly int _index;

    private OneOf(object value, int index) { _value = value; _index = index; }

    public static implicit operator OneOf<T0, T1>(T0 value) => new(value!, 0);
    public static implicit operator OneOf<T0, T1>(T1 value) => new(value!, 1);

    public void Switch(Action<T0> onT0, Action<T1> onT1)
    {
        if (_index == 0) onT0((T0)_value); else onT1((T1)_value);
    }

    public TResult Match<TResult>(Func<T0, TResult> onT0, Func<T1, TResult> onT1)
        => _index == 0 ? onT0((T0)_value) : onT1((T1)_value);

    public bool TryGetT0(out T0 value)
    {
        if (_index == 0) { value = (T0)_value; return true; }
        value = default!; return false;
    }

    public bool TryGetT1(out T1 value)
    {
        if (_index == 1) { value = (T1)_value; return true; }
        value = default!; return false;
    }
}

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public override string ToString() => $"User(Id={Id}, Username='{Username}', Email='{Email}')";
}

public class CreateUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class ValidationError
{
    public string Field { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public override string ToString() => $"{Field}: {Message}";
}

public class MultiError
{
    public List<string> Errors { get; } = new();
    public MultiError(string error) { Errors.Add(error); }
    public MultiError(IEnumerable<string> errors) { Errors.AddRange(errors); }
    public override string ToString() => string.Join(" | ", Errors);
}

public class UserService
{
    private readonly HashSet<string> _usernames = new(StringComparer.OrdinalIgnoreCase)
        { "admin", "user1", "maria.garcia" };
    private readonly HashSet<string> _emails = new(StringComparer.OrdinalIgnoreCase)
        { "admin@sistema.com", "maria@email.com" };

    public User CreateUser_WithExceptions(CreateUserRequest r)
    {
        if (string.IsNullOrWhiteSpace(r.Username))
            throw new ArgumentException("Username obligatorio");
        if (r.Username.Length < 3)
            throw new ArgumentException("Username min 3 chars");
        if (_usernames.Contains(r.Username))
            throw new InvalidOperationException("Username ya existe");
        if (string.IsNullOrWhiteSpace(r.Email))
            throw new ArgumentException("Email obligatorio");
        if (_emails.Contains(r.Email))
            throw new InvalidOperationException("Email ya registrado");
        if (r.Password != r.ConfirmPassword)
            throw new ArgumentException("Contraseñas no coinciden");
        if (r.Password.Length < 8)
            throw new ArgumentException("Contraseña min 8 chars");
        return new User { Username = r.Username, Email = r.Email };
    }

    public Result<User> CreateUser_WithResult(CreateUserRequest r)
    {
        if (string.IsNullOrWhiteSpace(r.Username)) return Result<User>.Failure("Username obligatorio");
        if (r.Username.Length < 3) return Result<User>.Failure("Username min 3 chars");
        if (_usernames.Contains(r.Username)) return Result<User>.Failure("Username ya existe");
        if (string.IsNullOrWhiteSpace(r.Email)) return Result<User>.Failure("Email obligatorio");
        if (_emails.Contains(r.Email)) return Result<User>.Failure("Email ya registrado");
        if (r.Password != r.ConfirmPassword) return Result<User>.Failure("Contraseñas no coinciden");
        if (r.Password.Length < 8) return Result<User>.Failure("Contraseña min 8 chars");
        return Result<User>.Success(new User { Username = r.Username, Email = r.Email });
    }

    public OneOf<User, ValidationError> CreateUser_WithOneOf(CreateUserRequest r)
    {
        if (string.IsNullOrWhiteSpace(r.Username))
            return new ValidationError { Field = "Username", Message = "Username obligatorio" };
        if (r.Username.Length < 3)
            return new ValidationError { Field = "Username", Message = "Min 3 caracteres" };
        if (_usernames.Contains(r.Username))
            return new ValidationError { Field = "Username", Message = "Ya existe" };
        if (string.IsNullOrWhiteSpace(r.Email))
            return new ValidationError { Field = "Email", Message = "Email obligatorio" };
        if (_emails.Contains(r.Email))
            return new ValidationError { Field = "Email", Message = "Ya registrado" };
        if (r.Password != r.ConfirmPassword)
            return new ValidationError { Field = "Password", Message = "No coinciden" };
        if (r.Password.Length < 8)
            return new ValidationError { Field = "Password", Message = "Min 8 caracteres" };
        return new User { Username = r.Username, Email = r.Email };
    }

    public Result<User> CreateUser_WithMultipleErrors(CreateUserRequest r, out MultiError? allErrors)
    {
        List<string> errors = new();
        if (string.IsNullOrWhiteSpace(r.Username)) errors.Add("Username obligatorio");
        if (r.Username.Length < 3) errors.Add("Username min 3 chars");
        if (_usernames.Contains(r.Username)) errors.Add("Username ya existe");
        if (string.IsNullOrWhiteSpace(r.Email)) errors.Add("Email obligatorio");
        if (_emails.Contains(r.Email)) errors.Add("Email ya registrado");
        if (r.Password != r.ConfirmPassword) errors.Add("Contraseñas no coinciden");
        if (r.Password.Length < 8) errors.Add("Contraseña min 8 chars");
        if (errors.Count > 0) { allErrors = new MultiError(errors); return Result<User>.Failure(string.Join("; ", errors)); }
        allErrors = null;
        return Result<User>.Success(new User { Username = r.Username, Email = r.Email });
    }
}
