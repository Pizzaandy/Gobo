using Gobo;
using Gobo.Parser;

var input = $$"""
#region a
""";

var result = GmlFormatter.Format(input, new FormatOptions { ValidateOutput = false });
Console.WriteLine(result.Output);
var secondResult = new GmlParser(result.Output).Parse();
Console.WriteLine(secondResult.Ast.ToString());
