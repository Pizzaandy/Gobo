using Antlr4.Runtime;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class VariableDeclarator : GmlSyntaxNode
    {
        public GmlSyntaxNode Id { get; set; }
        public GmlSyntaxNode Initializer { get; set; }

        public VariableDeclarator(
            ParserRuleContext context,
            CommonTokenStream tokenStream,
            GmlSyntaxNode id,
            GmlSyntaxNode initializer
        )
            : base(context, tokenStream)
        {
            Id = AsChild(id);
            Initializer = AsChild(initializer);
        }

        public override Doc Print()
        {
            if (Initializer.IsEmpty)
            {
                return Id.Print();
            }
            else
            {
                return RightHandSide.Print(Id, Doc.Concat(" ", "="), Initializer);
            }
        }
    }
}
