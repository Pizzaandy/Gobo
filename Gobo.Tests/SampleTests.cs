using System.Collections;
using System.Data;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Gobo.Tests;

public class SampleTests
{
    private readonly FormatOptions options = FormatOptions.DefaultTestOptions;

    private readonly ITestOutputHelper output;

    public const string TestFileExtension = ".test";
    public const string ActualFileExtension = ".actual";

    public SampleTests(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Theory]
    [ClassData(typeof(SampleFileProvider))]
    public async Task FormatSamples(TestFile test)
    {
        var filePath = test.FilePath;

        var input = await File.ReadAllTextAsync(filePath);

        var firstPass = GmlFormatter.Format(input, options);

        output.WriteLine($"Parse: {firstPass.ParseTimeMs}");
        output.WriteLine($"Format: {firstPass.FormatTimeMs}");
        output.WriteLine($"Total: {firstPass.TotalTimeMs}");

        var secondPass = GmlFormatter.Format(firstPass.Output, options);

        var secondDiff = StringDiffer.PrintFirstDifference(firstPass.Output, secondPass.Output);
        if (secondDiff != string.Empty)
        {
            throw new XunitException($"Second pass:\n{secondDiff}");
        }

        await File.WriteAllTextAsync(
            filePath.Replace(TestFileExtension, ActualFileExtension),
            firstPass.Output
        );
    }
}

public class SampleFileProvider : IEnumerable<object[]>
{
    private readonly DirectoryInfo rootDirectory = DirectoryFinder.FindParent("Gobo.Tests");

    public SampleFileProvider() { }

    public IEnumerator<object[]> GetEnumerator()
    {
        var filePath = Path.Combine(rootDirectory.FullName, "Gml", "Samples");
        var files = Directory.EnumerateFiles(filePath, $"*{SampleTests.TestFileExtension}");
        return files.Select(fp => new object[] { new TestFile(fp) }).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
