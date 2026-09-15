using System.Diagnostics.CodeAnalysis;

namespace Cubusky.BuildingBlocks.Collections;

public interface IOperationMap
{
    void Set<TConformance, TOperation>(OperationCallback<TConformance, TOperation> callback)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>;

    void Add<TConformance, TOperation>(OperationCallback<TConformance, TOperation> callback)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>;

    void Clear();

    bool ContainsKey<TConformance, TOperation>()
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>;

    bool Remove<TConformance, TOperation>()
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>;

    bool TryGetValue<TConformance, TOperation>([NotNullWhen(true)] out OperationCallback<TConformance, TOperation>? callback)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>;
}
