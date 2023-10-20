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
            return PrintInBlock(PrintHelper.PrintStatements(Body));
        }

        public static Doc PrintInBlock(GmlSyntaxNode node)
        {
            return PrintInBlock(node.Print());
        }

        public static Doc PrintInBlock(Doc bodyDoc)
        {
            if (bodyDoc == Doc.Null)
            {
                return EmptyBlock();
            }
            return Doc.Concat("{", Doc.Indent(Doc.HardLine, bodyDoc), Doc.HardLine, "}");
        }

        public static Doc EmptyBlock()
        {
            return "{ }";
        }
    }
}
