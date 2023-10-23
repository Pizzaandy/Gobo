using PrettierGML.Nodes.SyntaxNodes;
using System.Diagnostics;

namespace PrettierGML.Nodes.PrintHelpers
{
    internal static class Statement
    {
        public static Doc PrintSingleClauseStatement(
            PrintContext ctx,
            string keyword,
            GmlSyntaxNode clause,
            GmlSyntaxNode body
        )
        {
            return Doc.Concat(
                keyword,
                " ",
                EnsureExpressionInParentheses(ctx, clause),
                " ",
                EnsureStatementInBlock(ctx, body)
            );
        }

        public static Doc EnsureStatementInBlock(PrintContext ctx, GmlSyntaxNode statement)
        {
            if (statement is Block)
            {
                return statement.Print(ctx);
            }
            else
            {
                return Block.PrintInBlock(ctx, PrintStatement(ctx, statement));
            }
        }

        public static Doc EnsureExpressionInParentheses(PrintContext ctx, GmlSyntaxNode expression)
        {
            expression = UnwrapParenthesizedExpression(expression);

            return Doc.Group(
                "(",
                Doc.Indent(Doc.SoftLine, expression.Print(ctx)),
                Doc.SoftLine,
                ")"
            );
        }

        public static GmlSyntaxNode UnwrapParenthesizedExpression(GmlSyntaxNode expression)
        {
            while (expression is ParenthesizedExpression parenthesized)
            {
                expression = parenthesized.Expression;
            }
            return expression;
        }

        public static Doc PrintStatement(PrintContext ctx, GmlSyntaxNode statement)
        {
            var lineSuffix = NeedsSemicolon(statement) ? ";" : "";
            return Doc.Concat(statement.Print(ctx), lineSuffix);
        }

        public static Doc PrintStatements(PrintContext ctx, GmlSyntaxNode statements)
        {
            Debug.Assert(statements is NodeList or EmptyNode);

            var parts = new List<Doc>();

            foreach (var child in statements.Children)
            {
                Doc childDoc =
                    child != statements.Children.First() && HasLeadingLine(ctx, child)
                        ? Doc.Concat(Doc.HardLine, PrintStatement(ctx, child))
                        : PrintStatement(ctx, child);

                parts.Add(childDoc);
            }

            return parts.Count == 0 ? Doc.Null : Doc.Join(Doc.HardLine, parts);
        }

        public static bool HasLeadingLine(PrintContext ctx, GmlSyntaxNode node)
        {
            var leadingTokens = ctx.Tokens.GetHiddenTokensToLeft(node.SourceInterval.a);
            return leadingTokens is not null
                && leadingTokens.Count(token => token.Text.Contains('\n')) >= 2;
        }

        public static bool NeedsSemicolon(GmlSyntaxNode node)
        {
            return node
                is CallExpression
                    or AssignmentExpression
                    or VariableDeclarationList
                    or BreakStatement
                    or ContinueStatement
                    or ReturnStatement
                    or ExitStatement
                    or IncDecStatement
                    or ThrowStatement;
        }
    }
}
