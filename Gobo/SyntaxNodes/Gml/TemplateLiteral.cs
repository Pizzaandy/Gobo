using Gobo.Printer.DocTypes;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class TemplateLiteral : GmlSyntaxNode
{
    public GmlSyntaxNode[] Parts => Children;

    public TemplateLiteral(TextSpan span, GmlSyntaxNode[] atoms)
        : base(span)
    {
        Children = atoms;
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        return Doc.Concat("$\"", Doc.Concat(PrintChildren(ctx)), "\"");
    }
}
