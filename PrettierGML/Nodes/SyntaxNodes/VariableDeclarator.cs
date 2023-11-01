using Antlr4.Runtime;
using PrettierGML.Nodes.PrintHelpers;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class VariableDeclarator : GmlSyntaxNode
    {
        public GmlSyntaxNode Id { get; set; }
        public GmlSyntaxNode Type { get; set; }
        public GmlSyntaxNode Initializer { get; set; }

        public VariableDeclarator(
            ParserRuleContext context,
            GmlSyntaxNode id,
            GmlSyntaxNode type,
            GmlSyntaxNode initializer
        )
            : base(context)
        {
            Id = AsChild(id);
            Type = AsChild(type);
            Initializer = AsChild(initializer);
        }

        public override Doc Print(PrintContext ctx)
        {
            var typeAnnotation = Type.Print(ctx);

            if (Initializer.IsEmpty)
            {
                return Doc.Concat(Id.Print(ctx), typeAnnotation);
            }
            else
            {
                return RightHandSide.Print(
                    ctx,
                    Id,
                    Doc.Concat(typeAnnotation, " ", "="),
                    Initializer
                );
            }
        }
    }
}
