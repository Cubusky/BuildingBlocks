using Chickensoft.Sync;
using Chickensoft.Sync.Primitives;
using Cubusky.BuildingBlocks.Collections;
using Shouldly;

namespace Cubusky.BuildingBlocks.Reactivity.Tests;

public class StatTest
{
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
            var operationCallback = OperationMap.GetValueOrDefault<TConformance, TOperation, TBroadcaster>()
                ?? OperationMap.Shared.Get<TConformance, TOperation, TBroadcaster>();
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

            internal Broadcaster(SyncSubject subject)
            {
                Subject = subject;
            }

            public void Broadcast<TOperation>(in TOperation operation)
                where TOperation : struct, IOperation<Stat>
            {
                Subject.Broadcast(in operation);
            }
        }

        public class Binding : SyncBinding
        {
            internal Binding(ISyncSubject subject) : base(subject) { }

            public Binding On<TOperation>(Callback<TOperation> callback, Condition<TOperation>? condition = null)
                where TOperation : struct, IOperation<Stat>
            {
                AddCallback(callback, condition);
                return this;
            }
        }
    }

    public interface IHitPoints : IStat, IOperator<HitPoints>, IAutoObject<HitPoints.Binding>
    {
        new void Perform<TOperation>(in TOperation operation)
            where TOperation : struct, IOperation<HitPoints>
            => ((IOperator<HitPoints>)this).Perform(in operation);

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
            internal Broadcaster(SyncSubject subject) : base(subject) { }

            public new void Broadcast<TOperation>(in TOperation operation)
                where TOperation : struct, IOperation<HitPoints>
            {
                Subject.Broadcast(in operation);
            }
        }

        public new class Binding : Stat.Binding
        {
            internal Binding(ISyncSubject subject) : base(subject) { }

            public new Binding On<TOperation>(Callback<TOperation> callback, Condition<TOperation>? condition = null)
                where TOperation : struct, IOperation<HitPoints>
            {
                AddCallback(callback, condition);
                return this;
            }
        }
    }

    public readonly record struct Increase(int Value) : IOperation<Stat>
    {
        static Increase()
        {
            OperationMap.Shared.Add<Stat, Increase, Stat.Broadcaster>(Execute);
        }

        internal static void Execute(Stat owner, in Increase operation, Stat.Broadcaster broadcaster)
        {
            owner.Value += operation.Value;
            broadcaster.Broadcast(in operation);
        }
    }

    public readonly record struct Heal(int Value) : IOperation<HitPoints>
    {
        static Heal()
        {
            OperationMap.Shared.Add<HitPoints, Heal, HitPoints.Broadcaster>(Execute);
        }

        internal static void Execute(HitPoints owner, in Heal operation, HitPoints.Broadcaster broadcaster)
        {
            owner.Value += operation.Value;
            broadcaster.Broadcast(in operation);
            broadcaster.Broadcast(new Increase(operation.Value));
        }
    }

    [Fact]
    public void Test1()
    {
        int value = 0;

        var stat = new Stat();
        using var binding = stat.Bind()
            .On((in Increase increase) => value = increase.Value);

        stat.Perform(new Increase(10));

        value.ShouldBe(10);
    }

    [Fact]
    public void Test2()
    {
        int value1 = 0;
        int value2 = 0;

        var hitPoints = new HitPoints();
        using var binding = hitPoints.Bind()
            .On((in Increase increase) => value1 = increase.Value)
            .On((in Heal heal) => value2 = heal.Value);

        hitPoints.Perform(new Heal(10));

        value1.ShouldBe(10);
        value2.ShouldBe(10);
    }

    [Fact]
    public void Test3()
    {
        int value = 0;

        IStat stat = new Stat();
        using var binding = stat.Bind()
            .On((in Increase increase) => value = increase.Value);

        stat.Perform(new Increase(10));

        value.ShouldBe(10);
    }

    [Fact]
    public void Test4()
    {
        int value1 = 0;
        int value2 = 0;

        IHitPoints hitPoints = new HitPoints();
        using var binding = hitPoints.Bind()
            .On((in Increase increase) => value1 = increase.Value)
            .On((in Heal heal) => value2 = heal.Value);

        hitPoints.Perform(new Heal(10));

        value1.ShouldBe(10);
        value2.ShouldBe(10);
    }
}
