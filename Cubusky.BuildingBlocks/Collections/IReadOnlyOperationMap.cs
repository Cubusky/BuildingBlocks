using System.Diagnostics.CodeAnalysis;

namespace Cubusky.BuildingBlocks.Collections;

public interface IReadOnlyOperationMap
{
    int Count { get; }

    bool ContainsKey<TConformance, TOperation>()
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>;

    OperationCallback<TConformance, TOperation> Get<TConformance, TOperation>()
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>;

    bool TryGetValue<TConformance, TOperation>([NotNullWhen(true)] out OperationCallback<TConformance, TOperation>? callback)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>;
}
