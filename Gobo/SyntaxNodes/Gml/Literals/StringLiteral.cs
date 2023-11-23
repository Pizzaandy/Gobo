namespace Gobo.SyntaxNodes.Gml.Literals
{
    internal sealed class StringLiteral : Literal
    {
        public StringLiteral(TextSpan span, string text)
            : base(span, text) { }
    }
}
