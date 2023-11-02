using Antlr4.Runtime;
using PrettierGML.Nodes.PrintHelpers;
using PrettierGML.Printer;
using PrettierGML.Printer.DocTypes;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class VariableDeclarationList : GmlSyntaxNode
    {
        public List<GmlSyntaxNode> Declarations { get; set; }
        public string Modifier { get; set; }

        public VariableDeclarationList(
            ParserRuleContext context,
            List<GmlSyntaxNode> declarations,
            string modifier
        )
            : base(context)
        {
            Declarations = AsChildren(declarations);
            Modifier = modifier;
        }

        public override Doc Print(PrintContext ctx)
        {
            var parts = new List<Doc>() { Modifier, " " };

            if (Children.Count == 1)
            {
                parts.Add(Children.First().Print(ctx));
            }
            else if (Children.Any())
            {
                var printedArguments = DelimitedList.Print(ctx, Declarations, ",");
                parts.Add(Doc.Indent(printedArguments));
            }

            return Doc.Group(parts);
        }
    }
}
