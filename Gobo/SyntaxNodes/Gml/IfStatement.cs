﻿using Gobo.Printer.DocTypes;
using Gobo.SyntaxNodes.PrintHelpers;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class IfStatement : GmlSyntaxNode
{
    public GmlSyntaxNode Test { get; set; }
    public GmlSyntaxNode Consequent { get; set; }
    public GmlSyntaxNode Alternate { get; set; }

    public IfStatement(
        TextSpan span,
        GmlSyntaxNode test,
        GmlSyntaxNode consequent,
        GmlSyntaxNode alternate
    )
        : base(span)
    {
        Children = [test, consequent, alternate];
        Test = test;
        Consequent = consequent;
        Alternate = alternate;
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        var parts = new List<Doc>
        {
            Statement.PrintControlFlowStatement(ctx, "if", Test, Consequent)
        };

        if (Alternate is not EmptyNode)
        {
            Doc leadingWhitespace =
                ctx.Options.BraceStyle == BraceStyle.NewLine ? Doc.HardLineIfNoPreviousLine : " ";

            parts.Add(Doc.Concat(leadingWhitespace, "else", " "));

            if (Alternate is IfStatement)
            {
                parts.Add(Alternate.Print(ctx));
            }
            else
            {
                parts.Add(Statement.EnsureStatementInBlock(ctx, Alternate));
            }
        }

        return Doc.Concat(parts);
    }
}
