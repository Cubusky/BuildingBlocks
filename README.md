# 🧱 BuildingBlocks

BuildingBlocks is a generic implementation of the command pattern that allows you to add type-constrained commands onto objects, as if you were extending them with custom-made methods. It is designed for high performance, extensibility and type-safety.

<p align="center">
<img alt="Cubusky.BuildingBlocks" src="Cubusky.BuildingBlocks/icon.png" width="200">
</p>

BuildingBlocks allows you to make as many commands, hereto referred to as operations, as you want in the form of structs that implement the `IOperation<T>` interface, where `T` is the type of the class that they should be allowed to perform operations on. This allows you to add operations to a class without modifying its underlying data or exposing its internal state. Because of this rigid type-safety, we can ensure that operations are never boxed and can only be invoked on types that they conform to.

## 🐕 Installation

Find the latest version of [Cubusky.BuildingBlocks][nuget] on nuget.

```sh
dotnet add package Cubusky.BuildingBlocks
```

## 🪾 Extendable and Overridable

Ever wanted to add a method to a class that you don't own? Or maybe you want to add a method to a class that you do own, but you want to allow users of your class to override the behavior of that method? BuildingBlocks allows you to do both.

It allows **you** to provide sensible defaults and **your users** the freedom to extend and override them.

## 🪽 Quick Start

Create a class that implements the `IOperator<T>` interface, where `T` is the type of the class. This sets up the class to be able to perform operations.

```csharp
public class First(int Value) : IOperator<First>
{
  public int Value { get; set; } = Value;

  public void Perform<TOperation>(in TOperation operation)
    where TOperation : struct, IOperation<First>
  {
    var operationCallback = OperationMap.Shared.Get<First, TOperation>();
    operationCallback(this, operation);
  }
}
```

Then, you can add operations to that class by creating structs that implement the `IOperation<T>` interface, where `T` is the type of the class, and registering them with the `OperationMap`.

```csharp
public readonly record struct Addition : IOperation<First>
{
  public int Value { get; }

  public Addition(int value)
  {
    Value = value;
    OperationMap.Shared.TryAdd<First, Addition>(Callback);
  }

  public static void Callback(First first, in Addition addition) 
    => first.Value += addition.Value;
}
```

From then on, you can call the operation as many times as you want with the following code:

```csharp
var first = new First(5);
first.Perform(new Addition(5));
Console.WriteLine(first.Value); // Outputs 10
```

<br/>

> [!Tip]
> You can easily support custom operations on an instance of an object by adding an `OperationMap` property to your class. This allows you to add operations that are only available on that instance of the object, without affecting other instances of the same class.
> ```csharp
> public class First : IOperator<First>
> {
>   public OperationMap OperationMap { get; } = new();
>
>   public void Perform<TOperation>(in TOperation operation)
>     where TOperation : struct, IOperation<First>
>   {
>     var operationCallback = OperationMap.GetValueOrDefault<First, TOperation>()
>       ?? OperationMap.Shared.Get<First, TOperation>();
>     operationCallback(this, operation);
>   }
> }
> ```

## 🪺 Initializer pattern

The above code will call `OperationMap.Shared.TryAdd<First, Addition>(Callback);` every time you create a new `Addition` struct, which is not ideal.

There's a little trick you can do to ensure the initialization code is only called once. Add the following `Initializer<TValueType>` class to your project:

```csharp
public static class Initializer<TValueType>
  where TValueType : struct
{
  // This will ensure that the static constructor of TValueType is called
  static Initializer() => RuntimeHelpers.RunClassConstructor(typeof(TValueType).TypeHandle);

  public static void Ensure() { }
}
```

Call the empty `Ensure()` method in your `IOperator<T>`: this will trigger the static constructor of the `Initializer<TValueType>` to run.

```csharp
public class First(int Value) : IOperator<First>
{
  public int Value { get; set; } = Value;

  public void Perform<TOperation>(in TOperation operation)
    where TOperation : struct, IOperation<First>
  {
    Initializer<TOperation>.Ensure();
    var operationCallback = OperationMap.Shared.Get<First, TOperation>();
    operationCallback(this, operation);
  }
}
```

Now, you can move the `OperationMap.Shared.TryAdd<First, Addition>(Callback);` call into a static constructor of the `Addition` struct. This will ensure that the initialization code is only called once, when the `Addition` struct is first used.

```csharp
public readonly record struct Addition(int Value) : IOperation<First>
{
  static Addition() => OperationMap.Shared.TryAdd<First, Addition>(Callback);

  public static void Callback(First first, in Addition addition) 
    => first.Value += addition.Value;
}
```

<br/>

