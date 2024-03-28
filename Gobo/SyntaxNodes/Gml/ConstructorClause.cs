﻿using Gobo.Printer.DocTypes;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class ConstructorClause : GmlSyntaxNode
{
    public GmlSyntaxNode Id { get; set; }
    public GmlSyntaxNode Arguments { get; set; }

    public ConstructorClause(TextSpan span, GmlSyntaxNode id, GmlSyntaxNode arguments)
        : base(span)
    {
        Children = [id, arguments];
        Id = id;
        Arguments = arguments;
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        if (!Id.IsEmpty)
        {
            return Doc.Concat(" : ", Id.Print(ctx), Arguments.Print(ctx), " ", "constructor");
        }
        else
        {
            return Doc.Concat(" ", "constructor");
        }
    }
}
