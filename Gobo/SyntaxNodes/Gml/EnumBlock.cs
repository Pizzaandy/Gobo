using Gobo.Printer.DocTypes;
using Gobo.SyntaxNodes.PrintHelpers;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class EnumBlock : GmlSyntaxNode
{
    public List<GmlSyntaxNode> Members => Children;

    public EnumBlock(TextSpan span, List<GmlSyntaxNode> members)
        : base(span)
    {
        AsChildren(members);
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        return DelimitedList.PrintInBrackets(
            ctx,
            "{",
            this,
            "}",
            ",",
            allowTrailingSeparator: true,
            forceBreak: true
        );
    }
}
