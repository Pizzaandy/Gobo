﻿using Gobo.Printer.DocTypes;
using Gobo.SyntaxNodes.PrintHelpers;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class GlobalVariableStatement : GmlSyntaxNode
{
    public List<GmlSyntaxNode> Declarations => Children;

    public GlobalVariableStatement(TextSpan span, List<GmlSyntaxNode> declarations)
        : base(span)
    {
        AsChildren(declarations);
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        var parts = new List<Doc>() { "globalvar", " " };

        if (Children.Count == 1)
        {
            parts.Add(Children.First().Print(ctx));
        }
        else if (Children.Count > 0)
        {
            var printedArguments = DelimitedList.Print(ctx, Declarations, ",");
            parts.Add(Doc.Indent(printedArguments));
        }

        return Doc.Group(parts);
    }
}
