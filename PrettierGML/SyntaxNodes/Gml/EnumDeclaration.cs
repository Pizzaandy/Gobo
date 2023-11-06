using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal sealed class EnumDeclaration : GmlSyntaxNode
    {
        public GmlSyntaxNode Name { get; set; }
        public GmlSyntaxNode Members { get; set; }

        public EnumDeclaration(ParserRuleContext context, GmlSyntaxNode name, GmlSyntaxNode members)
            : base(context)
        {
            Name = AsChild(name);
            Members = AsChild(members);
        }

        public override Doc PrintNode(PrintContext ctx)
        {
            return Doc.Concat(
                "enum",
                " ",
                Name.Print(ctx),
                ctx.Options.BraceStyle == BraceStyle.NewLine || Name.EndsWithSingleLineComment()
                    ? Doc.HardLine
                    : " ",
                Members.Print(ctx)
            );
        }
    }
}
