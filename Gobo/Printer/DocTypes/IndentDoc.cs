namespace Gobo.Printer.DocTypes;

internal sealed class IndentDoc : Doc, IHasContents
{
    public Doc Contents { get; set; } = Null;
}
