using Antlr4.Runtime;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class DefineStatement : GmlSyntaxNode
    {
        public string Name { get; set; }

        public DefineStatement(ParserRuleContext context, string name)
            : base(context)
        {
            Name = name;
        }

        public override Doc Print(PrintContext ctx)
        {
            return Doc.Concat("#define", " ", Name);
        }
    }
}
