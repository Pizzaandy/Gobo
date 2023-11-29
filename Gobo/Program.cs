using Gobo.Parser;

var input = $$"""
$"{foo bar $"another {one}" }" = <> #define sefwefweg w4eg 3984u59384534
""";

var lexer = new GmlLexer(new StringReader(input));

while (!lexer.HitEof)
{
    var token = lexer.NextToken();
    Console.WriteLine($"{token.Kind}: `{token.Text}`");
}
