using Gobo.Printer.DocTypes;
using Gobo.SyntaxNodes.PrintHelpers;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class MemberDotExpression : GmlSyntaxNode, IMemberChainable
{
    public GmlSyntaxNode Object { get; set; }
    public GmlSyntaxNode Property { get; set; }

    public MemberDotExpression(TextSpan span, GmlSyntaxNode @object, GmlSyntaxNode property)
        : base(span)
    {
        Object = AsChild(@object);
        Property = AsChild(property);
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        return MemberChain.PrintMemberChain(ctx, this);
    }

    public Doc PrintInChain(PrintContext ctx)
    {
        Property.PrintOwnComments = false;

        return Doc.Concat(
            Property.PrintLeadingComments(ctx),
            ".",
            Property.Print(ctx),
            Property.PrintTrailingComments(ctx)
        );
    }

    public void SetObject(GmlSyntaxNode node)
    {
        Object = AsChild(node);
    }
}
