namespace PrettierGML.Printer.DocTypes
{
    /// <summary>
    /// Ensures that the comment is separated from other tokens by a single space
    /// </summary>
    internal class InlineComment : Doc, IHasContents
    {
        public Doc Contents { get; set; } = Null;

        public InlineComment(Doc contents)
        {
            Contents = contents;
        }
    }
}
