using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;
using PrettierGML.SyntaxNodes.PrintHelpers;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal sealed class SwitchStatement : GmlSyntaxNode
    {
        public GmlSyntaxNode Discriminant { get; set; }
        public GmlSyntaxNode Cases { get; set; }

        public SwitchStatement(
            ParserRuleContext context,
            GmlSyntaxNode discriminant,
            GmlSyntaxNode cases
        )
            : base(context)
        {
            Discriminant = AsChild(discriminant);
            Cases = AsChild(cases);
        }

        public override Doc PrintNode(PrintContext ctx)
        {
            return Statement.PrintSingleClauseStatement(ctx, "switch", Discriminant, Cases);
        }
    }
}
