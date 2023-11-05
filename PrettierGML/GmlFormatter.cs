using Antlr4.Runtime;
using PrettierGML.Parser;
using PrettierGML.Printer.DocPrinter;
using PrettierGML.SyntaxNodes;

namespace PrettierGML
{
    public static partial class GmlFormatter
    {
        public static string Format(string input, FormatOptions options)
        {
            input = input.ReplaceLineEndings();

            var parseResult = GmlParser.Parse(input);

            GmlSyntaxNode ast = parseResult.Ast;
            CommonTokenStream tokens = parseResult.TokenStream;

            Console.WriteLine(ast);

            var initialHash = options.CheckAst ? ast.GetHashCode() : -1;
            var docs = ast.Print(new PrintContext(options, tokens));
            ast.EnsureCommentsPrinted();

            var printOptions = new Printer.DocPrinterOptions()
            {
                Width = options.Width,
                TabWidth = options.TabWidth,
                UseTabs = options.UseTabs,
            };

            var output = DocPrinter.Print(docs, printOptions, Environment.NewLine);

            if (options.CheckAst)
            {
                var updatedParseResult = GmlParser.Parse(output);

                var resultHash = updatedParseResult.Ast.GetHashCode();
                Console.WriteLine(updatedParseResult.Ast);

                if (initialHash != resultHash)
                {
                    Console.Clear();

                    Console.WriteLine("--- ORIGINAL AST ---");
                    Console.WriteLine(ast);

                    Console.WriteLine("--- FORMATTED AST ---");
                    Console.WriteLine(updatedParseResult.Ast);
                    throw new Exception("Formatting transformed the AST!");
                }
            }

            return output;
        }

        public static async Task FormatFileAsync(string filePath, FormatOptions options)
        {
            string input = await File.ReadAllTextAsync(filePath);
            var formatted = Format(input, options);
            await File.WriteAllTextAsync(filePath, formatted);
        }
    }
}
