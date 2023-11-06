using PrettierGML;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
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
            filePath = AnsiConsole.Ask<string>("Path: ");
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
            await DebugFormatFile(filePath);
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
                return 0;
            }

            foreach (var file in files)
            {
                await DebugFormatFile(file.FullName);
            }
        }
        else
        {
            AnsiConsole.WriteException(new Exception("Something went wrong..."));
            return 2;
        }

        return 2;
    }

    private static async Task DebugFormatFile(string filePath)
    {
        var input = await File.ReadAllTextAsync(filePath);
        var result = GmlFormatter.Format(input, new() { GetDebugInfo = true });
        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Rule("Formatted Code").LeftJustified());
        AnsiConsole.Write(new Text(result.Output));
    }
}
