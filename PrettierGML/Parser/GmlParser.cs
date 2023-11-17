using Antlr4.Runtime;
using PrettierGML.SyntaxNodes;

namespace PrettierGML.Parser;

internal struct GmlParseResult
{
    public CommonTokenStream TokenStream;
    public GmlSyntaxNode Ast;
}

internal static class GmlParser
{
    public static GmlParseResult Parse(string input, bool attachComments = true)
    {
        // 1) Parse with Antlr
        ICharStream stream = CharStreams.fromString(input);
        var lexer = new GameMakerLanguageLexer(stream);
        var tokenStream = new CommonTokenStream(lexer);
        var parser = new GameMakerLanguageParser(tokenStream);
        parser.Interpreter.PredictionMode = Antlr4.Runtime.Atn.PredictionMode.SLL;
        parser.RemoveErrorListeners();
        parser.AddErrorListener(new GameMakerLanguageErrorListener());

        // 2) Build custom syntax tree with Antlr visitor
        var builder = new GmlAstBuilder();
        var ast = builder.Visit(parser.program());

        // 3) Attach comment groups
        if (attachComments)
        {
            var commentGroups = CreateCommentGroups(tokenStream);
            ast = new CommentMapper(new SourceText(input), commentGroups).AttachComments(ast);
        }

        return new GmlParseResult() { Ast = ast, TokenStream = tokenStream };
    }

    /// <summary>
    /// Iterate all tokens and group together comments on the same line
    /// </summary>
    private static List<CommentGroup> CreateCommentGroups(CommonTokenStream tokenStream)
    {
        tokenStream.Fill();

        var groups = new List<CommentGroup>();

        List<IToken>? currentGroup = null;

        foreach (var token in tokenStream.GetTokens())
        {
            bool breakGroup = false;

            if (IsWhiteSpace(token))
            {
                if (IsLineBreak(token))
                {
                    breakGroup = true;
                }
            }
            else if (IsComment(token))
            {
                currentGroup ??= new();
            }
            else
            {
                breakGroup = true;
            }

            if (breakGroup && currentGroup?.Count > 0)
            {
                // Remove whitespace from end of group
                var trimmedGroup = currentGroup.AsEnumerable();

                while (IsWhiteSpace(trimmedGroup.Last()))
                {
                    trimmedGroup = trimmedGroup.SkipLast(1);
                }

                var first = trimmedGroup.First();
                var last = trimmedGroup.Last();

                groups.Add(
                    new CommentGroup(
                        trimmedGroup.ToList(),
                        new TextSpan(first.StartIndex, last.StopIndex + 1)
                    )
                );

                currentGroup = IsComment(token) ? new() : null;
            }

            currentGroup?.Add(token);
        }

        return groups;
    }

    private static bool IsComment(IToken token)
    {
        return token.Type == GameMakerLanguageLexer.SingleLineComment
            || token.Type == GameMakerLanguageLexer.MultiLineComment;
    }

    private static bool IsLineBreak(IToken token)
    {
        return token.Type == GameMakerLanguageLexer.LineTerminator;
    }

    private static bool IsWhiteSpace(IToken token)
    {
        return token.Type == GameMakerLanguageLexer.WhiteSpaces
            || token.Type == GameMakerLanguageLexer.LineTerminator;
    }
}
