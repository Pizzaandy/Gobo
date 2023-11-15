using PrettierGML.Printer.DocTypes;
using PrettierGML.SyntaxNodes.PrintHelpers;

namespace PrettierGML.SyntaxNodes.Gml;

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
        Body = AsChild(body);
        Catch = AsChild(@catch);
        Finally = AsChild(@finally);
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
