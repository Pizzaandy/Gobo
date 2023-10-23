using Antlr4.Runtime;
using PrettierGML.Nodes.PrintHelpers;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class CallExpression : GmlSyntaxNode, IHasObject
    {
        public GmlSyntaxNode Object { get; set; }
        public GmlSyntaxNode Arguments { get; set; }

        public CallExpression(
            ParserRuleContext context,
            GmlSyntaxNode @object,
            GmlSyntaxNode arguments
        )
            : base(context)
        {
            Object = AsChild(@object);
            Arguments = AsChild(arguments);
        }

        public override Doc Print(PrintContext ctx)
        {
            Doc printedArguments;

            if (IsFunctionOrStructWrapper(Arguments))
            {
                var printedChildren = Arguments.PrintChildren(ctx);

                var last = printedChildren.Last();
                var allExceptLast = printedChildren.GetRange(0, printedChildren.Count - 1);

                var optionA = Doc.Concat("(", Doc.Join(", ", allExceptLast), ", ", last, ")");
                var optionB = Doc.Concat(
                    DelimitedList.PrintInBrackets(ctx, "(", Arguments, ")", ",")
                );

                printedArguments = Doc.ConditionalGroup(optionA, optionB);
            }
            else
            {
                printedArguments = Doc.Concat(
                    DelimitedList.PrintInBrackets(ctx, "(", Arguments, ")", ",")
                );
            }

            return Doc.Concat(Object.Print(ctx), printedArguments);
        }

        private static bool IsFunctionOrStructWrapper(GmlSyntaxNode arguments)
        {
            if (arguments is not NodeList || arguments.Children.Count == 0)
            {
                return false;
            }

            return arguments.Children.Last() is FunctionDeclaration or StructExpression
                && arguments.Children
                    .GetRange(0, arguments.Children.Count - 1)
                    .All(arg => arg is not FunctionDeclaration and not StructExpression);
        }
    }
}
