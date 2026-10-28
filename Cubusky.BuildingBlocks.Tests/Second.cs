using Cubusky.BuildingBlocks.Collections;

namespace Cubusky.BuildingBlocks.Tests
{
    public interface ISecond : IFirst, IOperator<Second>
    {
        int Value2 { get; }

        new void Perform<TOperation>(in TOperation operation)
            where TOperation : struct, IOperation<Second> => ((IOperator<Second>)this).Perform(in operation);
    }

    public class Second : First, ISecond
    {
        public int Value2 { get; set; }

        void IOperator<Second>.Perform<TOperation>(in TOperation operation) => Perform(this, in operation);
    }

    public readonly record struct SecondOp : IOperation<Second>
    {
        static SecondOp() => OperationMap.Shared.Add<Second, SecondOp>(Execute);

        internal static void Execute(Second owner, in SecondOp operation) => owner.Value2++;
    }
}
