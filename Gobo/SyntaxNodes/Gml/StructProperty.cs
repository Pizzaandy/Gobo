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
        Doc name;

        if (Name is StringLiteral stringLiteral && !stringLiteral.Text.Any(char.IsWhiteSpace))
        {
            var stringContents = stringLiteral.Text.Trim('"');
            name = stringLiteral.PrintWithOwnComments(ctx, stringContents);
        }
        else
        {
            name = Name.Print(ctx);
        }

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
