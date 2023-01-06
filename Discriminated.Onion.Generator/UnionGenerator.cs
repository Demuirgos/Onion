using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Generator]
public class UnionGenerator : ISourceGenerator
{
    private IEnumerable<RecordDeclarationSyntax> GetAllMarkedClasses(Compilation context, string attributeName)
            => context.SyntaxTrees
                .SelectMany(st => st.GetRoot()
                .DescendantNodes()
                .OfType<RecordDeclarationSyntax>()
                .Where(r => r.AttributeLists
                    .SelectMany(al => al.Attributes)
                    .Any(a => a.Name.GetText().ToString() == attributeName.Replace("Attribute", String.Empty))));

    private string HandleMarkedClass(RecordDeclarationSyntax recordSyntax)
    {

        // get all child item that RecordDeclarationSyntax
        var childNodes = recordSyntax.DescendantNodes()
            .OfType<RecordDeclarationSyntax>()
            .Where(r => r.Parent == recordSyntax)
            .Select(r => r.AddBaseListTypes(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(recordSyntax.Identifier.Text))))
            .ToList();

        StringBuilder sb = new StringBuilder();

        sb.Append($"public partial record {recordSyntax.Identifier} \r\n{{\n");

        foreach (var type in childNodes)
        {
            sb.Append($"    public partial record {type.Identifier}: {recordSyntax.Identifier};\n");
        }

        string partials = $$$"""
    public static T Handle<T>(
        {{{recordSyntax.Identifier}}} @this,
        {{{String.Join(",\n        ", childNodes.Select(r => $"System.Func<{r.Identifier}, T> {r.Identifier.ToString().ToLower()}Handler"))}}}
    ) => @this switch
    {
        {{{String.Join(",\n        ", childNodes.Select(r => $"{r.Identifier} {r.Identifier.ToString().ToLower()} => {r.Identifier.ToString().ToLower()}Handler({r.Identifier.ToString().ToLower()})"))}}}
    };
};

""";
        sb.Append(partials);
        return sb.ToString();
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var compilation = context.Compilation;
        var markedClasses = GetAllMarkedClasses(compilation, nameof(UnionAttribute));

        foreach (var marked in markedClasses)
        {
            string text = HandleMarkedClass(marked);
            context.AddSource($"{marked.Identifier}.u.g.cs", text);
        }

    }

    public void Initialize(GeneratorInitializationContext context)
    {
    }
}
