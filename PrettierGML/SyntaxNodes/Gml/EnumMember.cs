using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal sealed class EnumMember : GmlSyntaxNode
    {
        public GmlSyntaxNode Id { get; set; }
        public GmlSyntaxNode Initializer { get; set; }

        public EnumMember(ParserRuleContext context, GmlSyntaxNode id, GmlSyntaxNode initializer)
            : base(context)
        {
            Id = AsChild(id);
            Initializer = AsChild(initializer);
        }

        public override Doc PrintNode(PrintContext ctx)
        {
            if (Initializer.IsEmpty)
            {
                return Id.Print(ctx);
            }
            else
            {
                return Doc.Concat(Id.Print(ctx), " ", "=", " ", Initializer.Print(ctx));
            }
        }
    }
}
