using Gobo.Printer.DocTypes;
using Gobo.SyntaxNodes.PrintHelpers;

namespace Gobo.SyntaxNodes.Gml;

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
        var caseText = Test.IsEmpty ? "default" : "case" + " ";
        var caseLabel = Doc.Concat(caseText, Doc.Concat(Test.Print(ctx), ":"));

        if (Statements.Count == 0)
        {
            return caseLabel;
        }

        if (ctx.Options.BraceSwitchCases)
        {
            var body = GetBody();

            return Doc.Concat(
                caseLabel,
                ShouldPrintOnOneLine(body)
                    ? Doc.Concat(" ", Doc.Join(" ", body.Select(s => Statement.PrintStatement(ctx, s))))
                    : Doc.Concat(" ", Block.WrapInBlock(ctx, Statement.PrintStatements(ctx, body)))
            );
        }

        var onlyBlock = Statements.Count == 1 && Statements.First() is Block;

        Doc printedStatements = onlyBlock
            ? Doc.Concat(" ", Statement.PrintStatement(ctx, Statements.First()))
            : Doc.Indent(Doc.HardLine, Statement.PrintStatements(ctx, Statements));

        return Doc.Concat(caseLabel, printedStatements);
    }

    /// <summary>
    /// The statements of the case, with a block around the whole body unwrapped.
    /// </summary>
    private List<GmlSyntaxNode> GetBody()
    {
        if (Statements.Count == 1 && Statements.First() is Block block)
        {
            return block.Statements;
        }

        return Statements;
    }

    /// <summary>
    /// A case earns braces once it holds more than one statement, not counting a trailing
    /// 'break'.
    /// </summary>
    private static bool ShouldPrintOnOneLine(List<GmlSyntaxNode> body)
    {
        return body.Count == 0 || Statement.HoldsOneStatement(body);
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();

        hashCode.Add(Kind);
        hashCode.Add(Test);

        foreach (var statement in GetBody())
        {
            hashCode.Add(statement);
        }

        return hashCode.ToHashCode();
    }
}
