using PrettierGML;

static string TestFormat(string input)
{
    var formatOptions = FormatOptions.DefaultTestOptions;

    FormatResult result = GmlFormatter.Format(input, formatOptions);

    Console.WriteLine(result);

    //FormatResult secondResult = GmlFormatter.Format(result.Output, formatOptions);

    //Console.WriteLine(secondResult);

    return result.Output;
}

var input = $$"""
    if (condition) // this comment should stay outside
    { 
        return;
    }

    if (condition) { // this comment should stay outside
        return;
    }
    
    """;

TestFormat(input);
