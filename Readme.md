# Usage : 
* Input 
```csharp
[Union]
public partial record Tree
{
    public partial record Leaf(int Value);
    public partial record Node(Tree Left, Tree Right);
    public partial record Empty;
}
```
* Output 
```csharp
public partial record Tree 
{    
    public partial record Leaf: Tree;
    public partial record Node: Tree;
    public partial record Empty: Tree;

    public static T Handle<T>(
        Tree @this,
        System.Func<Leaf, T> leafHandler,
        System.Func<Node, T> nodeHandler,
        System.Func<Empty, T> emptyHandler
    ) => @this switch
    {
        Leaf leaf => leafHandler(leaf),
        Node node => nodeHandler(node),
        Empty empty => emptyHandler(empty)
    };
};

```
* Consumed 
```csharp
Tree tr = new Tree.Node(
    new Tree.Leaf(23),
    new Tree.Node(new Tree.Leaf(69), new Tree.Leaf(123))
    );

Console.WriteLine(Tree.Traverse(tr));

string Traverse(Tree tree) => Tree.Handle(tree,
    (leaf) => leaf.Value.ToString(),
    (node) => $"{Traverse(node.Left)}, {Traverse(node.Right)}",
    (empty) => String.Empty
);
```