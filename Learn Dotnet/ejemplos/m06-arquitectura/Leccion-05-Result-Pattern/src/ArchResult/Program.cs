// =============================================================================
// Módulo 06 — Arquitectura y organización
// Lección 05 — Result Pattern (Result<T>)
// =============================================================================
// PROBLEMA: Usar excepciones para flujo de control
//
//   try {
//       User user = userService.CreateUser(username, email);
//   } catch (ValidationException ex) {
//       // Manejar error de validación
//   } catch (DuplicateUserException ex) {
//       // Manejar error de duplicado
//   }
//
// ❌ Las excepciones son CARAS (capturan stack trace, desenrollan la pila)
// ❌ El flujo del programa es difícil de seguir (salta de un lado a otro)
// ❌ No queda claro qué puede fallar sin mirar la implementación
// ❌ El compilador no te obliga a manejar los errores (a diferencia de checked exceptions)
//
// SOLUCIÓN: Result<T> Pattern
//
//   Result<User> result = userService.CreateUser(username, email);
//   result.Switch(
//       success: user => Console.WriteLine($"Creado: {user.Name}"),
//       failure: error => Console.WriteLine($"Error: {error}")
//   );
//
// ✅ El tipo de retorno documenta explícitamente que puede fallar
// ✅ El compilador te "obliga" a comprobar IsSuccess antes de usar Value
// ✅ Sin coste de excepciones (sin stack trace, sin unwind)
// ✅ El flujo es lineal y predecible
// ✅ Fácil de encadenar (railway-oriented programming)
//
// COMPARACIÓN CON LIBRERÍAS POPULARES:
//   • ErrorOr (NuGet): Result<T> con múltiples errores tipados
//   • FluentResults: Result<T> con errores, éxito, y razones
//   • OneOf: Discriminated unions para C#
//   • LanguageExt: Functional programming para C# (Option, Either, etc.)
//
// En esta lección implementamos nuestro propio Result<T> desde cero para entenderlo.
// =============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

// ═══════════════════════════════════════════════════════════════════
// IMPLEMENTACIÓN DE Result<T> DESDE CERO
// ═══════════════════════════════════════════════════════════════════
namespace ArchResult
{
    /// <summary>
    /// Representa el resultado de una operación que puede fallar.
    /// Puede estar en dos estados: Success (con valor) o Failure (con error).
    /// </summary>
    public sealed class Result<T>
    {
        /// <summary>
        /// Indica si la operación fue exitosa.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Indica si la operación falló.
        /// </summary>
        public bool IsFailure => !IsSuccess;

        /// <summary>
        /// El valor en caso de éxito. Null si es Failure.
        /// </summary>
        public T? Value { get; }

        /// <summary>
        /// El mensaje de error en caso de fallo. Null si es Success.
        /// </summary>
        public string? Error { get; }

        // Constructor privado: solo se crea a través de Success() o Failure()
        private Result(T? value, bool isSuccess, string? error)
        {
            Value = value;
            IsSuccess = isSuccess;
            Error = error;
        }

        /// <summary>
        /// Crea un Result exitoso con un valor.
        /// </summary>
        public static Result<T> Success(T value) =>
            new(value, true, null);

        /// <summary>
        /// Crea un Result fallido con un mensaje de error.
        /// </summary>
        public static Result<T> Failure(string error) =>
            new(default, false, error ?? throw new ArgumentNullException(nameof(error)));

        /// <summary>
        /// Ejecuta una acción según sea Success o Failure.
        /// Pattern matching manual.
        /// </summary>
        public void Switch(Action<T> onSuccess, Action<string> onFailure)
        {
            if (IsSuccess)
                onSuccess(Value!);
            else
                onFailure(Error!);
        }

        /// <summary>
        /// Transforma el resultado aplicando una función al valor si es Success.
        /// Equivalente a Map en programación funcional.
        /// </summary>
        public Result<TNew> Map<TNew>(Func<T, TNew> mapper)
        {
            if (IsFailure)
                return Result<TNew>.Failure(Error!);

            return Result<TNew>.Success(mapper(Value!));
        }

