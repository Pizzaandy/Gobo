namespace PrettierGML
{
    internal class Comment
    {
        internal enum CommentType
        {
            Leading,
            Trailing,
            Dangling,
        }

        public string Text { get; init; }
        public CommentType Type { get; set; }

        public Comment(string text)
        {
            Text = text;
        }
    }
}
