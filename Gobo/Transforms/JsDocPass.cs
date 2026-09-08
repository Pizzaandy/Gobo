using System.Text;
using Gobo.SyntaxNodes;
using Gobo.SyntaxNodes.Gml;
using Gobo.Text;

namespace Gobo.Transforms;

/// <summary>
/// Rewrites the doc comments above function declarations: wraps them in a collapsible region,
/// names the constructor a method belongs to, and writes a skeleton where there is none.
/// Works on the source text, so the result goes through the normal pipeline afterwards.
/// </summary>
internal static class JsDocPass
{
    /// <summary>
    /// Order breaks ties between insertions at the same offset; lower comes first.
    /// </summary>
    private const string DescriptionPlaceholder = "<desc>";

    private readonly record struct Insertion(int Offset, int Order, string Text);

    public static bool ShouldRun(FormatOptions options)
    {
        return options.WrapJsDocInRegion
            || options.AddSelfToMethods
            || options.GenerateMissingJsDoc;
    }

    /// <summary>
    /// Returns the rewritten source, or null when there is nothing to change.
    /// </summary>
    public static string? Apply(SourceText code, GmlSyntaxNode ast, FormatOptions options)
    {
        var insertions = new List<Insertion>();
        Visit(ast, code, options, null, insertions);

        if (insertions.Count == 0)
        {
            return null;
        }

        var text = code.ReadSpan(0, code.Length);
        var builder = new StringBuilder(text);

        foreach (
            var insertion in insertions
                .OrderByDescending(i => i.Offset)
                .ThenByDescending(i => i.Order)
        )
        {
            builder.Insert(insertion.Offset, insertion.Text);
        }

        return builder.ToString();
    }

    private static void Visit(
        GmlSyntaxNode node,
        SourceText code,
        FormatOptions options,
        string? constructorName,
        List<Insertion> insertions
    )
    {
        if (node is Block or Document)
        {
            VisitStatements(node.Children, code, options, constructorName, insertions);
        }
        else if (node is SwitchCase switchCase)
        {
            VisitStatements(switchCase.Statements, code, options, constructorName, insertions);
        }

        foreach (var child in node.Children)
        {
            var owner = child is FunctionDeclaration { IsConstructor: true } declaration
                ? GetName(declaration)
                : constructorName;

            Visit(child, code, options, owner, insertions);
        }
    }

    private static void VisitStatements(
        List<GmlSyntaxNode> statements,
        SourceText code,
        FormatOptions options,
        string? constructorName,
        List<Insertion> insertions
    )
    {
        for (var i = 0; i < statements.Count; i++)
        {
            var statement = statements[i];

            var (function, name) = GetDeclaredFunction(statement);

            if (function is null)
            {
                continue;
            }

            // A block already in its region has its comments attached to the '#endregion',
            // not to the function, so there is nothing here to read or to write.
            if (IsPrecededByJsDocRegion(statements, i))
            {
                continue;
            }

            var docLines = GetDocComments(statement);
            var statementStart = GetLineStart(code, statement.Span.Start);
            var indent = GetIndent(code, statementStart);

            if (docLines.Count == 0)
            {
                // An unnamed function has nothing to document it by.
                if (options.GenerateMissingJsDoc && name is not null)
                {
                    insertions.Add(
                        new Insertion(
                            statementStart,
                            0,
                            Generate(function, name, constructorName, indent, options)
                        )
                    );
                }

                continue;
            }

            var docStart = GetLineStart(code, docLines[0].Span.Start);

            if (options.AddSelfToMethods && constructorName is not null && !HasSelf(docLines))
            {
                insertions.Add(
                    new Insertion(statementStart, 0, $"{indent}/// @self {constructorName}\n")
                );
            }

            if (options.WrapJsDocInRegion)
            {
                insertions.Add(new Insertion(docStart, 0, $"{indent}#region jsDoc\n"));
                insertions.Add(new Insertion(statementStart, 1, $"{indent}#endregion\n"));
            }
        }
    }

