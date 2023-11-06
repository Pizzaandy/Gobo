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

            if (ShouldNotBreak(Expression) && !Expression.Comments.Any())
            {
                return Doc.Concat("(", Expression.Print(ctx), ")");
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

        private static bool ShouldNotBreak(GmlSyntaxNode node)
        {
            return node is FunctionDeclaration or StructExpression;
        }
    }
}
