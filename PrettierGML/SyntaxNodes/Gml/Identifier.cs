using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using PrettierGML.Printer.DocTypes;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal class Identifier : TerminalNode
    {
        public string Name { get; set; }

        public Identifier(ITerminalNode context, string name)
            : base(context)
        {
            Name = name;
        }

        public Identifier(ParserRuleContext context, string name)
            : base(context)
        {
            Name = name;
        }

        public override Doc Print(PrintContext ctx)
        {
            return DecorateWithComments(ctx, Name);
        }
    }
}
