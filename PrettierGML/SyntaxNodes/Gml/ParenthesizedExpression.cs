using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal sealed class ParenthesizedExpression : GmlSyntaxNode
    {
        public GmlSyntaxNode Expression { get; set; }

        public ParenthesizedExpression(ParserRuleContext context, GmlSyntaxNode expression)
            : base(context)
        {
            Expression = AsChild(expression);
        }

        public override Doc PrintNode(PrintContext ctx)
        {
            // Remove redundant parentheses
            if (Parent is ParenthesizedExpression)
            {
                return Expression.Print(ctx);
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
