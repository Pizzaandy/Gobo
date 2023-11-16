using System.Collections;
using System.Data;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace PrettierGML.Tests;

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
    public async Task FormatTests(TestFile test)
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

        var firstPass = GmlFormatter.Format(input, options);

        output.WriteLine(firstPass.ToString());

        await File.WriteAllTextAsync(actualFilePath, firstPass.Output);

        var expectedOutput = await File.ReadAllTextAsync(expectedFilePath);

        var firstDiff = StringDiffer.PrintFirstDifference(expectedOutput, firstPass.Output);
        if (firstDiff != string.Empty)
        {
            throw new XunitException($"Formatting error on first pass:\n{firstDiff}");
        }

        var secondPass = GmlFormatter.Format(firstPass.Output, options);

        var secondDiff = StringDiffer.PrintFirstDifference(expectedOutput, secondPass.Output);
        if (secondDiff != string.Empty)
        {
            throw new XunitException($"Formatting error on second pass:\n{secondDiff}");
        }
    }
}

public class FormattingTestProvider : IEnumerable<object[]>
{
    private readonly DirectoryInfo rootDirectory = DirectoryFinder.FindParent("PrettierGML.Tests");

    public FormattingTestProvider() { }

    public IEnumerator<object[]> GetEnumerator()
    {
        var filePath = Path.Combine(rootDirectory.FullName, "Gml", "FormattingTests");
        var files = Directory.EnumerateFiles(filePath, $"*{Samples.TestFileExtension}");
        return files.Select(fp => new object[] { new TestFile(fp) }).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
