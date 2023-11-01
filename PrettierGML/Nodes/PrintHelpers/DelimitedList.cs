namespace PrettierGML.Nodes.PrintHelpers
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
                parts.Add(list.Children[i].Print(ctx));

                if (i != list.Children.Count - 1)
                {
                    parts.Add(Doc.Concat(separator, Doc.Line));
                }
                else if (allowTrailingSeparator)
                {
                    parts.Add(Doc.IfBreak(separator, Doc.Null));
                }
            }

            return parts.Count == 0 ? Doc.Null : Doc.Concat(parts);
        }
    }
}
