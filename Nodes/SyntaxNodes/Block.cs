using Antlr4.Runtime;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class Block : GmlSyntaxNode
    {
        public GmlSyntaxNode Body { get; set; }

        public Block(ParserRuleContext context, CommonTokenStream tokenStream, GmlSyntaxNode body)
            : base(context, tokenStream)
        {
            Body = AsChild(body);
        }

        public override Doc Print()
        {
            if (Body.IsEmpty)
            {
                return "{ }";
            }
            return Doc.Concat(
                "{",
                Doc.Indent(Doc.HardLine, PrintHelper.PrintStatements(Body)),
                Doc.HardLine,
                "}"
            );
        }
    }
}
