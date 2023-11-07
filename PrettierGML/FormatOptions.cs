namespace PrettierGML
{
    public class FormatOptions
    {
        public BraceStyle BraceStyle { get; init; } = BraceStyle.SameLine;
        public bool UseTabs { get; init; } = false;
        public int TabWidth { get; init; } = 4;
        public int Width { get; init; } = 80;
        public bool CheckAst { get; init; } = true;
        public bool RemoveSyntaxExtensions { get; set; } = false;
        public bool GetDebugInfo { get; set; } = false;

        public static FormatOptions DefaultTestOptions { get; } =
            new() { Width = 100, GetDebugInfo = true };
    }

    public enum BraceStyle
    {
        SameLine,
        NewLine,
    }
}
