using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using PrettierGML.Printer.DocTypes;

namespace PrettierGML.SyntaxNodes.Gml
{
    // TODO: disambiguate literal types?
    internal sealed class Literal : GmlSyntaxNode
    {
        public string Text { get; set; }

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
            if (int.TryParse(Text, out var asInt))
            {
                return asInt.ToString();
            }
            else if (decimal.TryParse(Text, out var asDecimal))
            {
                return asDecimal.ToString();
            }
            return Text;
        }
    }
}
