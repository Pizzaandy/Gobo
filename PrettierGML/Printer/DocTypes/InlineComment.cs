namespace PrettierGML.Printer.DocTypes
{
    /// <summary>
    /// Ensures that the comment is separated from other tokens by a single space
    /// </summary>
    internal class InlineComment : Doc, IHasContents
    {
        public Doc Contents { get; set; } = Null;
        public string Id { get; init; }

        public InlineComment(Doc contents, string id)
        {
            Contents = contents;
            Id = id;
        }
    }
}