        /// <summary>
        /// Encadena operaciones que también devuelven Result.
        /// Equivalente a Bind/FlatMap en programación funcional.
        /// </summary>
        public Result<TNew> Bind<TNew>(Func<T, Result<TNew>> binder)
        {
            if (IsFailure)
                return Result<TNew>.Failure(Error!);

            return binder(Value!);
        }

        /// <summary>
        /// Intenta obtener el valor, lanzando excepción si es Failure.
        /// Útil en Composition Root o cuando sabes seguro que es Success.
        /// </summary>
        public T GetValueOrThrow()
        {
            if (IsFailure)
                throw new InvalidOperationException($"No se puede obtener el valor de un Result fallido: {Error}");
            return Value!;
        }

        public override string ToString() =>
            IsSuccess ? $"Success({Value})" : $"Failure({Error})";
    }

    /// <summary>
    /// Result sin valor (para operaciones void: Delete, Update, etc.).
    /// </summary>
    public sealed class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public string? Error { get; }

        private Result(bool isSuccess, string? error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new(true, null);
        public static Result Failure(string error) =>
            new(false, error ?? throw new ArgumentNullException(nameof(error)));

        public override string ToString() =>
            IsSuccess ? "Success" : $"Failure({Error})";
    }
}

// ═══════════════════════════════════════════════════════════════════
// DOMINIO DE EJEMPLO: Usuarios
// ═══════════════════════════════════════════════════════════════════
namespace ArchResult.Domain
{
    /// <summary>
    /// Entidad de dominio: User.
    /// </summary>
    public sealed class User
    {
        public Guid Id { get; }
        public string Username { get; }
        public string Email { get; }
        public DateTime CreatedAt { get; }

        public User(Guid id, string username, string email)
        {
            Id = id;
            Username = username;
            Email = email;
            CreatedAt = DateTime.UtcNow;
        }

        public override string ToString() => $"User({Id.ToString()[..8]}, {Username}, {Email})";
    }
}

// ═══════════════════════════════════════════════════════════════════
// SERVICIO CON Result<T> (SIN EXCEPCIONES)
// Todas las operaciones devuelven Result<T> en lugar de lanzar excepciones.
// ═══════════════════════════════════════════════════════════════════
namespace ArchResult.Services
{
    using ArchResult.Domain;

    /// <summary>
    /// Servicio de usuarios que usa Result<T> en lugar de excepciones.
    /// El tipo de retorno documenta explícitamente que la operación puede fallar.
    /// </summary>
    public sealed class UserService
    {
        // Simulamos una base de datos en memoria
        private readonly HashSet<string> _usedUsernames = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _usedEmails = new(StringComparer.OrdinalIgnoreCase);
        private readonly List<User> _users = new();

        /// <summary>
        /// Crea un usuario. Devuelve Result<User> con el usuario o un error.
        /// NUNCA lanza excepciones para flujo de control.
        /// </summary>
        public Result<User> CreateUser(string username, string email)
        {
            // 1. Validar username
            if (string.IsNullOrWhiteSpace(username))
                return Result<User>.Failure("El nombre de usuario es obligatorio.");

            if (username.Length < 3)
                return Result<User>.Failure("El nombre de usuario debe tener al menos 3 caracteres.");

            if (username.Length > 50)
                return Result<User>.Failure("El nombre de usuario no puede superar los 50 caracteres.");

            // 2. Validar email
            if (string.IsNullOrWhiteSpace(email))
                return Result<User>.Failure("El email es obligatorio.");

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return Result<User>.Failure($"El email '{email}' no tiene un formato válido.");

            // 3. Verificar unicidad
            if (!_usedUsernames.Add(username))
                return Result<User>.Failure($"El nombre de usuario '{username}' ya está en uso.");

            if (!_usedEmails.Add(email))
                return Result<User>.Failure($"El email '{email}' ya está registrado.");

            // 4. Crear el usuario
            User user = new(Guid.NewGuid(), username, email.ToLowerInvariant());
            _users.Add(user);

            return Result<User>.Success(user);
        }

