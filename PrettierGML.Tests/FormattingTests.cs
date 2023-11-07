using System.Collections;
using System.Data;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace PrettierGML.Tests
{
    public class FormattingTests
    {
        private readonly FormatOptions options = FormatOptions.DefaultTestOptions;

        private readonly ITestOutputHelper output;

        public const string TestFileExtension = ".test";

        public const string ExpectedFileExtension = ".expected";

        public const string ActualFileExtension = ".actual";

        public FormattingTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory]
        [ClassData(typeof(FormattingTestProvider))]
        public async Task FormatTests(FormatTestFile test)
        {
            var testFilePath = test.FilePath;
            var expectedFilePath = testFilePath.Replace(TestFileExtension, ExpectedFileExtension);
            var actualFilePath = testFilePath.Replace(TestFileExtension, ActualFileExtension);

            if (!Path.Exists(testFilePath))
            {
                throw new XunitException($"Test file {testFilePath} does not exist!");
            }
            if (!Path.Exists(expectedFilePath))
            {
                throw new XunitException($"Expected test file {expectedFilePath} does not exist!");
            }

            var input = await File.ReadAllTextAsync(testFilePath);

            var firstFormat = GmlFormatter.Format(input, options);

            output.WriteLine($"Parse: {firstFormat.ParseTimeMs}");
            output.WriteLine($"Format: {firstFormat.FormatTimeMs}");
            output.WriteLine($"Total: {firstFormat.TotalTimeMs}");

            await File.WriteAllTextAsync(actualFilePath, firstFormat.Output);

            var expectedOutput = await File.ReadAllTextAsync(expectedFilePath);

            var firstDiff = StringDiffer.PrintFirstDifference(expectedOutput, firstFormat.Output);
            if (firstDiff != string.Empty)
            {
                throw new XunitException($"First pass:\n{firstDiff}");
            }

            var secondFormat = GmlFormatter.Format(firstFormat.Output, options);

            var secondDiff = StringDiffer.PrintFirstDifference(expectedOutput, secondFormat.Output);
            if (secondDiff != string.Empty)
            {
                throw new XunitException($"Second pass:\n{secondDiff}");
            }
        }
    }

    public class FormattingTestProvider : IEnumerable<object[]>
    {
        private readonly DirectoryInfo rootDirectory = DirectoryFinder.FindParent(
            "PrettierGML.Tests"
        );

        public FormattingTestProvider() { }

        public IEnumerator<object[]> GetEnumerator()
        {
            var filePath = Path.Combine(rootDirectory.FullName, "Gml", "FormattingTests");
            var files = Directory.EnumerateFiles(filePath, $"*{Samples.TestFileExtension}");
            return files.Select(fp => new object[] { new FormatTestFile(fp) }).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
