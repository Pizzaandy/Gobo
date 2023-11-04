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
            Doc lineSuffix = NeedsSemicolon(statement) ? ";" : Doc.Null;

            return Doc.Concat(
                statement.PrintLeadingComments(ctx),
                statement.Print(ctx),
                lineSuffix,
                statement.PrintTrailingComments(ctx)
            );
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
            var isTopLevelFunction = node is FunctionDeclaration && node.Parent?.Parent is Document;

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
            var leadingTokens = ctx.Tokens
                .GetHiddenTokensToLeft(node.TokenRange.Start)
                ?.TakeWhile(IsWhiteSpace);

            return leadingTokens is not null
                && leadingTokens.Count(token => token.Type == GameMakerLanguageLexer.LineTerminator)
                    >= 2;
        }

        private static bool IsWhiteSpace(IToken token)
        {
            return token.Type == GameMakerLanguageLexer.LineTerminator
                || token.Type == GameMakerLanguageLexer.WhiteSpaces;
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
