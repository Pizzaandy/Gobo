using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;
using PrettierGML.SyntaxNodes.PrintHelpers;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal sealed class FunctionDeclaration : GmlSyntaxNode
    {
        public GmlSyntaxNode Id { get; set; }
        public GmlSyntaxNode Parameters { get; set; }
        public GmlSyntaxNode Body { get; set; }
        public GmlSyntaxNode ConstructorParent { get; set; }

        public FunctionDeclaration(
            ParserRuleContext context,
            GmlSyntaxNode id,
            GmlSyntaxNode parameters,
            GmlSyntaxNode body,
            GmlSyntaxNode parent
        )
            : base(context)
        {
            Id = AsChild(id);
            Parameters = AsChild(parameters);
            Body = AsChild(body);
            ConstructorParent = AsChild(parent);
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

            if (ctx.Options.BraceStyle == BraceStyle.NewLine)
            {
                parts.Add(Doc.HardLineIfNoPreviousLine);
            }
            else
            {
                parts.Add(" ");
            }

            parts.Add(Statement.EnsureStatementInBlock(ctx, Body));

            return Doc.Concat(parts);
        }

        public bool IsConstructor => !ConstructorParent.IsEmpty;
    }
}
