namespace Gobo.Transforms;

/// <summary>
/// Measuring and breaking comment text. Plain string work, so that what counts as a tab stop
/// here matches what the printer does.
/// </summary>
internal static class CommentText
{
    public static int VisualWidth(string text, FormatOptions options)
    {
        var width = 0;

        foreach (var character in text)
        {
            width =
                character == '\t'
                    ? ((width / options.TabWidth) + 1) * options.TabWidth
                    : width + 1;
        }

        return width;
    }

    /// <summary>
    /// A comment of nothing but punctuation is a divider, and is left exactly as drawn.
    /// </summary>
    public static bool IsDivider(string body)
    {
        if (body.Length == 0)
        {
            return false;
        }

        foreach (var character in body)
        {
            if (char.IsLetterOrDigit(character) || character == ' ')
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Breaks text that runs past the print width. The first line keeps its own start; every
    /// line after it begins with continuation, so it reads as more of the line above.
    /// </summary>
    public static List<string> Wrap(
        string head,
        string body,
        string continuation,
        FormatOptions options
    )
    {
        if (!options.WrapComments || VisualWidth(head + body, options) <= options.Width)
        {
            return new List<string> { (head + body).TrimEnd() };
        }

        var lines = new List<string>();
        var start = head;
        var available = options.Width - VisualWidth(head, options);
        var current = "";

        foreach (var word in body.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            if (current.Length > 0 && current.Length + 1 + word.Length > available)
            {
                lines.Add(start + current);
                start = continuation;
                available = options.Width - VisualWidth(continuation, options);
                current = word;
                continue;
            }

            current += current.Length == 0 ? word : " " + word;
        }

        lines.Add(start + current);
        return lines;
    }
}
