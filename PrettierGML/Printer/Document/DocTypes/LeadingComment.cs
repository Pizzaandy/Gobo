namespace PrettierGML.Printer.Document.DocTypes;

internal class LeadingComment : Doc
{
    public CommentFormat Type { get; init; }
    public string Comment { get; init; } = string.Empty;
}
