using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;
using PrettierGML.SyntaxNodes.PrintHelpers;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal class ParameterList : GmlSyntaxNode
    {
        public List<GmlSyntaxNode> Parameters => Children;

        public ParameterList(ParserRuleContext context, List<GmlSyntaxNode> parameters)
            : base(context)
        {
            AsChildren(parameters);
        }

        public override Doc PrintNode(PrintContext ctx)
        {
            return DelimitedList.PrintInBrackets(ctx, "(", this, ")", ",");
        }
    }
}
