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
            bool allowTrailingSeparator = false
        )
        {
            var parts = new List<Doc> { openToken };

            if (arguments.Children.Any())
            {
                Doc printedArguments = Print(ctx, arguments, separator, allowTrailingSeparator);

                parts.Add(Doc.Indent(Doc.SoftLine, printedArguments));
                parts.Add(Doc.SoftLine);
            }

            parts.Add(closeToken);

            return Doc.Group(parts);
        }

        public static Doc Print(
            PrintContext ctx,
            GmlSyntaxNode list,
            string separator,
            bool allowTrailingSeparator = false
        )
        {
            var parts = new List<Doc>();

            for (var i = 0; i < list.Children.Count; i++)
            {
                var child = list.Children[i];

                parts.Add(child.PrintLeadingComments(ctx));
                parts.Add(child.Print(ctx));

                var trailingComments = child.PrintTrailingComments(ctx);
                var hasTrailingComments = child.TrailingComments.Any();

                // Ensure end-of-line comments are printed after the separator
                // if the list breaks.
                var addCommentAfterSeparatorIfBreak =
                    hasTrailingComments
                    && child.TrailingComments.First().Placement == CommentPlacement.EndOfLine;

                if (hasTrailingComments)
                {
                    parts.Add(
                        addCommentAfterSeparatorIfBreak
                            ? Doc.IfBreak(Doc.Null, trailingComments)
                            : Doc.IfBreak(trailingComments, Doc.Null)
                    );
                }

                var isLastChild = i == list.Children.Count - 1;

                if (isLastChild)
                {
                    if (allowTrailingSeparator)
                    {
                        parts.Add(Doc.IfBreak(separator, Doc.Null));
                    }
                }
                else
                {
                    parts.Add(separator);
                }

                if (hasTrailingComments)
                {
                    parts.Add(
                        addCommentAfterSeparatorIfBreak
                            ? Doc.IfBreak(trailingComments, Doc.Null)
                            : Doc.IfBreak(Doc.Null, trailingComments)
                    );
                }

                if (!isLastChild)
                {
                    parts.Add(Doc.Line);
                }
            }

            return parts.Count == 0 ? Doc.Null : Doc.Concat(parts);
        }
    }
}
