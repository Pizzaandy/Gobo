using Gobo.Printer.DocTypes;
using Gobo.SyntaxNodes.PrintHelpers;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class ForStatement : GmlSyntaxNode
{
    public GmlSyntaxNode Init { get; set; }
    public GmlSyntaxNode Test { get; set; }
    public GmlSyntaxNode Update { get; set; }
    public GmlSyntaxNode Body { get; set; }

    public ForStatement(
        TextSpan span,
        GmlSyntaxNode init,
        GmlSyntaxNode test,
        GmlSyntaxNode update,
        GmlSyntaxNode body
    )
        : base(span)
    {
        Init = AsChild(init);
        Test = AsChild(test);
        Update = AsChild(update);
        Body = AsChild(body);
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        // GameMaker cannot parse a for header that spans lines: a '++' after a line break
        // reads as a statement rather than as part of the condition. It stays on one line
        // however long it gets.
        var header = new List<Doc>
        {
            Init.Print(ctx),
            ";",
            Test.IsEmpty ? Doc.Null : " ",
            Test.Print(ctx),
            ";",
            Update.IsEmpty ? Doc.Null : " ",
            Update.Print(ctx)
        };

        return Doc.Concat(
            "for",
            " ",
            "(",
            Doc.ForceFlat(header),
            ") ",
            Statement.EnsureStatementInBlock(ctx, Body)
        );
    }
}
