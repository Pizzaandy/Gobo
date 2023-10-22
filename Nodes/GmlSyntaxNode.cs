using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Newtonsoft.Json;

namespace PrettierGML.Nodes
{
    internal abstract class GmlSyntaxNode
    {
        [JsonIgnore]
        public Interval SourceInterval { get; init; }

        [JsonIgnore]
        public GmlSyntaxNode? Parent { get; protected set; }

        [JsonIgnore]
        public List<GmlSyntaxNode> Children { get; protected set; } = new();

        [JsonIgnore]
        public bool IsEmpty => this is EmptyNode;

        public string Kind => GetType().Name;

        public GmlSyntaxNode() { }

        public GmlSyntaxNode(ParserRuleContext node)
        {
            SourceInterval = node.SourceInterval;
        }

        public GmlSyntaxNode(ITerminalNode node)
        {
            SourceInterval = node.SourceInterval;
        }

        public static EmptyNode Empty => EmptyNode.Instance;

        public static NodeList List(ParserRuleContext context, IList<GmlSyntaxNode> contents) =>
            new(context, contents);

        protected GmlSyntaxNode AsChild(GmlSyntaxNode child)
        {
            Children.Add(child);
            child.Parent = this;
            return child;
        }

        public virtual Doc Print(PrintContext ctx) => throw new NotImplementedException();

        public List<Doc> PrintChildren(PrintContext ctx)
        {
            return Children.Select(child => child.Print(ctx)).ToList();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(Kind);
            foreach (var child in Children)
            {
                hashCode.Add(child);
            }
            return hashCode.ToHashCode();
        }

        //public bool HasLeadingLine()
        //{
        //    var leadingTokens = _tokenStream.GetHiddenTokensToLeft(_startTokenIndex);
        //    if (leadingTokens is not null)
        //    {
        //        foreach (var token in leadingTokens)
        //        {
        //            Console.WriteLine(token);
        //        }
        //    }

        //    return leadingTokens is not null
        //        && leadingTokens.Count(token => token.Text.Contains('\n')) >= 2;
        //}
    }

    internal interface IHasObject
    {
        public GmlSyntaxNode Object { get; set; }
    }
}
