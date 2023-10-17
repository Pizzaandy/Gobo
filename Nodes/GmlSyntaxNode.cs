using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Newtonsoft.Json;

namespace PrettierGML.Nodes
{
    internal abstract class GmlSyntaxNode
    {
        [JsonIgnore]
        public Interval SourceInterval { get; set; }

        [JsonIgnore]
        public string SourceText => _tokenStream.GetText(SourceInterval);

        [JsonIgnore]
        public IList<IToken> LeadingTrivia => _tokenStream.GetHiddenTokensToLeft(_startTokenIndex);

        [JsonIgnore]
        public IList<IToken> TrailingTrivia => _tokenStream.GetHiddenTokensToRight(_stopTokenIndex);

        [JsonIgnore]
        public GmlSyntaxNode? Parent { get; protected set; }

        [JsonIgnore]
        public List<GmlSyntaxNode> Children { get; protected set; } = new();

        [JsonIgnore]
        public bool IsEmpty => this is EmptyNode;

        public string Kind => GetType().Name;

        private readonly CommonTokenStream _tokenStream;
        private readonly int _startTokenIndex = -1;
        private readonly int _stopTokenIndex = -1;

        public GmlSyntaxNode() { }

        public GmlSyntaxNode(ParserRuleContext context, CommonTokenStream tokenStream)
        {
            SourceInterval = context.SourceInterval;
            _tokenStream = tokenStream;
            _startTokenIndex = context.Start.TokenIndex;
            _stopTokenIndex = context.Stop.TokenIndex;
        }

        public GmlSyntaxNode(ITerminalNode context, CommonTokenStream tokenStream)
        {
            SourceInterval = context.SourceInterval;
            _tokenStream = tokenStream;
            _startTokenIndex = context.Symbol.TokenIndex;
            _stopTokenIndex = context.Symbol.TokenIndex;
        }

        public static EmptyNode Empty => EmptyNode.Instance;

        public static NodeList List(
            ParserRuleContext context,
            CommonTokenStream tokenStream,
            IList<GmlSyntaxNode> contents
        ) => new(context, tokenStream, contents);

        protected GmlSyntaxNode AsChild(GmlSyntaxNode child)
        {
            Children.Add(child);
            child.Parent = this;
            return child;
        }

        public virtual Doc Print() => throw new NotImplementedException();

        public List<Doc> PrintChildren()
        {
            return Children.Select(child => child.Print()).ToList();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public override int GetHashCode()
        {
            var serialized = JsonConvert.SerializeObject(this);
            return serialized.GetHashCode();
        }

        //public static implicit operator Doc(GmlSyntaxNode node) => node.Print();
    }

    internal interface IHasObject
    {
        public GmlSyntaxNode Object { get; set; }
    }
}
