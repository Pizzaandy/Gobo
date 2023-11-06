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
        [ClassData(typeof(SampleFileProvider))]
        public async Task FormatSamples(SampleFileTest test)
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
            var filePath = Path.Combine(rootDirectory.FullName, "Samples");
            var files = Directory.EnumerateFiles(filePath, $"*{Samples.TestFileExtension}");
            return files.Select(fp => new object[] { new SampleFileTest(fp) }).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class SampleFileTest
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }

        public SampleFileTest(string filePath)
        {
            FilePath = filePath;
            FileName = Path.GetFileName(filePath);
        }

        public override string ToString()
        {
            return FileName;
        }
    }
}
