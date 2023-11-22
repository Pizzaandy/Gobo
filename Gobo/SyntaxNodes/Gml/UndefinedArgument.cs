using Gobo.Printer.DocTypes;
using Gobo.SyntaxNodes.Gml.Literals;

namespace Gobo.SyntaxNodes.Gml;

internal class UndefinedArgument : GmlSyntaxNode
{
    public UndefinedArgument(int position)
        : base(new TextSpan(position, position)) { }

    public override Doc PrintNode(PrintContext ctx)
    {
        return UndefinedLiteral.Undefined;
    }

    public override int GetHashCode()
    {
        return UndefinedLiteral.HashCode;
    }
}
