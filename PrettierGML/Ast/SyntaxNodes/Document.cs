using Antlr4.Runtime;
using PrettierGML.Nodes.PrintHelpers;
using PrettierGML.Printer.DocTypes;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class Document : GmlSyntaxNode
    {
        public List<GmlSyntaxNode> Body => Children;

        public Document(ParserRuleContext context, List<GmlSyntaxNode> body)
            : base(context)
        {
            AsChildren(body);
        }

        public override Doc Print(PrintContext ctx)
        {
            return Doc.Concat(
                Statement.PrintStatements(ctx, Children),
                Doc.HardLineIfNoPreviousLine
            );
        }
    }
}
