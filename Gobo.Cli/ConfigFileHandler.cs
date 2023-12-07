using System.Text.Json;

namespace Gobo.Cli;

public static class ConfigFileHandler
{
    private const string ConfigFileName = ".goborc.json";

    public static bool TryFindConfigFile(string filePath, out string configFilePath)
    {
        string? currentPath = filePath;

        // Start searching from the current directory and move up to the root directory
        while (!string.IsNullOrEmpty(currentPath))
        {
            var potentialConfigPath = Path.Combine(currentPath, ConfigFileName);

            if (File.Exists(potentialConfigPath))
            {
                configFilePath = potentialConfigPath;
                return true;
            }

            currentPath = Directory.GetParent(currentPath)?.FullName;
        }

        configFilePath = string.Empty;
        return false;
    }

    public static FormatOptions FindConfigOrDefault(string filePath)
    {
        if (TryFindConfigFile(filePath, out var configPath))
        {
            var json = File.ReadAllText(configPath);

            return JsonSerializer.Deserialize(json, FormatOptionsSerializer.Default.FormatOptions)
                ?? FormatOptions.Default;
        }
        else
        {
            return FormatOptions.Default;
        }
    }
}
