using Gobo.Printer.DocTypes;

namespace Gobo.SyntaxNodes.PrintHelpers;

internal static class DelimitedList
{
    public static Doc PrintInBrackets(
        PrintContext ctx,
        GmlSyntaxNode parent,
        string openToken,
        List<GmlSyntaxNode> arguments,
        string closeToken,
        string separator,
        bool allowTrailingSeparator = false,
        bool forceBreak = false,
        Doc? leadingContents = null
    )
    {
        var parts = new List<Doc> { openToken };

        var groupId = Guid.NewGuid().ToString();

        if (arguments.Count == 0 && leadingContents is null && !parent.DanglingComments.Any())
        {
            return Doc.Concat(openToken, closeToken);
        }

        leadingContents ??= Doc.Null;

        if (arguments.Count > 0)
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
            parts.Add(Doc.Indent(Doc.SoftLine, leadingContents, parent.PrintDanglingComments(ctx)));
        }

        parts.Add(Doc.SoftLine);
        parts.Add(closeToken);

        return Doc.GroupWithId(groupId, parts);
    }

    public static Doc Print(
        PrintContext ctx,
        List<GmlSyntaxNode> nodes,
        string separator,
        bool allowTrailingSeparator = false,
        bool forceBreak = false,
        Doc? leadingContents = null
    )
    {
        leadingContents ??= Doc.Null;

        if (nodes.Count == 0 && leadingContents is NullDoc)
        {
            return Doc.Null;
        }

        var parts = new List<Doc> { leadingContents };

        for (var i = 0; i < nodes.Count; i++)
        {
            var child = nodes[i];

            parts.Add(child.Print(ctx));

            if (i != nodes.Count - 1)
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
