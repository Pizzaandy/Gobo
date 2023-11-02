using PrettierGML;
using System.Diagnostics;

static string Format(string input)
{
    var formatOptions = new FormatOptions() { BraceStyle = BraceStyle.SameLine, CheckAst = true };

    Stopwatch sw = Stopwatch.StartNew();
    var result = GmlFormatter.Format(input, formatOptions);
    sw.Stop();

    Console.WriteLine(result);
    Console.WriteLine($"Total Time: {sw.ElapsedMilliseconds} ms");

    return result;
}

// example usage
var input = """
    function() {
    return $"text {
        expression_____________________________________________________________
    } text text text{
        expression_____________________________________________________________
    }";
    }
    if condition
    if condition2
    foo = @"woah
        woah a verbatimstring?
    thats crazy"
    """;

Format(input);
