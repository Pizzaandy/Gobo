namespace Gobo.Printer.DocTypes;

internal class IndentDoc : Doc, IHasContents
{
    public Doc Contents { get; set; } = Null;
}
