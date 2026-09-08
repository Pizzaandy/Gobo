using Gobo.Printer.DocTypes;
using Gobo.SyntaxNodes.PrintHelpers;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class IfStatement : GmlSyntaxNode
{
    public GmlSyntaxNode Test { get; set; }
    public GmlSyntaxNode Consequent { get; set; }
    public GmlSyntaxNode Alternate { get; set; }

    /// <summary>
    /// Region directives written between the consequent and the 'else'.
    /// </summary>
    public List<GmlSyntaxNode> TrailingRegions { get; set; }

    public IfStatement(
        TextSpan span,
        GmlSyntaxNode test,
        GmlSyntaxNode consequent,
        GmlSyntaxNode alternate,
        List<GmlSyntaxNode> trailingRegions
    )
        : base(span)
    {
        Test = AsChild(test);
        Consequent = AsChild(consequent);
        TrailingRegions = AsChildren(trailingRegions);
        Alternate = AsChild(alternate);
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        var parts = new List<Doc>
        {
            Statement.PrintControlFlowStatement(ctx, "if", Test, Consequent)
        };

        // Keep what sits between the branches, in source order.
        var trailing = TrailingRegions
            .Select(region => (region.Span.Start, Doc: region.Print(ctx)))
            .Concat(DanglingComments.Select(group => (group.Span.Start, Doc: group.Print(ctx))))
            .OrderBy(item => item.Start)
            .ToList();

        foreach (var item in trailing)
        {
            parts.Add(Doc.HardLine);
            parts.Add(item.Doc);
        }

        if (Alternate is not EmptyNode)
        {
            var elseOnNewLine =
                ctx.Options.ElseOnNewLine
                || ctx.Options.BraceStyle != BraceStyle.SameLine
                || trailing.Count > 0;

            Doc leadingWhitespace = elseOnNewLine ? Doc.HardLineIfNoPreviousLine : " ";

            // A comment between 'else' and its branch is printed ahead of the keyword, so the
            // two never end up on separate lines.
            Alternate.PrintOwnComments = false;

            parts.Add(Alternate.PrintLeadingComments(ctx));
            parts.Add(Doc.Concat(leadingWhitespace, "else", " "));

            parts.Add(
                Alternate is IfStatement
                    ? Alternate.Print(ctx)
                    : Statement.EnsureStatementInBlock(ctx, Alternate)
            );

            parts.Add(Alternate.PrintTrailingComments(ctx));
        }

        return Doc.Concat(parts);
    }
}
