using Antlr4.Runtime;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class ContinueStatement : GmlSyntaxNode
    {
        public ContinueStatement(ParserRuleContext context, CommonTokenStream tokenStream)
            : base(context, tokenStream) { }

        public override Doc Print()
        {
            return "continue";
        }
    }
}
