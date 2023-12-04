using Gobo.Printer.DocTypes;

namespace Gobo.SyntaxNodes.Gml.Literals
{
    internal sealed class VerbatimStringLiteral : Literal
    {
        public char QuoteCharacter { get; set; }

        public VerbatimStringLiteral(TextSpan span, string text)
            : base(span, text)
        {
            Text = text[2..^1].ReplaceLineEndings("\n");
            QuoteCharacter = text[1];
        }

        public override Doc PrintNode(PrintContext ctx)
        {
            return $"@{QuoteCharacter}{Text}{QuoteCharacter}";
        }
    }
}
