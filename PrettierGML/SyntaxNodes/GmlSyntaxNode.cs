using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Newtonsoft.Json;
using PrettierGML.Printer.DocTypes;

namespace PrettierGML.SyntaxNodes
{
    internal interface ISyntaxTree
    {
        public Range CharacterRange { get; init; }
    }

    internal abstract class GmlSyntaxNode : ISyntaxTree
    {
        [JsonIgnore]
        public Range CharacterRange { get; init; }

        [JsonIgnore]
        public Range TokenRange { get; init; }

        [JsonIgnore]
        public GmlSyntaxNode? Parent { get; protected set; }

        [JsonIgnore]
        public List<GmlSyntaxNode> Children { get; protected set; } = new();

        public List<GmlToken> Tokens { get; protected set; } = new();

        [JsonIgnore]
        public bool IsEmpty => this is EmptyNode;

        public string Kind => GetType().Name;

        public List<CommentGroup> Comments { get; set; } = new();

        [JsonIgnore]
        public List<CommentGroup> LeadingComments =>
            Comments.Where(c => c.Type == CommentType.Leading).ToList();

        [JsonIgnore]
        public List<CommentGroup> TrailingComments =>
            Comments.Where(c => c.Type == CommentType.Trailing).ToList();

        [JsonIgnore]
        public List<CommentGroup> DanglingComments =>
            Comments.Where(c => c.Type == CommentType.Dangling).ToList();

        public GmlSyntaxNode() { }

        public GmlSyntaxNode(ParserRuleContext node)
        {
            CharacterRange = new Range(node.Start.StartIndex, node.Stop.StopIndex);
            TokenRange = new Range(node.SourceInterval.a, node.SourceInterval.b);
        }

        public GmlSyntaxNode(ITerminalNode node)
        {
            CharacterRange = new Range(node.SourceInterval.a, node.SourceInterval.b);
            TokenRange = new Range(node.Symbol.StartIndex, node.Symbol.StartIndex);
        }

        public static EmptyNode Empty => EmptyNode.Instance;

        public GmlSyntaxNode AsChild(GmlSyntaxNode child)
        {
            Children.Add(child);
            child.Parent = this;
            return child;
        }

        public GmlToken AsChild(GmlToken child)
        {
            Tokens.Add(child);
            return child;
        }

        public List<GmlSyntaxNode> AsChildren(List<GmlSyntaxNode> children)
        {
            foreach (var child in children)
            {
                AsChild(child);
            }
            return children;
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

            foreach (var token in Tokens)
            {
                hashCode.Add(token);
            }

            return hashCode.ToHashCode();
        }

        public virtual Doc PrintLeadingComments(PrintContext ctx)
        {
            if (LeadingComments.Count == 0)
            {
                return Doc.Null;
            }

            var printedGroup = Doc.Join(
                Doc.Concat(Doc.HardLine, Doc.HardLine),
                LeadingComments.Select(c => c.Print()).ToList()
            );

            var lineBreaksBetween =
                ctx.Tokens
                    .GetHiddenTokensToRight(LeadingComments.Last().TokenRange.Stop)
                    ?.Count(token => token.Type == GameMakerLanguageLexer.LineTerminator) ?? 0;

            if (lineBreaksBetween == 0)
            {
                return Doc.Concat(printedGroup, " ");
            }

            var parts = new List<Doc>() { printedGroup };

            for (var i = 0; i < Math.Min(lineBreaksBetween, 2); i++)
            {
                parts.Add(Doc.HardLine);
            }

            return Doc.Concat(parts);
        }

        public virtual Doc PrintTrailingComments(PrintContext ctx)
        {
            if (TrailingComments.Count == 0)
            {
                return Doc.Null;
            }

            var printedGroup = Doc.Concat(TrailingComments.Select(c => c.Print()).ToList());

            var lineBreaksBetween =
                ctx.Tokens
                    .GetHiddenTokensToLeft(TrailingComments.First().TokenRange.Start)
                    ?.Count(token => token.Type == GameMakerLanguageLexer.LineTerminator) ?? 0;

            if (lineBreaksBetween == 0)
            {
                return Doc.Concat(" ", printedGroup);
            }

            var parts = new List<Doc>();

            for (var i = 0; i < Math.Min(lineBreaksBetween, 2); i++)
            {
                parts.Add(Doc.HardLine);
            }

            parts.Add(printedGroup);

            return Doc.Concat(parts);
        }

        public virtual Doc PrintDanglingComments(PrintContext ctx) =>
            throw new NotImplementedException();

        public static implicit operator GmlSyntaxNode(List<GmlSyntaxNode> contents) =>
            new NodeList(contents);
    }

    internal interface IHasObject
    {
        public GmlSyntaxNode Object { get; set; }
    }
}
