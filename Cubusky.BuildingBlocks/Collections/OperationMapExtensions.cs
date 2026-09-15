using System.Diagnostics.CodeAnalysis;

namespace Cubusky.BuildingBlocks.Collections;

public static class OperationMapExtensions
{
    public static bool ContainsKey<TConformance, TOperation>(this IReadOnlyOperationMap operationMap)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>
        => operationMap.TryGetValue<TConformance, TOperation>(out _);

    public static bool ContainsKey<TConformance, TOperation, TArgs>(this IReadOnlyOperationMap operationMap)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>
        => operationMap.TryGetValue<TConformance, TOperation, TArgs>(out _);

    public static OperationCallback<TConformance, TOperation> Get<TConformance, TOperation>(this IReadOnlyOperationMap operationMap)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>
        => operationMap.TryGetValue<TConformance, TOperation>(out var callback)
            ? callback
            : throw new KeyNotFoundException($"No operation callback registered for conformance {typeof(TConformance)} and operation {typeof(TOperation)}.");

    public static OperationCallback<TConformance, TOperation, TArgs> Get<TConformance, TOperation, TArgs>(this IReadOnlyOperationMap operationMap)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>
        => operationMap.TryGetValue<TConformance, TOperation, TArgs>(out var callback)
            ? callback
            : throw new KeyNotFoundException($"No operation callback registered for conformance {typeof(TConformance)} and operation {typeof(TOperation)} with arguments {typeof(TArgs)}.");

    public static OperationCallback<TConformance, TOperation>? GetValueOrDefault<TConformance, TOperation>(this IReadOnlyOperationMap operationMap)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>
        => operationMap.GetValueOrDefault<TConformance, TOperation>(default!);

    public static OperationCallback<TConformance, TOperation, TArgs>? GetValueOrDefault<TConformance, TOperation, TArgs>(this IReadOnlyOperationMap operationMap)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>
        => operationMap.GetValueOrDefault<TConformance, TOperation, TArgs>(default!);

    public static OperationCallback<TConformance, TOperation> GetValueOrDefault<TConformance, TOperation>(this IReadOnlyOperationMap operationMap, OperationCallback<TConformance, TOperation> defaultValue)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>
        => operationMap.TryGetValue<TConformance, TOperation>(out var callback)
            ? callback
            : defaultValue;

    public static OperationCallback<TConformance, TOperation, TArgs> GetValueOrDefault<TConformance, TOperation, TArgs>(this IReadOnlyOperationMap operationMap, OperationCallback<TConformance, TOperation, TArgs> defaultValue)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>
        => operationMap.TryGetValue<TConformance, TOperation, TArgs>(out var callback)
            ? callback
            : defaultValue;

    public static bool Remove<TConformance, TOperation>(this IOperationMap operationMap, [NotNullWhen(true)] out OperationCallback<TConformance, TOperation>? callback)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>
        => operationMap.TryGetValue(out callback)
        && operationMap.Remove<TConformance, TOperation>();

    public static bool Remove<TConformance, TOperation, TArgs>(this IOperationMap operationMap, [NotNullWhen(true)] out OperationCallback<TConformance, TOperation, TArgs>? callback)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>
        => operationMap.TryGetValue(out callback)
        && operationMap.Remove<TConformance, TOperation, TArgs>();
}
