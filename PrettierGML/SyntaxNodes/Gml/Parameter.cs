using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal class Parameter : GmlSyntaxNode
    {
        public GmlSyntaxNode Name { get; set; }
        public GmlSyntaxNode Type { get; set; }
        public GmlSyntaxNode Initializer { get; set; }

        public Parameter(
            ParserRuleContext context,
            GmlSyntaxNode name,
            GmlSyntaxNode type,
            GmlSyntaxNode initializer
        )
            : base(context)
        {
            Name = AsChild(name);
            Type = AsChild(type);
            Initializer = AsChild(initializer);
        }

        public override Doc PrintNode(PrintContext ctx)
        {
            var typeAnnotation = Type.Print(ctx);

            if (Initializer.IsEmpty)
            {
                return Doc.Concat(Name.Print(ctx), typeAnnotation);
            }
            else
            {
                return Doc.Concat(
                    Name.Print(ctx),
                    typeAnnotation,
                    " ",
                    "=",
                    " ",
                    Initializer.Print(ctx)
                );
            }
        }
    }
}
