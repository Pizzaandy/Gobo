using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal sealed class DeleteStatement : GmlSyntaxNode
    {
        public GmlSyntaxNode Argument { get; set; }

        public DeleteStatement(ParserRuleContext context, GmlSyntaxNode argument)
            : base(context)
        {
            Argument = AsChild(argument);
        }

        public override Doc PrintNode(PrintContext ctx)
        {
            return Doc.Concat("delete", " ", Argument.Print(ctx));
        }
    }
}
