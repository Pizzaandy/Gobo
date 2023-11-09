using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;
using PrettierGML.SyntaxNodes.PrintHelpers;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal sealed class StructExpression : GmlSyntaxNode
    {
        public List<GmlSyntaxNode> Properties => Children;

        public StructExpression(ParserRuleContext context, List<GmlSyntaxNode> properties)
            : base(context)
        {
            AsChildren(properties);
        }

        public override Doc PrintNode(PrintContext ctx)
        {
            if (Children.Count == 0 && DanglingComments.Count == 0)
            {
                return EmptyStruct;
            }
            else
            {
                return DelimitedList.PrintInBrackets(ctx, "{", this, "}", ",", true);
            }
        }

        public static string EmptyStruct => "{}";
    }
}
