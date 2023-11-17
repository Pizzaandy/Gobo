using Gobo.Printer.DocTypes;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class Identifier : GmlSyntaxNode
{
    public string Name { get; set; }

    public Identifier(TextSpan span, string name)
        : base(span)
    {
        Name = name;
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        return Name;
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}
