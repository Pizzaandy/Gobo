namespace PrettierGML.Printer.DocTypes;

internal class LiteralLine : LineDoc, IBreakParent
{
    public LiteralLine()
    {
        Type = LineType.Hard;
        IsLiteral = true;
    }
}
