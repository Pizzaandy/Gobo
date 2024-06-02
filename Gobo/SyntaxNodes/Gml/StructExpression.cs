using Gobo.Printer.DocTypes;
using Gobo.SyntaxNodes.PrintHelpers;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class StructExpression : GmlSyntaxNode
{
    public List<GmlSyntaxNode> Properties => Children;
    public static string EmptyStruct => "{}";

    public StructExpression(TextSpan span, List<GmlSyntaxNode> properties)
        : base(span)
    {
        AsChildren(properties);
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        if (Children.Count == 0 && !DanglingComments.Any())
        {
            return EmptyStruct;
        }
        else
        {
            return DelimitedList.PrintInBrackets(
                ctx,
                "{",
                this,
                "}",
                ",",
                allowTrailingSeparator: true
            );
        }
    }
}
