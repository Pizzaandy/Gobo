namespace Gobo.Printer.DocTypes;

internal class ForceFlat : Doc, IHasContents
{
    public Doc Contents { get; set; } = Null;
}
