using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PrettierGML.Printer.Document.DocTypes;
using PrettierGML.Nodes;
using PrettierGML.Printer;
using PrettierGML.Ast;

namespace PrettierGML
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

    internal class CommentGroup
    {
        public string Text { get; init; }

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

        private bool printed = false;

        public CommentGroup(string text, Range characterRange, Range tokenRange)
        {
            Text = text;
            CharacterRange = characterRange;
            TokenRange = tokenRange;
        }

        public virtual Doc Print(PrintContext ctx, bool checkPrinted = true)
        {
            if (checkPrinted)
            {
                if (printed)
                {
                    throw new InvalidOperationException($"Comment printed twice:\n{this}");
                }
                printed = true;
            }

            //return Type switch
            //{
            //    CommentType.Leading
            //        => Doc.LeadingComment(
            //            Text,
            //            Text.Contains('\n') ? CommentFormat.MultiLine : CommentFormat.SingleLine
            //        ),
            //    CommentType.Trailing
            //        => Doc.TrailingComment(
            //            Text,
            //            Text.Contains('\n') ? CommentFormat.MultiLine : CommentFormat.SingleLine
            //        ),
            //    CommentType.Dangling => Text,
            //    _ => throw new NotImplementedException()
            //};

            return Text;
        }

        public override string ToString()
        {
            return $"{Text}\nType: {Type}, Range: {CharacterRange}\n"
                + $"Enclosing: {EnclosingNode?.Kind}\nPreceding: {PrecedingNode?.Kind}\nFollowing: {FollowingNode?.Kind}\n";
        }
    }
}
