using Gobo.Printer.DocTypes;
using Gobo.SyntaxNodes.PrintHelpers;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class WithStatement : GmlSyntaxNode
{
    public GmlSyntaxNode Object { get; set; }
    public GmlSyntaxNode Body { get; set; }

    public WithStatement(TextSpan span, GmlSyntaxNode @object, GmlSyntaxNode body)
        : base(span)
    {
        Children = [@object, body];
        Object = @object;
        Body = body;
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        return Statement.PrintControlFlowStatement(ctx, "with", Object, Body);
    }
}
