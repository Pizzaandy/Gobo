using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using PrettierGML.Ast.Comments;
using PrettierGML.Nodes;
using PrettierGML.Parser.Antlr;

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
            ICharStream stream = CharStreams.fromString(input);
            var lexer = new GameMakerLanguageLexer(stream);
            var tokenStream = new CommonTokenStream(lexer);

            var parser = new GameMakerLanguageParser(tokenStream);
            parser.Interpreter.PredictionMode = Antlr4.Runtime.Atn.PredictionMode.SLL;
            parser.AddErrorListener(new GameMakerLanguageErrorListener());

            IParseTree tree = parser.program();
            var builder = new GmlAstBuilder();

            var ast = builder.Visit(tree);

            if (attachComments)
            {
                ast = new CommentMapper(tokenStream).AttachComments(ast);
            }

            return new GmlParseResult() { Ast = ast, TokenStream = tokenStream };
        }
    }
}
