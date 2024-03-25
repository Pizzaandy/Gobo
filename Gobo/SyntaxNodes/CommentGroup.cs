using Gobo.Parser;
using Gobo.Printer.DocTypes;
using System.Text.Json.Serialization;

namespace Gobo.SyntaxNodes;

internal enum CommentType
{
    Leading,
    Trailing,
    Dangling,
}

internal enum CommentPlacement
{
    OwnLine,
    EndOfLine,
    Remaining,
}

internal enum FormatCommandType
{
    None,
    Ignore,
}

/// <summary>
/// Represents a sequence of comments with no line breaks between them.
/// </summary>
internal class CommentGroup
{
    public string Text => string.Concat(CommentTokens.Select(t => t.Text));

    public int Start => Span.Start;
    public int End => Span.End;

    [JsonIgnore]
    public string Id { get; init; }

    public CommentType Type { get; set; }

    public CommentPlacement Placement { get; set; }

    [JsonIgnore]
    public Token[] CommentTokens { get; init; }

    [JsonIgnore]
    public TextSpan Span { get; set; }

    [JsonIgnore]
    public GmlSyntaxNode? EnclosingNode { get; set; }

    [JsonIgnore]
    public GmlSyntaxNode? PrecedingNode { get; set; }

    [JsonIgnore]
    public GmlSyntaxNode? FollowingNode { get; set; }

    /// <summary>
    /// Whether this comment group was ignored during formatting.
    /// </summary>
    [JsonIgnore]
    public bool PrintedRaw = false;

    [JsonIgnore]
    public FormatCommandType FormatCommand { get; init; }

    [JsonIgnore]
    public bool PrintedAsEndOfLine { get; set; } = false;

    private readonly bool endsWithSingleLineComment = false;

    private static readonly string[] newlines = new string[] { "\r\n", "\n" };

    public CommentGroup(Token[] commentTokens)
    {
        CommentTokens = commentTokens;
        Span = new TextSpan(commentTokens[0].StartIndex, commentTokens[^1].EndIndex);
        Id = Guid.NewGuid().ToString();

        for (var i = commentTokens.Length - 1; i >= 0; i--)
        {
            var token = commentTokens[i];
            if (token.Kind is TokenKind.SingleLineComment)
            {
                endsWithSingleLineComment = true;

                var trimmedText = token.Text[2..].Trim();

                FormatCommand = trimmedText switch
                {
                    "fmt-ignore" => FormatCommandType.Ignore,
                    _ => FormatCommandType.None
                };

                break;
            }
        }
    }

    public Doc Print(PrintContext ctx)
    {
        var parts = new List<Doc>();

        foreach (var token in CommentTokens)
        {
            if (token.Kind is TokenKind.SingleLineComment)
            {
                parts.Add(PrintSingleLineComment(ctx, token.Text));
            }
            else if (token.Kind == TokenKind.MultiLineComment)
            {
                parts.Add(PrintMultiLineComment(ctx, token.Text));
                if (token.Kind != CommentTokens[^1].Kind)
                {
                    parts.Add(" ");
                }
            }
        }

        if (endsWithSingleLineComment || Placement is CommentPlacement.EndOfLine)
        {
            parts.Add(Doc.BreakParent);
            PrintedAsEndOfLine = true;
            return Doc.EndOfLineComment(parts, Id);
        }
        else
        {
            return Doc.InlineComment(parts, Id);
        }
    }

    /// <summary>
    /// Prints multiple lines of comments together, accounting for whitespace and padding.
    /// </summary>
    public static Doc PrintGroups(
        PrintContext ctx,
        IEnumerable<CommentGroup> groups,
        CommentType type
    )
    {
        if (!groups.Any())
        {
            return Doc.Null;
        }

        var firstGroup = groups.First();
        var groupDocs = new List<Doc>() { firstGroup.Print(ctx) };

        // Add line breaks between comment groups
        foreach (var group in groups.Skip(1))
        {
            int lineBreaksBetween = ctx.SourceText.GetLineBreaksToLeft(group.Span);

            for (var i = 0; i < Math.Min(lineBreaksBetween, 2); i++)
            {
                groupDocs.Add(Doc.HardLine);
            }

            groupDocs.Add(group.Print(ctx));
        }

        var printedGroups = Doc.Concat(groupDocs);

        if (type == CommentType.Dangling)
        {
            return printedGroups;
        }

        var parts = new List<Doc>();

        // Add leading or trailing line breaks depending on type
        if (type == CommentType.Leading)
        {
            int trailingLineBreakCount = ctx.SourceText.GetLineBreaksToRight(groups.Last().Span);

            if (trailingLineBreakCount == 0)
            {
                return Doc.Concat(printedGroups, " ");
            }

            parts.Add(printedGroups);

            for (var i = 0; i < Math.Min(trailingLineBreakCount, 2); i++)
            {
                parts.Add(Doc.HardLine);
            }
        }
        else
        {
            int leadingLineBreakCount = ctx.SourceText.GetLineBreaksToLeft(firstGroup.Span);

            if (leadingLineBreakCount == 0)
            {
                Doc space = firstGroup.PrintedAsEndOfLine ? Doc.Null : " ";
                return Doc.Concat(space, printedGroups);
            }

            for (var i = 0; i < Math.Min(leadingLineBreakCount, 2); i++)
            {
                parts.Add(Doc.HardLine);
            }

            parts.Add(printedGroups);
        }

        return Doc.Concat(parts);
    }

    public static Doc PrintSingleLineComment(PrintContext ctx, string text)
    {
        return text;
    }

    public static Doc PrintMultiLineComment(PrintContext ctx, string text)
    {
        var lines = text.Split(newlines, StringSplitOptions.None).Select(line => (Doc)line);
        return Doc.Join(Doc.LiteralLine, lines);
    }

    public override string ToString()
    {
        return string.Join(
            '\n',
            $"Text: {string.Concat(CommentTokens.Select(t => t.Text))}",
            $"Type: {Type}",
            $"Placement: {Placement}",
            $"Range: {Span}",
            $"Enclosing: {EnclosingNode?.Kind}",
            $"Preceding: {PrecedingNode?.Kind}",
            $"Following: {FollowingNode?.Kind}",
            $"Id: {Id}\n"
        );
    }
}
