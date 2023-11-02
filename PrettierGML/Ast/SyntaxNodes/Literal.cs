using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using PrettierGML.Printer;
using PrettierGML.Printer.DocTypes;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class Literal : TerminalNode
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

        public override Doc Print(PrintContext ctx)
        {
            return Text;
        }
    }
}
