using Antlr4.Runtime;
using PrettierGML.Printer.Document.DocTypes;
using PrettierGML.Nodes.PrintHelpers;
using PrettierGML.Printer;

namespace PrettierGML.Nodes.SyntaxNodes
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
