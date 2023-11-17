namespace Gobo.Printer.DocTypes;

internal class EndOfLineComment : Doc, IHasContents
{
    public Doc Contents { get; set; } = Null;
    public string Id { get; init; }

    public EndOfLineComment(Doc contents, string id)
    {
        Contents = contents;
        Id = id;
    }
}
