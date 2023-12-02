using System.Text;

namespace Gobo;

public static class DiffChecker
{
    public static string PrintFirstDifference(TextReader expected, TextReader actual)
    {
        using var expectedReader = expected;
        using var actualReader = actual;

        var expectedLine = expectedReader.ReadLine();
        var actualLine = actualReader.ReadLine();
        var line = 1;
        string? previousExpectedLine = null;
        string? previousActualLine = null;
        var stringBuilder = new StringBuilder();

        while (expectedLine != null || actualLine != null)
        {
            if (expectedLine == actualLine)
            {
                line++;
                previousExpectedLine = expectedLine;
                previousActualLine = actualLine;
                expectedLine = expectedReader.ReadLine();
                actualLine = actualReader.ReadLine();
                continue;
            }

            stringBuilder.AppendLine(
                $"----------------------------- Expected: Around Line {line} -----------------------------"
            );
            if (previousExpectedLine != null)
            {
                stringBuilder.AppendLine(MakeWhiteSpaceVisible(previousExpectedLine));
            }
            stringBuilder.AppendLine(MakeWhiteSpaceVisible(expectedLine));
            var nextExpectedLine = expectedReader.ReadLine();
            if (nextExpectedLine != null)
            {
                stringBuilder.AppendLine(MakeWhiteSpaceVisible(nextExpectedLine));
            }

            stringBuilder.AppendLine(
                $"----------------------------- Actual: Around Line {line} -----------------------------"
            );
            if (previousActualLine != null)
            {
                stringBuilder.AppendLine(MakeWhiteSpaceVisible(previousActualLine));
            }
            stringBuilder.AppendLine(MakeWhiteSpaceVisible(actualLine));
            var nextActualLine = actualReader.ReadLine();
            if (nextActualLine != null)
            {
                stringBuilder.AppendLine(MakeWhiteSpaceVisible(nextActualLine));
            }

            return stringBuilder.ToString();
        }

        return string.Empty;
    }

    public static string PrintFirstDifference(string actual, string expected)
    {
        return PrintFirstDifference(new StringReader(expected), new StringReader(actual));
    }

    private static string? MakeWhiteSpaceVisible(string? value)
    {
        return value?.Replace(' ', '·').Replace('\t', '→');
    }
}
