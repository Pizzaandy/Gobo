using Gobo.Printer.DocTypes;
using Gobo.SyntaxNodes.PrintHelpers;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class SwitchBlock : GmlSyntaxNode
{
    public GmlSyntaxNode[] Cases => Children;

    public SwitchBlock(TextSpan span, GmlSyntaxNode[] cases)
        : base(span)
    {
        Children = cases;
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        if (Children.Length == 0)
        {
            return Block.PrintEmptyBlock(ctx, this);
        }
        else
        {
            return Block.WrapInBlock(ctx, Statement.PrintStatements(ctx, Children));
        }
    }
}
