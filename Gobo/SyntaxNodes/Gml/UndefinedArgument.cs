﻿using Gobo.Printer.DocTypes;

namespace Gobo.SyntaxNodes.Gml;

internal class UndefinedArgument : EmptyNode
{
    public static new UndefinedArgument Instance { get; } = new();

    public override Doc PrintNode(PrintContext ctx)
    {
        return Literal.Undefined;
    }

    public override int GetHashCode()
    {
        return Literal.Undefined.GetHashCode();
    }
}