using Gobo.Printer.DocTypes;
using Gobo.SyntaxNodes.PrintHelpers;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class ArgumentList : GmlSyntaxNode
{
    public GmlSyntaxNode[] Arguments => Children;
    public static Doc EmptyArguments => "()";

    public ArgumentList(TextSpan span, GmlSyntaxNode[] arguments)
        : base(span)
    {
        Children = arguments;
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        Doc result;

        PrintOwnComments = false;

        if (Children.Length == 0 && !DanglingComments.Any())
        {
            return EmptyArguments;
        }

        if (ShouldBreakOnLastArgument())
        {
            var printedArguments = PrintChildren(ctx);

            Doc optionA;

            if (Children.Length == 1)
            {
                optionA = Doc.Group("(", Doc.Concat(printedArguments), ")");
            }
            else
            {
                var last = printedArguments.Last();
                var allExceptLast = printedArguments.SkipLast(1);

                var separator = Doc.Concat(",", " ");

                optionA = Doc.Group("(", Doc.Join(separator, allExceptLast), separator, last, ")");
            }

            var optionB = DelimitedList.PrintInBrackets(ctx, this, "(", Children, ")", ",");

            result = Doc.ConditionalGroup(optionA, optionB);
        }
        else
        {
            result = DelimitedList.PrintInBrackets(
                ctx,
                this,
                "(",
                Children,
                ")",
                ",",
                leadingContents: PrintLeadingComments(ctx)
            );
        }

        return Doc.Concat(result, PrintTrailingComments(ctx));
    }

    private bool ShouldBreakOnLastArgument()
    {
        if (
            Children.Length == 0
            || LeadingComments.Any() // The leading comment(s) will end up inside the argument list
            || Children.Any(c => c.Comments.Any(g => g.Placement == CommentPlacement.OwnLine))
        )
        {
            return false;
        }

        if (Children.Length == 1)
        {
            return Children[0] is FunctionDeclaration or StructExpression;
        }

        return Children.Last() is FunctionDeclaration or StructExpression
            && Children.Take(Children.Length - 1).All(arg => arg is not FunctionDeclaration);
    }
}
