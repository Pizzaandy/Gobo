using PrettierGML.Printer.DocTypes;
using PrettierGML.SyntaxNodes.Gml;

namespace PrettierGML.SyntaxNodes
{
    internal class EmptyNode : GmlSyntaxNode
    {
        public static EmptyNode Instance { get; } = new();

        private readonly bool isArgument = false;

        public EmptyNode(bool isArgument = false)
        {
            this.isArgument = isArgument;
        }

        public override Doc PrintNode(PrintContext ctx)
        {
            // Empty nodes in argument lists are treated as undefined
            if (isArgument)
            {
                return Literal.Undefined;
            }
            return Doc.Null;
        }

        public override int GetHashCode()
        {
            if (isArgument)
            {
                return Literal.Undefined.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }
    }
}
