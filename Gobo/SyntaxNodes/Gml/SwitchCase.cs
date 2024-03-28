using Gobo.Printer.DocTypes;
using Gobo.SyntaxNodes.PrintHelpers;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class SwitchCase : GmlSyntaxNode
{
    public GmlSyntaxNode Test { get; set; }
    public GmlSyntaxNode[] Statements { get; set; }

    public SwitchCase(TextSpan span, GmlSyntaxNode test, GmlSyntaxNode[] statements)
        : base(span)
    {
        Children = [test, .. statements];
        Test = test;
        Statements = statements;
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        var caseText = Test.IsEmpty ? "default" : "case" + " ";

        Doc printedStatements = Doc.Null;

        if (Statements.Length > 0)
        {
            var onlyBlock = Statements.Length == 1 && Statements.First() is Block;

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
            Statements.Length > 0 ? printedStatements : Doc.Null
        );
    }
}
