using Gobo.Printer.DocTypes;
using Gobo.SyntaxNodes.PrintHelpers;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class Document : GmlSyntaxNode
{
    public List<GmlSyntaxNode> Statements => Children;

    public Document(TextSpan span, List<GmlSyntaxNode> body)
        : base(span)
    {
        AsChildren(body);
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
