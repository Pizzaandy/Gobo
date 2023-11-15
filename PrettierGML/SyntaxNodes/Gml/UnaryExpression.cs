using PrettierGML.Printer.DocTypes;

namespace PrettierGML.SyntaxNodes.Gml;

internal sealed class UnaryExpression : GmlSyntaxNode
{
    public string Operator { get; set; }
    public GmlSyntaxNode Argument { get; set; }
    public bool IsPrefix { get; set; }

    public UnaryExpression(
        TextSpan span,
        string @operator,
        GmlSyntaxNode argument,
        bool isPrefix
    )
        : base(span)
    {
        Operator = @operator;
        Argument = AsChild(argument);
        IsPrefix = isPrefix;
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        if (IsPrefix)
        {
            return Doc.Concat(Operator, Argument.Print(ctx));
        }
        else
        {
            return Doc.Concat(Argument.Print(ctx), Operator);
        }
    }
}
