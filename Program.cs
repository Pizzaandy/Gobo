using CSharpier;
using PrettierGML;
using System.Diagnostics;

static void Format(string input)
{
    Stopwatch sw = Stopwatch.StartNew();

    //var astString = GmlParser.Parse(input).ToString();
    //Console.WriteLine(astString);

    var result = GmlFormatter.Format(input, new PrinterOptions() { Width = 80 }, checkAst: true);
    Console.WriteLine(result);

    sw.Stop();
    Console.WriteLine($"Total Time: {sw.ElapsedMilliseconds} ms");
}

Format(
    """
switch foo
{
    case x:
        break;
    case y:
    foo.bar.baz()
        break;
}
"""
);
