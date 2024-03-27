using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Gobo;
using Gobo.Tests;

namespace CSharpier.Benchmarks;

[MemoryDiagnoser]
public class Benchmarks
{
    private readonly string largeCode = File.ReadAllText(
        Path.Combine(DirectoryFinder.FindParent("Gobo.Benchmarks").FullName, "BigFile.gml")
    );

    [Benchmark]
    public void DefaultFormat()
    {
        GmlFormatter.Format(largeCode, FormatOptions.Default);
    }
}

public class Program
{
    public static void Main()
    {
        var summary = BenchmarkRunner.Run<Benchmarks>();
    }
}
