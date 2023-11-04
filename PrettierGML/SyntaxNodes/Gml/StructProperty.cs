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
            if (Initializer.IsEmpty)
            {
                return Name.Print(ctx);
            }
            else
            {
                return Doc.Concat(Name.Print(ctx), ":", " ", Initializer.Print(ctx));
            }
        }
    }
}
