using Gobo.Printer.DocTypes;
using Gobo.SyntaxNodes.PrintHelpers;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class ParameterList : GmlSyntaxNode
{
    public List<GmlSyntaxNode> Parameters => Children;
    public static Doc EmptyParameters => "()";

    public ParameterList(TextSpan span, List<GmlSyntaxNode> parameters)
        : base(span)
    {
        AsChildren(parameters);
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        return DelimitedList.PrintInBrackets(ctx, "(", this, ")", ",");
    }
}
