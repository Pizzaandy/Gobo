using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;
using PrettierGML.SyntaxNodes.PrintHelpers;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal sealed class ConstructorClause : GmlSyntaxNode
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

        public override Doc PrintNode(PrintContext ctx)
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
