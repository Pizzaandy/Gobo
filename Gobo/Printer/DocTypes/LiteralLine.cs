namespace Gobo.Printer.DocTypes;

internal sealed class LiteralLine : LineDoc, IBreakParent
{
    public LiteralLine()
    {
        Type = LineType.Hard;
        IsLiteral = true;
    }
}
