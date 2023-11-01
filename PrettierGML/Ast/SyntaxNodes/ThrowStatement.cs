using Antlr4.Runtime;
using PrettierGML.Printer;
using PrettierGML.Printer.Document.DocTypes;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class ThrowStatement : GmlSyntaxNode
    {
        public GmlSyntaxNode Expression { get; set; }

        public ThrowStatement(ParserRuleContext context, GmlSyntaxNode expression)
            : base(context)
        {
            Expression = AsChild(expression);
        }

        public override Doc Print(PrintContext ctx)
        {
            return Doc.Concat("throw", " ", Expression.Print(ctx));
        }
    }
}
