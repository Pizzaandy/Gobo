using Gobo.Printer.DocTypes;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class NewExpression : GmlSyntaxNode
{
    public GmlSyntaxNode Argument { get; set; }

    public NewExpression(TextSpan span, GmlSyntaxNode argument)
        : base(span)
    {
        Argument = AsChild(argument);
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        return Doc.Concat("new", Argument is ArgumentList ? Doc.Null : " ", Argument.Print(ctx));
    }
}
