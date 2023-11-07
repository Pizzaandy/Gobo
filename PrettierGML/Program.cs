using PrettierGML;
using PrettierGML.Printer;
using PrettierGML.Printer.DocPrinter;
using PrettierGML.Printer.DocTypes;

static string TestFormat(string input)
{
    var formatOptions = FormatOptions.DefaultTestOptions;

    FormatResult result = GmlFormatter.Format(input, formatOptions);

    Console.WriteLine(result);

    FormatResult secondResult = GmlFormatter.Format(result.Output, formatOptions);

    Console.WriteLine(secondResult);

    return result.Output;
}

var docs = Doc.Group(
    "(",
    Doc.Concat(
        Doc.Indent(Doc.HardLine, Doc.LineSuffix("// thing")),
        Doc.Group("xyzxyzabc2_____________________", Doc.HardLine),
        Doc.Concat(Doc.BreakParent, Doc.LineSuffix("thing"))
    ),
    ")"
);

Console.WriteLine(docs);

var output = DocPrinter.Print(docs, new DocPrinterOptions(), Environment.NewLine);
Console.WriteLine(output);
