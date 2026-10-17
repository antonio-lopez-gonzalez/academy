// ──────────────────────────────────────────────────────────────────────────
// UserValidator — Validador de la entidad User (SRP)
// Módulo 3: SOLID — Lección 1: Single Responsibility Principle
// ──────────────────────────────────────────────────────────────────────────
// Validación de usuarios: más compleja que otras entidades porque
// incluye reglas de formato de email, longitud de username y validación
// de password hash.
//
// NOTA: En módulos posteriores (Módulo 11: Autenticación) añadiremos
// validación de fortaleza de contraseña con BCrypt.
// ──────────────────────────────────────────────────────────────────────────

using Cms.Domain;

namespace Cms.Solid.Services;

/// <summary>
/// Validador exclusivo para la entidad User.
/// Contiene reglas de integridad de datos del usuario.
/// </summary>
public sealed class UserValidator
{
    // ──────────────────────────────────────────────────────────────────
    // Constantes de validación
    // ──────────────────────────────────────────────────────────────────
    private const int UsernameMinLength = 3;
    private const int UsernameMaxLength = 50;

    /// <summary>
    /// Valida un usuario según las reglas de negocio.
    /// </summary>
    /// <param name="user">El usuario a validar.</param>
    /// <param name="errors">Lista de errores encontrados (out).</param>
    /// <returns>true si es válido; false si hay errores.</returns>
    public bool Validate(User? user, out List<string> errors)
    {
        errors = [];

        // ── Regla 1: El usuario no puede ser null ──
        if (user is null)
        {
            errors.Add("El usuario no puede ser nulo.");
            return false;
        }

        // ── Regla 2: Username entre 3 y 50 caracteres ──
        if (string.IsNullOrWhiteSpace(user.Username))
        {
            errors.Add("El nombre de usuario (Username) es obligatorio y no puede estar vacío.");
        }
        else
        {
            if (user.Username.Length < UsernameMinLength)
            {
                errors.Add(
                    $"El nombre de usuario debe tener al menos {UsernameMinLength} caracteres. " +
                    $"Longitud actual: {user.Username.Length}.");
            }

            if (user.Username.Length > UsernameMaxLength)
            {
                errors.Add(
                    $"El nombre de usuario no puede superar los {UsernameMaxLength} caracteres. " +
                    $"Longitud actual: {user.Username.Length}.");
            }
        }

        // ── Regla 3: Email requerido y con formato básico (contiene @) ──
        // En producción se usaría una validación más completa con regex o
        // MailAddress.TryCreate, pero para este ejemplo es suficiente.
        if (string.IsNullOrWhiteSpace(user.Email))
        {
            errors.Add("El email es obligatorio y no puede estar vacío.");
        }
        else if (!user.Email.Contains('@'))
        {
            errors.Add($"El email '{user.Email}' no tiene un formato válido (debe contener '@').");
        }

        // ── Regla 4: PasswordHash no puede estar vacío ──
        // En un sistema real, el hash se genera con BCrypt antes de guardar.
        // Aquí validamos que al menos no esté vacío.
        if (string.IsNullOrWhiteSpace(user.PasswordHash))
        {
            errors.Add("El hash de la contraseña (PasswordHash) no puede estar vacío.");
        }

        // ── Regla 5: DisplayName requerido (se muestra públicamente) ──
        if (string.IsNullOrWhiteSpace(user.DisplayName))
        {
            errors.Add("El nombre público (DisplayName) es obligatorio y no puede estar vacío.");
        }

        return errors.Count == 0;
    }
}
