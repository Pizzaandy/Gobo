using Antlr4.Runtime;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class WhileStatement : GmlSyntaxNode
    {
        public GmlSyntaxNode Test { get; set; }
        public GmlSyntaxNode Body { get; set; }

        public WhileStatement(ParserRuleContext context, GmlSyntaxNode test, GmlSyntaxNode body)
            : base(context)
        {
            Test = AsChild(test);
            Body = AsChild(body);
        }

        public override Doc Print(PrintContext ctx)
        {
            return PrintHelper.PrintSingleClauseStatement(ctx, "while", Test, Body);
        }
    }
}
