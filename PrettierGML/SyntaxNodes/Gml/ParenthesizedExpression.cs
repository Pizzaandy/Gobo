using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal sealed class ParenthesizedExpression : GmlSyntaxNode
    {
        public GmlSyntaxNode Expression { get; set; }

        public ParenthesizedExpression(TextSpan span, GmlSyntaxNode expression)
            : base(span)
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
}
