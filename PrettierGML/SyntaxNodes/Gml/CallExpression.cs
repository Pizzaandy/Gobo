using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;
using PrettierGML.SyntaxNodes.PrintHelpers;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal sealed class CallExpression : GmlSyntaxNode, IMemberChainable
    {
        public GmlSyntaxNode Object { get; set; }
        public GmlSyntaxNode Arguments { get; set; }

        public CallExpression(
            ParserRuleContext context,
            GmlSyntaxNode @object,
            GmlSyntaxNode arguments
        )
            : base(context)
        {
            Object = AsChild(@object);
            Arguments = AsChild(arguments);
        }

        public override Doc PrintNode(PrintContext ctx)
        {
            return MemberChain.PrintMemberChain(ctx, this);
        }

        public Doc PrintInChain(PrintContext ctx)
        {
            return Arguments.Print(ctx);
        }

        public void SetObject(GmlSyntaxNode node)
        {
            Object = AsChild(node);
        }
    }
}
