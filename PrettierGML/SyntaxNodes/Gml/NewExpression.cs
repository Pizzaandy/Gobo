using PrettierGML.Printer.DocTypes;
using PrettierGML.SyntaxNodes.PrintHelpers;

namespace PrettierGML.SyntaxNodes.Gml;

internal sealed class NewExpression : GmlSyntaxNode
{
    public GmlSyntaxNode Name { get; set; }
    public GmlSyntaxNode Arguments { get; set; }

    public NewExpression(TextSpan span, GmlSyntaxNode name, GmlSyntaxNode arguments)
        : base(span)
    {
        Name = AsChild(name);
        Arguments = AsChild(arguments);
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        return Doc.Concat(
            "new",
            Name.IsEmpty ? Doc.Null : " ",
            Name.Print(ctx),
            DelimitedList.PrintInBrackets(ctx, "(", Arguments, ")", ",")
        );
    }
}
