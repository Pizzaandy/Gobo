using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal sealed class BreakStatement : GmlSyntaxNode
    {
        public BreakStatement(ParserRuleContext context)
            : base(context) { }

        public override Doc PrintNode(PrintContext ctx)
        {
            return "break";
        }
    }
}
