using Antlr4.Runtime;
using PrettierGML.Nodes.PrintHelpers;
using PrettierGML.Printer.Document.DocTypes;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class AssignmentExpression : GmlSyntaxNode
    {
        public string Operator { get; set; }
        public GmlSyntaxNode Left { get; set; }
        public GmlSyntaxNode Right { get; set; }
        public GmlSyntaxNode Type { get; set; }

        public AssignmentExpression(
            ParserRuleContext context,
            string @operator,
            GmlSyntaxNode left,
            GmlSyntaxNode right,
            GmlSyntaxNode type
        )
            : base(context)
        {
            Operator = @operator;
            Left = AsChild(left);
            Right = AsChild(right);
            Type = AsChild(type);
        }

        public override Doc Print(PrintContext ctx)
        {
            var typeAnnotation = Type.Print(ctx);
            return RightHandSide.Print(ctx, Left, Doc.Concat(typeAnnotation, " ", Operator), Right);
        }
    }
}
