using Antlr4.Runtime;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class ConstructorClause : GmlSyntaxNode
    {
        public GmlSyntaxNode Id { get; set; }
        public GmlSyntaxNode Parameters { get; set; }

        public ConstructorClause(
            ParserRuleContext context,
            CommonTokenStream tokenStream,
            GmlSyntaxNode id,
            GmlSyntaxNode parameters
        )
            : base(context, tokenStream)
        {
            Id = AsChild(id);
            Parameters = AsChild(parameters);
        }

        public override Doc Print()
        {
            if (!Id.IsEmpty)
            {
                return Doc.Concat(
                    ": ",
                    Id.Print(),
                    PrintHelper.PrintArgumentListLikeSyntax("(", Parameters, ")", ","),
                    " ",
                    "constructor"
                );
            }
            else
            {
                return Doc.Concat(" ", "constructor");
            }
        }
    }
}
