namespace PrettierGML.SyntaxNodes
{
    internal interface ISyntaxNode<T>
        where T : ISyntaxNode<T>
    {
        public abstract Range CharacterRange { get; init; }
        public abstract T? Parent { get; protected set; }
        public abstract List<T> Children { get; protected set; }
    }
}
