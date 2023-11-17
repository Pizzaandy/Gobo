using Gobo.Printer.DocTypes;

namespace Gobo.SyntaxNodes;

/// <summary>
/// A utility node used to build the AST. This should not end up in the final AST!
/// </summary>
internal class NodeList : GmlSyntaxNode
{
    public NodeList(List<GmlSyntaxNode> contents)
    {
        AsChildren(contents);
    }

    public override Doc PrintNode(PrintContext ctx) =>
        throw new NotImplementedException("NodeList cannot be printed directly");
}
