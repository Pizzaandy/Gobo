using Gobo.Printer.DocTypes;
using Gobo.SyntaxNodes.PrintHelpers;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class VariableDeclarator : GmlSyntaxNode
{
    public GmlSyntaxNode Id { get; set; }
    public GmlSyntaxNode Type { get; set; }
    public GmlSyntaxNode Initializer { get; set; }

    public VariableDeclarator(
        TextSpan span,
        GmlSyntaxNode id,
        GmlSyntaxNode type,
        GmlSyntaxNode initializer
    )
        : base(span)
    {
        Id = AsChild(id);
        Type = AsChild(type);
        Initializer = AsChild(initializer);
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        var typeAnnotation = Type.Print(ctx);

        if (Initializer.IsEmpty)
        {
            return Doc.Concat(Id.Print(ctx), typeAnnotation);
        }
        else
        {
            return RightHandSide.Print(ctx, Id, Doc.Concat(typeAnnotation, " ", "="), Initializer);
        }
    }
}
