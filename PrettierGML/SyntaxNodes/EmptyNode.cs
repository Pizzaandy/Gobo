using PrettierGML.Printer.DocTypes;

namespace PrettierGML.SyntaxNodes
{
    internal class EmptyNode : GmlSyntaxNode
    {
        public static EmptyNode Instance { get; } = new();

        private EmptyNode() { }

        public override Doc Print(PrintContext ctx)
        {
            return Doc.Null;
        }
    }
}
