using Gobo.Printer.DocTypes;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class BinaryExpression : GmlSyntaxNode
{
    public string Operator { get; set; }
    public GmlSyntaxNode Left { get; set; }
    public GmlSyntaxNode Right { get; set; }

    public BinaryExpression(
        TextSpan span,
        string @operator,
        GmlSyntaxNode left,
        GmlSyntaxNode right
    )
        : base(span)
    {
        Operator = @operator switch
        {
            "and" => "&&",
            "or" => "||",
            "xor" => "^^",
            "<>" => "!=",
            "mod" => "%",
            "=" => "==",
            ":=" => "==",
            _ => @operator
        };

        Left = AsChild(left);
        Right = AsChild(right);
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        var docs = PrintBinaryExpression(this, ctx);

        var trueParent = Parent;
        while (trueParent is ParenthesizedExpression)
        {
            trueParent = trueParent.Parent;
        }

        if (ctx.Options.FlatExpressions)
        {
            return Doc.ForceFlat(docs);
        }

        var shouldNotIndent =
            trueParent
                is AssignmentExpression
                    or VariableDeclarator
                    or IfStatement
                    or DoStatement
                    or WhileStatement
                    or SwitchStatement
            || Parent is ConditionalExpression conditionalExpression
                && conditionalExpression.WhenTrue != this
                && conditionalExpression.WhenFalse != this;

        return shouldNotIndent
            ? Doc.Concat(docs)
            : Doc.Group(docs[0], Doc.Indent(docs.Skip(1).ToList()));
    }

    // Because GML operator precedence is inconsistent across platforms, we can't
    // group binary expressions in a syntactically meaningful way like Prettier. Our parser uses an
    // inaccurate operator precedence to optimize for readability rather than correctness.
    public static List<Doc> PrintBinaryExpression(GmlSyntaxNode node, PrintContext ctx)
    {
        if (node is not BinaryExpression binaryExpression)
        {
            return new List<Doc> { Doc.Group(node.Print(ctx)) };
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
            parts.Add(leftBinary.PrintLeadingComments(ctx));
            parts.AddRange(PrintBinaryExpression(leftBinary, ctx));
            parts.Add(leftBinary.PrintTrailingComments(ctx));
        }
        else
        {
            parts.Add(binaryExpression.Left.Print(ctx));
        }

        binaryExpression.Right.PrintOwnComments = false;

        var right = Doc.Concat(
            Doc.Line,
            binaryExpression.Right.PrintWithOwnComments(
                ctx,
                Doc.Concat(binaryExpression.Operator, " ", binaryExpression.Right.Print(ctx))
            )
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
