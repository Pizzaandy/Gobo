using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;
using PrettierGML.SyntaxNodes.PrintHelpers;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal class SwitchCaseBody : GmlSyntaxNode
    {
        public List<GmlSyntaxNode> Statements => Children;

        public SwitchCaseBody(ParserRuleContext context, List<GmlSyntaxNode> statements)
            : base(context)
        {
            AsChildren(statements);
        }

        public override Doc Print(PrintContext ctx)
        {
            return Statement.PrintStatements(ctx, Children);
        }
    }
}
