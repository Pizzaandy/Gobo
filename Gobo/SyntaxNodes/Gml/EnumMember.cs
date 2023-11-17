using Gobo.Printer.DocTypes;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class EnumMember : GmlSyntaxNode
{
    public GmlSyntaxNode Id { get; set; }
    public GmlSyntaxNode Initializer { get; set; }

    public EnumMember(TextSpan span, GmlSyntaxNode id, GmlSyntaxNode initializer)
        : base(span)
    {
        Id = AsChild(id);
        Initializer = AsChild(initializer);
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        if (Initializer.IsEmpty)
        {
            return Id.Print(ctx);
        }
        else
        {
            return Doc.Concat(Id.Print(ctx), " ", "=", " ", Initializer.Print(ctx));
        }
    }
}
