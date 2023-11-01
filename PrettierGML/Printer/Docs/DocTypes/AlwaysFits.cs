namespace PrettierGML.Printer.Docs.DocTypes;

internal class AlwaysFits : Doc
{
    public readonly Doc Contents;

    public AlwaysFits(Doc printedTrivia)
    {
        Contents = printedTrivia;
    }
}
