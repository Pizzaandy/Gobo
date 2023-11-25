using System.Diagnostics;
using Gobo.Parser;
using Gobo.Printer.DocPrinter;
using Gobo.SyntaxNodes;

namespace Gobo;

public struct FormatResult
{
    public static readonly FormatResult Empty = new();

    public string Output;

    public string Ast = string.Empty;
    public string DocTree = string.Empty;

    public double? ParseTimeMs = null;
    public double? FormatTimeMs = null;
    public double? TotalTimeMs = null;

    public FormatResult(string output)
    {
        Output = output;
    }

    public override readonly string ToString()
    {
        var result = "";

        if (Ast != string.Empty)
            result += $"--- ABSTRACT SYNTAX TREE ---\n\n{Ast}\n\n";

        if (DocTree != string.Empty)
            result += $"--- DOC TREE ---\n\n{DocTree}\n\n";

        result += $"--- PRINTED OUTPUT ---\n{Output}\n\n";

        if (ParseTimeMs != null)
            result += $"Parse: {ParseTimeMs} ms\n";

        if (FormatTimeMs != null)
            result += $"Format: {FormatTimeMs} ms\n";

        if (TotalTimeMs != null)
            result += $"Total: {TotalTimeMs} ms\n";

        return result;
    }
}

public static partial class GmlFormatter
{
    public static FormatResult Format(string code, FormatOptions options)
    {
        long parseStart = 0;
        long parseStop = 0;
        long formatStart = 0;
        long formatStop = 0;

        var getDebugInfo = options.GetDebugInfo;

        if (getDebugInfo)
        {
            parseStart = Stopwatch.GetTimestamp();
        }

        var parseResult = GmlParser.Parse(code);

        if (getDebugInfo)
        {
            parseStop = Stopwatch.GetTimestamp();
            formatStart = Stopwatch.GetTimestamp();
        }

        GmlSyntaxNode ast = parseResult.Ast;

        var initialHash = options.ValidateOutput ? ast.GetHashCode() : -1;
        var docs = ast.Print(new PrintContext(options, new SourceText(code)));

        var printOptions = new Printer.DocPrinterOptions()
        {
            Width = options.Width,
            TabWidth = options.TabWidth,
            UseTabs = options.UseTabs,
        };

        var result = DocPrinter.Print(docs, printOptions, "\n");
        var output = result.Output;

        if (getDebugInfo)
        {
            formatStop = Stopwatch.GetTimestamp();
        }

        if (options.ValidateOutput)
        {
            // Check that all comments are printed once
            var comments = ast.GetFormattedCommentGroups();

            var unprinted = new List<CommentGroup>();
            var doublePrinted = new List<CommentGroup>();

            foreach (var comment in comments)
            {
                if (!result.CommentsPrinted.Contains(comment.Id))
                {
                    unprinted.Add(comment);
                }
                else if (result.CommentsPrintedTwice.Contains(comment.Id))
                {
                    doublePrinted.Add(comment);
                }
            }

            if (unprinted.Count > 0 || doublePrinted.Count > 0)
            {
                string message = "";

                if (unprinted.Count > 0)
                {
                    message +=
                        $"{unprinted.Count} comment group(s) were not printed:\n{string.Join('\n', unprinted)}\n\n{output}";
                }
                if (doublePrinted.Count > 0)
                {
                    message +=
                        $"{doublePrinted.Count} comment group(s) were printed multiple times:\n{string.Join('\n', doublePrinted)}\n\n{output}";
                }

                throw new Exception(message);
            }

            GmlParseResult updatedParseResult;

            try
            {
                updatedParseResult = GmlParser.Parse(output);
            }
            catch (GmlSyntaxErrorException ex)
            {
                throw new Exception(
                    "Formatting made the code invalid!\n" + ex.Message + "\n\n" + output
                );
            }

            var resultHash = updatedParseResult.Ast.GetHashCode();

            if (initialHash != resultHash)
            {
                ISyntaxNode<GmlSyntaxNode>.TryFindDifference(
                    parseResult.Ast,
                    updatedParseResult.Ast,
                    out var difference
                );
                throw new Exception(
                    $"Formatting transformed the AST:\nExpected:\n{difference.Item1}\n\nActual:\n{difference.Item2}"
                );
            }
        }

        if (getDebugInfo)
        {
            return new FormatResult(output)
            {
                Ast = ast.ToString(),
                DocTree = docs.ToString(),
                ParseTimeMs = Stopwatch.GetElapsedTime(parseStart, parseStop).TotalMilliseconds,
                FormatTimeMs = Stopwatch.GetElapsedTime(formatStart, formatStop).TotalMilliseconds,
                TotalTimeMs = Stopwatch.GetElapsedTime(parseStart, formatStop).TotalMilliseconds
            };
        }
        else
        {
            return new FormatResult(output);
        }
    }

    public static bool Check(string code, FormatOptions options)
    {
        var result = Format(code, options);
        return result.Output == code;
    }

    public static async Task FormatFileAsync(string filePath, FormatOptions options)
    {
        string input = await File.ReadAllTextAsync(filePath);
        string formatted;

        try
        {
            var result = Format(input, options);
            formatted = result.Output;
        }
        catch (Exception)
        {
            return;
        }

        await File.WriteAllTextAsync(filePath, formatted);
    }
}
