using Gobo.Printer.DocTypes;

namespace Gobo.SyntaxNodes.Gml.Literals
{
    internal sealed class DecimalLiteral : Literal
    {
        public DecimalLiteral(TextSpan span, string text)
            : base(span, text) { }

        public override Doc PrintNode(PrintContext ctx)
        {
            var trimmed = Text.TrimStart('0');

            if (trimmed[0] == '.')
            {
                return "0" + trimmed;
            }

            return trimmed;
        }

        public override int GetHashCode()
        {
            return Text.Trim('0').GetHashCode();
        }
    }
}
