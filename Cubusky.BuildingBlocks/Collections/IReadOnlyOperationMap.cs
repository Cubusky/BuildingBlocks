using System.Diagnostics.CodeAnalysis;

namespace Cubusky.BuildingBlocks.Collections;

/// <summary>
/// Represents a read-only map of operations that can be performed on conformance types.
/// </summary>
public interface IReadOnlyOperationMap
{
    /// <summary>
    /// Gets the number of elements contained in the <see cref="IReadOnlyOperationMap" /> object.
    /// </summary>
    /// <returns>
    /// The number of elements contained in the <see cref="IReadOnlyOperationMap" /> object.
    /// </returns>
    int Count { get; }

    /// <summary>
    /// Tries to get the value associated with the specified conformance and operation types.
    /// </summary>
    /// <param name="callback">When this method returns, contains the value associated with the specified conformance and operation, types, if the key is found; otherwise, <see langword="null"/>.</param>
    /// <returns><see langword="true" /> if the <see cref="IReadOnlyOperationMap" /> contains an element with the specified conformance and operation types; otherwise, <see langword="false" />.</returns>
    /// <inheritdoc cref="OperationCallback{TConformance, TOperation}"/>
    bool TryGetValue<TConformance, TOperation>([NotNullWhen(true)] out OperationCallback<TConformance, TOperation>? callback)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>;

    /// <summary>
    /// Tries to get the value associated with the specified conformance, operation, and argument types.
    /// </summary>
    /// <param name="callback">When this method returns, contains the value associated with the specified conformance, operation, and argument types, if the key is found; otherwise, <see langword="null"/>.</param>
    /// <returns><see langword="true" /> if the <see cref="IReadOnlyOperationMap" /> contains an element with the specified conformance, operation, and argument types; otherwise, <see langword="false" />.</returns>
    /// <inheritdoc cref="OperationCallback{TConformance, TOperation, TArgs}"/>
    bool TryGetValue<TConformance, TOperation, TArgs>([NotNullWhen(true)] out OperationCallback<TConformance, TOperation, TArgs>? callback)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>;
}

/// <summary>
/// Provides extension methods for the <see cref="IReadOnlyOperationMap"/> and <see cref="IOperationMap"/> interfaces.
/// </summary>
public static partial class OperationMapExtensions
{
    /// <summary>
    /// Determines whether the <see cref="IReadOnlyOperationMap"/> contains an operation callback for the specified conformance and operation types.
    /// </summary>
    /// <returns><see langword="true" /> if the <see cref="IReadOnlyOperationMap"/> contains an operation callback for the specified conformance and operation types; otherwise, <see langword="false" />.</returns>
    /// <inheritdoc cref="OperationCallback{TConformance, TOperation}"/>
    public static bool ContainsKey<TConformance, TOperation>(this IReadOnlyOperationMap operationMap)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance> => operationMap.TryGetValue<TConformance, TOperation>(out _);

    /// <summary>
    /// Determines whether the <see cref="IReadOnlyOperationMap"/> contains an operation callback for the specified conformance, operation, and argument types.
    /// </summary>
    /// <returns><see langword="true" /> if the <see cref="IReadOnlyOperationMap"/> contains an operation callback for the specified conformance, operation, and argument types; otherwise, <see langword="false" />.</returns>
    /// <inheritdoc cref="OperationCallback{TConformance, TOperation, TArgs}"/>
    public static bool ContainsKey<TConformance, TOperation, TArgs>(this IReadOnlyOperationMap operationMap)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance> => operationMap.TryGetValue<TConformance, TOperation, TArgs>(out _);

