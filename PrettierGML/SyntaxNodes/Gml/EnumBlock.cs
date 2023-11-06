using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;
using PrettierGML.SyntaxNodes.PrintHelpers;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal sealed class EnumBlock : GmlSyntaxNode
    {
        public List<GmlSyntaxNode> Members => Children;

        public EnumBlock(ParserRuleContext context, List<GmlSyntaxNode> members)
            : base(context)
        {
            AsChildren(members);
        }

        public override Doc PrintNode(PrintContext ctx)
        {
            return DelimitedList.PrintInBrackets(
                ctx,
                "{",
                this,
                "}",
                ",",
                allowTrailingSeparator: true,
                forceBreak: true
            );
        }
    }
}
