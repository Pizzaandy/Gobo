namespace Gobo;

internal static class StringExtensions
{
    public static string ReadSpan(this string text, TextSpan span)
    {
        return text.Substring(span.Start, span.Length);
    }

    public static string ReadSpan(this string text, int start, int end)
    {
        return text[start..end];
    }

    public static int GetLineBreaksToLeft(this string text, TextSpan span)
    {
        var start = span.Start - 1;

        if (start <= 0)
        {
            return 0;
        }

        var lineBreakCount = 0;

        for (var index = start; index >= 0; index--)
        {
            var character = text[index];
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

    public static int GetLineBreaksToRight(this string text, TextSpan span)
    {
        var end = span.End;

        if (end >= text.Length - 1)
        {
            return 0;
        }

        var lineBreakCount = 0;

        for (var index = end; index < text.Length; index++)
        {
            var character = text[index];
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
