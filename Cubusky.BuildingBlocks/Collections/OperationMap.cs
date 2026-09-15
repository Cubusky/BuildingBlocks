using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Cubusky.BuildingBlocks.Collections;

public sealed class OperationMap : IOperationMap, IReadOnlyOperationMap
{
    public static OperationMap Shared { get; } = new();

    private Dictionary<(Type Conformance, Type Operation), Delegate> Operations { get; } = [];

    private static class OperationInitializer<TOperation>
        where TOperation : struct
    {
        static OperationInitializer() => RuntimeHelpers.RunClassConstructor(typeof(TOperation).TypeHandle);

        internal static void Ensure() { }
    }

    #region IReadOnlyOperationMap Implementation
    public int Count => Operations.Count;

    public bool ContainsKey<TConformance, TOperation>()
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>
    {
        OperationInitializer<TOperation>.Ensure();
        for (Type type = typeof(TConformance); type != null; type = type.BaseType)
        {
            if (Operations.ContainsKey((type, typeof(TOperation))))
            {
                return true;
            }
        }
        return false;
    }

    public OperationCallback<TConformance, TOperation> Get<TConformance, TOperation>()
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>
    {
        OperationInitializer<TOperation>.Ensure();
        return TryGetValue<TConformance, TOperation>(out var callback)
            ? callback
            : throw new KeyNotFoundException($"No operation callback registered for conformance {typeof(TConformance)} and operation {typeof(TOperation)}.");
    }

    public bool TryGetValue<TConformance, TOperation>([NotNullWhen(true)] out OperationCallback<TConformance, TOperation>? callback)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>
    {
        OperationInitializer<TOperation>.Ensure();

        for (Type type = typeof(TConformance); type != null; type = type.BaseType)
        {
            if (Operations.TryGetValue((type, typeof(TOperation)), out var @delegate))
            {
                callback = (OperationCallback<TConformance, TOperation>)@delegate;
                return true;
            }
        }

        callback = null;
        return false;
    }
    #endregion

    #region IOperationMap Implementation
    public void Add<TConformance, TOperation>(OperationCallback<TConformance, TOperation> callback)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>
    {
        OperationInitializer<TOperation>.Ensure();
        Operations.Add((typeof(TConformance), typeof(TOperation)), callback);
    }

    public void Set<TConformance, TOperation>(OperationCallback<TConformance, TOperation> callback)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>
    {
        OperationInitializer<TOperation>.Ensure();
        Operations[(typeof(TConformance), typeof(TOperation))] = callback;
    }

    public void Clear() => Operations.Clear();

    public bool Remove<TConformance, TOperation>()
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>
    {
        OperationInitializer<TOperation>.Ensure();
        return Operations.Remove((typeof(TConformance), typeof(TOperation)));
    }
    #endregion
}