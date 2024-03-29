﻿using Gobo.SyntaxNodes;

namespace Gobo;

internal class PrintContext
{
    public FormatOptions Options { get; init; }
    public string SourceText { get; init; }

    // TODO: consider removing stack from context (it's currently unused)
    public Stack<GmlSyntaxNode> Stack = new();
    public int PrintDepth => Stack.Count;
    public string PrintedStack => $"[{string.Join(", ", Stack.Select(node => node.Kind))}]";

    public PrintContext(FormatOptions options, string sourceText)
    {
        Options = options;
        SourceText = sourceText;
    }
}
