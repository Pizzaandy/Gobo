namespace Gobo.Printer.DocTypes;

internal sealed class StringDoc : Doc
{
    public string Value { get; }
    public bool IsDirective { get; }

    public StringDoc(string value, bool isDirective = false)
    {
        Value = value;
        IsDirective = isDirective;
    }
}
