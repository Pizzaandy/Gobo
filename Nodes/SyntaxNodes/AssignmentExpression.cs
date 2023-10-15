using Antlr4.Runtime;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class AssignmentExpression : GmlSyntaxNode
    {
        public string Operator { get; set; }
        public GmlSyntaxNode Left { get; set; }
        public GmlSyntaxNode Right { get; set; }

        public AssignmentExpression(
            ParserRuleContext context,
            string @operator,
            GmlSyntaxNode left,
            GmlSyntaxNode right
        )
            : base(context)
        {
            Operator = @operator;
            Left = AsChild(left);
            Right = AsChild(right);
        }

        public override Doc Print()
        {
            if (Operator == ":=")
            {
                Operator = "=";
            }
            return RightHandSide.Print(Left, Doc.Concat(" ", Operator), Right);
        }
    }
}
