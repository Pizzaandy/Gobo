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
        Children = [init, test, update, body];
        Init = init;
        Test = test;
        Update = update;
        Body = body;
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        var items = new Doc[]
        {
            Init.Print(ctx),
            ";",
            Test.IsEmpty ? Doc.IfBreak(Doc.Line, Doc.Null) : Doc.Line,
            Test.Print(ctx),
            ";",
            Update.IsEmpty ? Doc.IfBreak(Doc.Line, Doc.Null) : Doc.Line,
            Update.Print(ctx)
        };

        return Doc.Concat(
            "for",
            " ",
            "(",
            Doc.Group(
                Doc.Indent(Doc.IfBreak(Doc.Line, Doc.Null), Doc.Concat(items)),
                Doc.IfBreak(Doc.Line, Doc.Null)
            ),
            ") ",
            Statement.EnsureStatementInBlock(ctx, Body)
        );
    }
}
