using Gobo;
using System.Diagnostics;

if (args.Length == 0)
{
    Console.WriteLine("Please specify a file or directory to format.");
    Console.WriteLine("Usage: gobo [file/dir]");
    return 1;
}

var filePath = args[0];

if (!Path.Exists(filePath))
{
    Console.WriteLine(
        $"{filePath} is not a valid path. Please specify a valid path to a file or directory."
    );
    return 1;
}

await Commands.Format(filePath);

return 0;

public static class Commands
{
    public const string GmlExtension = ".gml";

    public static async Task Format(string filePath)
    {
        if (File.Exists(filePath))
        {
            if (Path.GetExtension(filePath) != GmlExtension) { }
            var result = await DebugFormatFile(filePath);
            Console.WriteLine(result.Output);
            return;
        }
        else if (Directory.Exists(filePath))
        {
            var files = new DirectoryInfo(filePath).EnumerateFiles(
                $"*{GmlExtension}",
                SearchOption.AllDirectories
            );

            if (!files.Any())
            {
                Console.WriteLine($"No *{GmlExtension} files found in {filePath}");
                return;
            }

            var stopWatch = Stopwatch.StartNew();

            await Task.WhenAll(files.Select(file => DebugFormatFile(file.FullName)));

            stopWatch.Stop();

            Console.WriteLine(
                $"Formatted {files.Count()} files in {stopWatch.Elapsed:s\\.fff} seconds"
            );

            return;
        }
    }

    private static async Task<FormatResult> DebugFormatFile(string filePath)
    {
        var input = await File.ReadAllTextAsync(filePath);
        FormatResult result;

        try
        {
            result = GmlFormatter.Format(input, new() { GetDebugInfo = false });
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return FormatResult.Empty;
        }

        return result;
    }
}
