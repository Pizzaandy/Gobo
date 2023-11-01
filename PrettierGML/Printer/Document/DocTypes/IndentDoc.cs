namespace PrettierGML.Printer.Document.DocTypes;

internal class IndentDoc : Doc, IHasContents
{
    public Doc Contents { get; set; } = Null;
}
