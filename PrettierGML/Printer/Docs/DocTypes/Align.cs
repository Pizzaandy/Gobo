namespace PrettierGML.Printer.Docs.DocTypes;

internal class Align : Doc, IHasContents
{
    public int Width { get; }
    public Doc Contents { get; }

    public Align(int width, Doc contents)
    {
        if (width < 1)
        {
            throw new Exception($"{nameof(width)} must be >= 1");
        }

        Width = width;
        Contents = contents;
    }
}
