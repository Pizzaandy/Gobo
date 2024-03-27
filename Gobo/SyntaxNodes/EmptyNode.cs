using Gobo.Printer.DocTypes;

namespace Gobo.SyntaxNodes;

internal class EmptyNode : GmlSyntaxNode
{
    public EmptyNode()
        : base()
    {
        IsEmpty = true;
    }

    public static EmptyNode Instance { get; } = new();

    public override Doc PrintNode(PrintContext ctx)
    {
        return Doc.Null;
    }
}
