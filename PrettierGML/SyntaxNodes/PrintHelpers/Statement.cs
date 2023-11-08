using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;
using PrettierGML.SyntaxNodes.Gml;

namespace PrettierGML.SyntaxNodes.PrintHelpers
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
            Doc bodyDoc = EnsureStatementInBlock(ctx, body);

            Doc clauseDoc = EnsureExpressionInParentheses(ctx, clause);

            return Doc.Concat(keyword, " ", clauseDoc, " ", bodyDoc);
        }

        public static Doc EnsureStatementInBlock(PrintContext ctx, GmlSyntaxNode statement)
        {
            if (statement is Block or SwitchBlock)
            {
                return statement.Print(ctx);
            }
            else if (statement.IsEmpty)
            {
                return Block.PrintEmptyBlock(ctx);
            }
            else
            {
                return Block.WrapInBlock(ctx, PrintStatement(ctx, statement));
            }
        }

        public static Doc EnsureExpressionInParentheses(PrintContext ctx, GmlSyntaxNode expression)
        {
            if (expression is ParenthesizedExpression)
            {
                return expression.Print(ctx);
            }
            else
            {
                expression.PrintOwnComments = false;

                var printedGroup = ParenthesizedExpression.PrintInParens(ctx, expression);

                return expression.PrintWithOwnComments(ctx, printedGroup);
            }
        }

        public static Doc PrintStatement(PrintContext ctx, GmlSyntaxNode statement)
        {
            if (NeedsSemicolon(statement))
            {
                statement.PrintOwnComments = false;

                return statement.PrintWithOwnComments(ctx, Doc.Concat(statement.Print(ctx), ";"));
            }
            else
            {
                return statement.Print(ctx);
            }
        }

        public static Doc PrintStatements(PrintContext ctx, List<GmlSyntaxNode> statements)
        {
            var parts = new List<Doc>();
            bool nextStatementNeedsLineBreak = false;

            foreach (var child in statements)
            {
                var shouldAddLineBreakFromSource =
                    HasLeadingLineBreak(ctx, child) && child != statements.First();

                var isTopLevelFunctionOrMethod = IsTopLevelFunctionOrMethod(child);

                var shouldAddLineBreak =
                    (
                        shouldAddLineBreakFromSource
                        || isTopLevelFunctionOrMethod
                        || nextStatementNeedsLineBreak
                    )
                    && child != statements.First();

                parts.Add(
                    shouldAddLineBreak
                        ? Doc.Concat(Doc.HardLine, PrintStatement(ctx, child))
                        : PrintStatement(ctx, child)
                );

                nextStatementNeedsLineBreak = isTopLevelFunctionOrMethod;
            }

            return parts.Count == 0 ? Doc.Null : Doc.Join(Doc.HardLine, parts);
        }

        public static bool IsTopLevelFunctionOrMethod(GmlSyntaxNode node)
        {
            var isTopLevelFunction = node is FunctionDeclaration && node.Parent is Document;

            // Check for a static method declaration (i.e. static foo = function(){}) in a constructor
            var isMethod =
                node is VariableDeclarationList variableDeclarationList
                && variableDeclarationList.Modifier == "static"
                && variableDeclarationList.Declarations.Any(
                    c => c is VariableDeclarator decl && decl.Initializer is FunctionDeclaration
                )
                && variableDeclarationList.Parent?.Parent is Block block
                && block.Parent is FunctionDeclaration potentialConstructor
                && potentialConstructor.IsConstructor;

            return isTopLevelFunction || isMethod;
        }

        public static bool HasLeadingLineBreak(PrintContext ctx, GmlSyntaxNode node)
        {
            static bool IsWhiteSpace(IToken token)
            {
                return token.Type == GameMakerLanguageLexer.LineTerminator
                    || token.Type == GameMakerLanguageLexer.WhiteSpaces;
            }

            static bool IsLineTerminator(IToken token)
            {
                return token.Type == GameMakerLanguageLexer.LineTerminator;
            }

            int nodeStartIndex;

            if (node.LeadingComments.Any())
            {
                nodeStartIndex = node.LeadingComments.First().TokenRange.Start;
            }
            else
            {
                nodeStartIndex = node.TokenRange.Start;
            }

            var leadingTokens = ctx.Tokens.GetHiddenTokensToLeft(nodeStartIndex);

            if (leadingTokens is null)
            {
                return false;
            }

            var leadingWhitespace = leadingTokens.Reverse().TakeWhile(IsWhiteSpace);

            return leadingWhitespace is not null
                && (leadingWhitespace.Count(IsLineTerminator)) >= 2;
        }

        private static bool NeedsSemicolon(GmlSyntaxNode node)
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
                    or ThrowStatement
                    or GlobalVariableStatement;
        }
    }
}
