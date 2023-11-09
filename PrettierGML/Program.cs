using PrettierGML;
using PrettierGML.Printer.Utilities;

static string TestFormat(string input)
{
    var formatOptions = FormatOptions.DefaultTestOptions;

    formatOptions.ValidateOutput = true;

    FormatResult result = GmlFormatter.Format(input, formatOptions);

    FormatResult secondResult = GmlFormatter.Format(result.Output, formatOptions);

    Console.WriteLine(secondResult);

    Console.WriteLine(StringDiffer.PrintFirstDifference(result.Output, secondResult.Output));

    return result.Output;
}

var input = $$"""
    /* comment */ x = greeting // shid
    .slice // fuck
    ( 0, 1 ).toUpperCase() + greeting.slice(1).toLowerCase()
    """;

TestFormat(input);
