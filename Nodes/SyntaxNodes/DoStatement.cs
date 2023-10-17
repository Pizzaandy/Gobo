using Antlr4.Runtime;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class DoStatement : GmlSyntaxNode
    {
        public GmlSyntaxNode Body { get; set; }
        public GmlSyntaxNode Test { get; set; }

        public DoStatement(
            ParserRuleContext context,
            CommonTokenStream tokenStream,
            GmlSyntaxNode body,
            GmlSyntaxNode test
        )
            : base(context, tokenStream)
        {
            Body = AsChild(body);
            Test = AsChild(test);
        }

        public override Doc Print()
        {
            return Doc.Concat(
                "do ",
                PrintHelper.PrintStatementInBlock(Body),
                " until ",
                PrintHelper.PrintExpressionInParentheses(Test)
            );
        }
    }
}
