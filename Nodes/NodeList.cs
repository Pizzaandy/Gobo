namespace PrettierGML.Nodes
{
    /// <summary>
    /// A utility node used for building the AST. This should not end up in the final AST!
    /// </summary>
    internal class NodeList : GmlSyntaxNode
    {
        public NodeList(List<GmlSyntaxNode> contents)
        {
            AsChildren(contents);
        }

        public override Doc Print(PrintContext ctx) =>
            throw new NotImplementedException("NodeList cannot be printed directly");
    }
}
