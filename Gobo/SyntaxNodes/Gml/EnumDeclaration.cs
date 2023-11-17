using Gobo.Printer.DocTypes;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class EnumDeclaration : GmlSyntaxNode
{
    public GmlSyntaxNode Name { get; set; }
    public GmlSyntaxNode Members { get; set; }

    public EnumDeclaration(TextSpan span, GmlSyntaxNode name, GmlSyntaxNode members)
        : base(span)
    {
        Name = AsChild(name);
        Members = AsChild(members);
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        return Doc.Concat(
            "enum",
            " ",
            Name.Print(ctx),
            ctx.Options.BraceStyle == BraceStyle.NewLine
                ? Doc.HardLineIfNoPreviousLine
                : Doc.CollapsedSpace,
            Members.Print(ctx)
        );
    }
}
