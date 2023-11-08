namespace PrettierGML.Printer.DocTypes;

internal class EndOfLineComment : Doc, IHasContents
{
    public Doc Contents { get; set; } = Null;
    public bool OwnLine { get; set; }

    public EndOfLineComment(Doc contents, bool ownLine)
    {
        Contents = contents;
        OwnLine = ownLine;
    }
}