    /// <summary>
    /// The function a statement declares and the name it is declared under.
    /// </summary>
    private static (FunctionDeclaration? Function, string? Name) GetDeclaredFunction(
        GmlSyntaxNode statement
    )
    {
        switch (statement)
        {
            case FunctionDeclaration function:
                return (function, GetName(function));
            case VariableDeclarationList list:
                foreach (var declaration in list.Declarations)
                {
                    if (
                        declaration
                        is VariableDeclarator { Initializer: FunctionDeclaration fn } declarator
                    )
                    {
                        return (fn, GetName(fn) ?? GetIdentifierName(declarator.Id));
                    }
                }
                return (null, null);
            case AssignmentExpression { Right: FunctionDeclaration assigned } assignment:
                return (assigned, GetName(assigned) ?? GetIdentifierName(assignment.Left));
            default:
                return (null, null);
        }
    }

    private static string? GetIdentifierName(GmlSyntaxNode node)
    {
        return node is Identifier id ? id.Name : null;
    }

    private static List<CommentGroup> GetDocComments(GmlSyntaxNode statement)
    {
        return statement
            .LeadingComments.Where(group => group.Text.TrimStart().StartsWith("///"))
            .OrderBy(group => group.Span.Start)
            .ToList();
    }

    private static bool HasSelf(List<CommentGroup> docLines)
    {
        return docLines.Any(group =>
            group.Text.Contains("@self") || group.Text.Contains("@context")
        );
    }

    private static bool IsPrecededByJsDocRegion(List<GmlSyntaxNode> statements, int index)
    {
        foreach (var region in PrecedingRegions(statements, index))
        {
            if (!region.IsEndRegion)
            {
                return (region.Name ?? "").Trim() == "jsDoc";
            }
        }

        return false;
    }

    /// <summary>
    /// The region directives written above a statement, nearest first. An if statement holds
    /// the regions that follow its last branch, so they are read back out of it.
    /// </summary>
    private static IEnumerable<RegionStatement> PrecedingRegions(
        List<GmlSyntaxNode> statements,
        int index
    )
    {
        for (var i = index - 1; i >= 0; i--)
        {
            if (statements[i] is RegionStatement region)
            {
                yield return region;
                continue;
            }

            if (statements[i] is IfStatement { TrailingRegions.Count: > 0 } ifStatement)
            {
                for (var j = ifStatement.TrailingRegions.Count - 1; j >= 0; j--)
                {
                    if (ifStatement.TrailingRegions[j] is RegionStatement trailing)
                    {
                        yield return trailing;
                    }
                }
            }

            yield break;
        }
    }

    private static string Generate(
        FunctionDeclaration function,
        string? name,
        string? constructorName,
        string indent,
        FormatOptions options
    )
    {
        var lines = new List<string>();

        if (options.WrapJsDocInRegion)
        {
            lines.Add($"{indent}#region jsDoc");
        }

        lines.Add($"{indent}/// @function {name ?? "anonymous"}");
        lines.Add($"{indent}/// @description {DescriptionPlaceholder}");

        foreach (var parameter in function.Parameters.Children)
        {
            if (parameter is not Parameter { Name: Identifier id })
            {
                continue;
            }

            var optional = parameter is Parameter { Initializer: not EmptyNode };
            var printed = optional ? $"[{id.Name}]" : id.Name;
            lines.Add($"{indent}/// @param {{Any}} {printed} {DescriptionPlaceholder}");
        }

        if (ReturnsValue(function.Body))
        {
            lines.Add($"{indent}/// @returns {{Any}}");
        }

        if (options.AddSelfToMethods && constructorName is not null)
        {
            lines.Add($"{indent}/// @self {constructorName}");
        }

        if (options.WrapJsDocInRegion)
        {
            lines.Add($"{indent}#endregion");
        }

        return string.Join("\n", lines) + "\n";
    }

    /// <summary>
    /// True when the body returns something, ignoring the bodies of nested functions.
    /// </summary>
    private static bool ReturnsValue(GmlSyntaxNode node)
    {
        if (node is ReturnStatement statement)
        {
            return statement.Children.Any(child => child is not EmptyNode);
        }

        return node.Children.Any(child =>
            child is not FunctionDeclaration && ReturnsValue(child)
        );
    }

    private static string? GetName(FunctionDeclaration function)
    {
        return function.Id is Identifier id ? id.Name : null;
    }

    private static int GetLineStart(SourceText code, int position)
    {
        var start = position;

        while (start > 0 && code[start - 1] != '\n')
        {
            start -= 1;
        }

        return start;
    }

    private static string GetIndent(SourceText code, int lineStart)
    {
        var end = lineStart;

        while (end < code.Length && (code[end] == '\t' || code[end] == ' '))
        {
            end += 1;
        }

        return code.ReadSpan(lineStart, end);
    }
}
