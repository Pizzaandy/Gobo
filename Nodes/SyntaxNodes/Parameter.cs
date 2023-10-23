using Antlr4.Runtime;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class Parameter : GmlSyntaxNode
    {
        public GmlSyntaxNode Name { get; set; }
        public GmlSyntaxNode Initializer { get; set; }

        public Parameter(ParserRuleContext context, GmlSyntaxNode name, GmlSyntaxNode initializer)
            : base(context)
        {
            Name = AsChild(name);
            Initializer = AsChild(initializer);
        }

        public override Doc Print(PrintContext ctx)
        {
            if (Initializer.IsEmpty)
            {
                return Name.Print(ctx);
            }
            else
            {
                return Doc.Concat(Name.Print(ctx), " = ", Initializer.Print(ctx));
            }
        }
    }
}
