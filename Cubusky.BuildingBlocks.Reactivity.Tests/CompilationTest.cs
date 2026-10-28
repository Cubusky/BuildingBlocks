using Chickensoft.Sync;
using Cubusky.BuildingBlocks.Tests;
using Microsoft.CodeAnalysis;
using Shouldly;

namespace Cubusky.BuildingBlocks.Reactivity.Tests;

public class CompilationTest
{
    [Theory]
    [InlineData(typeof(Stat.Broadcaster))]
    [InlineData(typeof(Stat.Binding))]
    [InlineData(typeof(HitPoints.Broadcaster))]
    [InlineData(typeof(HitPoints.Binding))]
    public void Constructor_IsInaccessible(Type type)
    {
        var diagnostics = CompilationHelpers.GetDiagnostics(
            $"""
            _ = new {type.FullName!.Replace('+', '.')}(default!);
            """,
            typeof(Stat),
            typeof(SyncSubject)
        );

        var diagnostic = diagnostics.ShouldHaveSingleItem();
        diagnostic.Severity.ShouldBe(DiagnosticSeverity.Error);
        diagnostic.Id.ShouldBe("CS0122");
    }

    [Theory]
    [InlineData(nameof(Stat), nameof(Increase), true)]
    [InlineData(nameof(Stat), nameof(Heal), false)]
    [InlineData(nameof(HitPoints), nameof(Increase), true)]
    [InlineData(nameof(HitPoints), nameof(Heal), true)]
    public void Test(string autoObjectType, string operationType, bool shouldCompile)
    {
        var diagnostics = CompilationHelpers
            .GetDiagnostics(
                $$"""
                using Cubusky.BuildingBlocks.Reactivity.Tests;

                var stat = new {{autoObjectType}}();
                using var binding = stat.Bind().On((in {{operationType}} _) => { });
                """,
                typeof(Stat),
                typeof(SyncBinding),
                typeof(IOperation<>)
            )
            .Where(static diagnostic => diagnostic.Severity is DiagnosticSeverity.Warning or DiagnosticSeverity.Error);

        if (shouldCompile)
        {
            diagnostics.ShouldBeEmpty();
        }
        else
        {
            diagnostics.ShouldHaveSingleItem().Severity.ShouldBe(DiagnosticSeverity.Error);
        }
    }
}
