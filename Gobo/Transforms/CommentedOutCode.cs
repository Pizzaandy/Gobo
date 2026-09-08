using Gobo.Parser;
using Gobo.SyntaxNodes;
using Gobo.SyntaxNodes.Gml;
using Gobo.Text;

namespace Gobo.Transforms;

/// <summary>
/// Decides whether a comment holds code rather than prose, by parsing it. Prose almost never
/// parses, and the cases that do are held back by requiring punctuation only code carries.
/// </summary>
internal static class CommentedOutCode
{
    public static bool Detect(IReadOnlyList<string> bodies)
    {
        var text = string.Join("\n", bodies).Trim();

        if (text.Length < 4)
        {
            return false;
        }

        // Every line has to carry code punctuation of its own. Judging the run as a whole
        // lets one commented out statement drag the prose above it out with it.
        foreach (var body in bodies.Select(line => line.Trim()).Where(line => line.Length > 0))
        {
            if (!LooksLikeCode(body))
            {
                return false;
            }
        }

        GmlSyntaxNode ast;

        try
        {
            ast = new GmlParser(SourceText.From(text)).Parse().Ast;
        }
        catch (GmlSyntaxErrorException)
        {
            return false;
        }

        return ast.Children.Any(IsStatement);
    }

    /// <summary>
    /// Punctuation that prose does not usually carry. Without this, a comment beginning
    /// "if the player is grounded" parses as an if statement and would be deleted.
    /// </summary>
    private static bool LooksLikeCode(string text)
    {
        return text.EndsWith(';')
            || text.EndsWith('{')
            || text.EndsWith('}')
            || (text.Contains('(') && text.Contains(')'));
    }

    private static bool IsStatement(GmlSyntaxNode node)
    {
        return node
            is AssignmentExpression
                or CallExpression
                or VariableDeclarationList
                or IfStatement
                or ForStatement
                or WhileStatement
                or RepeatStatement
                or WithStatement
                or DoStatement
                or SwitchStatement
                or ReturnStatement
                or ExitStatement
                or BreakStatement
                or ContinueStatement
                or FunctionDeclaration
                or IncDecStatement;
    }
}
