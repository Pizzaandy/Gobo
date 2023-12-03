using Gobo.Printer.DocTypes;

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
        if (Children.Count == 0)
        {
            return Block.EmptyBlock;
        }

        var parts = new List<Doc>();

        for (var i = 0; i < Children.Count; i++)
        {
            var member = Children[i];

            if (member is EnumMember)
            {
                parts.Add(member.Print(ctx));
                parts.Add(",");
            }
            if (member is RegionStatement)
            {
                parts.Add(member.Print(ctx));
            }

            if (i != Children.Count - 1)
            {
                parts.Add(Doc.HardLine);
            }
        }

        return Doc.Concat("{", Doc.Indent(Doc.HardLine, Doc.Concat(parts)), Doc.HardLine, "}");
    }
}
