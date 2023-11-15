using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal sealed class ReturnStatement : GmlSyntaxNode
    {
        public GmlSyntaxNode Argument { get; set; }

        public ReturnStatement(TextSpan span, GmlSyntaxNode argument)
            : base(span)
        {
            Argument = AsChild(argument);
        }

        public override Doc PrintNode(PrintContext ctx)
        {
            return Doc.Concat("return", Argument.IsEmpty ? "" : " ", Argument.Print(ctx));
        }
    }
}
