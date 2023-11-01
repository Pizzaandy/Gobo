using Antlr4.Runtime;
using PrettierGML.Nodes.PrintHelpers;
using PrettierGML.Printer.Document.DocTypes;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class ConstructorClause : GmlSyntaxNode
    {
        public GmlSyntaxNode Id { get; set; }
        public GmlSyntaxNode Parameters { get; set; }

        public ConstructorClause(
            ParserRuleContext context,
            GmlSyntaxNode id,
            GmlSyntaxNode parameters
        )
            : base(context)
        {
            Id = AsChild(id);
            Parameters = AsChild(parameters);
        }

        public override Doc Print(PrintContext ctx)
        {
            if (!Id.IsEmpty)
            {
                return Doc.Concat(
                    ": ",
                    Id.Print(ctx),
                    DelimitedList.PrintInBrackets(ctx, "(", Parameters, ")", ","),
                    " ",
                    "constructor"
                );
            }
            else
            {
                return Doc.Concat(" ", "constructor");
            }
        }
    }
}
