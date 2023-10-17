using Antlr4.Runtime;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class SwitchStatement : GmlSyntaxNode
    {
        public GmlSyntaxNode Discriminant { get; set; }
        public GmlSyntaxNode Cases { get; set; }

        public SwitchStatement(
            ParserRuleContext context,
            CommonTokenStream tokenStream,
            GmlSyntaxNode discriminant,
            GmlSyntaxNode cases
        )
            : base(context, tokenStream)
        {
            Discriminant = AsChild(discriminant);
            Cases = AsChild(cases);
        }

        public override Doc Print()
        {
            var parts = new List<Doc>
            {
                Doc.Concat("switch ", PrintHelper.PrintExpressionInParentheses(Discriminant), " ")
            };

            if (Cases.Children.Any())
            {
                var cases = Cases.PrintChildren();
                parts.Add(Doc.Concat("{", Doc.Indent(cases), Doc.HardLine, "}"));
            }
            else
            {
                parts.Add("{}");
            }

            return Doc.Concat(parts);
        }
    }
}
