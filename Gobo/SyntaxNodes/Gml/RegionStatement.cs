using Gobo.Printer.DocTypes;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class RegionStatement : GmlSyntaxNode
{
    public string? Name { get; set; }
    public bool IsEndRegion { get; set; }

    public RegionStatement(TextSpan span, string? name, bool isEndRegion)
        : base(span)
    {
        Name = name;
        IsEndRegion = isEndRegion;
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        return IsEndRegion ? "#endregion" : Doc.Concat("#region", Name!);
    }
}
