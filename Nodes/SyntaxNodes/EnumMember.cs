using Antlr4.Runtime;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class EnumMember : GmlSyntaxNode
    {
        public GmlSyntaxNode Id { get; set; }
        public GmlSyntaxNode Initializer { get; set; }

        public EnumMember(
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
                return Doc.Concat(Id.Print(), " = ", Initializer.Print());
            }
        }
    }
}
