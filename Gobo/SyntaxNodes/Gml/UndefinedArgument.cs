using Gobo.Printer.DocTypes;
using Gobo.SyntaxNodes.Gml.Literals;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class UndefinedArgument : GmlSyntaxNode
{
    public UndefinedArgument(int position)
        : base(new TextSpan(position, position)) { }

    public override Doc PrintNode(PrintContext ctx)
    {
        return Doc.Null;
    }

    public override int GetHashCode()
    {
        return UndefinedLiteral.HashCode;
    }
}
