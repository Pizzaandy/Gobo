using Gobo.Printer.DocTypes;

namespace Gobo.SyntaxNodes.Gml.Literals
{
    internal sealed class IntegerLiteral : Literal
    {
        public IntegerLiteral(TextSpan span, string text)
            : base(span, text) { }

        public override Doc PrintNode(PrintContext ctx)
        {
            var trimmed = Text.TrimStart('0');

            if (trimmed.Length == 0)
            {
                return "0";
            }

            return trimmed;
        }

        public override int GetHashCode()
        {
            return Text.Trim('0').GetHashCode();
        }
    }
}
