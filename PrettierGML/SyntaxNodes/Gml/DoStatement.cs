using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;
using PrettierGML.SyntaxNodes.PrintHelpers;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal sealed class DoStatement : GmlSyntaxNode
    {
        public GmlSyntaxNode Body { get; set; }
        public GmlSyntaxNode Test { get; set; }

        public DoStatement(ParserRuleContext context, GmlSyntaxNode body, GmlSyntaxNode test)
            : base(context)
        {
            Body = AsChild(body);
            Test = AsChild(test);
        }

        public override Doc PrintNode(PrintContext ctx)
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
