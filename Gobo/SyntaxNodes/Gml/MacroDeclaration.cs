using Gobo.Printer.DocTypes;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class MacroDeclaration : GmlSyntaxNode
{
    public string Id { get; set; }

    public string? Config { get; set; }
    public string Body { get; set; }

    public MacroDeclaration(TextSpan span, string id, string body, string? configName)
        : base(span)
    {
        Id = id;
        Body = body.ReplaceLineEndings("\n");
        Config = configName;
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        foreach (var commentGroup in DanglingComments)
        {
            commentGroup.PrintedRaw = true;
        }

        var name = string.IsNullOrEmpty(Config) ? Id : $"{Config}:{Id}";

        return Doc.Concat("#macro", " ", name, " ", Body.TrimEnd());
    }

    public override int GetHashCode()
    {
        return (Id + Body).GetHashCode();
    }
}
