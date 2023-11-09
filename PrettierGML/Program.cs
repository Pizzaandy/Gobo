using PrettierGML;
using PrettierGML.Printer.Utilities;

static string TestFormat(string input)
{
    var formatOptions = FormatOptions.DefaultTestOptions;
    formatOptions.ValidateOutput = false;

    FormatResult result = GmlFormatter.Format(input, formatOptions);

    Console.WriteLine(result);

    FormatResult secondResult = GmlFormatter.Format(result.Output, formatOptions);

    Console.WriteLine(StringDiffer.PrintFirstDifference(result.Output, secondResult.Output));

    return result.Output;
}

var input = $$"""
    // c1
    x = CallMethod(
    // c2
            firstParameter____________________________,  // c3
            secondParameter___________________________
        ).b;

    """;

TestFormat(input);
