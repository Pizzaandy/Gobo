using Gobo.Text;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Gobo.Tests;

/// <summary>
/// These tests ensure that implementations of SourceText work as expected on large files
/// </summary>
public class SourceTextTests
{
    private readonly ITestOutputHelper output;

    public const string TestFileExtension = ".test";

    public SourceTextTests(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Theory]
    [ClassData(typeof(SampleFileProvider))]
    public async Task EnsureContentEquals(TestFile test)
    {
        var filePath = test.FilePath;

        var input = await File.ReadAllTextAsync(filePath);
        var wrongInput = "obviously wrong input";

        var sourceTextA = new StringText(input);
        var sourceTextB = new StringText(input);
        var sourceTextC = new StringText(wrongInput);

        if (!sourceTextA.ContentEquals(sourceTextB))
        {
            throw new XunitException($"Comparison failed");
        }

        if (sourceTextA.ContentEquals(sourceTextC))
        {
            throw new XunitException($"Something has gone horribly wrong");
        }
    }
}
