using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Newtonsoft.Json;
using PrettierGML.Printer.DocTypes;

namespace PrettierGML.SyntaxNodes
{
    internal abstract class GmlSyntaxNode : ISyntaxNode<GmlSyntaxNode>
    {
        [JsonIgnore]
        public Range CharacterRange { get; init; }

        [JsonIgnore]
        public Range TokenRange { get; init; }

        [JsonIgnore]
        public GmlSyntaxNode? Parent { get; set; }

        [JsonIgnore]
        public List<GmlSyntaxNode> Children { get; set; } = new();

        [JsonIgnore]
        public bool IsEmpty => this is EmptyNode;

        public string Kind => GetType().Name;

        public List<CommentGroup> Comments { get; set; } = new();

        [JsonIgnore]
        public bool PrintOwnComments { get; set; } = true;

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

        protected GmlSyntaxNode AsChild(GmlSyntaxNode child)
        {
            Children.Add(child);
            child.Parent = this;
            return child;
        }

        protected List<GmlSyntaxNode> AsChildren(List<GmlSyntaxNode> children)
        {
            foreach (var child in children)
            {
                AsChild(child);
            }
            return children;
        }

        public Doc Print(PrintContext ctx)
        {
            ctx.Stack.Push(this);

            Doc printed = PrintNode(ctx);

            if (PrintOwnComments && Comments.Count > 0)
            {
                printed = WithComments(ctx, this, printed);
            }

            ctx.Stack.Pop();

            return printed;
        }

        public abstract Doc PrintNode(PrintContext ctx);

        public List<Doc> PrintChildren(PrintContext ctx)
        {
            return Children.Select(child => child.Print(ctx)).ToList();
        }

        public virtual Doc PrintLeadingComments(PrintContext ctx)
        {
            return CommentGroup.PrintGroups(ctx, LeadingComments, CommentType.Leading);
        }

        public virtual Doc PrintTrailingComments(PrintContext ctx)
        {
            return CommentGroup.PrintGroups(ctx, TrailingComments, CommentType.Trailing);
        }

        public virtual Doc PrintDanglingComments(PrintContext ctx)
        {
            // Print dangling comments as leading by default
            return CommentGroup.PrintGroups(ctx, DanglingComments, CommentType.Leading);
        }

        public static Doc WithComments(PrintContext ctx, GmlSyntaxNode node, Doc nodeDoc)
        {
            // Print dangling comments as leading by default
            return Doc.Concat(
                node.PrintLeadingComments(ctx),
                node.PrintDanglingComments(ctx),
                nodeDoc,
                node.PrintTrailingComments(ctx)
            );
        }

        public bool EndsWithSingleLineComment()
        {
            if (!Comments.Any())
            {
                return false;
            }
            return TrailingComments.LastOrDefault()?.EndsWithSingleLineComment ?? false;
        }

        public bool EnsureCommentsPrinted(bool isRoot = true)
        {
            var allCommentsPrinted =
                Comments.All(c => c.Printed)
                && Children.All(c => c.EnsureCommentsPrinted(isRoot: false));

            if (!allCommentsPrinted && isRoot)
            {
                var unprintedGroups = GetUnprintedCommentGroups();
                var text = string.Join('\n', unprintedGroups);
                throw new Exception(
                    $"{unprintedGroups.Count} comment groups were not printed.\nComment Groups:\n{text}"
                );
            }

            return allCommentsPrinted;
        }

        public List<CommentGroup> GetUnprintedCommentGroups()
        {
            var unprinted = Comments.Where(c => !c.Printed).ToList();
            foreach (var child in Children)
            {
                unprinted.AddRange(child.GetUnprintedCommentGroups());
            }
            return unprinted;
        }

        public static implicit operator GmlSyntaxNode(List<GmlSyntaxNode> contents) =>
            new NodeList(contents);

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
    }

    internal interface IHasObject
    {
        public GmlSyntaxNode Object { get; set; }
    }
}
