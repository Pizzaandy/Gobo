using Antlr4.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PrettierGML.Printer.DocTypes;

namespace PrettierGML.SyntaxNodes
{
    internal enum CommentType
    {
        Leading,
        Trailing,
        Dangling,
    }

    internal enum CommentPlacement
    {
        OwnLine,
        EndOfLine,
        Remaining,
    }

    /// <summary>
    /// Represents a sequence of comments with no line breaks between them.
    /// </summary>
    internal class CommentGroup
    {
        [JsonIgnore]
        public List<IToken> Tokens { get; init; }

        public string Text => string.Concat(Tokens.Select(t => t.Text));

        [JsonConverter(typeof(StringEnumConverter))]
        public CommentType Type { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public CommentPlacement Placement { get; set; }

        [JsonIgnore]
        public Range CharacterRange { get; set; }

        [JsonIgnore]
        public Range TokenRange { get; set; }

        [JsonIgnore]
        public GmlSyntaxNode? EnclosingNode { get; set; }

        [JsonIgnore]
        public GmlSyntaxNode? PrecedingNode { get; set; }

        [JsonIgnore]
        public GmlSyntaxNode? FollowingNode { get; set; }

        [JsonIgnore]
        public bool Printed { get; set; } = false;

        [JsonIgnore]
        // TODO: Please rewrite this oh my god
        public bool EndsWithSingleLineComment =>
            Tokens
                .AsEnumerable()
                .Reverse()
                .SkipWhile(
                    token =>
                        token.Type != GameMakerLanguageLexer.SingleLineComment
                        && token.Type != GameMakerLanguageLexer.MultiLineComment
                )
                .Take(1)
                .FirstOrDefault()
                ?.Type == GameMakerLanguageLexer.SingleLineComment;

        public CommentGroup(List<IToken> text, Range characterRange, Range tokenRange)
        {
            Tokens = text;
            CharacterRange = characterRange;
            TokenRange = tokenRange;
        }

        public Doc Print()
        {
            if (Printed)
            {
                throw new Exception("Comment printed twice: " + Text);
            }
            Printed = true;

            var parts = new List<Doc>();

            bool shouldBreak = false;

            foreach (var token in Tokens)
            {
                if (token.Type == GameMakerLanguageLexer.SingleLineComment)
                {
                    parts.Add(PrintSingleLineComment(token.Text));
                    shouldBreak = true;
                }
                else if (token.Type == GameMakerLanguageLexer.MultiLineComment)
                {
                    parts.Add(PrintMultiLineComment(token.Text));
                }
                else if (token.Type == GameMakerLanguageLexer.WhiteSpaces)
                {
                    // Collapse whitespace to a single space
                    if (parts.Count > 0 && parts.Last() is not HardLine)
                    {
                        parts.Add(" ");
                    }
                }
            }

            if (shouldBreak)
            {
                parts.Add(Doc.BreakParent);
            }

            return Doc.Concat(parts);
        }

        public static Doc PrintGroups(PrintContext ctx, List<CommentGroup> groups, CommentType type)
        {
            if (groups.Count == 0)
            {
                return Doc.Null;
            }

            var printedGroups = Doc.Join(
                Doc.Concat(Doc.HardLine, Doc.HardLine),
                groups.Select(c => c.Print()).ToList()
            );

            if (type == CommentType.Dangling)
            {
                return printedGroups;
            }

            var parts = new List<Doc>();

            if (type == CommentType.Leading)
            {
                int lineBreaksBetween =
                    ctx.Tokens
                        .GetHiddenTokensToRight(groups.Last().TokenRange.Stop)
                        ?.Count(token => token.Type == GameMakerLanguageLexer.LineTerminator) ?? 0;

                if (lineBreaksBetween == 0)
                {
                    return Doc.Concat(printedGroups, " ");
                }

                parts.Add(printedGroups);

                for (var i = 0; i < Math.Min(lineBreaksBetween, 2); i++)
                {
                    parts.Add(Doc.HardLine);
                }
            }
            else
            {
                int lineBreaksBetween =
                    ctx.Tokens
                        .GetHiddenTokensToLeft(groups.First().TokenRange.Start)
                        ?.Count(token => token.Type == GameMakerLanguageLexer.LineTerminator) ?? 0;

                if (lineBreaksBetween == 0)
                {
                    return Doc.Concat(" ", printedGroups);
                }

                for (var i = 0; i < Math.Min(lineBreaksBetween, 2); i++)
                {
                    parts.Add(Doc.HardLine);
                }

                parts.Add(printedGroups);
            }

            return Doc.Concat(parts);
        }

        public static Doc PrintSingleLineComment(string text)
        {
            // TODO: decide whether to format comments
            return text;
        }

        public static Doc PrintMultiLineComment(string text)
        {
            var lines = text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)
                .Select(line => (Doc)line);

            return Doc.Join(Doc.HardLine, lines);
        }

        public override string ToString()
        {
            return string.Join(
                '\n',
                $"Text: {string.Concat(Tokens.Select(t => t.Text))}",
                $"Type: {Type}",
                $"Range: {CharacterRange}",
                $"Enclosing: {EnclosingNode?.Kind}",
                $"Preceding: {PrecedingNode?.Kind}",
                $"Following: {FollowingNode?.Kind}\n"
            );
        }
    }
}
