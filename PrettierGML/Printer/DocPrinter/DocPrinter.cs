using PrettierGML.Printer.DocTypes;
using PrettierGML.Printer.Utilities;
using System.Text;

namespace PrettierGML.Printer.DocPrinter;

internal class DocPrinter
{
    protected readonly Stack<PrintCommand> RemainingCommands = new();
    protected readonly Dictionary<string, PrintMode> GroupModeMap = new();
    protected int CurrentWidth;
    protected readonly StringBuilder Output = new();
    protected bool ShouldRemeasure;
    protected bool NewLineNextStringValue;
    protected bool SkipNextNewLine;
    protected readonly string EndOfLine;
    protected readonly DocPrinterOptions PrinterOptions;
    protected readonly Indenter Indenter;
    protected Stack<Indent> RegionIndents = new();
    protected int ConsecutiveIndents = 0;

    protected DocPrinter(Doc doc, DocPrinterOptions printerOptions, string endOfLine)
    {
        EndOfLine = endOfLine;
        PrinterOptions = printerOptions;
        Indenter = new Indenter(printerOptions);
        RemainingCommands.Push(new PrintCommand(Indenter.GenerateRoot(), PrintMode.Break, doc));
    }

    public static string Print(Doc document, DocPrinterOptions printerOptions, string endOfLine)
    {
        PropagateBreaks.RunOn(document);

        return new DocPrinter(document, printerOptions, endOfLine).Print();
    }

    public string Print()
    {
        while (RemainingCommands.Count > 0)
        {
            ProcessNextCommand();
        }

        EnsureOutputEndsWithSingleNewLine();

        var result = Output.ToString();
        if (PrinterOptions.TrimInitialLines)
        {
            result = result.TrimStart('\n', '\r');
        }

        return result;
    }

    private void EnsureOutputEndsWithSingleNewLine()
    {
        var trimmed = 0;
        for (; trimmed < Output.Length; trimmed++)
        {
            if (Output[^(trimmed + 1)] != '\r' && Output[^(trimmed + 1)] != '\n')
            {
                break;
            }
        }

        Output.Length -= trimmed;

        Output.Append(EndOfLine);
    }

    private void ProcessNextCommand()
    {
        var (indent, mode, doc) = RemainingCommands.Pop();
        if (doc == Doc.Null)
        {
            return;
        }

        if (doc is not IndentDoc)
        {
            ConsecutiveIndents = 0;
        }

        if (doc is StringDoc stringDoc)
        {
            ProcessString(stringDoc, indent);
        }
        else if (doc is Concat concat)
        {
            for (var x = concat.Contents.Count - 1; x >= 0; x--)
            {
                Push(concat.Contents[x], mode, indent);
            }
        }
        else if (doc is Fill fill)
        {
            ProcessFill(fill, mode, indent);
        }
        else if (doc is IndentDoc indentDoc)
        {
            Push(indentDoc.Contents, mode, Indenter.IncreaseIndent(indent));
        }
        else if (doc is Trim)
        {
            CurrentWidth -= Output.TrimTrailingWhitespace();
            NewLineNextStringValue = false;
        }
        else if (doc is Group group)
        {
            ProcessGroup(group, mode, indent);
        }
        else if (doc is IfBreak ifBreak)
        {
            var groupMode = mode;
            if (ifBreak.GroupId != null)
            {
                if (!GroupModeMap.TryGetValue(ifBreak.GroupId, out groupMode))
                {
                    throw new Exception("You cannot use an ifBreak before the group it targets.");
                }
            }

            var contents =
                groupMode == PrintMode.Break ? ifBreak.BreakContents : ifBreak.FlatContents;
            Push(contents, mode, indent);
        }
        else if (doc is LineDoc line)
        {
            ProcessLine(line, mode, indent);
        }
        else if (doc is BreakParent) { }
        else if (doc is LeadingComment leadingComment)
        {
            Output.TrimTrailingWhitespace();
            if (Output.Length != 0 && Output[^1] != '\n' || NewLineNextStringValue)
            {
                Output.Append(EndOfLine);
            }

            AppendComment(leadingComment, indent);

            CurrentWidth = indent.Length;
            NewLineNextStringValue = false;
            SkipNextNewLine = false;
        }
        else if (doc is TrailingComment trailingComment)
        {
            Output.TrimTrailingWhitespace();
            Output.Append(' ').Append(trailingComment.Comment);
            CurrentWidth = indent.Length;
            if (mode != PrintMode.ForceFlat)
            {
                NewLineNextStringValue = true;
                SkipNextNewLine = true;
            }
        }
        else if (doc is ForceFlat forceFlat)
        {
            Push(forceFlat.Contents, PrintMode.ForceFlat, indent);
        }
        else if (doc is Align align)
        {
            Push(align.Contents, mode, Indenter.AddAlign(indent, align.Width));
        }
        else if (doc is Region region)
        {
            if (region.IsEnd)
            {
                var regionIndent = RegionIndents.Pop();
                Output.Append(regionIndent.Value);
            }
            else
            {
                Output.Append(indent.Value);
                RegionIndents.Push(indent);
            }

            Output.Append(region.Text);
        }
        else if (doc is AlwaysFits temp)
        {
            Push(temp.Contents, mode, indent);
        }
        else
        {
            throw new Exception("didn't handle " + doc);
        }
    }

