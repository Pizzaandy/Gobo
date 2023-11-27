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
        Body = body;
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        // Macro identifiers can't have leading comments
        var printed = Doc.Concat("#macro", " ", Id, " ", Body.TrimEnd());

        return Doc.Concat(Doc.HardLineIfNoPreviousLine, printed);
    }
}
