using Antlr4.Runtime.Tree;

namespace PrettierGML.Nodes
{
    internal class NodeList : GmlSyntaxNode
    {
        public IList<GmlSyntaxNode> Contents { get; set; }

        public NodeList(ISyntaxTree context, IList<GmlSyntaxNode> contents)
            : base(context)
        {
            Contents = contents;
            foreach (var node in contents)
            {
                AsChild(node);
            }
        }

        public override Doc Print() => throw new NotImplementedException("NodeList cannot be printed");
    }
}
