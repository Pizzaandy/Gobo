using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;
using PrettierGML.SyntaxNodes.PrintHelpers;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal sealed class VariableDeclarationList : GmlSyntaxNode
    {
        public List<GmlSyntaxNode> Declarations { get; set; }
        public string Modifier { get; set; }

        public VariableDeclarationList(
            TextSpan span,
            List<GmlSyntaxNode> declarations,
            string modifier
        )
            : base(span)
        {
            Declarations = AsChildren(declarations);
            Modifier = modifier;
        }

        public override Doc PrintNode(PrintContext ctx)
        {
            var parts = new List<Doc>() { Modifier, " " };

            if (Children.Count == 1)
            {
                parts.Add(Children.First().Print(ctx));
            }
            else if (Children.Count > 0)
            {
                var printedArguments = DelimitedList.Print(ctx, Declarations, ",");
                parts.Add(Doc.Indent(printedArguments));
            }

            return Doc.Group(parts);
        }
    }
}