        /// <summary>
        /// Busca un usuario por email. Devuelve Result<User>.
        /// </summary>
        public Result<User> GetByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return Result<User>.Failure("El email es obligatorio.");

            User? user = _users.FirstOrDefault(u =>
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

            if (user is null)
                return Result<User>.Failure($"No se encontró un usuario con el email '{email}'.");

            return Result<User>.Success(user);
        }

        /// <summary>
        /// Elimina un usuario. Devuelve Result (sin valor).
        /// </summary>
        public Result DeleteUser(Guid userId)
        {
            User? user = _users.FirstOrDefault(u => u.Id == userId);

            if (user is null)
                return Result.Failure($"No se encontró un usuario con ID '{userId}'.");

            _users.Remove(user);
            _usedUsernames.Remove(user.Username);
            _usedEmails.Remove(user.Email);

            return Result.Success();
        }
    }
}

// ═══════════════════════════════════════════════════════════════════
// EJEMPLO: Servicio que compone múltiples Result
// ═══════════════════════════════════════════════════════════════════
namespace ArchResult.Services
{
    using ArchResult.Domain;

    /// <summary>
    /// Servicio de registro que compone UserService con lógica adicional.
    /// Demuestra cómo encadenar Result<T> con Bind/Map.
    /// </summary>
    public sealed class RegistrationService
    {
        private readonly UserService _userService;

        public RegistrationService(UserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Registra un usuario y devuelve un mensaje de bienvenida.
        /// Usa Bind para encadenar operaciones: CreateUser → Generar mensaje.
        /// </summary>
        public Result<string> RegisterAndWelcome(string username, string email)
        {
            // Encadenamos: crear usuario → si éxito, generar mensaje de bienvenida
            Result<User> createResult = _userService.CreateUser(username, email);

            return createResult.Map(user =>
                $"¡Bienvenido/a {user.Username}! Tu cuenta ha sido creada con el email {user.Email}.");
        }
    }
}

// ═══════════════════════════════════════════════════════════════════
// COMPARATIVA: Excepciones vs Result<T>
// ═══════════════════════════════════════════════════════════════════
namespace ArchResult.Comparison
{
    using ArchResult.Domain;

    /// <summary>
    /// Servicio MALO: usa excepciones para flujo de control.
    /// </summary>
    public sealed class BadUserService
    {
        private readonly HashSet<string> _usedUsernames = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// ❌ MAL: lanza excepciones para errores esperados.
        /// </summary>
        public User CreateUserWithExceptions(string username, string email)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("El nombre de usuario es obligatorio.");

            if (username.Length < 3)
                throw new ArgumentException("Nombre demasiado corto.");

            if (!_usedUsernames.Add(username))
                throw new InvalidOperationException($"Username '{username}' ya existe.");

            // Si llegamos aquí, todo bien
            return new User(Guid.NewGuid(), username, email);
        }
    }
}

// =========================================================================
// DEMO PRINCIPAL
// =========================================================================
namespace ArchResult
{
    using ArchResult.Domain;
    using ArchResult.Services;
    using ArchResult.Comparison;

    public static class Program
    {
        public static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║  Módulo 06 — Lección 05: Result Pattern     ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝");
            Console.WriteLine();

            UserService userService = new();

            // ─── DEMO 1: Crear usuario exitosamente ───
            Console.WriteLine("─── 1. CREAR USUARIO (éxito) ───");
            Console.WriteLine();

            Result<User> result1 = userService.CreateUser("maria_dev", "maria@example.com");
            Console.WriteLine($"  Result: {result1}");

