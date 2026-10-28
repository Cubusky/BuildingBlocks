using Shouldly;

namespace Cubusky.BuildingBlocks.Reactivity.Tests;

public class ReactivityTest
{
    [Fact]
    public void Perform_NotifiesBoundCallback_WithLatestOperation()
    {
        int value = 0;
        var stat = new Stat();
        using var binding = stat.Bind().On((in Increase increase) => value = increase.Value);

        stat.Perform(new Increase(10));
        value.ShouldBe(10);

        stat.Perform(new Increase(5));
        value.ShouldBe(5);
    }

    [Fact]
    public void Perform_NotifiesCallbacksForEveryBroadcastOperation()
    {
        int value1 = 0;
        int value2 = 0;
        var hitPoints = new HitPoints();
        using var binding = hitPoints
            .Bind()
            .On(
                (in Increase increase) =>
                {
                    value1 = increase.Value;
                }
            )
            .On(
                (in Heal heal) =>
                {
                    value2 = heal.Value;
                }
            );

        hitPoints.Perform(new Heal(10));
        value1.ShouldBe(10);
        value2.ShouldBe(10);

        hitPoints.Perform(new Increase(5));
        value1.ShouldBe(5);
        value2.ShouldBe(10);
    }

    [Fact]
    public void Perform_WithOverriddenOperation_DoesNotNotifyCallbacks()
    {
        int value1 = 0;
        int value2 = 0;
        var hitPoints = new HitPoints();
        using var binding = hitPoints
            .Bind()
            .On(
                (in Increase increase) =>
                {
                    value1 = increase.Value;
                }
            )
            .On(
                (in Heal heal) =>
                {
                    value2 = heal.Value;
                }
            );

        hitPoints.Perform(new Heal(10));
        value1.ShouldBe(10);
        value2.ShouldBe(10);

        hitPoints.OperationMap.Set<HitPoints, Heal, HitPoints.Broadcaster>(
            (owner, in operation, broadcaster) =>
            {
                owner.Value += operation.Value;
            }
        );

        hitPoints.Perform(new Heal(5));
        value1.ShouldBe(10);
        value2.ShouldBe(10);
    }
}
