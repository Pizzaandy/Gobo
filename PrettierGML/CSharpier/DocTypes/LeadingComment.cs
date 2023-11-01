namespace CSharpier.DocTypes;

internal class LeadingComment : Doc
{
    public CommentFormat Type { get; init; }
    public string Comment { get; init; } = string.Empty;
}
