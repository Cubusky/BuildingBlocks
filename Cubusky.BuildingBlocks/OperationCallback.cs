namespace Cubusky.BuildingBlocks;

public delegate void OperationCallback<in TConformance, TOperation>(TConformance owner, in TOperation operation)
    where TConformance : class
    where TOperation : struct, IOperation<TConformance>;

//public delegate void OperationCallback<in TConformance, TOperation, TArgs>(TConformance owner, in TOperation operation, in TArgs args)
//    where TOperation : struct, IOperation<TConformance>;
