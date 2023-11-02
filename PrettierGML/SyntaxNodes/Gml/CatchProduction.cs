using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;
using PrettierGML.SyntaxNodes.PrintHelpers;

namespace PrettierGML.SyntaxNodes.Gml
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
