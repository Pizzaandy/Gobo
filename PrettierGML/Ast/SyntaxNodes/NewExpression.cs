using Antlr4.Runtime;
using PrettierGML.Printer.Docs.DocTypes;
using PrettierGML.Nodes.PrintHelpers;
using PrettierGML.Printer;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class NewExpression : GmlSyntaxNode
    {
        public GmlSyntaxNode Name { get; set; }
        public GmlSyntaxNode Arguments { get; set; }

        public NewExpression(ParserRuleContext context, GmlSyntaxNode name, GmlSyntaxNode arguments)
            : base(context)
        {
            Name = AsChild(name);
            Arguments = AsChild(arguments);
        }

        public override Doc Print(PrintContext ctx)
        {
            return Doc.Concat(
                "new",
                Name.IsEmpty ? Doc.Null : " ",
                Name.Print(ctx),
                DelimitedList.PrintInBrackets(ctx, "(", Arguments, ")", ",")
            );
        }
    }
}
