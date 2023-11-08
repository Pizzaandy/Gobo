using PrettierGML;
using PrettierGML.Printer.Utilities;
using System.Diagnostics;

static string TestFormat(string input)
{
    var formatOptions = FormatOptions.DefaultTestOptions;

    formatOptions.ValidateOutput = false;
    //formatOptions.BraceStyle = BraceStyle.NewLine;

    FormatResult result = GmlFormatter.Format(input, formatOptions);

    //Console.WriteLine(result);

    FormatResult secondResult = GmlFormatter.Format(result.Output, formatOptions);

    Console.WriteLine(secondResult);

    Console.WriteLine(StringDiffer.PrintFirstDifference(result.Output, secondResult.Output));

    return result.Output;
}

var input = $$"""
    for (;;) return

    for (var i = 0; i < 42; {i++;}) {}

    while ((((condition))))
        return;

    while (
    (((longStatementName
    && longerStatementName
    && evenLongerStatementName
    && superLongStatementName)))
    ) 
    return;

    do statement() until ((condition));

    do operation()
    until 
    longStatementName 
    && longerStatementName
    && evenLongerStatementName
    && superLongStatementName

    repeat((3)) {}
    """;

TestFormat(input);
