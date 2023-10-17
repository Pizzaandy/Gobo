using Antlr4.Runtime;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class DefineStatement : GmlSyntaxNode
    {
        public string Name { get; set; }

        public DefineStatement(
            ParserRuleContext context,
            CommonTokenStream tokenStream,
            string name
        )
            : base(context, tokenStream)
        {
            Name = name;
        }

        public override Doc Print()
        {
            return Doc.Concat("#define", Name);
        }
    }
}
