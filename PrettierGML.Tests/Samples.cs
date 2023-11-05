using System.Collections;
using System.Data;
using Xunit.Abstractions;

namespace PrettierGML.Tests
{
    public class Samples
    {
        private readonly FormatOptions options = FormatOptions.TestOptions;

        private readonly ITestOutputHelper output;

        public const string TestFileExtension = ".test";

        private const string ActualFileExtension = ".actual";

        public Samples(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory]
        [ClassData(typeof(SampleFiles))]
        public async Task FormatSamples(string filePath)
        {
            var input = await File.ReadAllTextAsync(filePath);

            var formatted = GmlFormatter.Format(input, options);

            await File.WriteAllTextAsync(
                filePath.Replace(TestFileExtension, ActualFileExtension),
                formatted
            );
        }
    }

    public class SampleFiles : IEnumerable<object[]>
    {
        private readonly DirectoryInfo rootDirectory = DirectoryFinder.FindParent(
            "PrettierGML.Tests"
        );

        public SampleFiles() { }

        public IEnumerator<object[]> GetEnumerator()
        {
            var filePath = Path.Combine(rootDirectory.FullName, "Samples");
            var files = Directory.EnumerateFiles(filePath, $"*{Samples.TestFileExtension}");
            return files.Select(fp => new object[] { fp }).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
