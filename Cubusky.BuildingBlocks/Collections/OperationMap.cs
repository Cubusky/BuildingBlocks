using System.Diagnostics.CodeAnalysis;

namespace Cubusky.BuildingBlocks.Collections;

/// <summary>
/// Represents a map of operations that can be performed on conformance types.
/// </summary>
public class OperationMap : IOperationMap, IReadOnlyOperationMap
{
    /// <summary>
    /// Gets the shared instance of the <see cref="OperationMap"/> class.
    /// </summary>
    public static OperationMap Shared { get; } = new();

    private Dictionary<(Type Conformance, Type Operation, Type? Args), Delegate> Operations { get; } = [];

    /// <summary>
    /// Gets the number of elements contained in the <see cref="OperationMap" /> object.
    /// </summary>
    /// <returns>
    /// The number of elements contained in the <see cref="OperationMap" /> object.
    /// </returns>
    public int Count => Operations.Count;

    /// <summary>
    /// Removes all elements from the <see cref="OperationMap" /> object.
    /// </summary>
    public void Clear() => Operations.Clear();

    /// <inheritdoc/>
    public void Add<TConformance, TOperation>(OperationCallback<TConformance, TOperation> callback)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance> => Operations.Add((typeof(TConformance), typeof(TOperation), null), callback);

    /// <inheritdoc/>
    public void Add<TConformance, TOperation, TArgs>(OperationCallback<TConformance, TOperation, TArgs> callback)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance> => Operations.Add((typeof(TConformance), typeof(TOperation), typeof(TArgs)), callback);

    /// <inheritdoc/>
    public void Set<TConformance, TOperation>(OperationCallback<TConformance, TOperation> callback)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance> => Operations[(typeof(TConformance), typeof(TOperation), null)] = callback;

    /// <inheritdoc/>
    public void Set<TConformance, TOperation, TArgs>(OperationCallback<TConformance, TOperation, TArgs> callback)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance> => Operations[(typeof(TConformance), typeof(TOperation), typeof(TArgs))] = callback;

    /// <inheritdoc/>
    public bool Remove<TConformance, TOperation>()
        where TConformance : class
        where TOperation : struct, IOperation<TConformance> => Operations.Remove((typeof(TConformance), typeof(TOperation), null));

    /// <inheritdoc/>
    public bool Remove<TConformance, TOperation, TArgs>()
        where TConformance : class
        where TOperation : struct, IOperation<TConformance> => Operations.Remove((typeof(TConformance), typeof(TOperation), typeof(TArgs)));

    /// <returns><see langword="true" /> if the <see cref="OperationMap" /> contains an element with the specified conformance and operation types; otherwise, <see langword="false" />.</returns>
    /// <inheritdoc/>
    public bool TryGetValue<TConformance, TOperation>([NotNullWhen(true)] out OperationCallback<TConformance, TOperation>? callback)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>
    {
        var operationType = typeof(TOperation);
        for (var conformanceType = typeof(TConformance); conformanceType is not null; conformanceType = conformanceType.BaseType)
        {
            if (Operations.TryGetValue((conformanceType, operationType, null), out var voidDel))
            {
                callback = (OperationCallback<TConformance, TOperation>)voidDel;
                return true;
            }
        }

        callback = null;
        return false;
    }

    /// <returns><see langword="true" /> if the <see cref="OperationMap" /> contains an element with the specified conformance and operation types; otherwise, <see langword="false" />.</returns>
    /// <inheritdoc/>
    public bool TryGetValue<TConformance, TOperation, TArgs>([NotNullWhen(true)] out OperationCallback<TConformance, TOperation, TArgs>? callback)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>
    {
        var operationType = typeof(TOperation);
        for (var conformanceType = typeof(TConformance); conformanceType is not null; conformanceType = conformanceType.BaseType)
        {
            for (var argsType = typeof(TArgs); argsType is not null; argsType = argsType.BaseType)
            {
                if (Operations.TryGetValue((conformanceType, operationType, argsType), out var del))
                {
                    callback = (OperationCallback<TConformance, TOperation, TArgs>)del;
                    return true;
                }
            }

            if (Operations.TryGetValue((conformanceType, operationType, null), out var voidDel))
            {
                callback = new VoidCallbackAdapter<TConformance, TOperation, TArgs>((OperationCallback<TConformance, TOperation>)voidDel).Invoke;
                return true;
            }
        }

        callback = null;
        return false;
    }

    // Avoid allocating a closure object when adapting a void callback to a callback with arguments.
    private sealed class VoidCallbackAdapter<TConformance, TOperation, TArgs>(OperationCallback<TConformance, TOperation> inner)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>
    {
        public void Invoke(TConformance owner, in TOperation op, TArgs _) => inner(owner, in op);
    }
}
