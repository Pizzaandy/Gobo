using Gobo.Parser;
using Gobo.Printer.DocTypes;
using Gobo.SyntaxNodes.Gml;

namespace Gobo.SyntaxNodes.PrintHelpers;

internal static class Statement
{
    public static Doc PrintControlFlowStatement(
        PrintContext ctx,
        string keyword,
        GmlSyntaxNode clause,
        GmlSyntaxNode body
    )
    {
        Doc bodyDoc = EnsureStatementInBlock(ctx, body);

        Doc clauseDoc = EnsureExpressionInParentheses(ctx, clause);

        return Doc.Concat(keyword, Doc.CollapsedSpace, clauseDoc, Doc.CollapsedSpace, bodyDoc);
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
            return ParenthesizedExpression.PrintInParens(ctx, expression);
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

        for (var i = 0; i < statements.Count; i++)
        {
            var child = statements[i];

            var shouldAddLineBreakFromSource = HasLeadingLineBreak(ctx, child);
            var isTopLevelFunctionOrMethod = IsTopLevelFunctionOrMethod(child);

            var shouldAddLineBreak =
                (
                    shouldAddLineBreakFromSource
                    || isTopLevelFunctionOrMethod
                    || nextStatementNeedsLineBreak
                )
                && i != 0;

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

        // Check for a static function declaration (i.e. static foo = function(){})
        var isMethod =
            node is VariableDeclarationList variableDeclarationList
            && variableDeclarationList.Modifier == "static"
            && variableDeclarationList.Declarations.Any(
                c => c is VariableDeclarator decl && decl.Initializer is FunctionDeclaration
            );

        return isTopLevelFunction || isMethod;
    }

    public static bool HasLeadingLineBreak(PrintContext ctx, GmlSyntaxNode node)
    {
        int leadingLineBreaks = 0;

        if (node.LeadingComments.Any())
        {
            leadingLineBreaks = node.LeadingComments.First().LeadingLineBreaks;
        }
        else
        {
            var leadingTrivia = ctx.GetLeadingTrivia(node.Span);

            foreach (var token in leadingTrivia.Reverse())
            {
                if (token.Kind is TokenKind.SingleLineComment or TokenKind.MultiLineComment)
                {
                    break;
                }
                if (token.Kind is TokenKind.LineBreak)
                {
                    leadingLineBreaks++;
                }
            }
        }

        return leadingLineBreaks >= 2;
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
                or GlobalVariableStatement
                or DeleteStatement
                or UnaryExpression;
    }
}
