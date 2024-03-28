using Gobo.Printer.DocTypes;
using Gobo.SyntaxNodes.PrintHelpers;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class ParameterList : GmlSyntaxNode
{
    public GmlSyntaxNode[] Parameters => Children;
    public static Doc EmptyParameters => "()";

    public ParameterList(TextSpan span, GmlSyntaxNode[] parameters)
        : base(span)
    {
        Children = parameters;
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        return DelimitedList.PrintInBrackets(ctx, this, "(", Children, ")", ",");
    }
}
