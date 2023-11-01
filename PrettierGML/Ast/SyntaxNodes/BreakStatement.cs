using Antlr4.Runtime;
using PrettierGML.Printer.Document.DocTypes;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class BreakStatement : GmlSyntaxNode
    {
        public BreakStatement(ParserRuleContext context)
            : base(context) { }

        public override Doc Print(PrintContext ctx)
        {
            return "break";
        }
    }
}
