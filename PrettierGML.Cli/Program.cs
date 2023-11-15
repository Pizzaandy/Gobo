using PrettierGML;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

var app = new CommandApp<FormatCommand>();

app.Configure(config =>
{
    config.SetApplicationName("prettier-gml");
});

app.Run(args);

internal sealed class FormatCommand : AsyncCommand<FormatCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [Description("The file path or directory to format")]
        [CommandArgument(0, "[file/dir]")]
        public string? FilePath { get; init; }

        [CommandOption("--version")]
        [DefaultValue(false)]
        public bool Version { get; init; }
    }

    public override async Task<int> ExecuteAsync(
        [NotNull] CommandContext context,
        [NotNull] Settings settings
    )
    {
        if (settings.Version)
        {
            AnsiConsole.WriteLine(typeof(FormatCommand).Assembly.FullName ?? "Version not found");
        }

        var filePath = settings.FilePath ?? "";
        filePath = filePath.Trim('"');

        if (!Path.Exists(filePath))
        {
            Console.Title = "Prettier GML";
            AnsiConsole.Write(new FigletText("Prettier GML").LeftJustified());
            AnsiConsole.Markup("[bold yellow]Specify a file or directory to format:[/]\n");
        }

        while (true)
        {
            filePath = AnsiConsole.Ask<string>("");
            filePath = filePath.Trim('"');
            if (Path.Exists(filePath))
            {
                break;
            }
            else
            {
                AnsiConsole.Markup(
                    $"[red]{filePath}[/] [yellow]is not a valid path. Please specify a valid path to a file or directory.[/]\n"
                );
            }
        }

        if (File.Exists(filePath))
        {
            await DebugFormatFile(filePath, settings);
        }
        else if (Directory.Exists(filePath))
        {
            var files = new DirectoryInfo(filePath).EnumerateFiles(
                "*.gml",
                SearchOption.AllDirectories
            );

            if (!files.Any())
            {
                AnsiConsole.Write(new Markup($"[bold red]No .gml files found in {filePath}[/]"));
                return 2;
            }

            var stopWatch = Stopwatch.StartNew();

            await Task.WhenAll(files.Select(file => DebugFormatFile(file.FullName, settings)));

            stopWatch.Stop();

            AnsiConsole.WriteLine(
                $"Formatted {files.Count()} files in {stopWatch.Elapsed.ToString(@"s\.fff")} seconds"
            );
        }
        else
        {
            AnsiConsole.WriteException(new Exception("Something went wrong..."));
            return 2;
        }

        return 0;
    }

    private static async Task DebugFormatFile(string filePath, Settings settings)
    {
        var input = await File.ReadAllTextAsync(filePath);

        try
        {
            GmlFormatter.Format(input, new() { GetDebugInfo = false, ValidateOutput = false });
        }
        catch (Exception)
        {
            throw;
        }
    }
}
