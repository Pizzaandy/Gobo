using Gobo.Printer.DocTypes;
using Gobo.SyntaxNodes.PrintHelpers;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class RepeatStatement : GmlSyntaxNode
{
    public GmlSyntaxNode Test { get; set; }
    public GmlSyntaxNode Body { get; set; }

    public RepeatStatement(TextSpan span, GmlSyntaxNode test, GmlSyntaxNode body)
        : base(span)
    {
        Children = [test, body];
        Test = test;
        Body = body;
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        return Statement.PrintControlFlowStatement(ctx, "repeat", Test, Body);
    }
}
