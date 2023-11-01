using Antlr4.Runtime;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class SwitchBlock : GmlSyntaxNode
    {
        public List<GmlSyntaxNode> Cases => Children;

        public SwitchBlock(ParserRuleContext context, List<GmlSyntaxNode> cases)
            : base(context)
        {
            AsChildren(cases);
        }

        public override Doc Print(PrintContext ctx)
        {
            return Block.PrintInBlock(ctx, Doc.Join(Doc.HardLine, PrintChildren(ctx)));
        }
    }
}
