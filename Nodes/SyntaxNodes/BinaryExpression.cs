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
            string @operator,
            GmlSyntaxNode left,
            GmlSyntaxNode right
        )
            : base(context)
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
            var shouldNotIndent = Parent is AssignmentExpression or VariableDeclarator;

            if (Parent is IfStatement)
            {
                return Doc.Concat(docs);
            }

            return shouldNotIndent
                ? Doc.Group(docs)
                : Doc.Group(docs[0], Doc.Indent(docs.Skip(1).ToList()));
        }

        public static List<Doc> PrintBinaryExpression(GmlSyntaxNode node)
        {
            if (node is not BinaryExpression binaryExpression)
            {
                return new List<Doc> { Doc.Group(node.Print()) };
            }

            var parts = new List<Doc>();

            var shouldGroup =
                binaryExpression.Parent is BinaryExpression parentExpression
                && GetPrecedence(binaryExpression) != GetPrecedence(parentExpression)
                && binaryExpression.Left is not BinaryExpression
                && binaryExpression.Right is not BinaryExpression;

            if (
                binaryExpression.Left is BinaryExpression childBinary
                && ShouldFlatten(binaryExpression.Operator, childBinary.Operator)
            )
            {
                parts.AddRange(PrintBinaryExpression(childBinary));
            }
            else
            {
                parts.Add(binaryExpression.Left.Print());
            }

            var right = Doc.Concat(
                Doc.Line,
                binaryExpression.Operator,
                " ",
                binaryExpression.Right.Print()
            );

            parts.Add(shouldGroup ? Doc.Group(right) : right);
            return parts;
        }

        private static bool ShouldFlatten(string parentToken, string nodeToken)
        {
            return GetPrecedence(parentToken) == GetPrecedence(nodeToken);
        }

        private static int GetPrecedence(GmlSyntaxNode node)
        {
            if (node is BinaryExpression binaryExpression)
            {
                return GetPrecedence(binaryExpression.Operator);
            }

            return -1;
        }

        private static int GetPrecedence(string @operator)
        {
            return @operator switch
            {
                "*" => 21,
                "/" => 20,
                "%" => 19,
                "div" => 18,
                "+" => 17,
                "-" => 16,
                "??" => 15,
                "<<" => 14,
                ">>" => 13,
                "||" => 12,
                "&&" => 11,
                "^^" => 10,
                "==" => 9,
                "!=" => 8,
                "<" => 7,
                ">" => 6,
                "<=" => 5,
                ">=" => 4,
                "&" => 3,
                "|" => 2,
                "^" => 1,
                _ => throw new Exception($"No precedence defined for {@operator}")
            };
        }
    }
}
