using Gobo.Printer.DocTypes;
using System.Text.RegularExpressions;

namespace Gobo.SyntaxNodes.Gml;

// TODO: disambiguate literal types?
internal sealed partial class Literal : GmlSyntaxNode
{
    public string Text { get; set; }

    [GeneratedRegex("^[0-9]+$", RegexOptions.Compiled)]
    private static partial Regex IsInteger();

    [GeneratedRegex("^[0-9]*[.][0-9]+$", RegexOptions.Compiled)]
    private static partial Regex IsDecimal();

    private static Regex isInteger = IsInteger();

    private static Regex isDecimal = IsDecimal();

    public static string Undefined = "undefined";

    public Literal(TextSpan span, string text)
        : base(span)
    {
        Text = text;
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        if (isInteger.IsMatch(Text))
        {
            var trimmed = Text.TrimStart('0');
            if (trimmed.Length == 0)
            {
                return "0";
            }
            return trimmed;
        }
        else if (isDecimal.IsMatch(Text))
        {
            var trimmed = Text.TrimStart('0');
            if (trimmed[0] == '.')
            {
                return "0" + trimmed;
            }
            return trimmed;
        }
        return Text;
    }

    public override int GetHashCode()
    {
        // TODO: separate classes for literals
        if (Text == Undefined)
        {
            return Undefined.GetHashCode();
        }
        return base.GetHashCode();
    }
}
