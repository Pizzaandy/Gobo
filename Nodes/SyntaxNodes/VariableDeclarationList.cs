using Antlr4.Runtime;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class VariableDeclarationList : GmlSyntaxNode
    {
        public GmlSyntaxNode Declarations { get; set; }
        public string Modifier { get; set; }

        public VariableDeclarationList(
            ParserRuleContext context,
            GmlSyntaxNode declarations,
            string modifier
        )
            : base(context)
        {
            Declarations = AsChild(declarations);
            Modifier = modifier;
        }

        public override Doc Print()
        {
            return Doc.Concat(Modifier, " ", Doc.Group(PrintHelper.PrintSeparatedList(Declarations, ",")));
        }
    }
}
