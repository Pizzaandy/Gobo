using Antlr4.Runtime;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class IfStatement : GmlSyntaxNode
    {
        public GmlSyntaxNode Test { get; set; }
        public GmlSyntaxNode Consequent { get; set; }
        public GmlSyntaxNode Alternate { get; set; }

        public IfStatement(
            ParserRuleContext context,
            GmlSyntaxNode test,
            GmlSyntaxNode consequent,
            GmlSyntaxNode alternate
        )
            : base(context)
        {
            Test = AsChild(test);
            Consequent = AsChild(consequent);
            Alternate = AsChild(alternate);
        }

        public override Doc Print()
        {
            var parts = new List<Doc>
            {
                PrintHelper.PrintSingleClauseStatement("if", Test, Consequent)
            };

            if (Alternate is not EmptyNode)
            {
                parts.Add(" else ");
                if (Alternate is IfStatement)
                {
                    parts.Add(Alternate.Print());
                }
                else
                {
                    parts.Add(PrintHelper.PrintStatementInBlock(Alternate));
                }
            }

            return Doc.Concat(parts);
        }
    }
}
