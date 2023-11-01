namespace PrettierGML.Printer.Document.DocTypes;

internal class Fill : Doc
{
    public IList<Doc> Contents { get; set; }

    public Fill(IList<Doc> contents)
    {
        Contents = contents;
    }
}
