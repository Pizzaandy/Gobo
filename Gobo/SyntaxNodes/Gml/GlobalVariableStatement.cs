using Gobo.Printer.DocTypes;
using Gobo.SyntaxNodes.PrintHelpers;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class GlobalVariableStatement : GmlSyntaxNode
{
    public GmlSyntaxNode[] Declarations => Children;

    public GlobalVariableStatement(TextSpan span, GmlSyntaxNode[] declarations)
        : base(span)
    {
        Children = declarations;
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        var parts = new List<Doc>() { "globalvar", " " };

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
