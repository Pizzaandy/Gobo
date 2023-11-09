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
        public bool IsFormatCommand { get; init; }

        [JsonIgnore]
        public string? FormatCommandText { get; init; }

        private bool endsWithSingleLineComment = false;

        private bool printedAsEndOfLine = false;

        public const string FormatCommandPrefix = "#fmt-";

        public CommentGroup(List<IToken> tokens, Range characterRange, Range tokenRange)
        {
            Tokens = tokens;
            CharacterRange = characterRange;
            TokenRange = tokenRange;

            for (var i = tokens.Count - 1; i >= 0; i--)
            {
                var token = tokens[i];

                if (token.Type == GameMakerLanguageLexer.SingleLineComment)
                {
                    endsWithSingleLineComment = true;

                    var trimmedText = token.Text[2..].Trim();

                    if (trimmedText.StartsWith(FormatCommandPrefix))
                    {
                        IsFormatCommand = true;
                        FormatCommandText = trimmedText[FormatCommandPrefix.Length..];
                    }

                    break;
                }
            }
        }

        public Doc Print()
        {
            if (Printed)
            {
                //throw new Exception("Comment printed twice: " + Text);
            }
            Printed = true;

            var parts = new List<Doc>();

            foreach (var token in Tokens)
            {
                if (token.Type == GameMakerLanguageLexer.SingleLineComment)
                {
                    parts.Add(PrintSingleLineComment(token.Text));
                }
                else if (token.Type == GameMakerLanguageLexer.MultiLineComment)
                {
                    parts.Add(PrintMultiLineComment(token.Text));
                    if (token != Tokens.Last())
                    {
                        parts.Add(" ");
                    }
                }
            }

            if (endsWithSingleLineComment || Placement is CommentPlacement.EndOfLine)
            {
                printedAsEndOfLine = true;
                parts.Add(Doc.BreakParent);
                return Doc.EndOfLineComment(Doc.Concat(parts));
            }
            else
            {
                return Doc.Concat(parts);
            }
        }

        public static Doc PrintGroups(PrintContext ctx, List<CommentGroup> groups, CommentType type)
        {
            if (groups.Count == 0)
            {
                return Doc.Null;
            }

            var groupDocs = new List<Doc>() { groups.First().Print() };

            // Add line breaks between comment groups
            foreach (var group in groups.Skip(1))
            {
                int leadingLineBreaks =
                    ctx.Tokens
                        .GetHiddenTokensToLeft(group.TokenRange.Start)
                        .Reverse()
                        .TakeWhile(token => !IsComment(token))
                        ?.Count(token => token.Type == GameMakerLanguageLexer.LineTerminator) ?? 0;

                for (var i = 0; i < Math.Min(leadingLineBreaks, 2); i++)
                {
                    groupDocs.Add(Doc.HardLine);
                }

                groupDocs.Add(group.Print());
            }

            var printedGroups = Doc.Concat(groupDocs);

            if (type == CommentType.Dangling)
            {
                return printedGroups;
            }

            var parts = new List<Doc>();

            // Add leading or trailing line breaks depending on type
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
                    if (!groups.Last().printedAsEndOfLine)
                    {
                        return Doc.Concat(" ", printedGroups);
                    }
                    else
                    {
                        return Doc.Concat(printedGroups);
                    }
                }

                for (var i = 0; i < Math.Min(lineBreaksBetween, 2); i++)
                {
                    parts.Add(Doc.HardLine);
                }

                parts.Add(printedGroups);
            }

            return Doc.Concat(parts);
        }

        private static bool IsComment(IToken token)
        {
            return token.Type == GameMakerLanguageLexer.SingleLineComment
                || token.Type == GameMakerLanguageLexer.MultiLineComment;
        }

        public static Doc PrintSingleLineComment(string text)
        {
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
                $"Placement: {Placement}",
                $"Range: {CharacterRange}",
                $"Enclosing: {EnclosingNode?.Kind}",
                $"Preceding: {PrecedingNode?.Kind}",
                $"Following: {FollowingNode?.Kind}\n"
            );
        }
    }
}
