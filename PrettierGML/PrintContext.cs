﻿using PrettierGML.SyntaxNodes;

namespace PrettierGML;

internal class PrintContext
{
    public FormatOptions Options { get; init; }
    public SourceText SourceText { get; init; }
    public Stack<GmlSyntaxNode> Stack = new();
    public int PrintDepth => Stack.Count;
    public string PrintedStack => $"[{string.Join(", ", Stack.Select(node => node.Kind))}]";

    public PrintContext(FormatOptions options, SourceText sourceText)
    {
        Options = options;
        SourceText = sourceText;
    }
}
