using Antlr4.Runtime;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class DoStatement : GmlSyntaxNode
    {
        public GmlSyntaxNode Body { get; set; }
        public GmlSyntaxNode Test { get; set; }

        public DoStatement(ParserRuleContext context, GmlSyntaxNode body, GmlSyntaxNode test)
            : base(context)
        {
            Body = AsChild(body);
            Test = AsChild(test);
        }

        public override Doc Print(PrintContext ctx)
        {
            return Doc.Concat(
                "do ",
                PrintHelper.EnsureStatementInBlock(ctx, Body),
                " until ",
                PrintHelper.PrintExpressionInParentheses(ctx, Test)
            );
        }
    }
}
