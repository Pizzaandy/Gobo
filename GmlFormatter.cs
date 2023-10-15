using CSharpier;
using CSharpier.DocPrinter;

namespace PrettierGML
{
    internal static class GmlFormatter
    {
        public static string Format(string input, PrinterOptions options, bool checkAst = true)
        {
            var ast = GmlParser.Parse(input);
            var initialHash = checkAst ? ast.GetHashCode() : -1;
            var docs = ast.Print();
            Console.WriteLine("--- DOCS ---\n" + docs.ToString());

            var result = DocPrinter.Print(docs, options, "\n");

            if (checkAst)
            {
                var newAst = GmlParser.Parse(result);
                var resultHash = newAst.GetHashCode();
                if (initialHash != resultHash)
                {
                    throw new Exception("Formatting transformed the AST!");
                }
            }

            return result;
        }
    }
}
