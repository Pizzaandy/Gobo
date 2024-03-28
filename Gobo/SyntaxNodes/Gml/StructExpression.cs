using Gobo.Printer.DocTypes;
using Gobo.SyntaxNodes.PrintHelpers;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class StructExpression : GmlSyntaxNode
{
    public GmlSyntaxNode[] Properties => Children;
    public static string EmptyStruct => "{}";

    public StructExpression(TextSpan span, GmlSyntaxNode[] properties)
        : base(span)
    {
        Children = properties;
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        if (Children.Length == 0 && !DanglingComments.Any())
        {
            return EmptyStruct;
        }
        else
        {
            return DelimitedList.PrintInBrackets(
                ctx,
                this,
                "{",
                Children,
                "}",
                ",",
                allowTrailingSeparator: true
            );
        }
    }
}
