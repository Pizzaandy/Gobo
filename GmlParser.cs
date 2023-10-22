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
            var builder = new GmlAstBuilder(sourceTokens);

            var ast = builder.Visit(tree);

            if (handleComments)
            {
                new CommentMapper(ast, sourceTokens).AttachComments();
            }

            return ast;
        }
    }
}

internal class GmlNodeComparer : Comparer<GmlSyntaxNode>
{
    public override int Compare(GmlSyntaxNode? nodeA, GmlSyntaxNode? nodeB)
    {
        if (nodeA!.SourceInterval.a == nodeB!.SourceInterval.a)
        {
            return nodeA.SourceInterval.b - nodeB.SourceInterval.b;
        }
        else
        {
            return nodeA.SourceInterval.a - nodeB.SourceInterval.a;
        }
    }
}
