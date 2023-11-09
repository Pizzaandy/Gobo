using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;
using PrettierGML.SyntaxNodes.PrintHelpers;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal sealed class MemberDotExpression : GmlSyntaxNode, IMemberChainable
    {
        public GmlSyntaxNode Object { get; set; }
        public GmlSyntaxNode Property { get; set; }

        public MemberDotExpression(
            ParserRuleContext context,
            GmlSyntaxNode @object,
            GmlSyntaxNode property
        )
            : base(context)
        {
            Object = AsChild(@object);
            Property = AsChild(property);
        }

        public override Doc PrintNode(PrintContext ctx)
        {
            return MemberChain.PrintMemberChain(ctx, this);
        }

        public Doc PrintChain(PrintContext ctx)
        {
            return Doc.Concat(".", Property.Print(ctx));
        }
    }
}
