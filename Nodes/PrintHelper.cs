using PrettierGML.Nodes.SyntaxNodes;
using System.Diagnostics;

namespace PrettierGML.Nodes
{
    internal static class PrintHelper
    {
        public static Doc PrintSingleClauseStatement(
            string keyword,
            GmlSyntaxNode clause,
            GmlSyntaxNode body
        )
        {
            return Doc.Concat(
                keyword,
                " ",
                Doc.Group("(", Doc.Indent(Doc.SoftLine, clause.Print()), Doc.SoftLine, ")"),
                " ",
                EnsureStatementInBlock(body)
            );
        }

        public static Doc EnsureStatementInBlock(GmlSyntaxNode statement)
        {
            if (statement is Block)
            {
                return statement.Print();
            }
            else
            {
                return Block.PrintInBlock(PrintStatement(statement));
            }
        }

        public static Doc PrintExpressionInParentheses(GmlSyntaxNode expression)
        {
            return Doc.Group("(", Doc.Indent(Doc.SoftLine, expression.Print()), Doc.SoftLine, ")");
        }

        public static Doc PrintStatement(GmlSyntaxNode statement)
        {
            var lineSuffix = NeedsSemicolon(statement) ? ";" : "";
            return Doc.Concat(statement.Print(), lineSuffix);
        }

        public static Doc PrintStatements(GmlSyntaxNode statements, bool isTopLevel = false)
        {
            Debug.Assert(statements is NodeList or EmptyNode);

            var parts = new List<Doc>();
            foreach (var child in statements.Children)
            {
                Doc childDoc = PrintStatement(child);
                parts.Add(childDoc);
            }

            return parts.Count == 0 ? Doc.Null : Doc.Join(Doc.HardLine, parts);
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
                    or IncDecStatement;
        }

        public static Doc PrintArgumentListLikeSyntax(
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

        public static Doc PrintSeparatedList(GmlSyntaxNode items, string separator)
        {
            var parts = new List<Doc>();
            if (items.Children.Count == 1)
            {
                parts.Add(items.Children[0].Print());
            }
            else if (items.Children.Any())
            {
                var printedArguments = PrintSeparatedSyntaxList(items, separator);
                parts.Add(Doc.Indent(printedArguments));
            }

            return Doc.Group(parts);
        }

        private static Doc PrintSeparatedSyntaxList(
            GmlSyntaxNode list,
            string separator,
            bool allowTrailingSeparator = false,
            int startingIndex = 0
        )
        {
            Debug.Assert(list is EmptyNode or NodeList);
            var parts = new List<Doc>();

            for (var i = startingIndex; i < list.Children.Count; i++)
            {
                parts.Add(list.Children[i].Print());

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