            result1.Switch(
                onSuccess: user =>
                {
                    Console.WriteLine($"  ✅ ¡Usuario creado! ID: {user.Id}");
                    Console.WriteLine($"     Username: {user.Username}");
                    Console.WriteLine($"     Email: {user.Email}");
                    Console.WriteLine($"     Creado: {user.CreatedAt:u}");
                },
                onFailure: error => Console.WriteLine($"  ❌ Error: {error}")
            );

            // ─── DEMO 2: Errores de validación ───
            Console.WriteLine();
            Console.WriteLine("─── 2. ERRORES DE VALIDACIÓN ───");
            Console.WriteLine();

            // Username vacío
            Result<User> result2 = userService.CreateUser("", "test@example.com");
            Console.WriteLine($"  Username vacío → {result2}");

            // Username muy corto
            Result<User> result3 = userService.CreateUser("ab", "test@example.com");
            Console.WriteLine($"  Username corto → {result3}");

            // Email inválido
            Result<User> result4 = userService.CreateUser("valid_user", "esto-no-es-email");
            Console.WriteLine($"  Email inválido → {result4}");

            // ─── DEMO 3: Duplicados ───
            Console.WriteLine();
            Console.WriteLine("─── 3. DUPLICADOS ───");
            Console.WriteLine();

            Result<User> duplicate = userService.CreateUser("maria_dev", "otro@example.com");
            Console.WriteLine($"  Username duplicado → {duplicate}");

            Result<User> duplicateEmail = userService.CreateUser("otro_user", "maria@example.com");
            Console.WriteLine($"  Email duplicado → {duplicateEmail}");

            // ─── DEMO 4: Pattern matching con switch expression ───
            Console.WriteLine();
            Console.WriteLine("─── 4. PATTERN MATCHING CON Result<T> ───");
            Console.WriteLine();

            Result<User> result5 = userService.CreateUser("carlos_dev", "carlos@example.com");

            // Switch expression (más conciso que Switch)
            string message = result5 switch
            {
                { IsSuccess: true, Value: User u } => $"✅ Creado: {u.Username} ({u.Email})",
                { IsFailure: true, Error: string e } => $"❌ Error: {e}",
                _ => "Estado desconocido"
            };
            Console.WriteLine($"  {message}");

            // ─── DEMO 5: MAP (transformar el valor) ───
            Console.WriteLine();
            Console.WriteLine("─── 5. MAP (transformar Result) ───");
            Console.WriteLine();

            Result<User> userResult = userService.GetByEmail("carlos@example.com");

            // Map transforma el valor si es Success, mantiene el error si es Failure
            Result<string> mappedResult = userResult.Map(u => $"Hola, {u.Username}!");

            Console.WriteLine($"  Original: {userResult}");
            Console.WriteLine($"  Mapeado:  {mappedResult}");

            // Map sobre un Failure no ejecuta la función
            Result<User> notFound = userService.GetByEmail("noexiste@example.com");
            Result<string> mappedNotFound = notFound.Map(u => $"Hola, {u.Username}!");
            Console.WriteLine($"  Búsqueda fallida: {notFound}");
            Console.WriteLine($"  Map sobre fallo:  {mappedNotFound}");

            // ─── DEMO 6: BIND (encadenar operaciones que devuelven Result) ───
            Console.WriteLine();
            Console.WriteLine("─── 6. BIND (encadenar operaciones) ───");
            Console.WriteLine();

            RegistrationService registrationService = new(userService);

            // Bind: el resultado de CreateUser se pasa a la siguiente operación
            Result<User> createdUser = userService.CreateUser("laura_dev", "laura@example.com");

            // Con Map transformamos, con Bind encadenamos otro Result
            Result<string> welcomeMessage = createdUser.Map(user =>
                $"¡Bienvenido/a {user.Username}! Tu cuenta ha sido creada.");

            Console.WriteLine($"  Mensaje de bienvenida: {welcomeMessage}");

            welcomeMessage.Switch(
                onSuccess: msg => Console.WriteLine($"  ✅ {msg}"),
                onFailure: err => Console.WriteLine($"  ❌ {err}")
            );

