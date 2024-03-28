﻿using Gobo.Printer.DocTypes;
using Gobo.SyntaxNodes.PrintHelpers;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class FinallyProduction : GmlSyntaxNode
{
    public GmlSyntaxNode Body { get; set; }

    public FinallyProduction(TextSpan span, GmlSyntaxNode body)
        : base(span)
    {
        Children = [body];
        Body = body;
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        return Doc.Concat("finally", " ", Statement.EnsureStatementInBlock(ctx, Body));
    }
}
