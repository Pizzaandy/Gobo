using Antlr4.Runtime;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class NewExpression : GmlSyntaxNode
    {
        public GmlSyntaxNode Name { get; set; }
        public GmlSyntaxNode Arguments { get; set; }

        public NewExpression(
            ParserRuleContext context,
            CommonTokenStream tokenStream,
            GmlSyntaxNode name,
            GmlSyntaxNode arguments
        )
            : base(context, tokenStream)
        {
            Name = AsChild(name);
            Arguments = AsChild(arguments);
        }

        public override Doc Print()
        {
            return Doc.Concat(
                "new ",
                Name.Print(),
                PrintHelper.PrintArgumentListLikeSyntax("(", Arguments, ")", ",")
            );
        }
    }
}
