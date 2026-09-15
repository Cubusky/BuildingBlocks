namespace Cubusky.BuildingBlocks;

public interface IOperator<out TConformance>
    where TConformance : class
{
    void Perform<TOperation>(in TOperation operation)
        where TOperation : struct, IOperation<TConformance>;
}

public static class OperatorExtensions
{
    public static void Perform<TConformance, TOperation>(this TConformance owner, in TOperation operation)
        where TConformance : class, IOperator<TConformance>
        where TOperation : struct, IOperation<TConformance>
    {
        owner.Perform(in operation);
    }
}
