using Antlr4.Runtime;
using PrettierGML.Nodes.PrintHelpers;

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

        public override Doc Print(PrintContext ctx)
        {
            var parts = new List<Doc>() { Modifier, " " };

            if (Declarations.Children.Count == 1)
            {
                parts.Add(Declarations.Children.First().Print(ctx));
            }
            else if (Declarations.Children.Any())
            {
                var printedArguments = DelimitedList.Print(ctx, Declarations, ",");
                parts.Add(Doc.Indent(printedArguments));
            }

            return Doc.Group(parts);
        }
    }
}
