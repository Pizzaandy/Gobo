using Antlr4.Runtime;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class Block : GmlSyntaxNode
    {
        public GmlSyntaxNode Body { get; set; }

        public Block(ParserRuleContext context, GmlSyntaxNode body)
            : base(context)
        {
            Body = AsChild(body);
        }

        public override Doc Print()
        {
            return Doc.Concat("{", Doc.Indent(Doc.HardLine, PrintHelper.PrintStatements(Body)), Doc.HardLine, "}");
        }
    }
}
