using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class Identifier : GmlSyntaxNode
    {
        public string Name { get; set; }

        public Identifier(ITerminalNode context, CommonTokenStream tokenStream, string name)
            : base(context, tokenStream)
        {
            Name = name;
        }

        public Identifier(ParserRuleContext context, CommonTokenStream tokenStream, string name)
            : base(context, tokenStream)
        {
            Name = name;
        }

        public override Doc Print()
        {
            return Name;
        }
    }
}
