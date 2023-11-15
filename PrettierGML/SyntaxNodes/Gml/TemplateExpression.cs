﻿using PrettierGML.Printer.DocTypes;

namespace PrettierGML.SyntaxNodes.Gml;

internal sealed class TemplateExpression : GmlSyntaxNode
{
    public GmlSyntaxNode Expression { get; set; }

    public TemplateExpression(TextSpan span, GmlSyntaxNode expression)
        : base(span)
    {
        Expression = AsChild(expression);
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        return Doc.Group(
            "{",
            Doc.Indent(Doc.SoftLine, Expression.Print(ctx)),
            Doc.SoftLine,
            "}"
        );
    }
}
