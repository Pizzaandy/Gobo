namespace PrettierGML.Printer.Document.DocTypes;

internal class NullDoc : Doc
{
    public static NullDoc Instance { get; } = new();

    private NullDoc() { }
}
