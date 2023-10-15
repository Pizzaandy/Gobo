using Antlr4.Runtime;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class BreakStatement : GmlSyntaxNode
    {
        public BreakStatement(ParserRuleContext context)
            : base(context) { }

        public override Doc Print()
        {
            return "break";
        }
    }
}
