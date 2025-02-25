namespace Gobo.Printer.DocTypes;

internal class IfBreak : Doc
{
    public Doc FlatContents { get; set; } = Null;
    public Doc BreakContents { get; set; } = Null;
    public string? GroupId { get; set; }
}

internal sealed class IndentIfBreak : IfBreak
{
    public IndentIfBreak(Doc contents, string groupId)
    {
        BreakContents = Indent(contents);
        FlatContents = contents;
        GroupId = groupId;
    }
}
