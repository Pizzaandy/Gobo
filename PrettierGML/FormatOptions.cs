namespace PrettierGML
{
    public class FormatOptions
    {
        public BraceStyle BraceStyle { get; set; } = BraceStyle.SameLine;
        public bool UseTabs { get; set; } = false;
        public int TabWidth { get; set; } = 4;
        public int Width { get; set; } = 90;
        public bool ValidateOutput { get; set; } = true;
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
