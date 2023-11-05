using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;
using PrettierGML.SyntaxNodes.PrintHelpers;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal class EnumDeclaration : GmlSyntaxNode
    {
        public GmlSyntaxNode Name { get; set; }
        public List<GmlSyntaxNode> Members { get; set; }

        public EnumDeclaration(
            ParserRuleContext context,
            GmlSyntaxNode name,
            List<GmlSyntaxNode> members
        )
            : base(context)
        {
            Name = AsChild(name);
            Members = AsChildren(members);
        }

        public override Doc PrintNode(PrintContext ctx)
        {
            var printedBlock = DelimitedList.PrintInBrackets(
                ctx,
                "{",
                Members,
                "}",
                ",",
                allowTrailingSeparator: true,
                forceBreak: true
            );

            return Doc.Concat(
                "enum",
                " ",
                Name.Print(ctx),
                ctx.Options.BraceStyle == BraceStyle.NewLine ? Doc.HardLine : " ",
                printedBlock
            );
        }
    }
}
