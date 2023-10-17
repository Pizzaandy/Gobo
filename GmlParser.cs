using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using PrettierGML.Nodes;

namespace PrettierGML
{
    internal static class GmlParser
    {
        public static GmlSyntaxNode Parse(string input)
        {
            ICharStream stream = CharStreams.fromString(input);
            var lexer = new GameMakerLanguageLexer(stream);
            var tokens = new CommonTokenStream(lexer);

            var parser = new GameMakerLanguageParser(tokens);
            parser.Interpreter.PredictionMode = Antlr4.Runtime.Atn.PredictionMode.SLL;
            parser.AddErrorListener(new GameMakerLanguageErrorListener());

            IParseTree tree = parser.program();
            var builder = new GmlAstBuilder(tokens);
            return builder.Visit(tree);
        }
    }
}
