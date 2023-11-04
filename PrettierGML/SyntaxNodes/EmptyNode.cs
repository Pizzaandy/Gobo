using PrettierGML.Printer.DocTypes;

namespace PrettierGML.SyntaxNodes
{
    internal class EmptyNode : GmlSyntaxNode
    {
        public static EmptyNode Instance { get; } = new();

        private EmptyNode() { }

        public override Doc PrintNode(PrintContext ctx)
        {
            return Doc.Null;
        }
    }
}
