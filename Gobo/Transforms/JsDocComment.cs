namespace Gobo.Transforms;

/// <summary>
/// One '///' block, parsed into its tags so they can be renamed, reordered and lined up.
/// </summary>
internal sealed class JsDocComment
{
    private sealed class Entry
    {
        public string Tag = "";
        public string Body = "";
        public List<string> Continuations = new();
    }

    /// <summary>
    /// The order tags are printed in. Anything unrecognised keeps its place at the end.
    /// </summary>
    private static readonly string[] TagOrder =
    {
        "@category",
        "@function",
        "@description",
        "@param",
        "@returns",
        "@self",
        "@ignore",
    };

    private static readonly Dictionary<string, string> TagNames =
        new()
        {
            ["@desc"] = "@description",
            ["@func"] = "@function",
            ["@arg"] = "@param",
            ["@argument"] = "@param",
            ["@parameter"] = "@param",
            ["@return"] = "@returns",
            ["@context"] = "@self",
        };

    /// <summary>
    /// Types Feather does not know, mapped to the ones it does. Anything already qualified
    /// with a dot is left as the author wrote it.
    /// </summary>
    private static readonly Dictionary<string, string> TypeNames =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["int"] = "Real",
            ["integer"] = "Real",
            ["real"] = "Real",
            ["number"] = "Real",
            ["float"] = "Real",
            ["string"] = "String",
            ["str"] = "String",
            ["bool"] = "Bool",
            ["boolean"] = "Bool",
            ["array"] = "Array",
            ["struct"] = "Struct",
            ["function"] = "Function",
            ["method"] = "Function",
            ["any"] = "Any",
            ["undefined"] = "Undefined",
            ["pointer"] = "Pointer",
            ["ptr"] = "Pointer",
            ["buffer"] = "Id.Buffer",
            ["instance"] = "Id.Instance",
            ["sprite"] = "Asset.GMSprite",
            ["object"] = "Asset.GMObject",
            ["sound"] = "Asset.GMSound",
            ["font"] = "Asset.GMFont",
            ["room"] = "Asset.GMRoom",
        };

    private readonly List<Entry> entries = new();

    public static JsDocComment Parse(IEnumerable<string> bodies)
    {
        var comment = new JsDocComment();

        foreach (var body in bodies)
        {
            if (SplitTag(body.TrimStart(), out var tag, out var rest))
            {
                comment.entries.Add(new Entry { Tag = tag, Body = rest });
            }
            else if (comment.entries.Count > 0)
            {
                comment.entries[^1].Continuations.Add(body.Trim());
            }
            else
            {
                comment.entries.Add(new Entry { Body = body.Trim() });
            }
        }

        return comment;
    }

    public void Normalize(FormatOptions options)
    {
        if (options.NormalizeJsDocTags)
        {
            foreach (var entry in entries.Where(entry => entry.Tag.Length > 0))
            {
                if (TagNames.TryGetValue(entry.Tag, out var canonical))
                {
                    entry.Tag = canonical;
                }

                entry.Body = NormalizeType(entry.Body);
            }
        }

        if (options.SortJsDocTags)
        {
            Sort();
        }
    }

    /// <summary>
    /// Renders the block back to '///' lines, lining parameter descriptions up one tab past
    /// the longest name and breaking anything that runs past the print width.
    /// </summary>
    public List<string> Render(FormatOptions options, int indentColumns)
    {
        var descriptionColumn = options.AlignJsDocDescriptions ? DescriptionColumn(options) : 0;
        var lines = new List<string>();

        foreach (var entry in entries)
        {
            var hanging = entry.Tag == "@param" && descriptionColumn > 0 ? descriptionColumn : 4;
            var prefix = ContinuationPrefix(hanging, options);

            var (head, body) = RenderEntry(entry, descriptionColumn, options);
            lines.AddRange(CommentText.Wrap(head, body, prefix, options));

            foreach (var continuation in entry.Continuations)
            {
                lines.AddRange(CommentText.Wrap(prefix, continuation, prefix, options));
            }
        }

        return lines;
    }

    /// <summary>
    /// The fixed start of the line and the text that may be broken away from it.
    /// </summary>
    private (string Head, string Body) RenderEntry(
        Entry entry,
        int descriptionColumn,
        FormatOptions options
    )
    {
        if (entry.Tag.Length == 0)
        {
            return ("/// ", entry.Body);
        }

        if (descriptionColumn == 0 || entry.Tag != "@param")
        {
            return ($"/// {entry.Tag} ", entry.Body);
        }

        var (prefix, description) = SplitParam(entry);
        var start = $"/// {entry.Tag} {prefix}";

        if (description.Length == 0)
        {
            return (start, "");
        }

        var padding = Padding(CommentText.VisualWidth(start, options), descriptionColumn, options);
        return (start + padding, description);
    }

    private int DescriptionColumn(FormatOptions options)
    {
        var widest = 0;

        foreach (var entry in entries.Where(entry => entry.Tag == "@param"))
        {
            var (prefix, description) = SplitParam(entry);

            if (description.Length > 0)
            {
                widest = Math.Max(
                    widest,
                    CommentText.VisualWidth($"/// {entry.Tag} {prefix}", options)
                );
            }
        }

        // One whole tab clear of the longest name.
        return widest == 0 ? 0 : ((widest / options.TabWidth) + 2) * options.TabWidth;
    }

    /// <summary>
    /// The start of a line carrying more of the tag above it. Wrapping and reprinting both
    /// use it, so a wrapped block reprints as itself.
    /// </summary>
    private static string ContinuationPrefix(int hangingColumn, FormatOptions options)
    {
        return options.UseTabs
            ? "///" + new string('\t', Math.Max(1, hangingColumn / options.TabWidth))
            : "///" + new string(' ', Math.Max(1, hangingColumn - 3));
    }

    private void Sort()
    {
        var sorted = entries
            .Select((entry, index) => (entry, index))
            .OrderBy(pair => Rank(pair.entry.Tag))
            .ThenBy(pair => pair.index)
            .Select(pair => pair.entry)
            .ToList();

        entries.Clear();
        entries.AddRange(sorted);
    }

    private static int Rank(string tag)
    {
        // Free text keeps its place above the tags. Below them it would read as a
        // continuation of whichever tag it landed under.
        if (tag.Length == 0)
        {
            return -1;
        }

        var index = Array.IndexOf(TagOrder, tag);
        return index < 0 ? TagOrder.Length : index;
    }

    private static string NormalizeType(string body)
    {
        if (!SplitType(body, out var declared, out var rest))
        {
            return body;
        }

        var types = declared.Split(
            ',',
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries
        );

        var mapped = types.Select(type =>
            !type.Contains('.') && TypeNames.TryGetValue(type, out var feather) ? feather : type
        );

        return $"{{{string.Join(",", mapped)}}} {rest}".TrimEnd();
    }

    private static (string Prefix, string Description) SplitParam(Entry entry)
    {
        var body = entry.Body;
        var prefix = "";

        if (SplitType(body, out var type, out var afterType))
        {
            prefix = "{" + type + "} ";
            body = afterType;
        }

        var (name, description) = SplitName(body);
        return (prefix + name, description);
    }

    /// <summary>
    /// Splits '@tag rest'. The tag runs to the first character that cannot be part of it.
    /// </summary>
    private static bool SplitTag(string text, out string tag, out string rest)
    {
        tag = "";
        rest = "";

        if (text.Length < 2 || text[0] != '@')
        {
            return false;
        }

        var end = 1;

        while (end < text.Length && (char.IsLetterOrDigit(text[end]) || text[end] == '_'))
        {
            end++;
        }

        if (end == 1)
        {
            return false;
        }

        tag = text[..end];
        rest = text[end..].TrimStart();
        return true;
    }

    /// <summary>
    /// Splits a leading '{type}' from the rest of a tag body.
    /// </summary>
    private static bool SplitType(string text, out string type, out string rest)
    {
        type = "";
        rest = "";

        if (text.Length == 0 || text[0] != '{')
        {
            return false;
        }

        var close = text.IndexOf('}');

        if (close < 0)
        {
            return false;
        }

        type = text[1..close];
        rest = text[(close + 1)..].TrimStart();
        return true;
    }

    private static (string Name, string Description) SplitName(string text)
    {
        var end = 0;

        while (end < text.Length && !char.IsWhiteSpace(text[end]))
        {
            end++;
        }

        return (text[..end], text[end..].Trim());
    }

    private static string Padding(int from, int to, FormatOptions options)
    {
        if (!options.UseTabs)
        {
            return new string(' ', Math.Max(1, to - from));
        }

        var tabs = 0;
        var column = from;

        while (column < to)
        {
            column = ((column / options.TabWidth) + 1) * options.TabWidth;
            tabs++;
        }

        return new string('\t', Math.Max(1, tabs));
    }
}
