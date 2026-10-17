// ──────────────────────────────────────────────────────────────────────────
// PostValidator — Validador de la entidad Post (SRP)
// Módulo 3: SOLID — Lección 1: Single Responsibility Principle
// ──────────────────────────────────────────────────────────────────────────
// ANTES (violación de SRP): InMemoryPostService validaba, persistía y
// gestionaba fechas TODO en una misma clase.
//
// AHORA (SRP aplicado): PostValidator tiene UNA sola razón de cambio:
// las reglas de validación del Post. Si cambian las reglas, solo se
// modifica esta clase. Ni el repositorio ni el servicio se tocan.
// ──────────────────────────────────────────────────────────────────────────

using Cms.Domain;

namespace Cms.Solid.Services;

/// <summary>
/// Responsable ÚNICAMENTE de validar las reglas de negocio de un Post.
/// No persiste, no notifica, no transforma. Solo valida.
/// </summary>
public sealed class PostValidator
{
    // ──────────────────────────────────────────────────────────────────
    // Constantes de validación (fáciles de cambiar sin buscar en el código)
    // ──────────────────────────────────────────────────────────────────
    private const int TitleMaxLength = 200;

    /// <summary>
    /// Valida un Post según las reglas de negocio del CMS.
    /// </summary>
    /// <param name="post">El post a validar. Puede ser null.</param>
    /// <param name="errors">Lista de mensajes de error (out).</param>
    /// <returns>true si el post es válido; false en caso contrario.</returns>
    public bool Validate(Post? post, out List<string> errors)
    {
        errors = [];

        // ── Regla 1: El post no puede ser null ──
        if (post is null)
        {
            errors.Add("El post no puede ser nulo.");
            return false;
        }

        // ── Regla 2: Title requerido, no vacío, máximo 200 caracteres ──
        if (string.IsNullOrWhiteSpace(post.Title))
        {
            errors.Add("El título del post es obligatorio y no puede estar vacío.");
        }
        else if (post.Title.Length > TitleMaxLength)
        {
            errors.Add(
                $"El título del post no puede superar los {TitleMaxLength} caracteres. " +
                $"Longitud actual: {post.Title.Length}.");
        }

        // ── Regla 3: Slug requerido y no vacío ──
        // El slug se usa en la URL amigable: /posts/mi-post -> slug = "mi-post"
        if (string.IsNullOrWhiteSpace(post.Slug))
        {
            errors.Add("El slug del post es obligatorio y no puede estar vacío.");
        }

        // ── Regla 4: Content requerido y no vacío ──
        // Un post sin contenido no tiene sentido publicarlo
        if (string.IsNullOrWhiteSpace(post.Content))
        {
            errors.Add("El contenido del post es obligatorio y no puede estar vacío.");
        }

        // ── Devolvemos true solo si no hay errores ──
        return errors.Count == 0;
    }
}
