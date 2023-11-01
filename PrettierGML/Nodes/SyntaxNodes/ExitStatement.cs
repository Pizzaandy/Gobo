using Antlr4.Runtime;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class ExitStatement : GmlSyntaxNode
    {
        public ExitStatement(ParserRuleContext context)
            : base(context) { }

        public override Doc Print(PrintContext ctx)
        {
            return "exit";
        }
    }
}