    private void AppendComment(LeadingComment leadingComment, Indent indent)
    {
        int CalculateIndentLength(string line)
        {
            var result = 0;
            foreach (var character in line)
            {
                if (character == ' ')
                {
                    result += 1;
                }
                else if (character == '\t')
                {
                    result += PrinterOptions.TabWidth;
                }
                else
                {
                    break;
                }
            }

            return result;
        }

        var stringReader = new StringReader(leadingComment.Comment);
        var line = stringReader.ReadLine();
        var numberOfSpacesToAddOrRemove = 0;
        if (leadingComment.Type == CommentFormat.MultiLine && line != null)
        {
            // in order to maintain the formatting inside of a multiline comment
            // we calculate how much the indentation of the first line is changing
            // and then change the indentation of all other lines the same amount
            var firstLineIndentLength = CalculateIndentLength(line);
            var currentIndent = CalculateIndentLength(indent.Value);
            numberOfSpacesToAddOrRemove = currentIndent - firstLineIndentLength;
        }

        while (line != null)
        {
            if (leadingComment.Type == CommentFormat.SingleLine)
            {
                Output.Append(indent.Value);
            }
            else
            {
                var spacesToAppend = CalculateIndentLength(line) + numberOfSpacesToAddOrRemove;
                if (PrinterOptions.UseTabs)
                {
                    var indentLength = CalculateIndentLength(indent.Value);
                    if (spacesToAppend >= indentLength)
                    {
                        Output.Append(indent.Value);
                        spacesToAppend -= indentLength;
                    }
                }
                if (spacesToAppend > 0)
                {
                    Output.Append(' ', spacesToAppend);
                }
            }

            Output.Append(line.Trim());
            line = stringReader.ReadLine();
            if (line == null)
            {
                return;
            }

            Output.Append(EndOfLine);
        }
    }

    private void ProcessString(StringDoc stringDoc, Indent indent)
    {
        if (string.IsNullOrEmpty(stringDoc.Value))
        {
            return;
        }

        // this ensures we don't print extra spaces after a trailing comment
        // newLineNextStringValue & skipNextNewLine are set to true when we print a trailing comment
        // when they are set we new line the next string we find. If we new line and then print a " " we end up with an extra space
        if (NewLineNextStringValue && SkipNextNewLine && stringDoc.Value == " ")
        {
            return;
        }

        if (NewLineNextStringValue)
        {
            Output.TrimTrailingWhitespace();
            Output.Append(EndOfLine).Append(indent.Value);
            CurrentWidth = indent.Length;
            NewLineNextStringValue = false;
        }

        Output.Append(stringDoc.Value);
        CurrentWidth += stringDoc.Value.GetPrintedWidth();
    }

    private void ProcessLine(LineDoc line, PrintMode mode, Indent indent)
    {
        if (mode is PrintMode.Flat or PrintMode.ForceFlat)
        {
            if (line.Type == LineDoc.LineType.Soft)
            {
                return;
            }

            if (line.Type == LineDoc.LineType.Normal)
            {
                Output.Append(' ');
                CurrentWidth += 1;
                return;
            }

            // This line was forced into the output even if we were in flattened mode, so we need to tell the next
            // group that no matter what, it needs to remeasure because the previous measurement didn't accurately
            // capture the entire expression (this is necessary for nested groups)
            ShouldRemeasure = true;
        }

        if (line.Squash && Output.Length > 0 && Output.EndsWithNewLineAndWhitespace())
        {
            return;
        }

        if (line.IsLiteral)
        {
            if (Output.Length > 0)
            {
                Output.Append(EndOfLine);
                CurrentWidth = 0;
            }
        }
        else
        {
            if (!SkipNextNewLine || !NewLineNextStringValue)
            {
                Output.TrimTrailingWhitespace();
                Output.Append(EndOfLine).Append(indent.Value);
                CurrentWidth = indent.Length;
            }

            if (SkipNextNewLine)
            {
                SkipNextNewLine = false;
            }
        }
    }

