using PrettierGML.Printer.DocTypes;

namespace PrettierGML.SyntaxNodes.GmlExtensions;

internal class TypeAnnotation : GmlSyntaxNode
{
    public List<string> Types { get; set; }

    public TypeAnnotation(TextSpan span, List<string> types)
        : base(span)
    {
        Types = types;
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        if (ctx.Options.RemoveSyntaxExtensions)
        {
            return Doc.Null;
        }

        var parts = new List<Doc>();
        foreach (var typeName in Types)
        {
            parts.Add(typeName);
        }
        return Doc.Concat(":", " ", Doc.Join(" | ", parts));
    }
}
