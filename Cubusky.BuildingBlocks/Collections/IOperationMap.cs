using System.Diagnostics.CodeAnalysis;

namespace Cubusky.BuildingBlocks.Collections;

public interface IOperationMap
{
    void Clear();

    void Add<TConformance, TOperation>(OperationCallback<TConformance, TOperation> callback)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>;

    void Add<TConformance, TOperation, TArgs>(OperationCallback<TConformance, TOperation, TArgs> callback)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>;

    void Set<TConformance, TOperation>(OperationCallback<TConformance, TOperation> callback)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>;

    void Set<TConformance, TOperation, TArgs>(OperationCallback<TConformance, TOperation, TArgs> callback)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>;

    bool Remove<TConformance, TOperation>()
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>;

    bool Remove<TConformance, TOperation, TArgs>()
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>;

    bool TryGetValue<TConformance, TOperation>([NotNullWhen(true)] out OperationCallback<TConformance, TOperation>? callback)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>;

    bool TryGetValue<TConformance, TOperation, TArgs>([NotNullWhen(true)] out OperationCallback<TConformance, TOperation, TArgs>? callback)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>;
}