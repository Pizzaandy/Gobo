using Gobo.Printer.DocTypes;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class TemplateText : GmlSyntaxNode
{
    public string Text { get; set; }

    public TemplateText(TextSpan span, string text)
        : base(span)
    {
        Text = text;
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        // Template strings don't contain line breaks
        return Text;
    }
}
