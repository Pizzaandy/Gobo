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
            CharacterRange = new Range(node.Symbol.StartIndex, node.Symbol.StopIndex);
            TokenRange = new Range(node.SourceInterval.a, node.SourceInterval.b);
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

            Doc printed;

            if (LeadingComments.Count > 0 && LeadingComments.Any(c => c.IsFormatCommand))
            {
                var formatCommand = LeadingComments.Last(c => c.IsFormatCommand);
                printed = formatCommand.FormatCommandText switch
                {
                    "ignore" => PrintRaw(ctx),
                    _ => PrintNode(ctx)
                };
            }
            else
            {
                printed = PrintNode(ctx);
            }

            if (PrintOwnComments && Comments.Count > 0)
            {
                printed = PrintWithOwnComments(ctx, printed);
            }

            ctx.Stack.Pop();

            return printed;
        }

        public abstract Doc PrintNode(PrintContext ctx);

        public Doc PrintRaw(PrintContext ctx)
        {
            Children.ForEach(child => child.MarkCommentsAsPrinted());

            // TODO: factor out Antlr dependency
            return ctx.Tokens.GetText(
                new Antlr4.Runtime.Misc.Interval(TokenRange.Start, TokenRange.Stop)
            );
        }

        private void MarkCommentsAsPrinted()
        {
            Comments.ForEach(commentGroup => commentGroup.Printed = true);
            Children.ForEach(child => child.MarkCommentsAsPrinted());
        }

        public List<Doc> PrintChildren(PrintContext ctx)
        {
            return Children.Select(child => child.Print(ctx)).ToList();
        }

        // TODO: Move comment logic?

        public virtual Doc PrintLeadingComments(
            PrintContext ctx,
            CommentType asType = CommentType.Leading
        )
        {
            return CommentGroup.PrintGroups(ctx, LeadingComments, asType);
        }

        public virtual Doc PrintTrailingComments(
            PrintContext ctx,
            CommentType asType = CommentType.Trailing
        )
        {
            return CommentGroup.PrintGroups(ctx, TrailingComments, asType);
        }

        public virtual Doc PrintDanglingComments(
            PrintContext ctx,
            CommentType asType = CommentType.Dangling
        )
        {
            // Print dangling comments as leading by default
            return CommentGroup.PrintGroups(ctx, DanglingComments, asType);
        }

        /// <summary>
        /// Wraps a doc in comments attached to the callee.
        /// </summary>
        public virtual Doc PrintWithOwnComments(PrintContext ctx, Doc nodeDoc)
        {
            // Dangling comments should be handled manually
            return Doc.Concat(PrintLeadingComments(ctx), nodeDoc, PrintTrailingComments(ctx));
        }

        public bool EnsureCommentsPrinted(bool isRoot = true)
        {
            var allCommentsPrinted =
                Comments.All(c => c.Printed)
                && Children.All(c => c.EnsureCommentsPrinted(isRoot: false));

            if (!allCommentsPrinted && isRoot)
            {
                var unprintedGroups = GetUnprintedCommentGroups();
                throw new Exception(
                    $"{unprintedGroups.Count} comment groups were not printed.\nComment Groups:\n{string.Join('\n', unprintedGroups)}"
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

        public void TransferComments(GmlSyntaxNode target, Func<CommentGroup, bool> predicate)
        {
            var commentsToTransfer = Comments.Where(predicate);

            Comments = Comments.Where(c => !predicate(c)).ToList();

            target.Comments.AddRange(commentsToTransfer);
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
