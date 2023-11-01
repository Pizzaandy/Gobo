using Antlr4.Runtime;
using PrettierGML.Printer.Document.DocTypes;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class EnumDeclaration : GmlSyntaxNode
    {
        public GmlSyntaxNode Name { get; set; }
        public GmlSyntaxNode Members { get; set; }

        public EnumDeclaration(ParserRuleContext context, GmlSyntaxNode name, GmlSyntaxNode members)
            : base(context)
        {
            Name = AsChild(name);
            Members = AsChild(members);
        }

        public override Doc Print(PrintContext ctx)
        {
            return Doc.Concat("enum", " ", Name.Print(ctx), " ", Members.Print(ctx));
        }
    }
}
