using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;
using PrettierGML.SyntaxNodes.PrintHelpers;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal class Document : GmlSyntaxNode
    {
        public List<GmlSyntaxNode> Statements => Children;

        public Document(ParserRuleContext context, List<GmlSyntaxNode> body)
            : base(context)
        {
            AsChildren(body);
        }

        public override Doc PrintNode(PrintContext ctx)
        {
            return Doc.Concat(
                Statement.PrintStatements(ctx, Children),
                Doc.HardLineIfNoPreviousLine
            );
        }
    }
}
