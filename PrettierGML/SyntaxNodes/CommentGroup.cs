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
    /// Represents a sequence of comments with no empty lines between them.
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

        public CommentGroup(List<IToken> text, Range characterRange, Range tokenRange)
        {
            Tokens = text;
            CharacterRange = characterRange;
            TokenRange = tokenRange;
        }

        public Doc Print(bool checkPrinted = true)
        {
            if (checkPrinted)
            {
                if (Printed)
                {
                    throw new InvalidOperationException($"Comment printed twice:\n{this}");
                }
                Printed = true;
            }

            var parts = new List<Doc>();
            int consecutiveLines = 0;

            foreach (var token in Tokens)
            {
                if (token.Type == GameMakerLanguageLexer.SingleLineComment)
                {
                    parts.Add(PrintSingleLineComment(token.Text));
                }
                else if (token.Type == GameMakerLanguageLexer.MultiLineComment)
                {
                    parts.Add(PrintMultiLineComment(token.Text));
                }

                if (token.Type == GameMakerLanguageLexer.LineTerminator)
                {
                    if (consecutiveLines < 2)
                    {
                        parts.Add(Doc.HardLine);
                    }
                    consecutiveLines += 1;
                }
                else
                {
                    consecutiveLines = 0;
                }
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
            return $"{Tokens}\nType: {Type}, Range: {CharacterRange}\n"
                + $"Enclosing: {EnclosingNode?.Kind}\nPreceding: {PrecedingNode?.Kind}\nFollowing: {FollowingNode?.Kind}\n";
        }
    }
}
