using Antlr4.Runtime;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class ArrayExpression : GmlSyntaxNode
    {
        public GmlSyntaxNode Elements { get; set; }

        public ArrayExpression(
            ParserRuleContext context,
            CommonTokenStream tokenStream,
            GmlSyntaxNode elements
        )
            : base(context, tokenStream)
        {
            Elements = AsChild(elements);
        }

        public override Doc Print()
        {
            return PrintHelper.PrintArgumentListLikeSyntax("[", Elements, "]", ",");
        }
    }
}
