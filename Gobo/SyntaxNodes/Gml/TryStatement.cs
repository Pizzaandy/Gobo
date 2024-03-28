using Gobo.Printer.DocTypes;
using Gobo.SyntaxNodes.PrintHelpers;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class TryStatement : GmlSyntaxNode
{
    public GmlSyntaxNode Body { get; set; }
    public GmlSyntaxNode Catch { get; set; }
    public GmlSyntaxNode Finally { get; set; }

    public TryStatement(
        TextSpan span,
        GmlSyntaxNode body,
        GmlSyntaxNode @catch,
        GmlSyntaxNode @finally
    )
        : base(span)
    {
        Children = [body, @catch, @finally];
        Body = body;
        Catch = @catch;
        Finally = @finally;
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        Doc leadingWhitespace =
            ctx.Options.BraceStyle == BraceStyle.NewLine
                ? Doc.HardLineIfNoPreviousLine
                : Doc.CollapsedSpace;

        var parts = new List<Doc>
        {
            Doc.Concat("try", " ", Statement.EnsureStatementInBlock(ctx, Body))
        };

        if (!Catch.IsEmpty)
        {
            parts.Add(Doc.Concat(leadingWhitespace, Catch.Print(ctx)));
        }

        if (!Finally.IsEmpty)
        {
            parts.Add(Doc.Concat(leadingWhitespace, Finally.Print(ctx)));
        }

        return Doc.Concat(parts);
    }
}
