﻿using Gobo.Printer.DocTypes;
using Gobo.SyntaxNodes.PrintHelpers;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class WhileStatement : GmlSyntaxNode
{
    public GmlSyntaxNode Test { get; set; }
    public GmlSyntaxNode Body { get; set; }

    public WhileStatement(TextSpan span, GmlSyntaxNode test, GmlSyntaxNode body)
        : base(span)
    {
        Children = [test, body];
        Test = test;
        Body = body;
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        return Statement.PrintControlFlowStatement(ctx, "while", Test, Body);
    }
}
