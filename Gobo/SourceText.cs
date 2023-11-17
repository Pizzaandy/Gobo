namespace Gobo;

internal class SourceText
{
    public readonly string Text;

    public readonly string? FilePath;

    public int Length => Text.Length;

    public SourceText(string text, string? filePath = null)
    {
        Text = text;
        FilePath = filePath;
    }

    public string GetSpan(TextSpan span)
    {
        return Text.Substring(span.Start, span.Length);
    }

    public string GetSpan(int start, int end)
    {
        return Text[start..end];
    }

    public int GetLineBreaksToLeft(TextSpan span)
    {
        var start = span.Start;

        if (start <= 0)
        {
            return 0;
        }

        var lineBreakCount = 0;

        for (var index = start - 1; index >= 0; index--)
        {
            var character = Text[index];
            if (character == '\n')
            {
                lineBreakCount++;
            }
            else if (!char.IsWhiteSpace(character))
            {
                break;
            }
        }

        return lineBreakCount;
    }

    public int GetLineBreaksToRight(TextSpan span)
    {
        var end = span.End;

        if (end >= Text.Length - 1)
        {
            return 0;
        }

        var lineBreakCount = 0;

        for (var index = end; index < Text.Length; index++)
        {
            var character = Text[index];
            if (character == '\n')
            {
                lineBreakCount++;
            }
            else if (!char.IsWhiteSpace(character))
            {
                break;
            }
        }

        return lineBreakCount;
    }
}
