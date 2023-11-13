using PrettierGML;

static string TestFormat(string input)
{
    var formatOptions = FormatOptions.DefaultTestOptions;

    formatOptions.ValidateOutput = true;
    formatOptions.BraceStyle = BraceStyle.SameLine;

    FormatResult result = GmlFormatter.Format(input, formatOptions);

    Console.WriteLine(result);

    FormatResult secondResult = GmlFormatter.Format(result.Output, formatOptions);

    Console.WriteLine(StringDiffer.PrintFirstDifference(result.Output, secondResult.Output));

    return result.Output;
}

var input = $$$"""
x = 000.01
""";

TestFormat(input);
