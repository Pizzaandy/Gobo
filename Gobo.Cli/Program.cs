using DocoptNet;
using Gobo;
using System.Diagnostics;

const string usage =
    @"Usage:
  gobo [options] <file-or-directory>

Options:
  -h --help       Show this screen.
  -v --version    Show version.
  --check         Check that the files are formatted. Will not write any changes.
  --fast          Skip validation of formatted syntax tree and comments.
  --write-stdout  Write the results of formatting any files to stdout.
  --skip-write    Skip writing changes. Used for testing to ensure Gobo doesn't throw any errors.

";

return await Docopt
    .CreateParser(usage)
    .WithVersion("Gobo 0.2")
    .Parse(args)
    .Match(
        Run,
        result => ShowHelp(result.Help),
        result => ShowVersion(result.Version),
        result => OnError(result.Usage)
    );

static async Task<int> ShowHelp(string help)
{
    Console.WriteLine(help);
    return 0;
}

static async Task<int> ShowVersion(string version)
{
    Console.WriteLine(version);
    return 0;
}

static async Task<int> OnError(string usage)
{
    Console.Error.WriteLine(usage);
    return 1;
}

// TODO: Clean this up...
static async Task<int> Run(IDictionary<string, ArgValue> arguments)
{
    var filePath = arguments["<file-or-directory>"].ToString();

    if (!Path.Exists(filePath))
    {
        Console.WriteLine(
            $"{filePath} is not a valid path. Please specify a valid path to a file or directory."
        );
        return 1;
    }

    var check = arguments["--check"].IsTrue;
    Func<string, IDictionary<string, ArgValue>, Task> processFile = check ? CheckFile : FormatFile;

    const string GmlExtension = ".gml";

    if (File.Exists(filePath))
    {
        if (Path.GetExtension(filePath) != GmlExtension)
        {
            Console.WriteLine($"{filePath} is not a *.gml file.");
            return 1;
        }

        Console.WriteLine(check ? "Checking" : "Formatting" + $" {Path.GetFileName(filePath)}...");

        var stopWatch = Stopwatch.StartNew();
        await processFile(filePath, arguments);
        stopWatch.Stop();

        Console.WriteLine($"Done in {stopWatch.ElapsedMilliseconds} ms");
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
            return 1;
        }

        Console.WriteLine(check ? "Checking" : "Formatting" + " files...");
        var stopWatch = Stopwatch.StartNew();

        await Task.WhenAll(files.Select(file => processFile(file.FullName, arguments)));

        stopWatch.Stop();

        Console.WriteLine(
            (check ? "Checked" : "Formatted")
                + $" {files.Count()} files in {stopWatch.Elapsed:s\\.fff} seconds"
        );
    }

    return 0;
}

static async Task FormatFile(string filePath, IDictionary<string, ArgValue> arguments)
{
    var input = new SourceFile(filePath);
    FormatResult result;

    var options = new FormatOptions
    {
        ValidateOutput = arguments["--fast"].IsFalse,
        GetDebugInfo = false
    };

    try
    {
        result = GmlFormatter.Format(input, options);
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
        return;
    }

    if (arguments["--skip-write"].IsFalse)
    {
        await File.WriteAllTextAsync(filePath, result.Output);
    }

    if (arguments["--write-stdout"].IsTrue)
    {
        Console.WriteLine(result.Output);
    }
}

static async Task CheckFile(string filePath, IDictionary<string, ArgValue> arguments)
{
    var input = new SourceFile(filePath);
    bool success;

    var options = new FormatOptions { GetDebugInfo = false };

    try
    {
        success = GmlFormatter.Check(input, options);
    }
    catch (Exception e)
    {
        Console.Error.WriteLine(e.Message);
        return;
    }

    if (!success)
    {
        Console.WriteLine($"[Warn] {filePath}");
    }
}
