# Usage : 
* Input 
```csharp

[Union]
public partial record Option<T>
{
    public partial record Some(T Value);
    public partial record None;

    public static Option<T> operator |(Option<T> left, Option<T> right)
        => left is Some _ ? left : right;
    }

    public static Option<T> operator &(Option<T> left, Option<T> right)
        => left is None ? left : right;
}
```
* Output 
```csharp
public partial record Option<T> 
{
    public partial record Some: Option<T>;
    public partial record None: Option<T>;
    public static TResult Handle<TResult>(
        Option<T> @this,
        System.Func<Some, TResult> someHandler,
        System.Func<None, TResult> noneHandler
    ) => @this switch
    {
        Some some => someHandler(some),
        None none => noneHandler(none)
    };
};
```
* Consumed (Full Example)
```csharp
// See https://aka.ms/new-console-template for more information

using System.Numerics;

Tree<int> tr = new Tree<int>.Node(
    new Tree<int>.Node(
        new Tree<int>.Leaf(23),
        new Tree<int>.Empty()
        ),
    new Tree<int>.Leaf(69)
    );

if (tr.Search(23) is Option<int>.Some value)
{
    Console.WriteLine(value);
}
else
{
    Console.WriteLine("not found");
}

[Union]
public partial record Option<T>
{
    public partial record Some(T Value);
    public partial record None;

    public static Option<T> operator |(Option<T> left, Option<T> right)
    {
        if (left is Some _)
        {
            return left;
        }
        return right;
    }

    public static Option<T> operator &(Option<T> left, Option<T> right)
    {
        if (left is None)
        {
            return left;
        }
        return right;
    }

}

[Union]
public partial record Tree<T> where T : IEqualityOperators<T, T, bool>
{
    public partial record Leaf(T Value);
    public partial record Node(Tree<T> Left, Tree<T> Right);
    public partial record Empty;

    public Option<T> Search(T value)
        => Tree<T>.Handle(
            this,
            leaf => leaf.Value == value ? new Option<T>.Some(value) : new Option<T>.None() as Option<T>,
            node => node.Left.Search(value) | node.Right.Search(value),
            _ => new Option<T>.None()
            );
}
```