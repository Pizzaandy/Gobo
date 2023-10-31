﻿using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Newtonsoft.Json;

namespace PrettierGML.Nodes
{
    internal abstract class GmlSyntaxNode
    {
        [JsonIgnore]
        public Range CharacterRange { get; init; }

        [JsonIgnore]
        public Range TokenRange { get; init; }

        [JsonIgnore]
        public GmlSyntaxNode? Parent { get; protected set; }

        [JsonIgnore]
        public List<GmlSyntaxNode> Children { get; protected set; } = new();

        [JsonIgnore]
        public bool IsEmpty => this is EmptyNode;

        public List<CommentGroup> Comments { get; set; } = new();

        [JsonIgnore]
        public virtual bool PrintsOwnComments { get; } = false;

        public string Kind => GetType().Name;

        [JsonIgnore]
        public bool HasLeadingComments => LeadingComments.Any();

        [JsonIgnore]
        public bool HasTrailingComments => TrailingComments.Any();

        [JsonIgnore]
        public bool HasDanglingComments => DanglingComments.Any();

        protected IEnumerable<CommentGroup> LeadingComments =>
            Comments.Where(c => c.Type == CommentType.Leading);

        protected IEnumerable<CommentGroup> TrailingComments =>
            Comments.Where(c => c.Type == CommentType.Trailing);

        protected IEnumerable<CommentGroup> DanglingComments =>
            Comments.Where(c => c.Type == CommentType.Dangling);

        public GmlSyntaxNode() { }

        public GmlSyntaxNode(ParserRuleContext node)
        {
            CharacterRange = new Range(node.Start.StartIndex, node.Stop.StopIndex);
            TokenRange = new Range(node.SourceInterval.a, node.SourceInterval.b);
        }

        public GmlSyntaxNode(ITerminalNode token)
        {
            CharacterRange = new Range(token.SourceInterval.a, token.SourceInterval.b);
            TokenRange = new Range(token.Symbol.StartIndex, token.Symbol.StartIndex);
        }

        public static EmptyNode Empty => EmptyNode.Instance;

        public static NodeList List(ParserRuleContext context, IList<GmlSyntaxNode> contents) =>
            new(context, contents);

        public GmlSyntaxNode AsChild(GmlSyntaxNode child)
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

        public virtual Doc PrintLeadingComments(PrintContext ctx)
        {
            if (!HasLeadingComments)
            {
                return Doc.Null;
            }

            return Doc.Concat(
                Doc.Concat(LeadingComments.Select(c => c.Print(ctx)).ToList()),
                Doc.HardLine
            );
        }

        public virtual Doc PrintTrailingComments(PrintContext ctx)
        {
            if (!HasTrailingComments)
            {
                return Doc.Null;
            }
            return Doc.Concat(TrailingComments.Select(c => c.Print(ctx)).ToList());
        }

        public virtual Doc PrintDanglingComments(PrintContext ctx) =>
            throw new NotImplementedException();
    }

    internal interface IHasObject
    {
        public GmlSyntaxNode Object { get; set; }
    }
}
