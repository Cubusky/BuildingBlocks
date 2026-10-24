namespace Cubusky.BuildingBlocks;

/// <summary>
/// Represents an operation that can be performed on a conformance type.
/// </summary>
/// <inheritdoc cref="OperationCallback{TConformance,TOperation}"/>
public interface IOperation<in TConformance>
    where TConformance : class;
