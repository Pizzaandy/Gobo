using System.Collections;
using System.Data;
using System.Text.Json;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Gobo.Tests;

/// <summary>
/// General purpose formatting tests to avoid regressions
/// </summary>
public class FormattingTests
{
    private readonly ITestOutputHelper output;

    public const string TestFileExtension = ".test";
    public const string ExpectedFileExtension = ".expected";
    public const string ActualFileExtension = ".actual";
    public const string OptionsFileExtension = ".options.json";

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

        var options = await ReadOptionsAsync(testFilePath);

        var firstPass = GmlFormatter.Format(input, options);

        output.WriteLine(firstPass.ToString());

        await File.WriteAllTextAsync(actualFilePath, firstPass.Output);

        // Normalize line endings in case they are added manually on windows
        var expectedOutput = (await File.ReadAllTextAsync(expectedFilePath)).ReplaceLineEndings(
            "\n"
        );

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

    /// <summary>
    /// A test may sit next to '[test name].options.json' to format with its own settings.
    /// </summary>
    private static async Task<FormatOptions> ReadOptionsAsync(string testFilePath)
    {
        var optionsFilePath = testFilePath.Replace(TestFileExtension, OptionsFileExtension);

        if (!Path.Exists(optionsFilePath))
        {
            return FormatOptions.DefaultTestOptions;
        }

        var json = await File.ReadAllTextAsync(optionsFilePath);

        FormatOptions options;

        try
        {
            options =
                JsonSerializer.Deserialize(json, FormatOptionsSerializer.Default.FormatOptions)
                ?? throw new XunitException($"{optionsFilePath} deserialized to null.");
        }
        catch (JsonException ex)
        {
            throw new XunitException($"{optionsFilePath} could not be parsed:\n{ex.Message}");
        }

        options.GetDebugInfo = true;
        return options;
    }
}

public class FormattingTestProvider : IEnumerable<object[]>
{
    private readonly DirectoryInfo rootDirectory = DirectoryFinder.FindParent("Gobo.Tests");

    public FormattingTestProvider() { }

    public IEnumerator<object[]> GetEnumerator()
    {
        var filePath = Path.Combine(rootDirectory.FullName, "Gml", "FormattingTests");
        var files = Directory.EnumerateFiles(filePath, $"*{SampleTests.TestFileExtension}");
        return files.Select(fp => new object[] { new TestFile(fp) }).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
