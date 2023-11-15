using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;
using PrettierGML.SyntaxNodes.PrintHelpers;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal sealed class CatchProduction : GmlSyntaxNode
    {
        public GmlSyntaxNode Id { get; set; }
        public GmlSyntaxNode Body { get; set; }

        public CatchProduction(TextSpan span, GmlSyntaxNode id, GmlSyntaxNode body)
            : base(span)
        {
            Id = AsChild(id);
            Body = AsChild(body);
        }

        public override Doc PrintNode(PrintContext ctx)
        {
            if (Id.IsEmpty)
            {
                return Doc.Concat("catch", " ", Statement.EnsureStatementInBlock(ctx, Body));
            }
            else
            {
                return Statement.PrintControlFlowStatement(ctx, "catch", Id, Body);
            }
        }
    }
}
