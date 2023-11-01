using Antlr4.Runtime;
using PrettierGML.Printer.Docs.DocTypes;
using PrettierGML.Nodes.PrintHelpers;
using PrettierGML.Printer;

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
