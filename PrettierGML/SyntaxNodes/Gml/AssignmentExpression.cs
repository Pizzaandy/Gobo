using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;
using PrettierGML.SyntaxNodes.PrintHelpers;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal sealed class AssignmentExpression : GmlSyntaxNode
    {
        public string Operator { get; set; }
        public GmlSyntaxNode Left { get; set; }
        public GmlSyntaxNode Right { get; set; }
        public GmlSyntaxNode Type { get; set; }

        public AssignmentExpression(
            TextSpan span,
            string @operator,
            GmlSyntaxNode left,
            GmlSyntaxNode right,
            GmlSyntaxNode type
        )
            : base(span)
        {
            Operator = @operator;
            Left = AsChild(left);
            Right = AsChild(right);
            Type = AsChild(type);
        }

        public override Doc PrintNode(PrintContext ctx)
        {
            var typeAnnotation = Type.Print(ctx);
            return RightHandSide.Print(ctx, Left, Doc.Concat(typeAnnotation, " ", Operator), Right);
        }
    }
}
