namespace PrettierGML.Printer.Document.DocTypes;

internal class Region : Doc
{
    public Region(string text)
    {
        Text = text;
    }

    public string Text { get; }
    public bool IsEnd { get; init; }
}
