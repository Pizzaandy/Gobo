using Antlr4.Runtime;
using PrettierGML.Nodes.PrintHelpers;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class SwitchStatement : GmlSyntaxNode
    {
        public GmlSyntaxNode Discriminant { get; set; }
        public GmlSyntaxNode Cases { get; set; }

        public SwitchStatement(
            ParserRuleContext context,
            GmlSyntaxNode discriminant,
            GmlSyntaxNode cases
        )
            : base(context)
        {
            Discriminant = AsChild(discriminant);
            Cases = AsChild(cases);
        }

        public override Doc Print(PrintContext ctx)
        {
            var parts = new List<Doc>
            {
                Doc.Concat(
                    "switch",
                    " ",
                    Statement.EnsureExpressionInParentheses(ctx, Discriminant),
                    " "
                )
            };

            if (Cases.Children.Any())
            {
                var caseList = Cases.PrintChildren(ctx);
                parts.Add(Block.PrintInBlock(ctx, Doc.Join(Doc.HardLine, caseList)));
            }
            else
            {
                parts.Add(Block.EmptyBlock);
            }

            return Doc.Concat(parts);
        }
    }
}
