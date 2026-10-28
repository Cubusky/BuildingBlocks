using Cubusky.BuildingBlocks.Collections;
using Shouldly;

namespace Cubusky.BuildingBlocks.Tests.Collections;

public class OperationMapTest
{
    internal static void First_FirstOp(First owner, in FirstOp operation) => owner.Value++;

    internal static void First_FirstOp_A(First owner, in FirstOp operation, A a) => owner.Value++;

    internal class A;

    internal class B : A;

    [Fact]
    public void Add_Duplicate_Throws()
    {
        var operationMap = new OperationMap();
        operationMap.Add<First, FirstOp>(First_FirstOp);

        Should.Throw<ArgumentException>(() => operationMap.Add<First, FirstOp>(First_FirstOp));
    }

    [Fact]
    public void Set_Overwrites()
    {
        OperationCallback<First, FirstOp> original = First_FirstOp;
        OperationCallback<First, FirstOp> replacement = (First owner, in FirstOp operation) => owner.Value += 10;

        var operationMap = new OperationMap();
        operationMap.Add(original);
        operationMap.Set(replacement);

        operationMap.Count.ShouldBe(1);
        operationMap.Get<First, FirstOp>().ShouldBeSameAs(replacement);
    }

    [Fact]
    public void Remove_And_Clear()
    {
        var operationMap = new OperationMap();
        operationMap.Add<First, FirstOp>(First_FirstOp);
        operationMap.Add<First, FirstOp, A>(First_FirstOp_A);
        operationMap.Count.ShouldBe(2);

        operationMap.Remove<First, FirstOp>().ShouldBeTrue();
        operationMap.Remove<First, FirstOp>().ShouldBeFalse();
        operationMap.Count.ShouldBe(1);

        operationMap.Clear();
        operationMap.Count.ShouldBe(0);
    }

    [Fact]
    public void Get_Unregistered_Throws()
    {
        var operationMap = new OperationMap();

        operationMap.TryGetValue<First, FirstOp>(out _).ShouldBeFalse();
        Should.Throw<KeyNotFoundException>(() => operationMap.Get<First, FirstOp>());
        Should.Throw<KeyNotFoundException>(() => operationMap.Get<First, FirstOp, A>());
    }

    [Fact]
    public void Get_DoesNotResolveFromDerivedRegistration()
    {
        var operationMap = new OperationMap();
        operationMap.Add((Second owner, in SecondOp operation) => owner.Value++);

        operationMap.TryGetValue<First, FirstOp>(out _).ShouldBeFalse();
        operationMap.TryGetValue<Third, SecondOp>(out var callback).ShouldBeTrue();
        callback.ShouldNotBeNull();
    }

    [Fact]
    public void Get_ResolvesRegisteredAndBaseConformanceCallbacks()
    {
        OperationCallback<First, FirstOp> first_firstOp = First_FirstOp;
        OperationCallback<First, FirstOp, A> first_firstOp_a = First_FirstOp_A;

        var operationMap = new OperationMap();
        operationMap.Add(first_firstOp);
        operationMap.Add(first_firstOp_a);

        operationMap.Get<First, FirstOp>().ShouldBeSameAs(first_firstOp);
        operationMap.Get<First, FirstOp, A>().ShouldBeSameAs(first_firstOp_a);

        operationMap.Get<Third, FirstOp>().ShouldBeSameAs(first_firstOp);
        operationMap.Get<Third, FirstOp, B>().ShouldBeSameAs(first_firstOp_a);
    }

    [Fact]
    public void Get_WithArgs_FallsBackToVoidCallback()
    {
        var operationMap = new OperationMap();
        operationMap.Add<First, FirstOp>(First_FirstOp);

        var first = new First();
        operationMap.Get<First, FirstOp, A>()(first, new FirstOp(), new A());

        first.Value.ShouldBe(1);
    }

    [Fact]
    public void Get_WithArgs_ResolvesBaseArgsType()
    {
        OperationCallback<First, FirstOp, A> callback = First_FirstOp_A;
        var operationMap = new OperationMap();
        operationMap.Add(callback);

        operationMap.Get<First, FirstOp, B>().ShouldBeSameAs(callback);
    }

    [Fact]
    public void Get_WithArgs_PrefersArgsCallbackOverVoidCallback()
    {
        OperationCallback<First, FirstOp, A> callback = First_FirstOp_A;
        var operationMap = new OperationMap();
        operationMap.Add<First, FirstOp>(First_FirstOp);
        operationMap.Add(callback);

        operationMap.Get<First, FirstOp, A>().ShouldBeSameAs(callback);
    }
}
