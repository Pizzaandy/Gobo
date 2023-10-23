using Antlr4.Runtime;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class ArrayExpression : GmlSyntaxNode
    {
        public GmlSyntaxNode Elements { get; set; }

        public ArrayExpression(ParserRuleContext context, GmlSyntaxNode elements)
            : base(context)
        {
            Elements = AsChild(elements);
        }

        public override Doc Print(PrintContext ctx)
        {
            return DelimitedList.PrintInBrackets(ctx, "[", Elements, "]", ",");
        }
    }
}
