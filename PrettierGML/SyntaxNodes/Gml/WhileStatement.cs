﻿using PrettierGML.Printer.DocTypes;
using PrettierGML.SyntaxNodes.PrintHelpers;

namespace PrettierGML.SyntaxNodes.Gml;

internal sealed class WhileStatement : GmlSyntaxNode
{
    public GmlSyntaxNode Test { get; set; }
    public GmlSyntaxNode Body { get; set; }

    public WhileStatement(TextSpan span, GmlSyntaxNode test, GmlSyntaxNode body)
        : base(span)
    {
        Test = AsChild(test);
        Body = AsChild(body);
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        return Statement.PrintControlFlowStatement(ctx, "while", Test, Body);
    }
}