    /// <summary>
    /// Gets the registered callback for the specified conformance and operation types.
    /// </summary>
    /// <returns>The registered operation callback for the specified type arguments.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when no callback is registered for the specified conformance and operation types.</exception>
    /// <inheritdoc cref="OperationCallback{TConformance, TOperation}"/>
    public static OperationCallback<TConformance, TOperation> Get<TConformance, TOperation>(this IReadOnlyOperationMap operationMap)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance> => operationMap.TryGetValue<TConformance, TOperation>(out var callback) ? callback : throw new KeyNotFoundException($"No operation callback registered for conformance {typeof(TConformance)} and operation {typeof(TOperation)}.");

    /// <summary>
    /// Gets the registered operation callback for the specified conformance, operation, and argument types.
    /// </summary>
    /// <returns>The registered operation callback for the specified type arguments.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when no callback is registered for the specified conformance, operation, and argument types.</exception>
    /// <inheritdoc cref="OperationCallback{TConformance, TOperation, TArgs}"/>
    public static OperationCallback<TConformance, TOperation, TArgs> Get<TConformance, TOperation, TArgs>(this IReadOnlyOperationMap operationMap)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance> => operationMap.TryGetValue<TConformance, TOperation, TArgs>(out var callback) ? callback : throw new KeyNotFoundException($"No operation callback registered for conformance {typeof(TConformance)} and operation {typeof(TOperation)} with arguments {typeof(TArgs)}.");

    /// <returns>The mapped operation callback when found; otherwise, <see langword="null"/>.</returns>
    /// <inheritdoc cref="GetValueOrDefault{TConformance, TOperation}(IReadOnlyOperationMap, OperationCallback{TConformance, TOperation})" />
    public static OperationCallback<TConformance, TOperation>? GetValueOrDefault<TConformance, TOperation>(this IReadOnlyOperationMap operationMap)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance> => operationMap.GetValueOrDefault<TConformance, TOperation>(default!);

    /// <returns>The mapped operation callback when found; otherwise, <see langword="null"/>.</returns>
    /// <inheritdoc cref="GetValueOrDefault{TConformance, TOperation, TArgs}(IReadOnlyOperationMap, OperationCallback{TConformance, TOperation, TArgs})" />
    public static OperationCallback<TConformance, TOperation, TArgs>? GetValueOrDefault<TConformance, TOperation, TArgs>(this IReadOnlyOperationMap operationMap)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance> => operationMap.GetValueOrDefault<TConformance, TOperation, TArgs>(default!);

    /// <summary>
    /// Gets the operation callback registered for the specified conformance and operation types, or returns a provided fallback when no mapping exists.
    /// </summary>
    /// <param name="operationMap"></param>
    /// <param name="defaultValue">The operation callback to return when the operation is not found in the map.</param>
    /// <returns>The mapped operation callback when found; otherwise, <paramref name="defaultValue"/>.</returns>
    /// <inheritdoc cref="OperationCallback{TConformance, TOperation}"/>
    public static OperationCallback<TConformance, TOperation> GetValueOrDefault<TConformance, TOperation>(this IReadOnlyOperationMap operationMap, OperationCallback<TConformance, TOperation> defaultValue)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance> => operationMap.TryGetValue<TConformance, TOperation>(out var callback) ? callback : defaultValue;

    /// <summary>
    /// Gets the operation callback registered for the specified conformance, operation, and argument types, or returns a provided fallback when no mapping exists.
    /// </summary>
    /// <param name="operationMap"></param>
    /// <param name="defaultValue">The operation callback to return when the operation is not found in the map.</param>
    /// <returns>The mapped operation callback when found; otherwise, <paramref name="defaultValue"/>.</returns>
    /// <inheritdoc cref="OperationCallback{TConformance, TOperation, TArgs}"/>
    public static OperationCallback<TConformance, TOperation, TArgs> GetValueOrDefault<TConformance, TOperation, TArgs>(this IReadOnlyOperationMap operationMap, OperationCallback<TConformance, TOperation, TArgs> defaultValue)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance> => operationMap.TryGetValue<TConformance, TOperation, TArgs>(out var callback) ? callback : defaultValue;
}
