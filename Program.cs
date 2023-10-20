using CSharpier;
using PrettierGML;
using System.Diagnostics;

static void Format(string input)
{
    Stopwatch sw = Stopwatch.StartNew();

    var options = new PrinterOptions() { Width = 80 };

    var astString = GmlParser.Parse(input).ToString();
    Console.WriteLine(astString);

    var result = GmlFormatter.Format(input, options, checkAst: false);
    Console.WriteLine(result);

    sw.Stop();
    Console.WriteLine($"Total Time: {sw.ElapsedMilliseconds} ms");
}

Format(
    """
if (
    one - two == three
    || one + two == three
    || one * two == three
    || one / two == three
    || one % two == three
    || one != three
    || one < two
    || one > two
    || one <= two
    || one >= two
    || one == null
    || one == Something == null
    || one - two > three
    || someLongThing - someOtherLongThing__________________________________
        > anotherLongThing
) { }
"""
);
