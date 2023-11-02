using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;
using PrettierGML.SyntaxNodes.PrintHelpers;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal class WithStatement : GmlSyntaxNode
    {
        public GmlSyntaxNode Object { get; set; }
        public GmlSyntaxNode Body { get; set; }

        public WithStatement(ParserRuleContext context, GmlSyntaxNode @object, GmlSyntaxNode body)
            : base(context)
        {
            Object = AsChild(@object);
            Body = AsChild(body);
        }

        public override Doc Print(PrintContext ctx)
        {
            return Statement.PrintSingleClauseStatement(ctx, "with", Object, Body);
        }
    }
}
