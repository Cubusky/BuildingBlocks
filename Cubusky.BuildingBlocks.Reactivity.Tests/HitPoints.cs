using Chickensoft.Sync;
using Chickensoft.Sync.Primitives;
using Cubusky.BuildingBlocks.Collections;

namespace Cubusky.BuildingBlocks.Reactivity.Tests;

public interface IHitPoints : IStat, IOperator<HitPoints>, IAutoObject<HitPoints.Binding>
{
    new void Perform<TOperation>(in TOperation operation)
        where TOperation : struct, IOperation<HitPoints> => ((IOperator<HitPoints>)this).Perform(in operation);

    new HitPoints.Binding Bind();
}

public class HitPoints : Stat, IHitPoints
{
    private readonly Broadcaster _broadcaster;

    public HitPoints()
    {
        _broadcaster = new Broadcaster(Subject);
    }

    public new Binding Bind() => new(Subject);

    void IOperator<HitPoints>.Perform<TOperation>(in TOperation operation) => Perform(this, in operation, _broadcaster);

    public new class Broadcaster : Stat.Broadcaster
    {
        internal Broadcaster(SyncSubject subject)
            : base(subject) { }

        public new void Broadcast<TOperation>(in TOperation operation)
            where TOperation : struct, IOperation<HitPoints> => Subject.Broadcast(in operation);
    }

    public new class Binding : Stat.Binding
    {
        internal Binding(ISyncSubject subject)
            : base(subject) { }

        public new Binding On<TOperation>(Callback<TOperation> callback, Condition<TOperation>? condition = null)
            where TOperation : struct, IOperation<HitPoints>
        {
            AddCallback(callback, condition);
            return this;
        }
    }
}

public readonly record struct Heal(int Value) : IOperation<HitPoints>
{
    static Heal() => OperationMap.Shared.Add<HitPoints, Heal, HitPoints.Broadcaster>(Execute);

    public static void Execute(HitPoints owner, in Heal operation, HitPoints.Broadcaster broadcaster)
    {
        owner.Value += operation.Value;
        broadcaster.Broadcast(in operation);
        broadcaster.Broadcast(new Increase(operation.Value));
    }
}
