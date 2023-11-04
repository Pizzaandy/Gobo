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

var input = $$"""
    /* this
        entire
         comment*/
    /* sequence */   /*should*/ //be
    // grouped

    // but not this one!
    call(
    foo,
    bar, // e
    baz)    // eee

    var foo = {
        /*e*/a:b, // thing
        c:d
    }

    // break
    var arr = [
        a123___________________,
        456, /* eee*/
        abc_____________________________________
    ]
    
    // flat
    var arr2 = [
        a123,
        456, /* eee*/
        abc
    ]

    enum /*comment*/ foo {
    bar, baz=567, qux
    }

    switch (foo) {
      case thing:

      // comment
      thing()

    break;   
    default:
    break;
    }

    foo = {"a":"b"}
    """;

Format(input);
