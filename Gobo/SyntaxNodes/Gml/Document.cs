using Gobo.Printer.DocTypes;
using Gobo.SyntaxNodes.PrintHelpers;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class Document : GmlSyntaxNode
{
    public GmlSyntaxNode[] Statements => Children;

    public Document(TextSpan span, GmlSyntaxNode[] body)
        : base(span)
    {
        Children = body;
    }

    public Document()
        : base() { }

    public override Doc PrintNode(PrintContext ctx)
    {
        return Doc.Concat(
            PrintDanglingComments(ctx),
            Statement.PrintStatements(ctx, Children),
            Doc.HardLineIfNoPreviousLine
        );
    }
}
