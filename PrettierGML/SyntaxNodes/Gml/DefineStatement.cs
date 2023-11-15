using PrettierGML.Printer.DocTypes;

namespace PrettierGML.SyntaxNodes.Gml;

internal sealed class DefineStatement : GmlSyntaxNode
{
    public string Name { get; set; }

    public DefineStatement(TextSpan span, string name)
        : base(span)
    {
        Name = name;
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        return Doc.Concat("#define", " ", Name);
    }
}
