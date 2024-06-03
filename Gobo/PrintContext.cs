namespace Gobo;

internal class PrintContext
{
    public FormatOptions Options { get; init; }
    public string SourceText { get; init; }

    public PrintContext(FormatOptions options, string sourceText)
    {
        Options = options;
        SourceText = sourceText;
    }
}
