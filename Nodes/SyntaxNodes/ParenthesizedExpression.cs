using Antlr4.Runtime;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class ParenthesizedExpression : GmlSyntaxNode
    {
        public GmlSyntaxNode Expression { get; set; }

        public ParenthesizedExpression(ParserRuleContext context, GmlSyntaxNode expression)
            : base(context)
        {
            Expression = AsChild(expression);
        }

        public override Doc Print(PrintContext ctx)
        {
            // remove redundant parens
            while (Expression is ParenthesizedExpression other)
            {
                Expression = other.Expression;
            }
            return Doc.Group(
                "(",
                Doc.Indent(Doc.SoftLine, Expression.Print(ctx)),
                Doc.SoftLine,
                ")"
            );
        }

        public override int GetHashCode()
        {
            return Expression.GetHashCode();
        }
    }
}
