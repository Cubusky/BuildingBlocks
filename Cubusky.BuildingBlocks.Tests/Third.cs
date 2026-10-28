using Cubusky.BuildingBlocks.Collections;

namespace Cubusky.BuildingBlocks.Tests
{
    public interface IThird : ISecond, IOperator<Third>
    {
        int Value3 { get; }

        new void Perform<TOperation>(in TOperation operation)
            where TOperation : struct, IOperation<Third> => ((IOperator<Third>)this).Perform(in operation);
    }

    public class Third : Second, IThird
    {
        public int Value3 { get; set; }

        void IOperator<Third>.Perform<TOperation>(in TOperation operation) => Perform(this, in operation);
    }

    public readonly record struct ThirdOp : IOperation<Third>
    {
        static ThirdOp() => OperationMap.Shared.Add<Third, ThirdOp>(Execute);

        internal static void Execute(Third owner, in ThirdOp operation) => owner.Value3++;
    }
}
