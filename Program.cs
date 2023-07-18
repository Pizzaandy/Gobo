using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Tree;
using Newtonsoft.Json;
using PrettierGML;
using System.Diagnostics;

static void PrintTokens(string input)
{
    ICharStream stream = CharStreams.fromString(input);
    var lexer = new GameMakerLanguageLexer(stream);

    for (var token = lexer.NextToken(); !lexer.HitEOF; token = lexer.NextToken())
    {
        var name = lexer.Vocabulary.GetSymbolicName(token.Type);
        Console.WriteLine(name);
    }
    lexer.Reset();
}

static void Format(string input)
{
    Stopwatch sw = Stopwatch.StartNew();

    ICharStream stream = CharStreams.fromString(input);
    var lexer = new GameMakerLanguageLexer(stream);
    var tokens = new CommonTokenStream(lexer);
    var parser = new GameMakerLanguageParser(tokens) { };
    parser.Interpreter.PredictionMode = PredictionMode.SLL;
    IParseTree tree = parser.program();

    var builder = new GameMakerASTBuilder();
    var output = builder.Visit(tree);

    Console.WriteLine(JsonConvert.SerializeObject(output, Formatting.Indented));

    sw.Stop();
    Console.WriteLine($"Total Time: {sw.ElapsedMilliseconds} ms");
}

Format(
    """ 
switch a {
    case "e":
        a()
        break;
    case a:
    case b:
        bruh()
}

"""
);
