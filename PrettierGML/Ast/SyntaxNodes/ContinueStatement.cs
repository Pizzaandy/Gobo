using Antlr4.Runtime;
using PrettierGML.Printer.Document.DocTypes;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class ContinueStatement : GmlSyntaxNode
    {
        public ContinueStatement(ParserRuleContext context)
            : base(context) { }

        public override Doc Print(PrintContext ctx)
        {
            return "continue";
        }
    }
}
