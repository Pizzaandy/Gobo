using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal class StructProperty : GmlSyntaxNode
    {
        public GmlSyntaxNode Name { get; set; }
        public GmlSyntaxNode Initializer { get; set; }

        public StructProperty(
            ParserRuleContext context,
            GmlSyntaxNode name,
            GmlSyntaxNode initializer
        )
            : base(context)
        {
            Name = AsChild(name);
            Initializer = AsChild(initializer);
        }

        public override Doc PrintNode(PrintContext ctx)
        {
            var name = Name is Literal literal ? literal.Text.Trim('"') : Name.Print(ctx);

            if (Initializer.IsEmpty)
            {
                return name;
            }
            else
            {
                return Doc.Concat(name, ":", " ", Initializer.Print(ctx));
            }
        }
    }
}
