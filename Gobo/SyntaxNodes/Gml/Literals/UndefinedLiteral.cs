using Gobo.Printer.DocTypes;

namespace Gobo.SyntaxNodes.Gml.Literals
{
    internal sealed class UndefinedLiteral : Literal
    {
        public UndefinedLiteral(TextSpan span, string text)
            : base(span, text) { }

        public static string Undefined = "undefined";

        public static int HashCode = Undefined.GetHashCode();

        public override Doc PrintNode(PrintContext ctx)
        {
            return Undefined;
        }

        public override int GetHashCode()
        {
            return HashCode;
        }
    }
}
