using Antlr4.Runtime;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class BreakStatement : GmlSyntaxNode
    {
        public BreakStatement(ParserRuleContext context, CommonTokenStream tokenStream)
            : base(context, tokenStream) { }

        public override Doc Print()
        {
            return "break";
        }
    }
}
