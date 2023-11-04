using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal class DefineStatement : GmlSyntaxNode
    {
        public string Name { get; set; }

        public DefineStatement(ParserRuleContext context, string name)
            : base(context)
        {
            Name = name;
        }

        public override Doc PrintNode(PrintContext ctx)
        {
            return Doc.Concat("#define", " ", Name);
        }
    }
}
