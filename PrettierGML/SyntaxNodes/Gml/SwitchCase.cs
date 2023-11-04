using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;
using PrettierGML.SyntaxNodes.PrintHelpers;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal class SwitchCase : GmlSyntaxNode
    {
        public GmlSyntaxNode Test { get; set; }
        public List<GmlSyntaxNode> Statements { get; set; }

        public SwitchCase(
            ParserRuleContext context,
            GmlSyntaxNode test,
            List<GmlSyntaxNode> statements
        )
            : base(context)
        {
            Test = AsChild(test);
            Statements = AsChildren(statements);
        }

        public override Doc Print(PrintContext ctx)
        {
            var caseText = Test.IsEmpty ? "default" : $"{"case"} ";
            return Doc.Concat(
                caseText,
                Test.Print(ctx),
                ":",
                Statements.Count > 0
                    ? Doc.Indent(Doc.HardLine, Statement.PrintStatements(ctx, Statements))
                    : Doc.Null
            );
        }
    }
}
