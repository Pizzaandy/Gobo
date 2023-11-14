using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;
using PrettierGML.SyntaxNodes.PrintHelpers;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal sealed class SwitchBlock : GmlSyntaxNode
    {
        public List<GmlSyntaxNode> Cases => Children;

        public SwitchBlock(TextSpan span, List<GmlSyntaxNode> cases)
            : base(span)
        {
            AsChildren(cases);
        }

        public override Doc PrintNode(PrintContext ctx)
        {
            if (Children.Count == 0)
            {
                return Block.PrintEmptyBlock(ctx, this);
            }
            else
            {
                return Block.WrapInBlock(ctx, Statement.PrintStatements(ctx, Children));
            }
        }
    }
}
