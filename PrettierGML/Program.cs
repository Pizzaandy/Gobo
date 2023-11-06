using PrettierGML;

static string TestFormat(string input)
{
    var formatOptions = new FormatOptions()
    {
        BraceStyle = BraceStyle.SameLine,
        CheckAst = true,
        Debug = true
    };

    FormatResult result = GmlFormatter.Format(input, formatOptions);

    Console.WriteLine(result);

    return result.Output;
}

var input = $$"""
    /* this
        entire
         comment*/
    /* sequence *//*should*///be
    // grouped

    // but not this one!
    call(
        foo,
        bar, // a
        baz
    ); // b
    // break
    var arr = [
        a123___________________,
        456, /* eee*/
        abc_____________________________________
    ];

    // flat
    var arr2 = [a123, 456 /* eee*/, abc];
    """;

TestFormat(input);