    private void ProcessGroup(Group group, PrintMode mode, Indent indent)
    {
        if (mode is PrintMode.Flat or PrintMode.ForceFlat && !ShouldRemeasure)
        {
            Push(group.Contents, group.Break ? PrintMode.Break : mode, indent);
        }
        else
        {
            ShouldRemeasure = false;
            var possibleCommand = new PrintCommand(indent, PrintMode.Flat, group.Contents);

            if (!group.Break && Fits(possibleCommand))
            {
                RemainingCommands.Push(possibleCommand);
            }
            else if (group is ConditionalGroup conditionalGroup)
            {
                if (group.Break)
                {
                    Push(conditionalGroup.Options.Last(), PrintMode.Break, indent);
                }
                else
                {
                    var foundSomethingThatFits = false;
                    foreach (var option in conditionalGroup.Options.Skip(1))
                    {
                        possibleCommand = new PrintCommand(indent, mode, option);
                        if (!Fits(possibleCommand))
                        {
                            continue;
                        }

                        RemainingCommands.Push(possibleCommand);
                        foundSomethingThatFits = true;
                        break;
                    }

                    if (!foundSomethingThatFits)
                    {
                        RemainingCommands.Push(possibleCommand);
                    }
                }
            }
            else
            {
                Push(group.Contents, PrintMode.Break, indent);
            }
        }

        if (group.GroupId != null)
        {
            GroupModeMap[group.GroupId] = RemainingCommands.Peek().Mode;
        }
    }

    private void ProcessFill(Fill fill, PrintMode mode, Indent indent)
    {
        if (fill.Contents.Count == 0)
        {
            return;
        }

        var content = fill.Contents[0];
        var contentFlatCmd = new PrintCommand(indent, PrintMode.Flat, content);
        var contentBreakCmd = new PrintCommand(indent, PrintMode.Break, content);
        bool contentFits = Fits(contentFlatCmd);

        if (fill.Contents.Count == 1)
        {
            if (contentFits)
            {
                RemainingCommands.Push(contentFlatCmd);
            }
            else
            {
                RemainingCommands.Push(contentBreakCmd);
            }
            return;
        }

        var whitespace = fill.Contents[1];
        var whitespaceFlatCmd = new PrintCommand(indent, PrintMode.Flat, whitespace);
        var whitespaceBreakCmd = new PrintCommand(indent, PrintMode.Break, whitespace);

        if (fill.Contents.Count == 2)
        {
            if (contentFits)
            {
                RemainingCommands.Push(whitespaceFlatCmd);
            }
            else
            {
                RemainingCommands.Push(whitespaceBreakCmd);
            }
            RemainingCommands.Push(contentFlatCmd);
            return;
        }

        fill.Contents.RemoveAt(0);
        fill.Contents.RemoveAt(0);

        var remainingCmd = new PrintCommand(indent, mode, Doc.Fill(fill.Contents));
        var secondContent = fill.Contents[0];

        var firstAndSecondContentFlatCmd = new PrintCommand(
            indent,
            PrintMode.Flat,
            Doc.Concat(new[] { content, whitespace, secondContent })
        );
        var firstAndSecondContentFits = Fits(firstAndSecondContentFlatCmd);

        if (firstAndSecondContentFits)
        {
            RemainingCommands.Push(remainingCmd);
            RemainingCommands.Push(whitespaceFlatCmd);
            RemainingCommands.Push(contentFlatCmd);
        }
        else if (contentFits)
        {
            RemainingCommands.Push(remainingCmd);
            RemainingCommands.Push(whitespaceBreakCmd);
            RemainingCommands.Push(contentFlatCmd);
        }
        else
        {
            RemainingCommands.Push(remainingCmd);
            RemainingCommands.Push(whitespaceBreakCmd);
            RemainingCommands.Push(contentBreakCmd);
        }
    }

    private bool Fits(PrintCommand possibleCommand)
    {
        return DocFitter.Fits(
            possibleCommand,
            RemainingCommands,
            PrinterOptions.Width - CurrentWidth,
            GroupModeMap,
            Indenter
        );
    }

    private void Push(Doc doc, PrintMode printMode, Indent indent)
    {
        RemainingCommands.Push(new PrintCommand(indent, printMode, doc));
    }
}

internal record PrintCommand(Indent Indent, PrintMode Mode, Doc Doc);

internal enum PrintMode
{
    Flat,
    Break,
    ForceFlat
}
