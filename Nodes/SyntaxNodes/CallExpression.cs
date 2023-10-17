using Antlr4.Runtime;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class CallExpression : GmlSyntaxNode, IHasObject
    {
        public GmlSyntaxNode Object { get; set; }
        public GmlSyntaxNode Arguments { get; set; }

        public CallExpression(
            ParserRuleContext context,
            CommonTokenStream tokenStream,
            GmlSyntaxNode @object,
            GmlSyntaxNode arguments
        )
            : base(context, tokenStream)
        {
            Object = AsChild(@object);
            Arguments = AsChild(arguments);
        }

        public override Doc Print()
        {
            return Doc.Concat(
                Object.Print(),
                PrintHelper.PrintArgumentListLikeSyntax("(", Arguments, ")", ",")
            );
        }
    }
}
