using Antlr4.Runtime;
using PrettierGML.Printer;

namespace PrettierGML
{
    internal class PrintContext
    {
        public FormatOptions Options { get; set; }
        public CommonTokenStream Tokens { get; set; }
        public bool RemoveSyntaxExtensions { get; set; } = false;

        public PrintContext(FormatOptions options, CommonTokenStream tokenStream)
        {
            Options = options;
            Tokens = tokenStream;
        }
    }
}
