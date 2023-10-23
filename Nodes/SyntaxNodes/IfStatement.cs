using Antlr4.Runtime;
using PrettierGML.Nodes.PrintHelpers;

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

        public override Doc Print(PrintContext ctx)
        {
            var parts = new List<Doc>
            {
                Statement.PrintSingleClauseStatement(ctx, "if", Test, Consequent)
            };

            if (Alternate is not EmptyNode)
            {
                Doc leadingWhitespace =
                    ctx.Options.BraceStyle == BraceStyle.NewLine ? Doc.HardLine : " ";

                parts.Add(Doc.Concat(leadingWhitespace, "else", " "));

                if (Alternate is IfStatement)
                {
                    parts.Add(Alternate.Print(ctx));
                }
                else
                {
                    parts.Add(Statement.EnsureStatementInBlock(ctx, Alternate));
                }
            }

            return Doc.Concat(parts);
        }
    }
}
