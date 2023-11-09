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

            if (ShouldBreakOnLastArgument())
            {
                var printedArguments = PrintChildren(ctx);

                var last = printedArguments.Last();
                var allExceptLast = printedArguments.GetRange(0, printedArguments.Count - 1);

                var separator = Doc.Concat(",", " ");

                var optionA = Doc.Concat(
                    "(",
                    Doc.Join(separator, allExceptLast),
                    separator,
                    last,
                    ")"
                );

                var optionB = Doc.Concat(DelimitedList.PrintInBrackets(ctx, "(", this, ")", ","));

                result = Doc.ConditionalGroup(optionA, optionB);
            }
            else
            {
                result = DelimitedList.PrintInBrackets(ctx, "(", this, ")", ",");
            }

            return result;
        }

        private bool ShouldBreakOnLastArgument()
        {
            if (Children.Count == 0)
            {
                return false;
            }

            return Children.Last() is FunctionDeclaration or StructExpression
                && Children.Take(Children.Count - 1).All(arg => arg is not FunctionDeclaration);
        }
    }
}
