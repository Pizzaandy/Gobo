using Antlr4.Runtime;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class MemberIndexExpression : GmlSyntaxNode, IHasObject
    {
        public GmlSyntaxNode Object { get; set; }
        public GmlSyntaxNode Properties { get; set; }
        public string Accessor { get; set; }

        public MemberIndexExpression(
            ParserRuleContext context,
            CommonTokenStream tokenStream,
            GmlSyntaxNode @object,
            GmlSyntaxNode properties,
            string accessor
        )
            : base(context, tokenStream)
        {
            Object = AsChild(@object);
            Properties = AsChild(properties);
            Accessor = accessor;
        }

        public override Doc Print()
        {
            return Doc.Concat(
                Object.Print(),
                PrintHelper.PrintArgumentListLikeSyntax(Accessor, Properties, "]", ",")
            );
        }
    }
}
