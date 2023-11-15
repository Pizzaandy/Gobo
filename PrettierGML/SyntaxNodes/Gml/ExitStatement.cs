using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal sealed class ExitStatement : GmlSyntaxNode
    {
        public ExitStatement(TextSpan span)
            : base(span) { }

        public override Doc PrintNode(PrintContext ctx)
        {
            return "exit";
        }
    }
}
