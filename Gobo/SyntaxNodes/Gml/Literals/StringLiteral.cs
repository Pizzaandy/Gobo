using Gobo.Printer.DocTypes;

namespace Gobo.SyntaxNodes.Gml.Literals
{
    internal sealed class StringLiteral : Literal
    {
        public StringLiteral(TextSpan span, string text)
            : base(span, text)
        {
            Text = text[1..^1];
        }

        public override Doc PrintNode(PrintContext ctx)
        {
            return $"\"{Text}\"";
        }
    }
}
