using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;
using PrettierGML.SyntaxNodes.PrintHelpers;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal sealed class ArgumentList : GmlSyntaxNode
    {
        public List<GmlSyntaxNode> Arguments => Children;

        public ArgumentList(ParserRuleContext context, List<GmlSyntaxNode> arguments)
            : base(context)
        {
            AsChildren(arguments);
        }

        public override Doc PrintNode(PrintContext ctx)
        {
            Doc result;

            PrintOwnComments = false;

            if (ShouldBreakOnLastArgument())
            {
                var printedArguments = PrintChildren(ctx);

                Doc optionA;

                if (Children.Count == 1)
                {
                    optionA = Doc.Concat("(", Doc.Concat(printedArguments), ")");
                }
                else
                {
                    var last = printedArguments.Last();
                    var allExceptLast = printedArguments.GetRange(0, printedArguments.Count - 1);

                    var separator = Doc.Concat(",", " ");

                    optionA = Doc.Concat(
                        "(",
                        Doc.Join(separator, allExceptLast),
                        separator,
                        last,
                        ")"
                    );
                }

                var optionB = DelimitedList.PrintInBrackets(ctx, "(", this, ")", ",");

                result = Doc.ConditionalGroup(optionA, optionB);
            }
            else
            {
                result = DelimitedList.PrintInBrackets(
                    ctx,
                    "(",
                    this,
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
                Children.Count == 0
                || LeadingComments.Count > 0 // Leading comments will end up inside the argument list
                || Children.Any(c => c.Comments.Any(g => g.Placement == CommentPlacement.OwnLine))
            )
            {
                return false;
            }

            if (Children.Count == 1)
            {
                return Children[0] is FunctionDeclaration or StructExpression;
            }

            return Children.Last() is FunctionDeclaration or StructExpression
                && Children.Take(Children.Count - 1).All(arg => arg is not FunctionDeclaration);
        }
    }
}
