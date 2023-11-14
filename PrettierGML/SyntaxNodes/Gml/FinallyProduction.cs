using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;
using PrettierGML.SyntaxNodes.PrintHelpers;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal sealed class FinallyProduction : GmlSyntaxNode
    {
        public GmlSyntaxNode Body { get; set; }

        public FinallyProduction(TextSpan span, GmlSyntaxNode body)
            : base(span)
        {
            Body = AsChild(body);
        }

        public override Doc PrintNode(PrintContext ctx)
        {
            return Doc.Concat("finally", " ", Statement.EnsureStatementInBlock(ctx, Body));
        }
    }
}
