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
if (CATSPEAK_DEBUG_MODE) {
    __catspeak_check_arg("operator", operator, is_numeric); // TODO :: proper bounds check
    __catspeak_check_arg_struct("lhs", lhs, "type", is_numeric);
    __catspeak_check_arg_struct("rhs", rhs, "type", is_numeric);
}
""";

TestFormat(input);
