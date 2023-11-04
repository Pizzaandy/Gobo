namespace PrettierGML.SyntaxNodes
{
    internal readonly struct GmlToken : ISyntaxTree
    {
        public int TokenIndex { get; init; }
        public Range CharacterRange { get; init; }
        public int Kind { get; init; }
        public string Text { get; init; }

        public GmlToken(int tokenIndex, Range characterRange, int kind, string text)
        {
            TokenIndex = tokenIndex;
            CharacterRange = characterRange;
            Kind = kind;
            Text = text;
        }

        public override readonly int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(Kind);
            return hashCode.ToHashCode();
        }
    }
}
