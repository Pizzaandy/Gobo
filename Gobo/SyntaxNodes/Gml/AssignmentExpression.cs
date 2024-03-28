﻿using Gobo.Printer.DocTypes;
using Gobo.SyntaxNodes.PrintHelpers;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class AssignmentExpression : GmlSyntaxNode
{
    public string Operator { get; set; }
    public GmlSyntaxNode Left { get; set; }
    public GmlSyntaxNode Right { get; set; }
    public GmlSyntaxNode Type { get; set; }

    public AssignmentExpression(
        TextSpan span,
        string @operator,
        GmlSyntaxNode left,
        GmlSyntaxNode right,
        GmlSyntaxNode type
    )
        : base(span)
    {
        Children = [left, right, type];

        if (@operator == ":=")
        {
            @operator = "=";
        }
        Operator = @operator;
        Left = left;
        Right = right;
        Type = type;
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        var typeAnnotation = Type.Print(ctx);
        return RightHandSide.Print(ctx, Left, Doc.Concat(typeAnnotation, " ", Operator), Right);
    }
}
