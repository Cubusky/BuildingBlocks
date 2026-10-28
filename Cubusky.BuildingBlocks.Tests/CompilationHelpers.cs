using Basic.Reference.Assemblies;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Cubusky.BuildingBlocks.Tests;

public class CompilationHelpers
{
    /// <summary>Compiles top-level <paramref name="source"/> and returns its errors and warnings.</summary>
    public static IReadOnlyList<Diagnostic> GetDiagnostics(string source, params IEnumerable<Type> referenceTypes)
    {
        var references = Net100.References.All.Concat(referenceTypes.Select(type => MetadataReference.CreateFromFile(type.Assembly.Location))).ToArray();

        var compilation = CSharpCompilation.Create(nameof(CompilationHelpers), [CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Preview))], references, new CSharpCompilationOptions(OutputKind.ConsoleApplication, nullableContextOptions: NullableContextOptions.Enable).WithMetadataImportOptions(MetadataImportOptions.All));

        return compilation.GetDiagnostics();
    }
}
