using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;
using PrettierGML.SyntaxNodes.PrintHelpers;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal sealed class GlobalVariableStatement : GmlSyntaxNode
    {
        public List<GmlSyntaxNode> Declarations => Children;

        public GlobalVariableStatement(ParserRuleContext context, List<GmlSyntaxNode> declarations)
            : base(context)
        {
            AsChildren(declarations);
        }

        public override Doc PrintNode(PrintContext ctx)
        {
            var parts = new List<Doc>() { "globalvar", " " };

            if (Children.Count == 1)
            {
                parts.Add(Children.First().Print(ctx));
            }
            else if (Children.Any())
            {
                var printedArguments = DelimitedList.Print(ctx, Children, ",");
                parts.Add(Doc.Indent(printedArguments));
            }

            return Doc.Group(parts);
        }
    }
}
