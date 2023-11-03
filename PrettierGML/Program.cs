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
    if (indent) {
    // e
        // e 2

    // e 3
        /*thing*/call(); // eee
    }

    if (indent) {
        /* this
            entire
        comment*/
    /* sequence */ /*should*/ //not
    // break
       /**
    * This is the comments
    * This is the comments
    * @author Rookie
    * @since 2019-07-07
    * 
    */
        call();
    }
    
    """;

Format(input);
