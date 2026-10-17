using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Text;

namespace Cms.DtoGenerator;

/// <summary>
/// Source Generator que genera DTOs (Data Transfer Objects) a partir de clases
/// marcadas con el atributo [GenerateDto].
/// 
/// Para cada clase marcada, genera:
/// 1. Una clase DTO en un namespace {original}.Dtos con las mismas propiedades públicas
/// 2. Una clase de extensión con el método ToDto() para mapear la entidad al DTO
/// </summary>
[Generator]
public class DtoGenerator : IIncrementalGenerator
{
    // Formato para mostrar los nombres de tipo en el código generado.
    // MinimallyQualifiedFormat da nombres como: string, int, Guid, PostStatus, List<Post>
    private static readonly SymbolDisplayFormat TypeDisplayFormat =
        SymbolDisplayFormat.MinimallyQualifiedFormat;

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Paso 1: Encontrar todas las declaraciones de clase con atributos
        // Usamos un filtro rápido basado en sintaxis para descartar nodos que no nos interesan
        var classCandidates = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: (syntaxNode, _) => IsCandidateClass(syntaxNode),
                transform: (generatorContext, _) => GetTargetClass(generatorContext))
            .Where(classInfo => classInfo is not null);

        // Paso 2: Combinar con la compilación para verificar el atributo real
        var compilationAndClasses = context.CompilationProvider
            .Combine(classCandidates.Collect());

        // Paso 3: Generar el código fuente para cada clase encontrada
        context.RegisterSourceOutput(
            compilationAndClasses,
            (sourceProductionContext, source) =>
            {
                var (compilation, candidateClasses) = source;

                foreach (var classInfo in candidateClasses)
                {
                    if (classInfo is null) continue;

                    var sourceCode = GenerateDtoAndExtensions(classInfo);
                    var hintName = $"{classInfo.ClassName}Dto.g.cs";

                    sourceProductionContext.AddSource(hintName, sourceCode);
                }
            });
    }

    /// <summary>
    /// Filtro rápido de sintaxis: solo nos interesan clases con atributos.
    /// Esto evita llamar al modelo semántico para nodos que no lo necesitan.
    /// </summary>
    private static bool IsCandidateClass(SyntaxNode syntaxNode)
    {
        return syntaxNode is ClassDeclarationSyntax { AttributeLists.Count: > 0 };
    }

    /// <summary>
    /// Usa el modelo semántico para obtener la información completa de la clase
    /// si está marcada con [GenerateDto].
    /// </summary>
    private static TargetClassInfo? GetTargetClass(GeneratorSyntaxContext context)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;

        // Obtener el símbolo de la clase desde el modelo semántico
        if (context.SemanticModel.GetDeclaredSymbol(classDeclaration) is not INamedTypeSymbol classSymbol)
            return null;

        // Verificar si tiene el atributo [GenerateDto] o [GenerateDtoAttribute]
        var hasGenerateDtoAttribute = classSymbol.GetAttributes().Any(attr =>
            attr.AttributeClass?.Name is "GenerateDtoAttribute" or "GenerateDto");

        if (!hasGenerateDtoAttribute)
            return null;

        // Obtener el namespace de la clase original
        var namespaceName = classSymbol.ContainingNamespace.IsGlobalNamespace
            ? null
            : classSymbol.ContainingNamespace.ToDisplayString();

        // Determinar la accesibilidad de la clase origen para replicarla en el DTO
        var isPublic = classSymbol.DeclaredAccessibility == Accessibility.Public;

        // Extraer las propiedades públicas no estáticas
        var properties = new List<PropertyInfo>();
        foreach (var member in classSymbol.GetMembers())
        {
            if (member is not IPropertySymbol prop)
                continue;
            if (prop.DeclaredAccessibility != Accessibility.Public)
                continue;
            if (prop.IsStatic)
                continue;

            var typeName = prop.Type.ToDisplayString(TypeDisplayFormat);
            properties.Add(new PropertyInfo(prop.Name, typeName));
        }

        return new TargetClassInfo(
            ClassName: classSymbol.Name,
            Namespace: namespaceName ?? string.Empty,
            IsPublic: isPublic,
            Properties: properties.ToImmutableArray()
        );
    }

    /// <summary>
    /// Genera el código fuente completo: DTO + método de extensión ToDto().
    /// </summary>
    private static string GenerateDtoAndExtensions(TargetClassInfo classInfo)
    {
        var sb = new StringBuilder();

        var dtoNamespace = string.IsNullOrEmpty(classInfo.Namespace)
            ? "Dtos"
            : $"{classInfo.Namespace}.Dtos";

        var accessModifier = classInfo.IsPublic ? "public " : "";

        // === Todos los usings van al principio del archivo (norma C#) ===
        // El DTO (en {original}.Dtos) necesita resolver tipos del namespace original
        if (!string.IsNullOrEmpty(classInfo.Namespace))
        {
            sb.AppendLine($"using {classInfo.Namespace};");
        }
        // La clase de extensión (en namespace original o global) necesita resolver los DTOs
        sb.AppendLine($"using {dtoNamespace};");
        sb.AppendLine();

        // ====================
        // DTO en {original}.Dtos
        // ====================
        sb.AppendLine($"namespace {dtoNamespace}");
        sb.AppendLine("{");
        sb.AppendLine($"    {accessModifier}class {classInfo.ClassName}Dto");
        sb.AppendLine("    {");
        foreach (var prop in classInfo.Properties)
        {
            sb.AppendLine($"        public {prop.TypeName} {prop.Name} {{ get; set; }}");
        }
        sb.AppendLine("    }");
        sb.AppendLine("}");
        sb.AppendLine();

        // ====================
        // Método de extensión en el namespace ORIGINAL (o global)
        // Así es visible sin necesidad de `using Dtos;` adicional
        // ====================
        var extNamespace = string.IsNullOrEmpty(classInfo.Namespace) ? null : classInfo.Namespace;

        if (extNamespace is not null)
        {
            sb.AppendLine($"namespace {extNamespace}");
            sb.AppendLine("{");
        }

        sb.AppendLine($"    {accessModifier}static class {classInfo.ClassName}Extensions");
        sb.AppendLine("    {");
        sb.AppendLine($"        public static {classInfo.ClassName}Dto ToDto(this {classInfo.ClassName} source)");
        sb.AppendLine("        {");
        sb.AppendLine($"            return new {classInfo.ClassName}Dto");
        sb.AppendLine("            {");
        foreach (var prop in classInfo.Properties)
        {
            sb.AppendLine($"                {prop.Name} = source.{prop.Name},");
        }
        sb.AppendLine("            };");
        sb.AppendLine("        }");
        sb.AppendLine("    }");

        if (extNamespace is not null)
        {
            sb.AppendLine("}");
        }

        return sb.ToString();
    }
}

/// <summary>
/// Información de una clase objetivo para la que se genera el DTO.
/// </summary>
internal sealed record TargetClassInfo(
    string ClassName,
    string Namespace,
    bool IsPublic,
    ImmutableArray<PropertyInfo> Properties
);

/// <summary>
/// Información de una propiedad pública de la clase origen.
/// </summary>
internal sealed record PropertyInfo(string Name, string TypeName);
