using Microsoft.CodeAnalysis;
using Shouldly;

namespace Cubusky.BuildingBlocks.Tests;

public class CompilationTest
{
    [Theory]
    [InlineData(nameof(First), nameof(FirstOp), true)]
    [InlineData(nameof(First), nameof(SecondOp), false)]
    [InlineData(nameof(First), nameof(ThirdOp), false)]
    [InlineData(nameof(Second), nameof(FirstOp), true)]
    [InlineData(nameof(Second), nameof(SecondOp), true)]
    [InlineData(nameof(Second), nameof(ThirdOp), false)]
    [InlineData(nameof(Third), nameof(FirstOp), true)]
    [InlineData(nameof(Third), nameof(SecondOp), true)]
    [InlineData(nameof(Third), nameof(ThirdOp), true)]
    [InlineData(nameof(IFirst), nameof(FirstOp), true)]
    [InlineData(nameof(IFirst), nameof(SecondOp), false)]
    [InlineData(nameof(IFirst), nameof(ThirdOp), false)]
    [InlineData(nameof(ISecond), nameof(FirstOp), true)]
    [InlineData(nameof(ISecond), nameof(SecondOp), true)]
    [InlineData(nameof(ISecond), nameof(ThirdOp), false)]
    [InlineData(nameof(IThird), nameof(FirstOp), true)]
    [InlineData(nameof(IThird), nameof(SecondOp), true)]
    [InlineData(nameof(IThird), nameof(ThirdOp), true)]
    public void Perform_CompilesOnlyForSupportedOperations(string operatorType, string operationType, bool shouldCompile)
    {
        var diagnostics = CompilationHelpers
            .GetDiagnostics(
                $"""
                using Cubusky.BuildingBlocks;
                using Cubusky.BuildingBlocks.Tests;

                default({operatorType})!.Perform(default({operationType}));
                """,
                typeof(IOperation<>),
                typeof(CompilationTest)
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
