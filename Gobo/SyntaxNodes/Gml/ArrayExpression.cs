using Gobo.Printer.DocTypes;
using Gobo.SyntaxNodes.PrintHelpers;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class ArrayExpression : GmlSyntaxNode
{
    public GmlSyntaxNode[] Elements => Children;

    public ArrayExpression(TextSpan span, GmlSyntaxNode[] elements)
        : base(span)
    {
        Children = elements;
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        return DelimitedList.PrintInBrackets(ctx, this, "[", Children, "]", ",");
    }
}
