using Antlr4.Runtime;
using PrettierGML.Nodes.PrintHelpers;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class VariableDeclarator : GmlSyntaxNode
    {
        public GmlSyntaxNode Id { get; set; }
        public GmlSyntaxNode Initializer { get; set; }

        public VariableDeclarator(
            ParserRuleContext context,
            GmlSyntaxNode id,
            GmlSyntaxNode initializer
        )
            : base(context)
        {
            Id = AsChild(id);
            Initializer = AsChild(initializer);
        }

        public override Doc Print(PrintContext ctx)
        {
            if (Initializer.IsEmpty)
            {
                return Id.Print(ctx);
            }
            else
            {
                return RightHandSide.Print(ctx, Id, Doc.Concat(" ", "="), Initializer);
            }
        }
    }
}
