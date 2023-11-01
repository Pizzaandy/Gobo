using Antlr4.Runtime;
using PrettierGML.Printer.Docs.DocTypes;
using PrettierGML.Nodes.PrintHelpers;
using PrettierGML.Printer;

namespace PrettierGML.Nodes.SyntaxNodes
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
