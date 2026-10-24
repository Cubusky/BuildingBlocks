using System.Diagnostics.CodeAnalysis;

namespace Cubusky.BuildingBlocks.Collections;

/// <summary>
/// Represents a map of operations that can be performed on conformance types.
/// </summary>
public interface IOperationMap
{
    /// <summary>
    /// Removes all elements from the <see cref="IOperationMap" /> object.
    /// </summary>
    void Clear();

    /// <summary>
    /// Adds an operation callback for the specified conformance and operation types.
    /// </summary>
    /// <param name="callback">The operation callback to add.</param>
    /// <exception cref="ArgumentException">Thrown when a callback is already registered for the specified conformance and operation types.</exception>
    /// <inheritdoc cref="OperationCallback{TConformance, TOperation}"/>
    void Add<TConformance, TOperation>(OperationCallback<TConformance, TOperation> callback)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>;

    /// <summary>
    /// Adds an operation callback for the specified conformance, operation, and argument types.
    /// </summary>
    /// <param name="callback">The operation callback to add.</param>
    /// <exception cref="ArgumentException">Thrown when a callback is already registered for the specified conformance, operation, and argument types.</exception>
    /// <inheritdoc cref="OperationCallback{TConformance, TOperation, TArgs}"/>
    void Add<TConformance, TOperation, TArgs>(OperationCallback<TConformance, TOperation, TArgs> callback)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>;

    /// <summary>
    /// Sets the operation callback for the specified conformance and operation types, replacing any existing callback.
    /// </summary>
    /// <param name="callback">The operation callback to set.</param>
    /// <inheritdoc cref="OperationCallback{TConformance, TOperation}"/>
    void Set<TConformance, TOperation>(OperationCallback<TConformance, TOperation> callback)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>;

    /// <summary>
    /// Sets the operation callback for the specified conformance, operation, and argument types, replacing any existing callback.
    /// </summary>
    /// <param name="callback">The operation callback to set.</param>
    /// <inheritdoc cref="OperationCallback{TConformance, TOperation, TArgs}"/>
    void Set<TConformance, TOperation, TArgs>(OperationCallback<TConformance, TOperation, TArgs> callback)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>;

    /// <summary>
    /// Removes the operation callback registered for the specified conformance and operation types.
    /// </summary>
    /// <returns><see langword="true" /> if the callback was found and removed; otherwise, <see langword="false" />.</returns>
    /// <inheritdoc cref="OperationCallback{TConformance, TOperation}"/>
    bool Remove<TConformance, TOperation>()
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>;

    /// <summary>
    /// Removes the operation callback registered for the specified conformance, operation, and argument types.
    /// </summary>
    /// <returns><see langword="true" /> if the callback was found and removed; otherwise, <see langword="false" />.</returns>
    /// <inheritdoc cref="OperationCallback{TConformance, TOperation, TArgs}"/>
    bool Remove<TConformance, TOperation, TArgs>()
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>;

    /// <returns><see langword="true" /> if the <see cref="IOperationMap" /> contains an element with the specified conformance and operation types; otherwise, <see langword="false" />.</returns>
    /// <inheritdoc cref="IReadOnlyOperationMap.TryGetValue{TConformance,TOperation}"/>
    bool TryGetValue<TConformance, TOperation>([NotNullWhen(true)] out OperationCallback<TConformance, TOperation>? callback)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>;

    /// <returns><see langword="true" /> if the <see cref="IOperationMap" /> contains an element with the specified conformance and operation types; otherwise, <see langword="false" />.</returns>
    /// <inheritdoc cref="IReadOnlyOperationMap.TryGetValue{TConformance,TOperation,TArgs}"/>
    bool TryGetValue<TConformance, TOperation, TArgs>([NotNullWhen(true)] out OperationCallback<TConformance, TOperation, TArgs>? callback)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>;
}

public static partial class OperationMapExtensions
{
    /// <summary>
    /// Adds an operation callback for the specified conformance and operation types if none is registered yet.
    /// </summary>
    /// <param name="operationMap"></param>
    /// <param name="callback">The operation callback to add.</param>
    /// <returns><see langword="true" /> if the callback was added; <see langword="false" /> if a callback was already registered.</returns>
    /// <inheritdoc cref="OperationCallback{TConformance, TOperation}"/>
    public static bool TryAdd<TConformance, TOperation>(this IOperationMap operationMap, OperationCallback<TConformance, TOperation> callback)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>
    {
        if (operationMap.TryGetValue<TConformance, TOperation>(out _))
            return false;

        operationMap.Add(callback);
        return true;
    }

    /// <summary>
    /// Adds an operation callback for the specified conformance, operation, and argument types if none is registered yet.
    /// </summary>
    /// <param name="operationMap"></param>
    /// <param name="callback">The operation callback to add.</param>
    /// <returns><see langword="true" /> if the callback was added; <see langword="false" /> if a callback was already registered.</returns>
    /// <inheritdoc cref="OperationCallback{TConformance, TOperation, TArgs}"/>
    public static bool TryAdd<TConformance, TOperation, TArgs>(this IOperationMap operationMap, OperationCallback<TConformance, TOperation, TArgs> callback)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>
    {
        if (operationMap.TryGetValue<TConformance, TOperation, TArgs>(out _))
            return false;

        operationMap.Add(callback);
        return true;
    }

    /// <summary>
    /// Removes the operation callback registered for the specified conformance and operation types and returns it.
    /// </summary>
    /// <param name="operationMap"></param>
    /// <param name="callback">When this method returns, contains the removed callback, if found; otherwise, <see langword="null"/>.</param>
    /// <returns><see langword="true" /> if the callback was found and removed; otherwise, <see langword="false" />.</returns>
    /// <inheritdoc cref="OperationCallback{TConformance, TOperation}"/>
    public static bool Remove<TConformance, TOperation>(this IOperationMap operationMap, [NotNullWhen(true)] out OperationCallback<TConformance, TOperation>? callback)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance> => operationMap.TryGetValue(out callback) && operationMap.Remove<TConformance, TOperation>();

    /// <summary>
    /// Removes the operation callback registered for the specified conformance, operation, and argument types and returns it.
    /// </summary>
    /// <param name="operationMap"></param>
    /// <param name="callback">When this method returns, contains the removed callback, if found; otherwise, <see langword="null"/>.</param>
    /// <returns><see langword="true" /> if the callback was found and removed; otherwise, <see langword="false" />.</returns>
    /// <inheritdoc cref="OperationCallback{TConformance, TOperation, TArgs}"/>
    public static bool Remove<TConformance, TOperation, TArgs>(this IOperationMap operationMap, [NotNullWhen(true)] out OperationCallback<TConformance, TOperation, TArgs>? callback)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance> => operationMap.TryGetValue(out callback) && operationMap.Remove<TConformance, TOperation, TArgs>();
}
