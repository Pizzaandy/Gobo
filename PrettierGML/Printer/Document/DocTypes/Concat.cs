namespace PrettierGML.Printer.Document.DocTypes;

internal class Concat : Doc
{
    public IList<Doc> Contents { get; set; }

    public Concat(IList<Doc> contents)
    {
        Contents = contents;
    }
}
