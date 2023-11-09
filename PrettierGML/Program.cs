using PrettierGML;
using PrettierGML.Printer.Utilities;

static string TestFormat(string input)
{
    var formatOptions = FormatOptions.DefaultTestOptions;
    formatOptions.ValidateOutput = false;

    FormatResult result = GmlFormatter.Format(input, formatOptions);

    Console.WriteLine(result);

    //FormatResult secondResult = GmlFormatter.Format(result.Output, formatOptions);

    //Console.WriteLine(StringDiffer.PrintFirstDifference(result.Output, secondResult.Output));

    return result.Output;
}

var input = $$"""
    y = a // comment
    .b().c()
    """;

TestFormat(input);
