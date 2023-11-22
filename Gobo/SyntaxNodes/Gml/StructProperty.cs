using Gobo.Printer.DocTypes;
using Gobo.SyntaxNodes.Gml.Literals;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class StructProperty : GmlSyntaxNode
{
    public GmlSyntaxNode Name { get; set; }
    public GmlSyntaxNode Initializer { get; set; }

    public StructProperty(TextSpan span, GmlSyntaxNode name, GmlSyntaxNode initializer)
        : base(span)
    {
        Name = AsChild(name);
        Initializer = AsChild(initializer);
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        var name = Name is Literal literal ? literal.Text.Trim('"') : Name.Print(ctx);

        if (Initializer.IsEmpty)
        {
            return name;
        }
        else
        {
            return Doc.Concat(name, ":", " ", Initializer.Print(ctx));
        }
    }
}
