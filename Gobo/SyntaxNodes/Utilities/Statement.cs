﻿using Gobo.Printer.DocTypes;
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

    public static Doc PrintStatements(PrintContext ctx, GmlSyntaxNode[] statements)
    {
        var parts = new List<Doc>(statements.Length);
        bool nextStatementNeedsLineBreak = false;

        for (var i = 0; i < statements.Length; i++)
        {
            var child = statements[i];

            var shouldAddLineBreakFromSource = i != 0 && HasLeadingEmptyLine(ctx, child);

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

        // Check for a static method declaration (i.e. static foo = function(){}) in a constructor
        var isMethod =
            node is VariableDeclarationList variableDeclarationList
            && variableDeclarationList.Modifier == "static"
            && variableDeclarationList.Declarations.Any(
                c => c is VariableDeclarator decl && decl.Initializer is FunctionDeclaration
            );

        return isTopLevelFunction || isMethod;
    }

    /// <summary>
    /// Count the number of line breaks preceding the statement.
    /// Ignore semicolons.
    /// </summary>
    public static bool HasLeadingEmptyLine(PrintContext ctx, GmlSyntaxNode node)
    {
        var startSpan = node.LeadingComments.Any() ? node.LeadingComments.First().Span : node.Span;

        var lineBreakCount = ctx.SourceText.GetLineBreaksToLeft(startSpan);

        return lineBreakCount >= 2;
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
                or UnaryExpression
                or Identifier;
    }
}
