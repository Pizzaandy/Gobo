using Antlr4.Runtime;
using PrettierGML.Printer.Docs.DocTypes;
using PrettierGML.Nodes.PrintHelpers;
using PrettierGML.Printer;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class FinallyProduction : GmlSyntaxNode
    {
        public GmlSyntaxNode Body { get; set; }

        public FinallyProduction(ParserRuleContext context, GmlSyntaxNode body)
            : base(context)
        {
            Body = AsChild(body);
        }

        public override Doc Print(PrintContext ctx)
        {
            return Doc.Concat("finally", " ", Statement.EnsureStatementInBlock(ctx, Body));
        }
    }
}
