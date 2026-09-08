using System.Text;
using Gobo.Parser;
using Gobo.Text;

namespace Gobo.Transforms;

/// <summary>
/// Rewrites comment text: the space after the slashes, jsDoc tag names and types, tag order,
/// and the column the parameter descriptions line up on. Comments are found with the lexer,
/// so a '//' inside a string is never mistaken for one.
/// </summary>
internal static class CommentPass
{
    private readonly record struct Line(int Start, int End, string Indent, string Body, bool IsDoc);

    public static bool ShouldRun(FormatOptions options)
    {
        return options.SpaceAfterCommentMarker
            || options.WrapComments
            || options.NormalizeJsDocTags
            || options.SortJsDocTags
            || options.AlignJsDocDescriptions
            || options.RemoveCommentedOutCode;
    }

    public static string? Apply(string text, FormatOptions options)
    {
        var comments = ReadComments(text);
        var edits = new List<(int Start, int End, string Replacement)>();
        var index = 0;

        while (index < comments.Count)
        {
            var run = comments.GetRange(index, TakeRun(comments, index));
            index += run.Count;

            var bodies = run.Select(line => line.Body).ToList();
            var indent = run[0].Indent;
            var start = run[0].Start;
            var end = run[^1].End;

            if (options.RemoveCommentedOutCode && !run[0].IsDoc && CommentedOutCode.Detect(bodies))
            {
                // Take the line break with it, so no blank line is left behind.
                var lineEnd = end < text.Length && text[end] == '\n' ? end + 1 : end;
                edits.Add((start, lineEnd, ""));
                continue;
            }

            var rewritten = run[0].IsDoc
                ? RewriteDoc(bodies, indent, options)
                : RewriteProse(bodies, indent, isDoc: false, options);

            var replacement = string.Join("\n", rewritten);

            if (replacement != text[start..end])
            {
                edits.Add((start, end, replacement));
            }
        }

        if (edits.Count == 0)
        {
            return null;
        }

        var builder = new StringBuilder(text);

        foreach (var (start, end, replacement) in edits.OrderByDescending(edit => edit.Start))
        {
            builder.Remove(start, end - start);
            builder.Insert(start, replacement);
        }

        return builder.ToString();
    }

    /// <summary>
    /// Every line comment standing on a line of its own, in source order. One that follows
    /// code on its line is left alone, because rewriting it would move it past that code.
    /// The parser supplies them: the lexer alone cannot tell a comment from the inside of a
    /// template string, because the parser is what drives its modes.
    /// </summary>
    private static List<Line> ReadComments(string text)
    {
        var result = new List<Line>();
        List<Token[]> trivia;

        try
        {
            trivia = new GmlParser(SourceText.From(text)).Parse().TriviaGroups;
        }
        catch (GmlSyntaxErrorException)
        {
            return result;
        }

        foreach (var token in trivia.SelectMany(group => group))
        {
            if (token.Kind is not TokenKind.SingleLineComment)
            {
                continue;
            }

            var lineStart = LineStart(text, token.StartIndex);
            var indent = text[lineStart..token.StartIndex];

            if (indent.Any(character => character is not (' ' or '\t')))
            {
                continue;
            }

            var slashes = CountSlashes(token.Text);
            result.Add(
                new Line(lineStart, token.EndIndex, indent, token.Text[slashes..], slashes == 3)
            );
        }

        result.Sort((left, right) => left.Start.CompareTo(right.Start));
        return result;
    }

    /// <summary>
    /// How many comments from index belong together: adjacent lines sharing an indent and a
    /// marker, with nothing between them.
    /// </summary>
    private static int TakeRun(List<Line> comments, int index)
    {
        var count = 1;

        while (index + count < comments.Count)
        {
            var previous = comments[index + count - 1];
            var next = comments[index + count];

            if (
                next.Indent != previous.Indent
                || next.IsDoc != previous.IsDoc
                || next.Start != previous.End + 1
            )
            {
                break;
            }

            count++;
        }

        return count;
    }

    private static int LineStart(string text, int position)
    {
        var start = position;

        while (start > 0 && text[start - 1] != '\n')
        {
            start--;
        }

        return start;
    }

    private static int CountSlashes(string commentText)
    {
        var count = 0;

        while (count < commentText.Length && count < 3 && commentText[count] == '/')
        {
            count++;
        }

        return count;
    }

    private static List<string> RewriteProse(
        List<string> bodies,
        string indent,
        bool isDoc,
        FormatOptions options
    )
    {
        var marker = isDoc ? "///" : "//";
        var head = $"{indent}{marker} ";
        var lines = new List<string>();

        foreach (var body in bodies)
        {
            var trimmed = body.TrimStart();

            // Leading whitespace of its own is left alone: inside a commented out block it
            // carries the indentation of the code.
            if (
                trimmed.Length == 0
                || CommentText.IsDivider(trimmed)
                || !options.SpaceAfterCommentMarker
                || (body.Length > 0 && char.IsWhiteSpace(body[0]))
            )
            {
                lines.Add($"{indent}{marker}{body}");
                continue;
            }

            lines.AddRange(CommentText.Wrap(head, trimmed, head, options));
        }

        return lines;
    }

    private static List<string> RewriteDoc(
        List<string> bodies,
        string indent,
        FormatOptions options
    )
    {
        if (
            !options.NormalizeJsDocTags
            && !options.SortJsDocTags
            && !options.AlignJsDocDescriptions
        )
        {
            return RewriteProse(bodies, indent, isDoc: true, options);
        }

        var comment = JsDocComment.Parse(bodies);
        comment.Normalize(options);

        return comment
            .Render(options, CommentText.VisualWidth(indent, options))
            .Select(line => indent + line)
            .ToList();
    }
}
