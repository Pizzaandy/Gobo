using Gobo.Printer.DocTypes;
using Gobo.SyntaxNodes.PrintHelpers;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class FunctionDeclaration : GmlSyntaxNode
{
    public GmlSyntaxNode Id { get; set; }
    public GmlSyntaxNode Parameters { get; set; }
    public GmlSyntaxNode Body { get; set; }
    public GmlSyntaxNode ConstructorParent { get; set; }

    public FunctionDeclaration(
        TextSpan span,
        GmlSyntaxNode id,
        GmlSyntaxNode parameters,
        GmlSyntaxNode body,
        GmlSyntaxNode constructorClause
    )
        : base(span)
    {
        Children = [id, parameters, body, constructorClause];
        Id = id;
        Parameters = parameters;
        Body = body;
        ConstructorParent = constructorClause;
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        var parts = new List<Doc>
        {
            Doc.Concat("function", Id.IsEmpty ? "" : " ", Id.Print(ctx)),
            Parameters.Print(ctx)
        };

        if (!ConstructorParent.IsEmpty)
        {
            parts.Add(ConstructorParent.Print(ctx));
        }

        parts.Add(" ");
        parts.Add(Statement.EnsureStatementInBlock(ctx, Body));

        return Doc.Concat(parts);
    }

    public bool IsConstructor => !ConstructorParent.IsEmpty;
}
