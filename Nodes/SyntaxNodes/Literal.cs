using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class Literal : GmlSyntaxNode
    {
        public string Text { get; set; }

        public Literal(ParserRuleContext context, CommonTokenStream tokenStream, string text)
            : base(context, tokenStream)
        {
            Text = text;
        }

        public Literal(ITerminalNode context, CommonTokenStream tokenStream, string text)
            : base(context, tokenStream)
        {
            Text = text;
        }

        public override Doc Print()
        {
            return Text;
        }
    }
}
