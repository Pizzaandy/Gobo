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
    var formatOptions = new FormatOptions() { BraceStyle = BraceStyle.SameLine };

    Stopwatch sw = Stopwatch.StartNew();
    var result = GmlFormatter.Format(input, formatOptions, checkAst: false);
    sw.Stop();

    Console.WriteLine(result);
    Console.WriteLine($"Total Time: {sw.ElapsedMilliseconds} ms");

    return result;
}

// example usage
var input = """
   
    call()
    call2()


    // test!

    call3()
    """;

Format(input);
