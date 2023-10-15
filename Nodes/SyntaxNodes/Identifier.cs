using Antlr4.Runtime.Tree;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class Identifier : GmlSyntaxNode
    {
        public string Name { get; set; }

        public Identifier(ISyntaxTree context, string name)
            : base(context)
        {
            Name = name;
        }

        public override Doc Print()
        {
            return Name;
        }
    }
}
