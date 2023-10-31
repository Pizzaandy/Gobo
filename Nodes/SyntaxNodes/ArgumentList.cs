using Antlr4.Runtime;
using PrettierGML.Nodes.PrintHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class ArgumentList : GmlSyntaxNode
    {
        public List<GmlSyntaxNode> Arguments => Children;
        public ArgumentList(ParserRuleContext context, List<GmlSyntaxNode> arguments)
            : base(context)
        {
            AsChildren(arguments);
        }

        public override Doc Print(PrintContext ctx)
        {
            Doc printedArguments;

            if (ShouldBreakOnLastArgument())
            {
                var printedChildren = PrintChildren(ctx);

                var last = printedChildren.Last();
                var allExceptLast = printedChildren.GetRange(0, printedChildren.Count - 1);

                var separator = Doc.Concat(",", " ");
                var optionA = Doc.Concat(
                    "(",
                    Doc.Join(separator, allExceptLast),
                    separator,
                    last,
                    ")"
                );
                var optionB = Doc.Concat(
                    DelimitedList.PrintInBrackets(ctx, "(", this, ")", ",")
                );

                printedArguments = Doc.ConditionalGroup(optionA, optionB);
            }
            else
            {
                printedArguments = Doc.Concat(
                    DelimitedList.PrintInBrackets(ctx, "(", this, ")", ",")
                );
            }

            return printedArguments;
        }

        private bool ShouldBreakOnLastArgument()
        {
            if (Children.Count == 0)
            {
                return false;
            }

            return Children.Last() is FunctionDeclaration or StructExpression
                && Children
                    .Take(Children.Count - 1)
                    .All(arg => arg is not FunctionDeclaration);
        }
    }
}
