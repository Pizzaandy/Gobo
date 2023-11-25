using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.Windows.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using Gobo;
using Gobo.Tests;

namespace CSharpier.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.NativeAot80)]
public class Benchmarks
{
    private readonly string largeCode = File.ReadAllText(
        Path.Combine(DirectoryFinder.FindParent("Gobo.Benchmarks").FullName, "BigFile.txt")
    );

    [Benchmark]
    public void Default_CodeFormatter()
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
