using Gobo.SyntaxNodes;

namespace Gobo;

internal sealed class PrintContext
{
    public FormatOptions Options { get; init; }
    public string SourceText { get; init; }

    public Stack<GmlSyntaxNode> Stack = new();
    public int PrintDepth => Stack.Count;
    public string PrintedStack => $"[{string.Join(", ", Stack.Select(node => node.Kind))}]";

    public PrintContext(FormatOptions options, string sourceText)
    {
        Options = options;
        SourceText = sourceText;
    }
}
