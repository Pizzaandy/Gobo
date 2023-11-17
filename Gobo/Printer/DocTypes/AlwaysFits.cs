namespace Gobo.Printer.DocTypes;

internal class AlwaysFits : Doc
{
    public readonly Doc Contents;

    public AlwaysFits(Doc printedTrivia)
    {
        Contents = printedTrivia;
    }
}
