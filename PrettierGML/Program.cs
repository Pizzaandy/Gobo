using PrettierGML;

static string TestFormat(string input)
{
    var formatOptions = new FormatOptions()
    {
        BraceStyle = BraceStyle.SameLine,
        CheckAst = true,
        GetDebugInfo = true
    };

    FormatResult result = GmlFormatter.Format(input, formatOptions);

    Console.WriteLine(result);

    return result.Output;
}

var input = $$"""
    // @fmt-ignore
    call(
        a, /* thing */     b // foo
    )
    """;

TestFormat(input);
