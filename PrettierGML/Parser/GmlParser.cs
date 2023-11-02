using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using PrettierGML.SyntaxNodes;

namespace PrettierGML.Parser
{
    internal struct GmlParseResult
    {
        public CommonTokenStream TokenStream;
        public GmlSyntaxNode Ast;
    }

    internal static class GmlParser
    {
        public static GmlParseResult Parse(string input, bool attachComments = true)
        {
            // 1) Parse with Antlr
            ICharStream stream = CharStreams.fromString(input);
            var lexer = new GameMakerLanguageLexer(stream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new GameMakerLanguageParser(tokenStream);
            parser.Interpreter.PredictionMode = Antlr4.Runtime.Atn.PredictionMode.SLL;
            parser.AddErrorListener(new GameMakerLanguageErrorListener());


            // 2) Build custom syntax tree with Antlr visitor
            var builder = new GmlAstBuilder();
            var ast = builder.Visit(parser.program());

            // 3) Handle comments TODO: 
            if (attachComments)
            {
                ast = new CommentMapper(tokenStream).AttachComments(ast);
            }

            return new GmlParseResult() { Ast = ast, TokenStream = tokenStream };
        }
    }
}
