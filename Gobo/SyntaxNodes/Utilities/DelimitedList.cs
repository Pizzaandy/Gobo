using Gobo.Printer.DocTypes;

namespace Gobo.SyntaxNodes.PrintHelpers;

internal class DelimitedList
{
    public static Doc PrintInBrackets(
        PrintContext ctx,
        string openToken,
        GmlSyntaxNode arguments,
        string closeToken,
        string separator,
        bool allowTrailingSeparator = false,
        bool forceBreak = false,
        Doc? leadingContents = null
    )
    {
        var parts = new List<Doc> { openToken };

        var groupId = Guid.NewGuid().ToString();

        if (
            arguments.Children.Count == 0
            && leadingContents is null
            && !arguments.DanglingComments.Any()
        )
        {
            return Doc.Concat(openToken, closeToken);
        }

        leadingContents ??= Doc.Null;

        if (arguments.Children.Count > 0)
        {
            Doc printedArguments = Print(
                ctx,
                arguments,
                separator,
                allowTrailingSeparator,
                forceBreak,
                leadingContents
            );

            var contents = Doc.Concat(forceBreak ? Doc.HardLine : Doc.SoftLine, printedArguments);
            parts.Add(Doc.IndentIfBreak(contents, groupId));
        }
        else
        {
            parts.Add(
                Doc.Indent(Doc.SoftLine, leadingContents, arguments.PrintDanglingComments(ctx))
            );
        }

        parts.Add(Doc.SoftLine);
        parts.Add(closeToken);

        return Doc.GroupWithId(groupId, parts);
    }

    public static Doc Print(
        PrintContext ctx,
        GmlSyntaxNode list,
        string separator,
        bool allowTrailingSeparator = false,
        bool forceBreak = false,
        Doc? leadingContents = null
    )
    {
        leadingContents ??= Doc.Null;

        if (list.Children.Count == 0 && leadingContents is NullDoc)
        {
            return Doc.Null;
        }

        var parts = new List<Doc> { leadingContents };

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
