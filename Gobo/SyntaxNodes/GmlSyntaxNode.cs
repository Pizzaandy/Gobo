using Gobo.Printer.DocTypes;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Gobo.SyntaxNodes;

internal abstract partial class GmlSyntaxNode : ISyntaxNode<GmlSyntaxNode>
{
    public string Kind => GetType().Name;

    public int Start => Span.Start;
    public int End => Span.End;

    [JsonIgnore]
    public TextSpan Span { get; set; }

    public List<CommentGroup> Comments { get; set; } = new();

    [JsonIgnore]
    public GmlSyntaxNode? Parent { get; set; }

    [JsonIgnore]
    public GmlSyntaxNode[] Children
    {
        get => children;
        init
        {
            children = value;
            foreach (var child in children)
            {
                child.Parent = this;
            }
        }
    }

    [JsonIgnore]
    public bool IsEmpty => this is EmptyNode;

    [JsonIgnore]
    public bool PrintOwnComments { get; set; } = true;

    [JsonIgnore]
    public IEnumerable<CommentGroup> LeadingComments =>
        Comments.Where(g => g.Type == CommentType.Leading);

    [JsonIgnore]
    public IEnumerable<CommentGroup> TrailingComments =>
        Comments.Where(g => g.Type == CommentType.Trailing);

    [JsonIgnore]
    public IEnumerable<CommentGroup> DanglingComments =>
        Comments.Where(g => g.Type == CommentType.Dangling);

    private GmlSyntaxNode[] children = Array.Empty<GmlSyntaxNode>();

    public GmlSyntaxNode() { }

    public GmlSyntaxNode(TextSpan textSpan)
    {
        Span = textSpan;
    }

    public static EmptyNode Empty => EmptyNode.Instance;

    public abstract Doc PrintNode(PrintContext ctx);

    public Doc Print(PrintContext ctx)
    {
        ctx.Stack.Push(this);

        Doc printed;

        var hasComments = Comments.Count > 0;

        if (hasComments)
        {
            var formatCommandComment = LeadingComments.LastOrDefault(
                c => c.FormatCommand is not FormatCommandType.None
            );

            if (formatCommandComment is not null)
            {
                printed = formatCommandComment.FormatCommand switch
                {
                    FormatCommandType.Ignore => PrintRaw(ctx),
                    _ => PrintNode(ctx)
                };
            }
            else
            {
                printed = PrintNode(ctx);
            }
        }
        else
        {
            printed = PrintNode(ctx);
        }

        if (hasComments && PrintOwnComments)
        {
            printed = PrintWithOwnComments(ctx, printed);
        }

        ctx.Stack.Pop();

        return printed;
    }

    public Doc PrintRaw(PrintContext ctx)
    {
        Children.ToList().ForEach(child => child.MarkCommentsAsPrinted());
        return ctx.SourceText.ReadSpan(Span).ReplaceLineEndings("\n");
    }

    public Doc[] PrintChildren(PrintContext ctx)
    {
        return Children.Select(child => child.Print(ctx)).ToArray();
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

    public List<CommentGroup> GetFormattedCommentGroups()
    {
        var unprinted = Comments.Where(c => !c.PrintedRaw).ToList();
        foreach (var child in Children)
        {
            unprinted.AddRange(child.GetFormattedCommentGroups());
        }
        return unprinted;
    }

    public override string ToString()
    {
        var result = JsonSerializer.Serialize(
            this,
            SyntaxNodeSerializerContext.Default.GmlSyntaxNode
        );
        return result;
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

    private void MarkCommentsAsPrinted()
    {
        Comments.ForEach(commentGroup => commentGroup.PrintedRaw = true);
        Children.ToList().ForEach(child => child.MarkCommentsAsPrinted());
    }
}
