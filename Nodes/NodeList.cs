using Antlr4.Runtime;

namespace PrettierGML.Nodes
{
    internal class NodeList : GmlSyntaxNode
    {
        public IList<GmlSyntaxNode> Contents { get; set; }

        public NodeList(
            ParserRuleContext context,
            CommonTokenStream tokenStream,
            IList<GmlSyntaxNode> contents
        )
            : base(context, tokenStream)
        {
            Contents = contents;
            foreach (var node in contents)
            {
                AsChild(node);
            }
        }

        public override Doc Print() =>
            throw new NotImplementedException("NodeList cannot be printed");
    }
}
