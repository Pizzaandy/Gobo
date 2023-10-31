namespace PrettierGML
{
    internal class FormatOptions
    {
        public BraceStyle BraceStyle { get; init; } = BraceStyle.SameLine;
        public bool UseTabs { get; init; } = false;
        public int TabWidth { get; init; } = 4;
        public int Width { get; init; } = 80;
        public bool CheckAst { get; init; } = true;
    }

    internal enum BraceStyle
    {
        SameLine,
        NewLine,
    }
}
