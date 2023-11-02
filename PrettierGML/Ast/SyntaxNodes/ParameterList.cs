using Antlr4.Runtime;
using PrettierGML.Nodes.PrintHelpers;
using PrettierGML.Printer;
using PrettierGML.Printer.DocTypes;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class ParameterList : GmlSyntaxNode
    {
        public List<GmlSyntaxNode> Parameters => Children;

        public ParameterList(ParserRuleContext context, List<GmlSyntaxNode> parameters)
            : base(context)
        {
            AsChildren(parameters);
        }

        public override Doc Print(PrintContext ctx)
        {
            return DelimitedList.PrintInBrackets(ctx, "(", this, ")", ",");
        }
    }
}
