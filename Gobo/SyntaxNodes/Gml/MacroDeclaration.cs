using Gobo.Printer.DocTypes;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class MacroDeclaration : GmlSyntaxNode
{
    public string Id { get; set; }
    public string Body { get; set; }

    public MacroDeclaration(TextSpan span, string id, string body)
        : base(span)
    {
        Id = id;
        Body = body.ReplaceLineEndings("\n");
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        foreach (var commentGroup in DanglingComments)
        {
            commentGroup.PrintedRaw = true;
        }

        return Doc.Concat("#macro", " ", Id, " ", Body.TrimEnd());
    }

    public override int GetHashCode()
    {
        return (Id + Body).GetHashCode();
    }
}
