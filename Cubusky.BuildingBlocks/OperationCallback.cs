namespace Cubusky.BuildingBlocks;

/// <summary>
/// Represents a callback delegate for an operation that can be performed on a conformance type.
/// </summary>
/// <inheritdoc cref="OperationCallback{TConformance,TOperation,TArgs}"/>
public delegate void OperationCallback<in TConformance, TOperation>(TConformance owner, in TOperation operation)
    where TConformance : class
    where TOperation : struct, IOperation<TConformance>;

/// <summary>
/// Represents a callback delegate for an operation that can be performed on a conformance type, with optional arguments.
/// </summary>
/// <typeparam name="TConformance">The type that conforms to the operation.</typeparam>
/// <typeparam name="TOperation">The type of the operation.</typeparam>
/// <typeparam name="TArgs">The type of the optional arguments.</typeparam>
/// <param name="owner">The instance of the conformance type on which the operation is performed.</param>
/// <param name="operation">The operation to be performed.</param>
/// <param name="args">The optional arguments for the operation.</param>
public delegate void OperationCallback<in TConformance, TOperation, in TArgs>(TConformance owner, in TOperation operation, TArgs args)
    where TConformance : class
    where TOperation : struct, IOperation<TConformance>;
