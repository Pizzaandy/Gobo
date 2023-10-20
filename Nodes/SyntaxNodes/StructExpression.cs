using Antlr4.Runtime;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class StructExpression : GmlSyntaxNode
    {
        public GmlSyntaxNode Properties { get; set; }

        public StructExpression(
            ParserRuleContext context,
            CommonTokenStream tokenStream,
            GmlSyntaxNode properties
        )
            : base(context, tokenStream)
        {
            Properties = AsChild(properties);
        }

        public override Doc Print()
        {
            if (Properties.Children.Any())
            {
                return PrintHelper.PrintArgumentListLikeSyntax("{", Properties, "}", ",", true);
            }
            else
            {
                return "{}";
            }
        }
    }
}
