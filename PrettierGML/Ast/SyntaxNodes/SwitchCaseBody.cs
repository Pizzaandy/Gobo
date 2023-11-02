using Antlr4.Runtime;
using PrettierGML.Nodes.PrintHelpers;
using PrettierGML.Printer;
using PrettierGML.Printer.DocTypes;

namespace PrettierGML.Nodes.SyntaxNodes
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
