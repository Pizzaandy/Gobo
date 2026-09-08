using Gobo.Printer.DocTypes;
using Gobo.SyntaxNodes.Gml;

namespace Gobo.SyntaxNodes.PrintHelpers;

internal static class Statement
{
    /// <summary>
    /// Keywords whose clause reads as a list of conditions. A switch discriminant, a repeat
    /// count and a with target do not.
    /// </summary>
    private static readonly string[] stackableKeywords = { "if", "while" };

    public static Doc PrintControlFlowStatement(
        PrintContext ctx,
        string keyword,
        GmlSyntaxNode clause,
        GmlSyntaxNode body
    )
    {
        Doc bodyDoc = EnsureStatementInBlock(ctx, body);

        Doc clauseDoc = stackableKeywords.Contains(keyword)
            ? PrintCondition(ctx, clause)
            : EnsureExpressionInParentheses(ctx, clause);

        return Doc.Concat(keyword, Doc.CollapsedSpace, clauseDoc, Doc.CollapsedSpace, bodyDoc);
    }

    /// <summary>
    /// Prints the clause of a control flow statement, stacking a multi line logical chain.
    /// </summary>
    public static Doc PrintCondition(PrintContext ctx, GmlSyntaxNode clause)
    {
        if (ctx.Options.StackedConditions && TryPrintStackedCondition(ctx, clause, out var stacked))
        {
            return stacked;
        }

        return EnsureExpressionInParentheses(ctx, clause);
    }

    private static bool TryPrintStackedCondition(
        PrintContext ctx,
        GmlSyntaxNode clause,
        out Doc result
    )
    {
        result = Doc.Null;

        var clauseLayers = new List<GmlSyntaxNode>();
        var expression = UnwrapParentheses(clause, clauseLayers);

        if (expression is not BinaryExpression binary || !IsLogicalOperator(binary.Operator))
        {
            return false;
        }

        var operands = new List<GmlSyntaxNode>();
        CollectOperands(binary, operands);

        if (!CanParenthesiseOperands(operands))
        {
            return false;
        }

        var parts = new List<Doc>();
        operands.Clear();
        PrintLogicalChain(ctx, binary, parts, operands);

        clauseLayers.Add(binary);
        var stacked = PrintWithLayerComments(ctx, Doc.Concat(parts), clauseLayers);

        // A chain already split stays split. One that fits stays on its line, so that a
        // stacked chain reprints as itself.
        result = HasLineBreakBetweenOperands(ctx, operands)
            ? stacked
            : Doc.ConditionalGroup(EnsureExpressionInParentheses(ctx, clause), stacked);

        return true;
    }

    /// <summary>
    /// Prints a chain of one repeated logical operator, one operand per line. A different
    /// operator ends the chain, so that reprinting cannot regroup it.
    /// </summary>
    private static void PrintLogicalChain(
        PrintContext ctx,
        BinaryExpression node,
        List<Doc> parts,
        List<GmlSyntaxNode> operands
    )
    {
        if (node.Left is BinaryExpression left && left.Operator == node.Operator)
        {
            parts.Add(left.PrintLeadingComments(ctx));
            PrintLogicalChain(ctx, left, parts, operands);
            parts.Add(left.PrintTrailingComments(ctx));
        }
        else
        {
            parts.Add(PrintOperand(ctx, node.Left));
            operands.Add(node.Left);
        }

        parts.Add(Doc.HardLine);
        parts.Add(Doc.Concat(node.Operator, " ", PrintOperand(ctx, node.Right)));
        operands.Add(node.Right);
    }

    private static bool HasLineBreakBetweenOperands(
        PrintContext ctx,
        List<GmlSyntaxNode> operands
    )
    {
        for (var i = 0; i < operands.Count - 1; i++)
        {
            var between = ctx.SourceText.ReadSpan(
                operands[i].Span.End,
                operands[i + 1].Span.Start
            );

            if (between.Contains('\n'))
            {
                return true;
            }
        }

        return false;
    }

    private static Doc PrintOperand(PrintContext ctx, GmlSyntaxNode operand)
    {
        var layers = new List<GmlSyntaxNode>();
        var expression = UnwrapParentheses(operand, layers);

        return PrintWithLayerComments(
            ctx,
            ParenthesizedExpression.PrintInParens(ctx, expression),
            layers
        );
    }

    private static GmlSyntaxNode UnwrapParentheses(
        GmlSyntaxNode node,
        List<GmlSyntaxNode> strippedLayers
    )
    {
        var inner = node;

        while (inner is ParenthesizedExpression parenthesized)
        {
            strippedLayers.Add(inner);
            inner = parenthesized.Expression;
        }

        return inner;
    }

    private static Doc PrintWithLayerComments(
        PrintContext ctx,
        Doc doc,
        List<GmlSyntaxNode> strippedLayers
    )
    {
        for (var i = strippedLayers.Count - 1; i >= 0; i--)
        {
            var layer = strippedLayers[i];

            if (layer.Comments.Count > 0)
            {
                doc = layer.PrintWithOwnComments(ctx, doc);
            }
        }

        return doc;
    }

    private static bool IsLogicalOperator(string @operator)
    {
        return @operator is "&&" or "||" or "^^";
    }

    private static void CollectOperands(BinaryExpression node, List<GmlSyntaxNode> operands)
    {
        if (node.Left is BinaryExpression left && left.Operator == node.Operator)
        {
            CollectOperands(left, operands);
        }
        else
        {
            operands.Add(node.Left);
        }

        operands.Add(node.Right);
    }

