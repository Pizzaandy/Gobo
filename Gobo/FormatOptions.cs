using System.Text.Json.Serialization;

namespace Gobo;

public enum BraceStyle
{
    SameLine,
    NewLine,
}

public class FormatOptions
{
    public bool UseTabs { get; set; } = false;
    public int TabWidth { get; set; } = 4;
    public int Width { get; set; } = 90;
    public bool FormatComments { get; set; } = false;

    [JsonIgnore]
    public BraceStyle BraceStyle { get; set; } = BraceStyle.SameLine;

    [JsonIgnore]
    public bool ValidateOutput { get; set; } = true;

    [JsonIgnore]
    public bool RemoveSyntaxExtensions { get; set; } = false;

    [JsonIgnore]
    public bool GetDebugInfo { get; set; } = false;

    public static FormatOptions DefaultTestOptions { get; } = new() { GetDebugInfo = true };

    public static FormatOptions Default { get; } = new();
}

[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(FormatOptions))]
public partial class FormatOptionsSerializer : JsonSerializerContext { }
