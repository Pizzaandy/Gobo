using PrettierGML.Printer.DocTypes;
using PrettierGML.SyntaxNodes.PrintHelpers;

namespace PrettierGML.SyntaxNodes.Gml;

internal sealed class WithStatement : GmlSyntaxNode
{
    public GmlSyntaxNode Object { get; set; }
    public GmlSyntaxNode Body { get; set; }

    public WithStatement(TextSpan span, GmlSyntaxNode @object, GmlSyntaxNode body)
        : base(span)
    {
        Object = AsChild(@object);
        Body = AsChild(body);
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        return Statement.PrintControlFlowStatement(ctx, "with", Object, Body);
    }
}
