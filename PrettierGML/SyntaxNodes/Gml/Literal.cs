using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using PrettierGML.Printer.DocTypes;
using System.Text.RegularExpressions;

namespace PrettierGML.SyntaxNodes.Gml
{
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

        public Literal(ParserRuleContext context, string text)
            : base(context)
        {
            Text = text;
        }

        public Literal(ITerminalNode context, string text)
            : base(context)
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
}
