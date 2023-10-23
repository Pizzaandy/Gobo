using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using PrettierGML.Nodes;

namespace PrettierGML
{
    internal static class GmlParser
    {
        public static GmlSyntaxNode Parse(
            string input,
            out CommonTokenStream sourceTokens,
            bool handleComments = true
        )
        {
            ICharStream stream = CharStreams.fromString(input);
            var lexer = new GameMakerLanguageLexer(stream);
            sourceTokens = new CommonTokenStream(lexer);

            var parser = new GameMakerLanguageParser(sourceTokens);
            parser.Interpreter.PredictionMode = Antlr4.Runtime.Atn.PredictionMode.SLL;
            parser.AddErrorListener(new GameMakerLanguageErrorListener());

            IParseTree tree = parser.program();
            var builder = new GmlAstBuilder();

            var ast = builder.Visit(tree);

            if (handleComments)
            {
                new CommentMapper(ast, sourceTokens).AttachComments();
            }

            return ast;
        }
    }
}

