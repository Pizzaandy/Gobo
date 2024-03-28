using Gobo.Printer.DocTypes;
using Gobo.SyntaxNodes.PrintHelpers;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class CatchProduction : GmlSyntaxNode
{
    public GmlSyntaxNode Id { get; set; }
    public GmlSyntaxNode Body { get; set; }

    public CatchProduction(TextSpan span, GmlSyntaxNode id, GmlSyntaxNode body)
        : base(span)
    {
        Children = [id, body];
        Id = id;
        Body = body;
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        if (Id.IsEmpty)
        {
            return Doc.Concat("catch", " ", Statement.EnsureStatementInBlock(ctx, Body));
        }
        else
        {
            return Statement.PrintControlFlowStatement(ctx, "catch", Id, Body);
        }
    }
}
