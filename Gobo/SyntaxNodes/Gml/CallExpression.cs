using Gobo.Printer.DocTypes;
using Gobo.SyntaxNodes.PrintHelpers;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class CallExpression : GmlSyntaxNode, IMemberChainable
{
    public GmlSyntaxNode Object { get; set; }
    public GmlSyntaxNode Arguments { get; set; }

    public CallExpression(TextSpan span, GmlSyntaxNode @object, GmlSyntaxNode arguments)
        : base(span)
    {
        Object = AsChild(@object);
        Arguments = AsChild(arguments);
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        return MemberChain.PrintMemberChain(ctx, this);
    }

    public Doc PrintInChain(PrintContext ctx)
    {
        return Arguments.Print(ctx);
    }

    public void SetObject(GmlSyntaxNode node)
    {
        Object = AsChild(node);
    }
}
