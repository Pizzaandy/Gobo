namespace Gobo.Printer;

internal class DocPrinterOptions
{
    public bool UseTabs { get; init; }
    public int TabWidth { get; init; } = 4;
    public int Width { get; init; } = 100;
    public bool TrimInitialLines { get; init; } = true;

    public const int WidthUsedByTests = 100;
}
