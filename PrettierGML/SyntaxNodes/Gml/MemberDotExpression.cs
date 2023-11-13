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

        public Doc PrintInChain(PrintContext ctx)
        {
            Property.PrintOwnComments = false;

            return Doc.Concat(
                Property.PrintLeadingComments(ctx),
                ".",
                Property.Print(ctx),
                Property.PrintTrailingComments(ctx)
            );
        }

        public void SetObject(GmlSyntaxNode node)
        {
            Object = AsChild(node);
        }
    }
}
