using Antlr4.Runtime;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class Literal : GmlSyntaxNode
    {
        public string Text { get; set; }

        public Literal(ParserRuleContext context, string text)
            : base(context)
        {
            Text = text;
        }

        public override Doc Print()
        {
            return Text;
        }
    }
}
