using Gobo.Parser;

namespace Gobo;

internal class PrintContext
{
    public FormatOptions Options { get; init; }
    public SourceText SourceText { get; init; }
    public List<Token[]> TriviaGroups { get; init; }

    public PrintContext(FormatOptions options, SourceText sourceText, List<Token[]> triviaGroups)
    {
        Options = options;
        SourceText = sourceText;
        TriviaGroups = triviaGroups;
    }

    public Token[] GetLeadingTrivia(TextSpan span)
    {
        return TriviaGroups.FirstOrDefault(g => span.Start == g[^1].EndIndex)
            ?? Array.Empty<Token>();
    }

    public Token[] GetTrailingTrivia(TextSpan span)
    {
        return TriviaGroups.FirstOrDefault(g => span.End == g[^1].StartIndex)
            ?? Array.Empty<Token>();
    }
}
