using Antlr4.Runtime;
using PrettierGML.Printer.Docs.DocTypes;
using PrettierGML.Nodes.PrintHelpers;
using PrettierGML.Printer;

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
