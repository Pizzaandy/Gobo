using PrettierGML;
using PrettierGML.Printer.Utilities;

static string TestFormat(string input)
{
    var formatOptions = FormatOptions.DefaultTestOptions;

    formatOptions.ValidateOutput = true;

    FormatResult result = GmlFormatter.Format(input, formatOptions);

    Console.WriteLine(result);

    FormatResult secondResult = GmlFormatter.Format(result.Output, formatOptions);

    Console.WriteLine(StringDiffer.PrintFirstDifference(result.Output, secondResult.Output));

    return result.Output;
}

var input = $$$"""
if (/*foo*/(((cond))) // comment
) // paisan
{// bar
return
}
""";

TestFormat(input);
