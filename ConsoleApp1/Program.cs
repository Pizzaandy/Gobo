using Gobo.Parser;

var input = $$"""
// comment
""";

var lexer = new GmlLexer(new StreamReader(input));

while (!lexer.HitEof)
{
    Console.WriteLine(lexer.NextToken().Kind.ToString());
}
