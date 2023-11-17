namespace Gobo.Printer.DocTypes;

internal class Fill : Doc
{
    public IList<Doc> Contents { get; set; }

    public Fill(IList<Doc> contents)
    {
        Contents = contents;
    }
}
