using Chickensoft.Sync;
using Chickensoft.Sync.Primitives;
using Cubusky.BuildingBlocks.Collections;
using Cubusky.BuildingBlocks.Tests;

namespace Cubusky.BuildingBlocks.Reactivity.Tests;

public interface IStat : IOperator<Stat>, IAutoObject<Stat.Binding>
{
    int Value { get; }
}

public class Stat : IStat
{
    public int Value { get; set; }
    public OperationMap OperationMap { get; } = new();

    protected SyncSubject Subject { get; }
    private readonly Broadcaster _broadcaster;

    public Stat()
    {
        Subject = new(this);
        _broadcaster = new(Subject);
    }

    protected void Perform<TConformance, TOperation, TBroadcaster>(in TConformance owner, in TOperation operation, in TBroadcaster broadcaster)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>
        where TBroadcaster : Broadcaster
    {
        Initializer<TOperation>.Ensure();
        var operationCallback = OperationMap.GetValueOrDefault<TConformance, TOperation, TBroadcaster>() ?? OperationMap.Shared.Get<TConformance, TOperation, TBroadcaster>();
        operationCallback(owner, operation, broadcaster);
    }

    void IOperator<Stat>.Perform<TOperation>(in TOperation operation) => Perform(this, in operation, _broadcaster);

    public Binding Bind() => new(Subject);

    public void ClearBindings() => Subject.ClearBindings();

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Subject.Dispose();
    }

    public class Broadcaster
    {
        protected SyncSubject Subject { get; }

        internal Broadcaster(SyncSubject subject) => Subject = subject;

        public void Broadcast<TOperation>(in TOperation operation)
            where TOperation : struct, IOperation<Stat> => Subject.Broadcast(in operation);
    }

    public class Binding : SyncBinding
    {
        internal Binding(ISyncSubject subject)
            : base(subject) { }

        public Binding On<TOperation>(Callback<TOperation> callback, Condition<TOperation>? condition = null)
            where TOperation : struct, IOperation<Stat>
        {
            AddCallback(callback, condition);
            return this;
        }
    }
}

public readonly record struct Increase(int Value) : IOperation<Stat>
{
    static Increase() => OperationMap.Shared.Add<Stat, Increase, Stat.Broadcaster>(Execute);

    public static void Execute(Stat owner, in Increase operation, Stat.Broadcaster broadcaster)
    {
        owner.Value += operation.Value;
        broadcaster.Broadcast(in operation);
    }
}
