using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;
using PrettierGML.SyntaxNodes.PrintHelpers;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal sealed class MemberIndexExpression : GmlSyntaxNode, IMemberChainable
    {
        public GmlSyntaxNode Object { get; set; }
        public GmlSyntaxNode Properties { get; set; }
        public string Accessor { get; set; }

        public MemberIndexExpression(
            ParserRuleContext context,
            GmlSyntaxNode @object,
            GmlSyntaxNode properties,
            string accessor
        )
            : base(context)
        {
            Object = AsChild(@object);
            Properties = AsChild(properties);
            Accessor = accessor;
        }

        public override Doc PrintNode(PrintContext ctx)
        {
            return MemberChain.PrintMemberChain(ctx, this);
        }

        public Doc PrintChain(PrintContext ctx)
        {
            var accessor = Accessor.Length > 1 ? Accessor + " " : Accessor;
            return DelimitedList.PrintInBrackets(ctx, accessor, Properties, "]", ",");
        }
    }
}
