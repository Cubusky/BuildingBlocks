namespace Cubusky.BuildingBlocks;

/// <summary>
/// Represents an operator that can perform operations on a conformance type.
/// </summary>
/// <inheritdoc cref="OperationCallback{TConformance,TOperation}"/>
public interface IOperator<out TConformance>
    where TConformance : class
{
    /// <inheritdoc cref="OperatorExtensions.Perform{TConformance, TOperation}(TConformance, in TOperation)"/>
    void Perform<TOperation>(in TOperation operation)
        where TOperation : struct, IOperation<TConformance>;
}

/// <summary>
/// Provides extension methods for the <see cref="IOperator{TConformance}"/> interface.
/// </summary>
public static class OperatorExtensions
{
    /// <summary>
    /// Performs the specified operation on the conformance type.
    /// </summary>
    /// <inheritdoc cref="OperationCallback{TConformance,TOperation}"/>
    public static void Perform<TConformance, TOperation>(this TConformance owner, in TOperation operation)
        where TConformance : class, IOperator<TConformance>
        where TOperation : struct, IOperation<TConformance>
    {
        owner.Perform(in operation);
    }
}
