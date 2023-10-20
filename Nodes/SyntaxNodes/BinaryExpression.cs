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

        // Because of cross-platform inconsistency with operator precedence in GML, we can't
        // group binary expressions in a syntactically meaningful way like Prettier. My parser uses an
        // inaccurate operator precedence to optimize for readability rather than correctness.
        public static List<Doc> PrintBinaryExpression(GmlSyntaxNode node)
        {
            if (node is not BinaryExpression binaryExpression)
            {
                return new List<Doc> { Doc.Group(node.Print()) };
            }

            var parts = new List<Doc>();

            var shouldGroup =
                GetKindOrOperator(binaryExpression.Parent!) != GetKindOrOperator(binaryExpression)
                && GetPrecedence(binaryExpression) != GetPrecedence(binaryExpression.Parent!)
                && binaryExpression.Left is not BinaryExpression
                && binaryExpression.Right is not BinaryExpression;

            if (
                binaryExpression.Left is BinaryExpression leftBinary
                && ShouldFlatten(binaryExpression.Operator, leftBinary.Operator)
            )
            {
                parts.AddRange(PrintBinaryExpression(leftBinary));
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

        private static string GetKindOrOperator(GmlSyntaxNode node)
        {
            if (node is BinaryExpression binaryExpression)
            {
                return binaryExpression.Operator;
            }
            return node is null ? "null" : node.Kind;
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
                "/" => 21,
                "%" => 21,
                "div" => 21,
                "+" => 16,
                "-" => 16,
                "??" => 15,
                "<<" => 14,
                ">>" => 14,
                "||" => 12,
                "&&" => 11,
                "^^" => 10,
                "==" => 9,
                "!=" => 8,
                "<" => 7,
                ">" => 7,
                "<=" => 7,
                ">=" => 7,
                "&" => 3,
                "|" => 2,
                "^" => 1,
                _ => throw new Exception($"No precedence defined for {@operator}")
            };
        }
    }
}
