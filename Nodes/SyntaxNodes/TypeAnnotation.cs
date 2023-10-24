using Antlr4.Runtime;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class TypeAnnotation : GmlSyntaxNode
    {
        public string Name { get; set; }

        public TypeAnnotation(ParserRuleContext context, string name)
            : base(context)
        {
            Name = name;
        }

        public override Doc Print(PrintContext ctx)
        {
            return Doc.Concat(":", " ", Name);
        }
    }
}
