using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using PrettierGML.Printer.DocTypes;

namespace PrettierGML.SyntaxNodes.Gml
{
    // TODO: disambiguate literal types?
    internal class Literal : GmlSyntaxNode
    {
        public string Text { get; set; }

        public Literal(ParserRuleContext context, string text)
            : base(context)
        {
            Text = text;
        }

        public Literal(ITerminalNode context, string text)
            : base(context)
        {
            Text = text;
        }

        public override Doc PrintNode(PrintContext ctx)
        {
            return Text;
        }
    }
}
