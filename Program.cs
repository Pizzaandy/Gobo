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
    var conditionalIndentation = someBoolean
            ? someLongValue____________________________________
                + someLongValue____________________________________
            : someLongValue____________________________________
                + someLongValue____________________________________;
                
    var a = someLongValue____________________________________, b = someLongValue____________________________________;

    for (
        i_____________________: int = 1;
        i_____________________ < 5;
        i_____________________++
    ) {
        sum += i;
    }

    if ((foo)) {return} else if bar do_something()

    function Vector3(x: real, y = 1, z: real | string = 0) constructor {
        X = x;
        Y = y;
        Z = z;
    }
    """;

Format(input);
