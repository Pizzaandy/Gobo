using PrettierGML.Printer.DocTypes;

namespace PrettierGML.SyntaxNodes.PrintHelpers
{
    internal class DelimitedList
    {
        public static Doc PrintInBrackets(
            PrintContext ctx,
            string openToken,
            GmlSyntaxNode arguments,
            string closeToken,
            string separator,
            bool allowTrailingSeparator = false,
            bool forceBreak = false
        )
        {
            var parts = new List<Doc> { openToken };

            if (arguments.Children.Count > 0)
            {
                Doc printedArguments = Print(
                    ctx,
                    arguments,
                    separator,
                    allowTrailingSeparator,
                    forceBreak
                );

                parts.Add(Doc.Indent(Doc.SoftLine, printedArguments));
            }
            else if (arguments.DanglingComments.Count > 0)
            {
                parts.Add(Doc.Indent(Doc.SoftLine, arguments.PrintDanglingComments(ctx)));
            }

            parts.Add(Doc.SoftLine);

            parts.Add(closeToken);

            return Doc.Group(parts);
        }

        public static Doc Print(
            PrintContext ctx,
            GmlSyntaxNode list,
            string separator,
            bool allowTrailingSeparator = false,
            bool forceBreak = false
        )
        {
            if (list.Children.Count == 0)
            {
                return Doc.Null;
            }

            var parts = new List<Doc>();

            for (var i = 0; i < list.Children.Count; i++)
            {
                var child = list.Children[i];

                parts.Add(child.Print(ctx));

                if (i != list.Children.Count - 1)
                {
                    parts.Add(separator);
                    parts.Add(forceBreak ? Doc.HardLine : Doc.Line);
                }
                else if (allowTrailingSeparator)
                {
                    parts.Add(Doc.IfBreak(separator, Doc.Null));
                }
            }

            return Doc.Concat(parts);
        }
    }
}
