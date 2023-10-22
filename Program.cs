using PrettierGML;
using System.Diagnostics;

static void FormatFile(string filePath)
{
    string input = File.ReadAllText(filePath);
    var formatted = Format(input);
    File.WriteAllText(filePath, formatted);
}

static string Format(string input)
{
    Stopwatch sw = Stopwatch.StartNew();

    var result = GmlFormatter.Format(input, new FormatOptions(), checkAst: true);
    Console.WriteLine(result);

    sw.Stop();
    Console.WriteLine($"Total Time: {sw.ElapsedMilliseconds} ms");

    return result;
}

var input =
    @"
var a = 0, b = 0, c = 0;
try
{
    c = a div b;
}
catch( _exception)
{
    show_debug_message(_exception.message);
    show_debug_message(_exception.longMessage);

    show_debug_message(_exception.script);

    show_debug_message(_exception.stacktrace).b();
} finally
{
    show_debug_message(""a = "" + string(a));
}
";

Format(input);
