using Gobo.Printer.DocTypes;
using Gobo.SyntaxNodes.PrintHelpers;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class VariableDeclarationList : GmlSyntaxNode
{
    public GmlSyntaxNode[] Declarations => Children;
    public string Modifier { get; set; }

    public VariableDeclarationList(TextSpan span, GmlSyntaxNode[] declarations, string modifier)
        : base(span)
    {
        Children = declarations;
        Modifier = modifier;
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        var parts = new List<Doc>() { Modifier, " " };

        if (Children.Length == 1)
        {
            parts.Add(Children.First().Print(ctx));
        }
        else if (Children.Length > 0)
        {
            var printedArguments = DelimitedList.Print(ctx, Children, ",");
            parts.Add(Doc.Indent(printedArguments));
        }

        return Doc.Group(parts);
    }
}
