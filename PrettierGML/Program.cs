using PrettierGML;

static string TestFormat(string input)
{
    var formatOptions = FormatOptions.DefaultTestOptions;

    formatOptions.ValidateOutput = false;
    formatOptions.BraceStyle = BraceStyle.SameLine;

    FormatResult result = GmlFormatter.Format(input, formatOptions);

    Console.WriteLine(result);

    FormatResult secondResult = GmlFormatter.Format(result.Output, formatOptions);

    Console.WriteLine(StringDiffer.PrintFirstDifference(result.Output, secondResult.Output));

    return result.Output;
}

var input = $$$"""
return some_condition
/*outside*/.
// inside
call_method(
    some_long_parameter____________________________,
    some_long_parameter____________________________
)
.something.call_method(
    some_long_parameter____________________________,
    some_long_parameter____________________________
);
""";

TestFormat(input);
