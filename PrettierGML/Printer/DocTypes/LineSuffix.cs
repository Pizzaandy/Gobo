namespace PrettierGML.Printer.DocTypes;

internal class LineSuffix : Doc, IHasContents
{
    public Doc Contents { get; set; } = Null;
    public bool OwnLine { get; set; }

    public LineSuffix(Doc contents, bool ownLine)
    {
        Contents = contents;
        OwnLine = ownLine;
    }
}
