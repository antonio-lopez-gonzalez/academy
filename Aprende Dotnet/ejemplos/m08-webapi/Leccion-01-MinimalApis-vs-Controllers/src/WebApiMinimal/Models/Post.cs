// ─────────────────────────────────────────────────────────────
// Modelo de datos compartido entre Minimal APIs y Controllers.
// ─────────────────────────────────────────────────────────────
namespace WebApiMinimal.Models;

/// <summary>
/// Representa un post del blog. Usamos record por inmutabilidad y
/// comparación por valor (ideal para DTOs y mensajes).
/// </summary>
public record Post(int Id, string Title, string Content, DateTime CreatedAt);
