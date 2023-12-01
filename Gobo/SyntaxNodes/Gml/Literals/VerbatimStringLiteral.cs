using Gobo.Printer.DocTypes;

namespace Gobo.SyntaxNodes.Gml.Literals
{
    internal sealed class VerbatimStringLiteral : Literal
    {
        public VerbatimStringLiteral(TextSpan span, string text)
            : base(span, text)
        {
            Text = text[2..^1];
        }

        public override Doc PrintNode(PrintContext ctx)
        {
            var singleQuoteCount = Text.Count(c => c == '\'');
            var doubleQuoteCount = Text.Count(c => c == '"');
            var quoteCharacter = doubleQuoteCount > singleQuoteCount ? '\'' : '"';

            return $"@{quoteCharacter}{Text}{quoteCharacter}";
        }
    }
}
