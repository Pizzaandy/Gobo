using Antlr4.Runtime;

namespace PrettierGML.Nodes
{
    internal class NodeList : GmlSyntaxNode
    {
        public IList<GmlSyntaxNode> Contents { get; set; }

        public NodeList(ParserRuleContext context, IList<GmlSyntaxNode> contents)
            : base(context)
        {
            Contents = contents;
            foreach (var node in contents)
            {
                AsChild(node);
            }
        }

        public override Doc Print(PrintContext ctx) =>
            throw new NotImplementedException("NodeList cannot be printed");
    }
}
