namespace PrettierGML.Printer.Docs.DocTypes;

internal class ForceFlat : Doc, IHasContents
{
    public Doc Contents { get; set; } = Null;
}
