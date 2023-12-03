using Gobo.Printer.DocTypes;
using Gobo.SyntaxNodes.Gml;
using Gobo.SyntaxNodes.Gml.Literals;

namespace Gobo.SyntaxNodes.PrintHelpers;

internal class RightHandSide
{
    private enum Layout
    {
        BasicConcatWithoutLine,
        BasicConcatWithSpace,
        BreakAfterOperator,
        Chain,
        ChainTail,
        Fluid,
    }

    public static Doc Print(
        PrintContext ctx,
        GmlSyntaxNode leftNode,
        Doc operatorDoc,
        GmlSyntaxNode rightNode
    )
    {
        var layout = DetermineLayout(leftNode, rightNode);
        var groupId = Guid.NewGuid().ToString();

        var leftDoc = leftNode.Print(ctx);
        var rightDoc = rightNode.Print(ctx);

        return layout switch
        {
            Layout.BasicConcatWithoutLine => Doc.Concat(leftDoc, operatorDoc, rightDoc),
            Layout.BasicConcatWithSpace => Doc.Concat(leftDoc, operatorDoc, " ", rightDoc),
            Layout.BreakAfterOperator
                => Doc.Group(
                    Doc.Group(leftDoc),
                    operatorDoc,
                    Doc.Group(Doc.Indent(Doc.Line, rightDoc))
                ),
            Layout.Chain => Doc.Concat(Doc.Group(leftDoc), operatorDoc, Doc.Line, rightDoc),
            Layout.ChainTail
                => Doc.Concat(Doc.Group(leftDoc), operatorDoc, Doc.Indent(Doc.Line, rightDoc)),
            Layout.Fluid
                => Doc.Group(
                    Doc.Group(leftDoc),
                    operatorDoc,
                    Doc.GroupWithId(groupId, Doc.Indent(Doc.Line)),
                    Doc.IndentIfBreak(rightDoc, groupId)
                ),
            _ => throw new Exception($"The layout type of {layout} was not handled.")
        };
    }

    private static Layout DetermineLayout(GmlSyntaxNode leftNode, GmlSyntaxNode rightNode)
    {
        return rightNode switch
        {
            Literal => Layout.BasicConcatWithSpace,
            BinaryExpression => Layout.BreakAfterOperator,
            _ => Layout.Fluid
        };
    }
}
