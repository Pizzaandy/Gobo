using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;
using PrettierGML.SyntaxNodes.PrintHelpers;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal class StructExpression : GmlSyntaxNode
    {
        public List<GmlSyntaxNode> Properties => Children;

        public StructExpression(ParserRuleContext context, List<GmlSyntaxNode> properties)
            : base(context)
        {
            AsChildren(properties);
        }

        public override Doc Print(PrintContext ctx)
        {
            if (Children.Any())
            {
                return DelimitedList.PrintInBrackets(ctx, "{", this, "}", ",", true);
            }
            else
            {
                return EmptyStruct;
            }
        }

        public static string EmptyStruct => "{}";
    }
}
