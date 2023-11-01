using Antlr4.Runtime;
using PrettierGML.Printer.Docs.DocTypes;
using PrettierGML.Nodes.PrintHelpers;
using PrettierGML.Printer;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class CatchProduction : GmlSyntaxNode
    {
        public GmlSyntaxNode Id { get; set; }
        public GmlSyntaxNode Body { get; set; }

        public CatchProduction(ParserRuleContext context, GmlSyntaxNode id, GmlSyntaxNode body)
            : base(context)
        {
            Id = AsChild(id);
            Body = AsChild(body);
        }

        public override Doc Print(PrintContext ctx)
        {
            if (Id.IsEmpty)
            {
                return Doc.Concat("catch", " ", Statement.EnsureStatementInBlock(ctx, Body));
            }
            else
            {
                return Statement.PrintSingleClauseStatement(ctx, "catch", Id, Body);
            }
        }
    }
}
