using Gobo.Printer.DocTypes;

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
                or SwitchStatement
                or ConditionalExpression;
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        // Always unwrap redundant parens
        if (Parent is ParenthesizedExpression)
        {
            return Expression.Print(ctx);
        }

        // Never unwrap parens for control flow arguments
        if (IsControlFlowArgument())
        {
            return PrintInParens(ctx, Expression);
        }

        // Fully unwrap parens when not syntactically meaningful
        var trueExpression = Expression;
        while (trueExpression is ParenthesizedExpression parenthesizedExpression)
        {
            trueExpression = parenthesizedExpression.Expression;
        }

        if (
            !(
                trueExpression
                is BinaryExpression
                    or FunctionDeclaration
                    or ConditionalExpression
                    or UnaryExpression
            )
        )
        {
            return Expression.Print(ctx);
        }

        // Remove parens from awkward unary expressions like: (!(...))
        if (
            trueExpression is UnaryExpression { Argument: ParenthesizedExpression }
            && !(Parent is UnaryExpression or CallExpression)
        )
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
        return node is FunctionDeclaration or StructExpression or ConditionalExpression;
    }
}
