using Antlr4.Runtime;
using PrettierGML.SyntaxNodes;

namespace PrettierGML
{
    internal class PrintContext
    {
        public FormatOptions Options { get; set; }
        public CommonTokenStream Tokens { get; set; }
        public Stack<GmlSyntaxNode> Stack = new();
        public int PrintDepth => Stack.Count;

        public PrintContext(FormatOptions options, CommonTokenStream tokenStream)
        {
            Options = options;
            Tokens = tokenStream;
        }
    }
}
