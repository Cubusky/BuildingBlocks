using Cubusky.BuildingBlocks.Collections;
using Shouldly;

namespace Cubusky.BuildingBlocks.Tests;

public class OperationTest
{
    #region Object Implementations
    public interface IFirst : IOperator<First>
    {
        int Value { get; }
    }

    public class First : IFirst
    {
        public int Value { get; set; }

        public OperationMap OperationMap { get; } = new();

        protected void Perform<TConformance, TOperation>(in TConformance owner, in TOperation operation)
            where TConformance : class
            where TOperation : struct, IOperation<TConformance>
        {
            Initializer<TOperation>.Ensure();
            var operationCallback = OperationMap.GetValueOrDefault<TConformance, TOperation>()
                ?? OperationMap.Shared.Get<TConformance, TOperation>();
            operationCallback(owner, operation);
        }

        void IOperator<First>.Perform<TOperation>(in TOperation operation) => Perform(this, in operation);
    }

    public interface ISecond : IFirst, IOperator<Second>
    {
        int Value2 { get; }

        new void Perform<TOperation>(in TOperation operation)
            where TOperation : struct, IOperation<Second>
            => ((IOperator<Second>)this).Perform(in operation);
    }

    public class Second : First, ISecond
    {
        public int Value2 { get; set; }

        void IOperator<Second>.Perform<TOperation>(in TOperation operation) => Perform(this, in operation);
    }

    public interface IThird : ISecond, IOperator<Third>
    {
        int Value3 { get; }

        new void Perform<TOperation>(in TOperation operation)
            where TOperation : struct, IOperation<Third>
            => ((IOperator<Third>)this).Perform(in operation);
    }

    public class Third : Second, IThird
    {
        public int Value3 { get; set; }

        void IOperator<Third>.Perform<TOperation>(in TOperation operation) => Perform(this, in operation);
    }
    #endregion

    #region Operation Implementations
    public readonly record struct FirstOp : IOperation<First>
    {
        static FirstOp()
        {
            OperationMap.Shared.Add<First, FirstOp>(Execute);
            OperationMap.Shared.Add<Second, FirstOp>(SecondOverride);
        }

        internal static void Execute(First owner, in FirstOp operation) => owner.Value++;

        internal static void SecondOverride(Second owner, in FirstOp operation) => owner.Value2--;
    }

    public readonly record struct SecondOp : IOperation<Second>
    {
        static SecondOp() => OperationMap.Shared.Add<Second, SecondOp>(Execute);

        internal static void Execute(Second owner, in SecondOp operation) => owner.Value2++;
    }
    #endregion

    #region Usage Implementations
    [Fact]
    public void First_FirstOp()
    {
        var first = new First();
        first.Perform(new FirstOp());
        first.Value.ShouldBe(1);
    }

    [Fact]
    public void Second_FirstOp()
    {
        var second = new Second();
        second.Perform(new FirstOp());
        second.Value2.ShouldBe(-1);
    }

    [Fact]
    public void Second_SecondOp()
    {
        var second = new Second();
        second.Perform(new SecondOp());
        second.Value2.ShouldBe(1);
    }

    [Fact]
    public void IFirst_FirstOp()
    {
        IFirst first = new First();
        first.Perform(new FirstOp());
        first.Value.ShouldBe(1);
    }

    [Fact]
    public void ISecond_FirstOp()
    {
        ISecond second = new Second();
        second.Perform(new FirstOp());
        second.Value2.ShouldBe(-1);
    }

    [Fact]
    public void ISecond_SecondOp()
    {
        ISecond second = new Second();
        second.Perform(new SecondOp());
        second.Value2.ShouldBe(1);
    }
    #endregion
}
