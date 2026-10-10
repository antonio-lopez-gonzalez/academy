namespace Cms.DtoGenerator;

/// <summary>
/// Marca una clase para que el Source Generator genere automáticamente
/// un DTO (Data Transfer Object) con todas sus propiedades públicas
/// y un método de extensión ToDto() para mapear al DTO.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class GenerateDtoAttribute : Attribute { }
