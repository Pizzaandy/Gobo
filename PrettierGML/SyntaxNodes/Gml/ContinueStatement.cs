using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal sealed class ContinueStatement : GmlSyntaxNode
    {
        public ContinueStatement(TextSpan span)
            : base(span) { }

        public override Doc PrintNode(PrintContext ctx)
        {
            return "continue";
        }
    }
}
