using Antlr4.Runtime;
using PrettierGML.Printer;
using PrettierGML.Printer.Docs.DocTypes;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class MemberDotExpression : GmlSyntaxNode, IHasObject
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

        public override Doc Print(PrintContext ctx)
        {
            return Doc.Concat(Object.Print(ctx), ".", Property.Print(ctx));
        }
    }
}
