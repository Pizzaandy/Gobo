using PrettierGML.Nodes.SyntaxNodes;
using System.Diagnostics;

namespace PrettierGML.Nodes
{
    internal static class PrintHelper
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
                Doc.Group("(", Doc.Indent(Doc.SoftLine, clause.Print(ctx)), Doc.SoftLine, ")"),
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

        public static Doc PrintExpressionInParentheses(PrintContext ctx, GmlSyntaxNode expression)
        {
            return Doc.Group(
                "(",
                Doc.Indent(Doc.SoftLine, expression.Print(ctx)),
                Doc.SoftLine,
                ")"
            );
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
                Doc childDoc = HasLeadingLine(ctx, child)
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

        public static Doc PrintArgumentListLikeSyntax(
            PrintContext ctx,
            string openToken,
            GmlSyntaxNode arguments,
            string closeToken,
            string separator,
            bool allowTrailingSeparator = false
        )
        {
            var parts = new List<Doc> { openToken };

            if (arguments.Children.Any())
            {
                Doc printedArguments = PrintSeparatedSyntaxList(
                    ctx,
                    arguments,
                    separator,
                    allowTrailingSeparator
                );

                parts.Add(Doc.Indent(Doc.SoftLine, printedArguments));
                parts.Add(Doc.SoftLine);
            }

            parts.Add(closeToken);

            return Doc.Group(parts);
        }

        public static Doc PrintSeparatedList(
            PrintContext ctx,
            GmlSyntaxNode items,
            string separator
        )
        {
            var parts = new List<Doc>();
            if (items.Children.Count == 1)
            {
                parts.Add(items.Children[0].Print(ctx));
            }
            else if (items.Children.Any())
            {
                var printedArguments = PrintSeparatedSyntaxList(ctx, items, separator);
                parts.Add(Doc.Indent(printedArguments));
            }

            return Doc.Group(parts);
        }

        private static Doc PrintSeparatedSyntaxList(
            PrintContext ctx,
            GmlSyntaxNode list,
            string separator,
            bool allowTrailingSeparator = false,
            int startingIndex = 0
        )
        {
            var parts = new List<Doc>();

            for (var i = startingIndex; i < list.Children.Count; i++)
            {
                parts.Add(list.Children[i].Print(ctx));

                if (i != list.Children.Count - 1)
                {
                    parts.Add(Doc.Concat(separator, Doc.Line));
                }
                else if (allowTrailingSeparator)
                {
                    parts.Add(Doc.IfBreak(separator, Doc.Null));
                }
            }

            return parts.Count == 0 ? Doc.Null : Doc.Concat(parts);
        }
    }
}
