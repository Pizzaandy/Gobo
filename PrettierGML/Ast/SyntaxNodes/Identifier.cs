using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using PrettierGML.Printer;
using PrettierGML.Printer.Docs.DocTypes;

namespace PrettierGML.Nodes.SyntaxNodes
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
            return Name;
        }
    }
}
