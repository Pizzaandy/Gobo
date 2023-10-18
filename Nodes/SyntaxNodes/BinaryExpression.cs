using Antlr4.Runtime;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class BinaryExpression : GmlSyntaxNode
    {
        public string Operator { get; set; }
        public GmlSyntaxNode Left { get; set; }
        public GmlSyntaxNode Right { get; set; }

        public BinaryExpression(
            ParserRuleContext context,
            CommonTokenStream tokenStream,
            string @operator,
            GmlSyntaxNode left,
            GmlSyntaxNode right
        )
            : base(context, tokenStream)
        {
            Operator = @operator switch
            {
                "and" => "&&",
                "or" => "||",
                "xor" => "^^",
                "<>" => "!=",
                "mod" => "%",
                "=" => "==",
                _ => @operator
            };

            Left = AsChild(left);
            Right = AsChild(right);
        }

        public override Doc Print()
        {
            var docs = PrintBinaryExpression(this);

            if (Parent is IfStatement)
            {
                return Doc.Concat(docs);
            }

            var shouldNotIndent =
                Parent
                    is AssignmentExpression
                        or VariableDeclarator
                        or WhileStatement
                        or ReturnStatement
                        or ParenthesizedExpression
                || Parent?.Parent is MemberIndexExpression
                || (
                    Parent is ConditionalExpression conditionalExpression
                    && conditionalExpression.WhenTrue != this
                    && conditionalExpression.WhenFalse != this
                );

            return shouldNotIndent
                ? Doc.Group(docs)
                : Doc.Group(docs[0], Doc.Indent(docs.Skip(1).ToList()));
        }

        // Because of cross-platform inconsistency with operator precedence in GML, we can't use
        // the same printing strategy as Prettier. Binary expressions not explicitly grouped by
        // parentheses will simply be flattened.
        public static List<Doc> PrintBinaryExpression(GmlSyntaxNode node)
        {
            if (node is not BinaryExpression binaryExpression)
            {
                return new List<Doc> { Doc.Group(node.Print()) };
            }

            var parts = new List<Doc>();

            if (binaryExpression.Left is BinaryExpression leftBinary)
            {
                parts.AddRange(PrintBinaryExpression(leftBinary));
            }
            else
            {
                parts.Add(binaryExpression.Left.Print());
            }

            bool isEqualityExpression =
                binaryExpression.Operator == "==" || binaryExpression.Operator == "!=";

            parts.Add(
                Doc.Concat(" ", binaryExpression.Operator, isEqualityExpression ? " " : Doc.Line)
            );

            if (binaryExpression.Right is BinaryExpression rightBinary)
            {
                parts.AddRange(PrintBinaryExpression(rightBinary));
            }
            else
            {
                parts.Add(binaryExpression.Right.Print());
            }

            return parts;
        }
    }
}
