using Gobo.Printer.DocTypes;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class Parameter : GmlSyntaxNode
{
    public GmlSyntaxNode Name { get; set; }
    public GmlSyntaxNode Type { get; set; }
    public GmlSyntaxNode Initializer { get; set; }

    public Parameter(
        TextSpan span,
        GmlSyntaxNode name,
        GmlSyntaxNode type,
        GmlSyntaxNode initializer
    )
        : base(span)
    {
        Children = [name, type, initializer];
        Name = name;
        Type = type;
        Initializer = initializer;
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        var typeAnnotation = Type.Print(ctx);

        if (Initializer.IsEmpty)
        {
            return Doc.Concat(Name.Print(ctx), typeAnnotation);
        }
        else
        {
            return Doc.Concat(
                Name.Print(ctx),
                typeAnnotation,
                " ",
                "=",
                " ",
                Initializer.Print(ctx)
            );
        }
    }
}
