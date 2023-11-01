namespace PrettierGML.Printer.Docs.DocTypes;

internal class IndentDoc : Doc, IHasContents
{
    public Doc Contents { get; set; } = Null;
}
