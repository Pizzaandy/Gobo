using Antlr4.Runtime;

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
