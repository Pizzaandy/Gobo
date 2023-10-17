using Antlr4.Runtime;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class ExitStatement : GmlSyntaxNode
    {
        public ExitStatement(ParserRuleContext context, CommonTokenStream tokenStream)
            : base(context, tokenStream) { }

        public override Doc Print()
        {
            return "exit";
        }
    }
}
