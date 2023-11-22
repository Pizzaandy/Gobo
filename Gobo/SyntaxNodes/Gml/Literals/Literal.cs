using Gobo.Printer.DocTypes;

namespace Gobo.SyntaxNodes.Gml.Literals;

internal partial class Literal : GmlSyntaxNode
{
    public string Text { get; set; }

    public Literal(TextSpan span, string text)
        : base(span)
    {
        Text = text;
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        return Text;
    }

    public override int GetHashCode()
    {
        return Text.GetHashCode();
    }
}
