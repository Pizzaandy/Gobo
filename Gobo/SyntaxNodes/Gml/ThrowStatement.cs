using Gobo.Printer.DocTypes;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class ThrowStatement : GmlSyntaxNode
{
    public GmlSyntaxNode Expression { get; set; }

    public ThrowStatement(TextSpan span, GmlSyntaxNode expression)
        : base(span)
    {
        Children = [expression];
        Expression = expression;
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        return Doc.Concat("throw", " ", Expression.Print(ctx));
    }
}
