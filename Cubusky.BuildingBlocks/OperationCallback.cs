namespace Cubusky.BuildingBlocks;

public delegate void OperationCallback<in TConformance, TOperation>(TConformance owner, in TOperation operation)
    where TConformance : class
    where TOperation : struct, IOperation<TConformance>;

public delegate void OperationCallback<in TConformance, TOperation, in TArgs>(TConformance owner, in TOperation operation, TArgs args)
    where TConformance : class
    where TOperation : struct, IOperation<TConformance>;
