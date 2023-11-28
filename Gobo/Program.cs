using Gobo.Parser;

var input = $$"""
   
// comment /*  */
/
// /* */


""";

var lexer = new GmlLexer(new StringReader(input));

while (!lexer.HitEof)
{
    var token = lexer.NextToken();
    Console.WriteLine($"{token.Kind.ToString()}: `{token.Text}`");
}
