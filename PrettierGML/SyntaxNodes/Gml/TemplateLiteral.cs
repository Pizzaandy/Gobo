using PrettierGML.Printer.DocTypes;

namespace PrettierGML.SyntaxNodes.Gml;

internal sealed class TemplateLiteral : GmlSyntaxNode
{
    public List<GmlSyntaxNode> Parts => Children;

    public TemplateLiteral(TextSpan span, List<GmlSyntaxNode> atoms)
        : base(span)
    {
        Children = AsChildren(atoms);
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        return Doc.Concat("$\"", Doc.Concat(PrintChildren(ctx)), "\"");
    }
}
