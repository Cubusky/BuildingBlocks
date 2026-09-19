using System.Diagnostics.CodeAnalysis;

namespace Cubusky.BuildingBlocks.Collections;

public sealed class OperationMap : IOperationMap, IReadOnlyOperationMap
{
    public static OperationMap Shared { get; } = new();

    private Dictionary<(Type Conformance, Type Operation, Type? Args), Delegate> Operations { get; } = [];

    public int Count => Operations.Count;

    public void Clear() => Operations.Clear();

    public void Add<TConformance, TOperation>(OperationCallback<TConformance, TOperation> callback)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>
        => Operations.Add((typeof(TConformance), typeof(TOperation), null), callback);

    public void Add<TConformance, TOperation, TArgs>(OperationCallback<TConformance, TOperation, TArgs> callback)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>
        => Operations.Add((typeof(TConformance), typeof(TOperation), typeof(TArgs)), callback);

    public void Set<TConformance, TOperation>(OperationCallback<TConformance, TOperation> callback)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>
        => Operations[(typeof(TConformance), typeof(TOperation), null)] = callback;

    public void Set<TConformance, TOperation, TArgs>(OperationCallback<TConformance, TOperation, TArgs> callback)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>
        => Operations[(typeof(TConformance), typeof(TOperation), typeof(TArgs))] = callback;

    public bool Remove<TConformance, TOperation>()
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>
        => Operations.Remove((typeof(TConformance), typeof(TOperation), null));

    public bool Remove<TConformance, TOperation, TArgs>()
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>
        => Operations.Remove((typeof(TConformance), typeof(TOperation), typeof(TArgs)));

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
                callback = (owner, in op, _) => ((OperationCallback<TConformance, TOperation>)voidDel)(owner, in op);
                return true;
            }
        }

        callback = null;
        return false;
    }
}
