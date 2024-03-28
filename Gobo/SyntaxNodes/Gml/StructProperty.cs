using Gobo.Printer.DocTypes;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class StructProperty : GmlSyntaxNode
{
    public GmlSyntaxNode Name { get; set; }
    public GmlSyntaxNode Initializer { get; set; }

    public StructProperty(TextSpan span, GmlSyntaxNode name, GmlSyntaxNode initializer)
        : base(span)
    {
        Children = [name, initializer];
        Name = name;
        Initializer = initializer;
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        if (Initializer.IsEmpty)
        {
            return Name.Print(ctx);
        }
        else
        {
            return Doc.Concat(Name.Print(ctx), ":", " ", Initializer.Print(ctx));
        }
    }
}
