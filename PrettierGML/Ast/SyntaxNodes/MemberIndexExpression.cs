using Antlr4.Runtime;
using PrettierGML.Nodes.PrintHelpers;
using PrettierGML.Printer;
using PrettierGML.Printer.DocTypes;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class MemberIndexExpression : GmlSyntaxNode, IHasObject
    {
        public GmlSyntaxNode Object { get; set; }
        public GmlSyntaxNode Properties { get; set; }
        public string Accessor { get; set; }

        public MemberIndexExpression(
            ParserRuleContext context,
            GmlSyntaxNode @object,
            GmlSyntaxNode properties,
            string accessor
        )
            : base(context)
        {
            Object = AsChild(@object);
            Properties = AsChild(properties);
            Accessor = accessor;
        }

        public override Doc Print(PrintContext ctx)
        {
            var accessor = Accessor.Length > 1 ? Accessor + " " : Accessor;
            return Doc.Concat(
                Object.Print(ctx),
                DelimitedList.PrintInBrackets(ctx, accessor, Properties, "]", ",")
            );
        }
    }
}
