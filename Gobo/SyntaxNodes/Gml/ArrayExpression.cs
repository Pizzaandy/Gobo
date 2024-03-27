using Gobo.Printer.DocTypes;
using Gobo.SyntaxNodes.PrintHelpers;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class ArrayExpression : GmlSyntaxNode
{
    public List<GmlSyntaxNode> Elements => Children;

    public ArrayExpression(TextSpan span, List<GmlSyntaxNode> elements)
        : base(span)
    {
        AsChildren(elements);
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        return DelimitedList.PrintInBrackets(ctx, this, "[", Elements, "]", ",");
    }
}
