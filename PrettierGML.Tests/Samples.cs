using System.Collections;
using System.Data;
using Xunit.Abstractions;

namespace PrettierGML.Tests
{
    public class Samples
    {
        private readonly FormatOptions options = FormatOptions.DefaultTestOptions;

        private readonly ITestOutputHelper output;

        public const string TestFileExtension = ".test";

        private const string ActualFileExtension = ".actual";

        public Samples(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory]
        [ClassData(typeof(SampleFileProvider))]
        public async Task FormatSamples(FormatTestFile test)
        {
            var filePath = test.FilePath;

            var input = await File.ReadAllTextAsync(filePath);

            var result = GmlFormatter.Format(input, options);

            output.WriteLine($"Parse: {result.ParseTimeMs}");
            output.WriteLine($"Format: {result.FormatTimeMs}");
            output.WriteLine($"Total: {result.TotalTimeMs}");

            await File.WriteAllTextAsync(
                filePath.Replace(TestFileExtension, ActualFileExtension),
                result.Output
            );
        }
    }

    public class SampleFileProvider : IEnumerable<object[]>
    {
        private readonly DirectoryInfo rootDirectory = DirectoryFinder.FindParent(
            "PrettierGML.Tests"
        );

        public SampleFileProvider() { }

        public IEnumerator<object[]> GetEnumerator()
        {
            var filePath = Path.Combine(rootDirectory.FullName, "Gml", "Samples");
            var files = Directory.EnumerateFiles(filePath, $"*{Samples.TestFileExtension}");
            return files.Select(fp => new object[] { new FormatTestFile(fp) }).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
