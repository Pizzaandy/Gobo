using PrettierGML.Printer.DocTypes;
using PrettierGML.SyntaxNodes.PrintHelpers;

namespace PrettierGML.SyntaxNodes.Gml;

internal sealed class DoStatement : GmlSyntaxNode
{
    public GmlSyntaxNode Body { get; set; }
    public GmlSyntaxNode Test { get; set; }

    public DoStatement(TextSpan span, GmlSyntaxNode body, GmlSyntaxNode test)
        : base(span)
    {
        Body = AsChild(body);
        Test = AsChild(test);
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
