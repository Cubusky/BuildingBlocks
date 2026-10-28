using Shouldly;

namespace Cubusky.BuildingBlocks.Tests;

public class OperationTest
{
    [Fact]
    public void First_Perform_FirstOp_IncrementsValue()
    {
        var first = new First();
        first.Perform(new FirstOp());
        first.Value.ShouldBe(1);
    }

    [Fact]
    public void Second_Perform_FirstOp_IncrementsOnlyValue()
    {
        var second = new Second();
        second.Perform(new FirstOp());
        second.Value.ShouldBe(1);
        second.Value2.ShouldBe(0);
    }

    [Fact]
    public void Second_Perform_SecondOp_IncrementsOnlyValue2()
    {
        var second = new Second();
        second.Perform(new SecondOp());
        second.Value.ShouldBe(0);
        second.Value2.ShouldBe(1);
    }

    [Fact]
    public void Third_Perform_FirstOp_IncrementsOnlyValue()
    {
        var third = new Third();
        third.Perform(new FirstOp());
        third.Value.ShouldBe(1);
        third.Value2.ShouldBe(0);
        third.Value3.ShouldBe(0);
    }

    [Fact]
    public void Third_Perform_SecondOp_IncrementsOnlyValue2()
    {
        var third = new Third();
        third.Perform(new SecondOp());
        third.Value.ShouldBe(0);
        third.Value2.ShouldBe(1);
        third.Value3.ShouldBe(0);
    }

    [Fact]
    public void Third_Perform_ThirdOp_IncrementsOnlyValue3()
    {
        var third = new Third();
        third.Perform(new ThirdOp());
        third.Value.ShouldBe(0);
        third.Value2.ShouldBe(0);
        third.Value3.ShouldBe(1);
    }
}