    /// <summary>
    /// Stacking puts every operand in parentheses. GameMaker reads '&&', '||' and '^^' as one
    /// precedence level, left to right, so parenthesising an operand that is itself a logical
    /// chain would regroup the condition. Those are left alone.
    /// </summary>
    private static bool CanParenthesiseOperands(List<GmlSyntaxNode> operands)
    {
        foreach (var operand in operands)
        {
            if (operand is ParenthesizedExpression)
            {
                continue;
            }

            if (operand is BinaryExpression binary && IsLogicalOperator(binary.Operator))
            {
                return false;
            }
        }

        return true;
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
            var body = new List<GmlSyntaxNode> { statement };

            return Block.WrapInBlock(
                ctx,
                PrintStatement(ctx, statement),
                CanInlineBody(ctx, statement.Parent, statement, body)
            );
        }
    }

    public static Doc PrintStatementsInline(PrintContext ctx, List<GmlSyntaxNode> statements)
    {
        return Doc.Join(Doc.Line, statements.Select(statement => PrintStatement(ctx, statement)));
    }

    public static bool CanInlineBody(
        PrintContext ctx,
        GmlSyntaxNode? owner,
        GmlSyntaxNode bodyNode,
        List<GmlSyntaxNode> body
    )
    {
        if (!ctx.Options.InlineShortBlocks || body.Count == 0)
        {
            return false;
        }

        if (ctx.Options.BraceStyle is not BraceStyle.SameLine)
        {
            return false;
        }

        if (!IsOnOneSourceLine(ctx, bodyNode))
        {
            return false;
        }

        if (
            owner
            is not (
                IfStatement
                or WhileStatement
                or ForStatement
                or RepeatStatement
                or WithStatement
                or DoStatement
            )
        )
        {
            return false;
        }

        // A comment anywhere in the statement would land past the closing brace and swallow
        // it. Which node it attaches to depends on the layout, so refuse on all of them.
        if (owner.Comments.Any(comment => comment.Type != CommentType.Leading))
        {
            return false;
        }

        if (owner.Children.Any(HasComments))
        {
            return false;
        }

        return HoldsOneStatement(body);
    }

    private static bool HasComments(GmlSyntaxNode node)
    {
        return node.Comments.Count > 0 || node.Children.Any(HasComments);
    }

    /// <summary>
    /// True when the author wrote the node on the line it starts on, following the keyword.
    /// </summary>
    private static bool IsOnOneSourceLine(PrintContext ctx, GmlSyntaxNode node)
    {
        return ctx.SourceText.GetLineBreaksToLeft(node.Span) == 0
            && !ctx.SourceText.ReadSpan(node.Span).Contains('\n');
    }

    /// <summary>
    /// True when a body holds a single statement. A trailing jump does not count, because it
    /// reads as the end of the body rather than as work it does.
    /// </summary>
    public static bool HoldsOneStatement(List<GmlSyntaxNode> body)
    {
        var count = body.Count;

        if (body[^1] is BreakStatement or ContinueStatement or ExitStatement or ReturnStatement)
        {
            count -= 1;
        }

        if (count > 1)
        {
            return false;
        }

        foreach (var statement in body)
        {
            if (!NeedsSemicolon(statement) || HasComments(statement))
            {
                return false;
            }
        }

        return true;
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

            var shouldAddLineBreakFromSource = i != 0 && HasLeadingEmptyLine(ctx, child);

            var isTopLevelFunctionOrMethod = IsTopLevelFunctionOrMethod(child);

            // A region directive belongs to the statement under it, so no blank line is added
            // between them. One the author wrote is still theirs to keep.
            var followsRegion = i > 0 && statements[i - 1] is RegionStatement;

            var shouldAddLineBreak =
                (
                    shouldAddLineBreakFromSource
                    || (
                        (isTopLevelFunctionOrMethod || nextStatementNeedsLineBreak)
                        && !followsRegion
                    )
                )
                && child != statements.First();

            if (i > 0)
            {
                if (SharesLineWithLoopHeader(ctx, statements[i - 1], child))
                {
                    parts.Add(" ");
                }
                else
                {
                    parts.Add(Doc.HardLine);

                    if (shouldAddLineBreak)
                    {
                        parts.Add(Doc.HardLine);
                    }
                }
            }

            parts.Add(PrintStatement(ctx, child));

            nextStatementNeedsLineBreak = isTopLevelFunctionOrMethod;
        }

        return parts.Count == 0 ? Doc.Null : Doc.Concat(parts);
    }

    /// <summary>
    /// Recognises a one line loop header, as in 'var _i = 0; repeat (n) {'.
    /// </summary>
    private static bool SharesLineWithLoopHeader(
        PrintContext ctx,
        GmlSyntaxNode previous,
        GmlSyntaxNode current
    )
    {
        if (!ctx.Options.PreserveGluedStatements)
        {
            return false;
        }

        if (current is not (RepeatStatement or WhileStatement or ForStatement or DoStatement))
        {
            return false;
        }

        if (previous.Comments.Count > 0 || current.Comments.Count > 0)
        {
            return false;
        }

        return !HasLineBreakBetween(ctx, previous, current);
    }

    public static bool HasLineBreakBetween(
        PrintContext ctx,
        GmlSyntaxNode first,
        GmlSyntaxNode second
    )
    {
        if (second.Span.Start < first.Span.End)
        {
            return true;
        }

        return ctx.SourceText.ReadSpan(first.Span.End, second.Span.Start).Contains('\n');
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
                or ThrowStatement
                or GlobalVariableStatement
                or DeleteStatement
                or UnaryExpression
                or Identifier;
    }
}
