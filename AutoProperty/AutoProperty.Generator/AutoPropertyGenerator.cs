using System.Net.Http.Headers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoProperty.Generator;

[Generator]
public class AutoPropertyGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Create a simple filter to find classes that might implement interfaces
        // change the return type to var for simplicity. 
        // Will be IncrementalValuesProvider<(string, string)?>
        var pipeline = context.SyntaxProvider.CreateSyntaxProvider(
                predicate: NodeIsEligibleForGeneration,
                transform: TransformNode)
            .Where(static x => x != null) // filter out null
            .Select(static (x, _) =>
                x); // force result to the nullable value so GenerateOutput doesn't receive null parameters

        context.RegisterSourceOutput(pipeline, GenerateOutput);
    }


    private static bool NodeIsEligibleForGeneration(SyntaxNode node, CancellationToken _)
        => node is ClassDeclarationSyntax {BaseList.Types.Count: > 0};

    private static ClassToGenerate TransformNode(
        GeneratorSyntaxContext generatorContext,
        CancellationToken cancellationToken)
    {
        var classDeclaration = (ClassDeclarationSyntax) generatorContext.Node;

        var symbol = generatorContext.SemanticModel.GetDeclaredSymbol(classDeclaration, cancellationToken);
        // Classes (and interfaces) are represented as an INamedTypeSymbol in the model
        // INamedTypeSymbol is conceptually similar to System.Type
        if (symbol is not INamedTypeSymbol classSymbol)
        {
            // this shouldn't happen, given we're filtering to ClassDeclarations. But having it puts a safe guard in place so the generator does fail/crash.
            return null;
        }

        // Get a list of all the properties implemented by the class.
        var classProperties = classSymbol.GetMembers().OfType<IPropertySymbol>().ToList();

        // Get all the interfaces implemented by the class, and all properties from each interface.
        var interfaceProperties =
            classSymbol.AllInterfaces.SelectMany(i => i.GetMembers().OfType<IPropertySymbol>()).ToList();

        // Add the unimplemented properties in the return result with the implementation format of:
        // public Type Name { get; set; } // or { get; init; }
        var unimplementedProperties = interfaceProperties
            .Where(i => !classProperties.Exists(c => c.Name == i.Name))
            .Select(i =>
            {
                var getAccessor = i.GetMethod != null ? "get;" : "";
                var setAccessor = i.SetMethod != null ? "set;" : "";

                setAccessor = i.SetMethod?.IsInitOnly ?? false ? "init;" : setAccessor;

                return $"public {i.Type} {i.Name} {{ {getAccessor} {setAccessor} }}";
            })
            .ToArray();

        // Add the unimplemented properties in the return result
        return new ClassToGenerate()
        {
            NamespaceName = classSymbol.ContainingNamespace.ToDisplayString(),
            ClassName = classSymbol.Name,
            Properties = unimplementedProperties
        };
    }

    private static void GenerateOutput(SourceProductionContext context,
        ClassToGenerate classToGenerate)
    {
        var properties = string.Join("\n\n\t\t\t", classToGenerate.Properties);

        var sourceText = $@"/// <auto-generated>
namespace {classToGenerate.NamespaceName}
{{
    partial class {classToGenerate.ClassName}
    {{
        {properties}
    }}
}}";

        context.AddSource($"{classToGenerate.NamespaceName}.{classToGenerate.ClassName}.g.cs", sourceText);
    }
}