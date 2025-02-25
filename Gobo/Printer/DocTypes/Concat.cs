namespace Gobo.Printer.DocTypes;

internal sealed class Concat : Doc
{
    public IList<Doc> Contents { get; set; }

    public Concat(IList<Doc> contents)
    {
        Contents = contents;
    }
}
