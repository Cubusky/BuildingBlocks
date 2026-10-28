using Cubusky.BuildingBlocks.Collections;

namespace Cubusky.BuildingBlocks.Tests
{
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
            var operationCallback = OperationMap.GetValueOrDefault<TConformance, TOperation>() ?? OperationMap.Shared.Get<TConformance, TOperation>();
            operationCallback(owner, operation);
        }

        void IOperator<First>.Perform<TOperation>(in TOperation operation) => Perform(this, in operation);
    }

    public readonly record struct FirstOp : IOperation<First>
    {
        static FirstOp() => OperationMap.Shared.Add<First, FirstOp>(Execute);

        internal static void Execute(First owner, in FirstOp operation) => owner.Value++;
    }
}