            // ─── DEMO 7: Result sin valor (Delete) ───
            Console.WriteLine();
            Console.WriteLine("─── 7. Result (sin valor, para Delete) ───");
            Console.WriteLine();

            Result deleteResult = userService.DeleteUser(createdUser.GetValueOrThrow().Id);
            Console.WriteLine($"  Eliminar usuario: {deleteResult}");

            Result deleteFails = userService.DeleteUser(Guid.NewGuid());
            Console.WriteLine($"  Eliminar inexistente: {deleteFails}");

            // ─── DEMO 8: COMPARATIVA EXCEPCIONES vs RESULT ───
            Console.WriteLine();
            Console.WriteLine("─── 8. EXCEPCIONES vs RESULT ───");
            Console.WriteLine();

            BadUserService badService = new();

            // ❌ Con excepciones: necesitas try-catch y no sabes qué puede fallar
            Console.WriteLine("  ❌ ESTILO CON EXCEPCIONES:");
            Console.WriteLine("  ─────────────────────────");
            try
            {
                User user1 = badService.CreateUserWithExceptions("test", "test@test.com");
                Console.WriteLine($"    Creado: {user1.Username}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"    Validation error: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"    Business error: {ex.Message}");
            }
            // ¿Y si hay más tipos de excepciones? El compilador no te avisa.

            Console.WriteLine();
            Console.WriteLine("  ✅ ESTILO CON RESULT<T>:");
            Console.WriteLine("  ────────────────────────");
            Result<User> resultUser = userService.CreateUser("test", "test@test.com");
            // El compilador te "obliga" a comprobar IsSuccess
            if (resultUser.IsSuccess)
            {
                Console.WriteLine($"    Creado: {resultUser.Value!.Username}");
            }
            else
            {
                Console.WriteLine($"    Error: {resultUser.Error}");
            }

            // =========================================================================
            // BUENAS PRÁCTICAS
            // =========================================================================
            Console.WriteLine();
            Console.WriteLine("═══════════════════════════════════════════════");
            Console.WriteLine("  BUENAS PRÁCTICAS CON Result<T>");
            Console.WriteLine("═══════════════════════════════════════════════");
            Console.WriteLine();
            Console.WriteLine("  ✅ HACER:");
            Console.WriteLine("    • Usar Result<T> para errores de dominio esperados");
            Console.WriteLine("      (validación, duplicados, no encontrado, negocio)");
            Console.WriteLine("    • Usar excepciones SOLO para errores inesperados");
            Console.WriteLine("      (conexión BD caída, out of memory, bug de código)");
            Console.WriteLine("    • Encadenar con Map y Bind para mantener el flujo limpio");
            Console.WriteLine("    • Usar librerías como ErrorOr o FluentResults en proyectos reales");
            Console.WriteLine();
            Console.WriteLine("  ❌ EVITAR:");
            Console.WriteLine("    • NO usar Result<T> para TODO (configuración, DI, errores fatales)");
            Console.WriteLine("    • NO lanzar excepciones para errores de validación");
            Console.WriteLine("    • NO ignorar IsSuccess y acceder a Value directamente");
            Console.WriteLine("    • NO hacer try-catch para control de flujo normal");
            Console.WriteLine();
            Console.WriteLine("  REGLA GENERAL:");
            Console.WriteLine("  • Si el error es ESPERADO → Result<T>");
            Console.WriteLine("  • Si el error es INESPERADO → excepción");
            Console.WriteLine("═══════════════════════════════════════════════");
            Console.WriteLine();
            Console.WriteLine("  LIBRERÍAS RECOMENDADAS:");
            Console.WriteLine("    • ErrorOr (NuGet): Result con errores tipados");
            Console.WriteLine("    • FluentResults: Result con razones y errores múltiples");
            Console.WriteLine("    • OneOf: Discriminated unions (F# style)");
            Console.WriteLine("    • LanguageExt: Programación funcional completa para C#");
        }
    }
}
