// See https://aka.ms/new-console-template for more information

Tree tr = new Tree.Node(
    new Tree.Leaf(23),
    new Tree.Node(new Tree.Leaf(69), new Tree.Leaf(123))
    );

Console.WriteLine(Tree.Traverse(tr));


[Union]
public partial record Tree
{
    public partial record Leaf(int Value);
    public partial record Node(Tree Left, Tree Right);
    public partial record Empty;
    public static string Traverse(Tree tree) => Tree.Handle(tree,
            (leaf) => leaf.Value.ToString(),
            (node) => $"{Traverse(node.Left)}, {Traverse(node.Right)}",
            (empty) => String.Empty
        );
}

