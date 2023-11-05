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
    if cond // yep
    {
    // eee
    }
    call(/* comment!

    */) // yay!

    switch (foo) // e
    {
        //bar
    }
    """;

TestFormat(input);
