using PrettierGML.Printer.DocPrinter;
using System.Text;

namespace PrettierGML.Printer.Utilities;

internal static class StringBuilderExtensions
{
    public static bool EndsWithNewLineAndWhitespace(this StringBuilder stringBuilder)
    {
        for (var index = 1; index <= stringBuilder.Length; index++)
        {
            var next = stringBuilder[^index];
            if (next == ' ' || next == '\t')
            {
                continue;
            }
            else if (next == '\n')
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
    }

    public static bool EndsWithSingleLineComment(this StringBuilder stringBuilder)
    {
        int consecutiveSlashes = 0;

        for (var index = 1; index <= stringBuilder.Length; index++)
        {
            var next = stringBuilder[^index];
            if (next == ' ' || next == '\t')
            {
                continue;
            }
            else if (next == '/')
            {
                consecutiveSlashes++;
                if (consecutiveSlashes >= 2)
                {
                    return true;
                }
                continue;
            }
            else if (next == '\n')
            {
                return false;
            }
            else
            {
                return false;
            }
        }

        return false;
    }

    public static int TrimTrailingWhitespace(this StringBuilder stringBuilder)
    {
        if (stringBuilder.Length == 0)
        {
            return 0;
        }

        var trimmed = 0;
        for (; trimmed < stringBuilder.Length; trimmed++)
        {
            if (stringBuilder[^(trimmed + 1)] != ' ' && stringBuilder[^(trimmed + 1)] != '\t')
            {
                break;
            }
        }

        stringBuilder.Length -= trimmed;
        return trimmed;
    }

    public static int TrimTrailingWhitespacePreserveIndent(
        this StringBuilder stringBuilder,
        Indent indent
    )
    {
        if (stringBuilder.Length == 0)
        {
            return 0;
        }

        if (stringBuilder.EndsWithNewLineAndWhitespace())
        {
            var trimmedLength = stringBuilder.TrimTrailingWhitespace();
            stringBuilder.Append(indent.Value);
            return trimmedLength - indent.Length;
        }
        else
        {
            return stringBuilder.TrimTrailingWhitespace();
        }
    }
}
