using System.Diagnostics.CodeAnalysis;

namespace Cubusky.BuildingBlocks.Collections;

public static class OperationMapExtensions
{
    public static OperationCallback<TConformance, TOperation>? GetValueOrDefault<TConformance, TOperation>(this IReadOnlyOperationMap operationMap)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>
        => operationMap.GetValueOrDefault<TConformance, TOperation>(default!);

    public static OperationCallback<TConformance, TOperation> GetValueOrDefault<TConformance, TOperation>(this IReadOnlyOperationMap operationMap, OperationCallback<TConformance, TOperation> defaultValue)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>
        => operationMap.TryGetValue<TConformance, TOperation>(out var callback)
            ? callback
            : defaultValue;

    public static bool Remove<TConformance, TOperation>(this IOperationMap operationMap, [NotNullWhen(true)] out OperationCallback<TConformance, TOperation>? callback)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>
        => operationMap.TryGetValue(out callback)
        && operationMap.Remove<TConformance, TOperation>();

    public static bool TryAdd<TConformance, TOperation>(this IOperationMap operationMap, OperationCallback<TConformance, TOperation> callback)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>
    {
        if (operationMap.ContainsKey<TConformance, TOperation>())
        {
            return false;
        }

        operationMap.Add(callback);
        return true;
    }
}
