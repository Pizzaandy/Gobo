using Gobo.Printer.DocTypes;
using Gobo.SyntaxNodes.PrintHelpers;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class MemberIndexExpression : GmlSyntaxNode, IMemberChainable
{
    public GmlSyntaxNode Object { get; set; }
    public List<GmlSyntaxNode> Properties { get; set; }
    public string Accessor { get; set; }

    public MemberIndexExpression(
        TextSpan span,
        GmlSyntaxNode @object,
        List<GmlSyntaxNode> properties,
        string accessor
    )
        : base(span)
    {
        Object = AsChild(@object);
        Properties = AsChildren(properties);
        Accessor = accessor;
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        return MemberChain.PrintMemberChain(ctx, this);
    }

    public Doc PrintInChain(PrintContext ctx)
    {
        var printedAccessor =
            ctx.Options.RemoveArrayCopyAccessor && Accessor == "[@" ? "[" : Accessor;

        var accessor = printedAccessor.Length > 1 ? printedAccessor + " " : printedAccessor;
        var printed = DelimitedList.PrintInBrackets(ctx, accessor, Properties, "]", ",");
        return printed;
    }

    public void SetObject(GmlSyntaxNode node)
    {
        Object = AsChild(node);
    }
}
