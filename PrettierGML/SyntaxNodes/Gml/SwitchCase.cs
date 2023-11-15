using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;
using PrettierGML.SyntaxNodes.PrintHelpers;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal sealed class SwitchCase : GmlSyntaxNode
    {
        public GmlSyntaxNode Test { get; set; }
        public List<GmlSyntaxNode> Statements { get; set; }

        public SwitchCase(TextSpan span, GmlSyntaxNode test, List<GmlSyntaxNode> statements)
            : base(span)
        {
            Test = AsChild(test);
            Statements = AsChildren(statements);
        }

        public override Doc PrintNode(PrintContext ctx)
        {
            var caseText = Test.IsEmpty ? "default" : $"{"case"} ";

            Doc printedStatements = Doc.Null;

            if (Statements.Count > 0)
            {
                var onlyBlock = Statements.Count == 1 && Statements.First() is Block;

                if (onlyBlock)
                {
                    printedStatements = Doc.Concat(
                        " ",
                        Statement.PrintStatement(ctx, Statements.First())
                    );
                }
                else
                {
                    printedStatements = Doc.Indent(
                        Doc.HardLine,
                        Statement.PrintStatements(ctx, Statements)
                    );
                }
            }

            return Doc.Concat(
                caseText,
                Doc.Concat(Test.Print(ctx), ":"),
                Statements.Count > 0 ? printedStatements : Doc.Null
            );
        }
    }
}
