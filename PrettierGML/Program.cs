using PrettierGML;

static string TestFormat(string input)
{
    var formatOptions = new FormatOptions()
    {
        BraceStyle = BraceStyle.NewLine,
        CheckAst = false,
        Debug = true
    };

    FormatResult result = GmlFormatter.Format(input, formatOptions);

    Console.WriteLine(result);

    return result.Output;
}

var input = $$"""
    (function foo() {return}).a()
    """;

TestFormat(input);