> [!Note]
> You would think the static constructor of `Addition` would be called when you first create a new `Addition` struct, but because of how structs default initialization works, the static constructor is not called until you first use the `Addition` struct in a method call. This is why we need to call `Initializer<TOperation>.Ensure();` in the `Perform` method of `IOperator<T>`.

## 🐣 Inheritance

Inheritance with `IOperator<T>` requires a specific implementation to avoid name collisions with derived classes, especially when combined with interfaces. The following example demonstrates how to safely implement `IOperator<T>` for a base class and a derived class whilst maintaining type-safety and polymorphic behavior.

The base class should provide a default implementation of the `Perform` method, which can be used by all derived classes. It explicitly implements the `IOperator<First>` interface to avoid name collisions with derived classes. For example:

```csharp
public interface IFirst : IOperator<First>;

public class First : IFirst
{
  // Your base class can provide a default implementation of Perform that will be used by all derived classes.
  protected void Perform<TConformance, TOperation, TArgs>(in TConformance owner, in TOperation operation)
    where TConformance : class
    where TOperation : struct, IOperation<TConformance>
  {
    Initializer<TOperation>.Ensure();
    var operationCallback = OperationMap.Shared.Get<TConformance, TOperation>();
    operationCallback(owner, operation);
  }

  // The IOperator<First> interface is implemented explicitly to avoid name collisions with derived classes.
  void IOperator<First>.Perform<TOperation>(in TOperation operation) 
    => Perform(this, in operation);
}
```

The derived class also explicitly implements the `IOperator<Second>` interface to avoid name collisions with derived classes. It also overrides the `Perform` method to allow for polymorphic behavior. For example:

```csharp
public interface ISecond : IFirst, IOperator<Second>
{
  // The Perform method is declared in the interface to allow for polymorphic behavior. This allows you to call Perform 
  // on an ISecond reference, and have it resolve to the correct implementation based on the actual type of the object.
  new void Perform<TOperation>(in TOperation operation)
    where TOperation : struct, IOperation<Second>
    => ((IOperator<Second>)this).Perform(in operation);
}

public class Second : First, ISecond
{
  // The IOperator<Second> interface is implemented explicitly to avoid name collisions with derived classes.
  void IOperator<Second>.Perform<TOperation>(in TOperation operation) 
    => Perform(this, in operation);
}
```

This is the safest way to implement `IOperator<T>` for a base class and a derived class whilst maintaining type-safety and polymorphic behavior.

```csharp
var first = new First();
var second = new Second();
var ifirst = (IFirst)first;
var isecond = (ISecond)second;

// The compiler resolves all calls correctly.
first.Perform(new Addition(5));
second.Perform(new Addition(5));
ifirst.Perform(new Addition(5));
isecond.Perform(new Addition(5));
```

## 🐦‍🔥 Operation Priority

Operation priority is determined by the arg type first, and the conformance type second. Both are prioritized by their inheritance hierarchy, with the most specific type being prioritized first. This allows you to create operations that can be invoked on a base class, but can also be overridden for more specific types.

```csharp
public class Example
{
  // Conformance
  public class First : IOperator<First>;
  public class Second : First, IOperator<Second>;

  // Operations
  public readonly record struct Operation : IOperation<First>;

  // Arguments
  public class A;
  public class B : A;

  // Callbacks
  public static void Min(First owner, in Operation operation) => Console.WriteLine("Min");
  public static void Max(Second owner, in Operation operation, B args) => Console.WriteLine("Max");
  public static void Mid(First owner, in Operation operation, A args) => Console.WriteLine("Mid");

  // Example
  public static void Main()
  {
    object callback;
    var operationMap = new OperationMap();

    operationMap.Set<First, Operation>(Min);
    callback = operationMap.Get<First, Operation>();     // Returns 'Min'.
    callback = operationMap.Get<Second, Operation, B>(); // Returns 'Min'.

    operationMap.Set<Second, Operation, B>(Max);
    callback = operationMap.Get<First, Operation>();     // Returns 'Min'.
    callback = operationMap.Get<Second, Operation, B>(); // Returns 'Max'.
    callback = operationMap.Get<Second, Operation, A>(); // Returns 'Min'.

    operationMap.Set<First, Operation, A>(Mid);
    callback = operationMap.Get<Second, Operation, B>(); // Returns 'Max'.
    callback = operationMap.Get<Second, Operation, A>(); // Returns 'Mid'.
    callback = operationMap.Get<Second, Operation>();    // Returns 'Min'.
  }
}
```

<!-- Links -->

[nuget]: https://www.nuget.org/packages/Cubusky.BuildingBlocks