using Gobo.Printer.DocTypes;
using Gobo.SyntaxNodes.Gml.Literals;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class ParenthesizedExpression : GmlSyntaxNode
{
    public GmlSyntaxNode Expression { get; set; }

    public ParenthesizedExpression(TextSpan span, GmlSyntaxNode expression)
        : base(span)
    {
        Expression = AsChild(expression);
    }

    public bool IsControlFlowArgument()
    {
        return Parent
            is IfStatement
                or WithStatement
                or WhileStatement
                or DoStatement
                or RepeatStatement
                or TryStatement
                or CatchProduction
                or SwitchStatement;
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        // Always remove redundant parens
        if (Parent is ParenthesizedExpression)
        {
            return Expression.Print(ctx);
        }

        // Never unwrap parens for control flow arguments
        if (IsControlFlowArgument())
        {
            return PrintInParens(ctx, Expression);
        }

        // Remove parens from awkward unary expressions like: (!(...))
        if (Expression is UnaryExpression { Argument: ParenthesizedExpression })
        {
            return Expression.Print(ctx);
        }

        // Remove parens from simple expressions like: (123)
        if (Expression is Literal)
        {
            return Expression.Print(ctx);
        }

        return PrintInParens(ctx, Expression);
    }

    public static Doc PrintInParens(PrintContext ctx, GmlSyntaxNode node)
    {
        if (ShouldNotBreak(node) && node.Comments.Count == 0)
        {
            return Doc.Concat("(", node.Print(ctx), ")");
        }

        return Doc.Group("(", Doc.Indent(Doc.SoftLine, node.Print(ctx)), Doc.SoftLine, ")");
    }

    public override int GetHashCode()
    {
        return Expression.GetHashCode();
    }

    private static bool ShouldNotBreak(GmlSyntaxNode node)
    {
        return node is FunctionDeclaration or StructExpression;
    }
}
