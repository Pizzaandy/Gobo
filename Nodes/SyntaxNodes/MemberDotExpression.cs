using Antlr4.Runtime;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class MemberDotExpression : GmlSyntaxNode, IHasObject
    {
        public GmlSyntaxNode Object { get; set; }
        public GmlSyntaxNode Property { get; set; }

        public MemberDotExpression(
            ParserRuleContext context,
            CommonTokenStream tokenStream,
            GmlSyntaxNode @object,
            GmlSyntaxNode property
        )
            : base(context, tokenStream)
        {
            Object = AsChild(@object);
            Property = AsChild(property);
        }

        public override Doc Print()
        {
            return Doc.Concat(Object.Print(), ".", Property.Print());
        }
    }
}
