using Gobo.Printer.DocTypes;
using Gobo.SyntaxNodes.PrintHelpers;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class DoStatement : GmlSyntaxNode
{
    public GmlSyntaxNode Body { get; set; }
    public GmlSyntaxNode Test { get; set; }

    public DoStatement(TextSpan span, GmlSyntaxNode body, GmlSyntaxNode test)
        : base(span)
    {
        Children = [body, test];
        Body = body;
        Test = test;
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        return Doc.Concat(
            "do",
            " ",
            Statement.EnsureStatementInBlock(ctx, Body),
            " until ",
            Statement.EnsureExpressionInParentheses(ctx, Test)
        );
    }
}
