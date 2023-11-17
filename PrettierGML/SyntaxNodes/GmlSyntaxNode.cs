using PrettierGML.Printer.DocTypes;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PrettierGML.SyntaxNodes;

internal abstract partial class GmlSyntaxNode : ISyntaxNode<GmlSyntaxNode>
{
    public string Kind => GetType().Name;

    public TextSpan Span { get; set; }

    [JsonIgnore]
    public List<CommentGroup> Comments { get; set; } = new();

    [JsonIgnore]
    public GmlSyntaxNode? Parent { get; set; }

    [JsonIgnore]
    public List<GmlSyntaxNode> Children { get; set; } = new();

    [JsonIgnore]
    public bool IsEmpty => this is EmptyNode;

    [JsonIgnore]
    public bool PrintOwnComments { get; set; } = true;

    [JsonIgnore]
    public List<CommentGroup> LeadingComments =>
        Comments.Where(g => g.Type == CommentType.Leading).ToList();

    [JsonIgnore]
    public List<CommentGroup> TrailingComments =>
        Comments.Where(g => g.Type == CommentType.Trailing).ToList();

    [JsonIgnore]
    public List<CommentGroup> DanglingComments =>
        Comments.Where(g => g.Type == CommentType.Dangling).ToList();

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

        if (LeadingComments.Any(c => c.IsFormatCommand))
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

    public Doc PrintRaw(PrintContext ctx)
    {
        Children.ForEach(child => child.MarkCommentsAsPrinted());
        return ctx.SourceText.GetSpan(Span);
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
        return JsonSerializer.Serialize(this, SyntaxNodeSerializerContext.Default.GmlSyntaxNode);
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

    public static implicit operator GmlSyntaxNode(List<GmlSyntaxNode> contents) =>
        new NodeList(contents);

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

    private void MarkCommentsAsPrinted()
    {
        Comments.ForEach(commentGroup => commentGroup.PrintedRaw = true);
        Children.ForEach(child => child.MarkCommentsAsPrinted());
    }
}
