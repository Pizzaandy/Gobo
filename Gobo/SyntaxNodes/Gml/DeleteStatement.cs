using Gobo.Printer.DocTypes;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class DeleteStatement : GmlSyntaxNode
{
    public GmlSyntaxNode Argument { get; set; }

    public DeleteStatement(TextSpan span, GmlSyntaxNode argument)
        : base(span)
    {
        Children = [argument];
        Argument = argument;
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        return Doc.Concat("delete", " ", Argument.Print(ctx));
    }
}
