using Antlr4.Runtime;
using PrettierGML.Nodes.PrintHelpers;
using PrettierGML.Printer.DocTypes;

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
                Statement.EnsureStatementInBlock(ctx, Body),
                " until ",
                Statement.EnsureExpressionInParentheses(ctx, Test)
            );
        }
    }
}
