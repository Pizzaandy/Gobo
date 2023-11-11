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
    protected readonly string EndOfLine;
    protected readonly DocPrinterOptions PrinterOptions;
    protected readonly Indenter Indenter;
    protected Stack<Indent> RegionIndents = new();
    protected Stack<PrintCommand> EndOfLineComments = new();
    protected int ConsecutiveIndents = 0;

    private static readonly char[] openingDelimiters = { '{', '(', '[' };

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

        if (EndOfLineComments.Count > 0)
        {
            foreach (var cmd in EndOfLineComments)
            {
                RemainingCommands.Push(cmd);
            }
        }

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
        else if (doc is CollapsedSpace)
        {
            CurrentWidth -= Output.TrimTrailingWhitespacePreserveIndent(indent);
            if (CurrentWidth != indent.Length)
            {
                Output.Append(' ');
            }
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
        else if (doc is EndOfLineComment endOfLineComment)
        {
            EndOfLineComments.Push(new PrintCommand(indent, mode, endOfLineComment.Contents));
        }
        else if (doc is InlineComment inlineComment)
        {
            Push(
                Doc.Concat(Doc.CollapsedSpace, inlineComment.Contents, Doc.CollapsedSpace),
                mode,
                indent
            );
        }
        else
        {
            throw new Exception("didn't handle " + doc);
        }
    }

    private void ProcessString(StringDoc stringDoc, Indent indent)
    {
        if (string.IsNullOrEmpty(stringDoc.Value))
        {
            return;
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

        // Append end-of-line comments
        if (EndOfLineComments.Count > 0)
        {
            Push(line, mode, indent);

            if (!Output.EndsWithNewLineAndWhitespace() && Output.Length > 0)
            {
                Output.TrimTrailingWhitespace();

                if (openingDelimiters.Contains(Output[^1]))
                {
                    // Move end-of-line comments to a new line if next to an opening bracket delimiter
                    // example:
                    /*
                     * before:
                     * if condition { // comment
                     * }
                     *
                     * after:
                     * if condition {
                     *     // comment
                     * }
                    */
                    Output.Append(EndOfLine).Append(indent.Value);
                }
                else
                {
                    Output.Append(' ');
                }
            }

            foreach (var comment in EndOfLineComments)
            {
                RemainingCommands.Push(comment);
            }

            EndOfLineComments.Clear();
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
            Output.TrimTrailingWhitespace();
            Output.Append(EndOfLine).Append(indent.Value);
            CurrentWidth = indent.Length;
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
