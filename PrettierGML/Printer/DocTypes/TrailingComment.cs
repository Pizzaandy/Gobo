namespace PrettierGML.Printer.DocTypes;

internal class TrailingComment : Doc
{
    public CommentFormat Type { get; set; }
    public string Comment { get; set; } = string.Empty;
}
