using PrettierGML;

static string TestFormat(string input)
{
    var formatOptions = FormatOptions.DefaultTestOptions;

    FormatResult result = GmlFormatter.Format(input, formatOptions);

    Console.WriteLine(result);

    FormatResult secondResult = GmlFormatter.Format(result.Output, formatOptions);

    Console.WriteLine(secondResult);

    return result.Output;
}

var input = $$"""
    x =
    longStatementName
    && longerStatementName // foo
    || evenLongerStatementName
    && superLongStatementName
    """;

TestFormat(input);
