namespace PrettierGML.Nodes
{
    internal class EmptyNode : GmlSyntaxNode
    {
        public static EmptyNode Instance { get; } = new();

        private EmptyNode() { }

        public override Doc Print()
        {
            return Doc.Null;
        }
    }
}
