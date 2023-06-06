using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Tree;
using CSharpier;
using CSharpier.DocPrinter;
using GMLParser;
using System.Diagnostics;
void printTokens(string input)
{
	ICharStream stream = CharStreams.fromString(input);
	var lexer = new GameMakerLanguageLexer(stream);

	for (
		var token = lexer.NextToken();
		!lexer.HitEOF;
		token = lexer.NextToken()
	)
	{
		var name = lexer.Vocabulary.GetSymbolicName(token.Type);
		Console.WriteLine(name);
	}
	lexer.Reset();
}

void parse(string input)
{
	Stopwatch sw = Stopwatch.StartNew();

	ICharStream stream = CharStreams.fromString(input);
	var lexer = new GameMakerLanguageLexer(stream);
	ITokenStream tokens = new CommonTokenStream(lexer);
	var parser = new GameMakerLanguageParser(tokens);
	parser.BuildParseTree = true;
	parser.Interpreter.PredictionMode = PredictionMode.SLL;
	IParseTree tree = parser.program();
	//Console.WriteLine(tree.ToStringTree());

	var printer = new VisitorAndPrinter();
	var doc = printer.Visit(tree);

	var serialized = DocSerializer.Serialize(doc);

	PrinterOptions options = new PrinterOptions();

	var printed = DocPrinter.Print(doc, options, "\n");

	Console.WriteLine(serialized);
	Console.WriteLine(printed);

	sw.Stop();
	Console.WriteLine($"Parse Time: {sw.ElapsedMilliseconds.ToString()} ms");
}

parse(@"
{}
");