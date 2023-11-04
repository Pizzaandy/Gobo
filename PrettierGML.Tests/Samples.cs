using Xunit.Abstractions;

namespace PrettierGML.Tests
{
    public class Samples
    {
        private readonly DirectoryInfo rootDirectory = DirectoryFinder.FindParent(
            "PrettierGML.Tests"
        );

        private readonly FormatOptions options = FormatOptions.TestOptions;

        private readonly ITestOutputHelper output;

        public Samples(ITestOutputHelper output)
        {
            this.output = output;
        }

        private const string TestFileExtension = ".test";
        private const string ActualFileExtension = ".actual";

        [Fact]
        public async Task FormatAllSamples()
        {
            var filePath = Path.Combine(rootDirectory.FullName, "Samples");
            var files = Directory.EnumerateFiles(filePath, $"*{TestFileExtension}");

            foreach (var file in files)
            {
                var input = await File.ReadAllTextAsync(file);

                var formatted = GmlFormatter.Format(input, options);

                await File.WriteAllTextAsync(
                    file.Replace(TestFileExtension, ActualFileExtension),
                    formatted
                );
            }
        }
    }
}
