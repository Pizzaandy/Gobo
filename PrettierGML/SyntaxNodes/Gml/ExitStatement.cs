using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal class ExitStatement : GmlSyntaxNode
    {
        public ExitStatement(ParserRuleContext context)
            : base(context) { }

        public override Doc PrintNode(PrintContext ctx)
        {
            return "exit";
        }
    }
}
