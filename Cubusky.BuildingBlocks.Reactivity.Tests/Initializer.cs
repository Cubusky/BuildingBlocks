using System.Runtime.CompilerServices;

namespace Cubusky.BuildingBlocks.Reactivity.Tests;

public static class Initializer<TValueType>
    where TValueType : struct
{
    static Initializer() => RuntimeHelpers.RunClassConstructor(typeof(TValueType).TypeHandle);

    public static void Ensure() { }
}
