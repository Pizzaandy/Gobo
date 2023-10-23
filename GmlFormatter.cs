using CSharpier.DocPrinter;

namespace PrettierGML
{
    internal static class GmlFormatter
    {
        public static string Format(string input, FormatOptions options, bool checkAst = true)
        {
            var ast = GmlParser.Parse(input, out var tokens);
            //Console.WriteLine(ast.ToString());

            var initialHash = checkAst ? ast.GetHashCode() : -1;
            var docs = ast.Print(new PrintContext(options, tokens));
            //Console.WriteLine("--- DOCS ---\n" + docs.ToString());

            var printOptions = new CSharpier.PrinterOptions()
            {
                Width = options.Width,
                TabWidth = options.TabWidth,
                UseTabs = options.UseTabs,
            };

            var result = DocPrinter.Print(docs, printOptions, "\n");

            if (checkAst)
            {
                var newAst = GmlParser.Parse(result, out _);
                var resultHash = newAst.GetHashCode();
                if (initialHash != resultHash)
                {
                    Console.Clear();

                    Console.WriteLine("--- ORIGINAL ---");
                    Console.WriteLine(ast);

                    Console.WriteLine("--- FORMATTED ---");
                    Console.WriteLine(newAst);
                    throw new Exception("Formatting transformed the AST!");
                }
            }

            return result;
        }
    }
}
