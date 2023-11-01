using Antlr4.Runtime;
using PrettierGML.Printer;
using PrettierGML.Printer.Document.DocTypes;

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
            // remove redundant parentheses
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
